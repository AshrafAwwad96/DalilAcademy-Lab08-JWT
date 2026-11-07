using DalilAcademy.API.Middleware;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Validators;
using FluentValidation;
using DalilAcademy.Core.Service;
using DalilAcademy.Infra.Data;
using DalilAcademy.Core.Security;
using DalilAcademy.Infra.Repository;
using DalilAcademy.Infra.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using DalilAcademy.Infra.Service;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ───────────── 1. Register services (DI container) ─────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());          // "Beginner" instead of 0
    });

// EF Core + PostgreSQL (provider + connection string come from appsettings.json)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection — interface → implementation (Scoped = one instance per HTTP request, same as AppDbContext)
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// ───────────── Authentication: JWT ─────────────
// Always read user-secrets (Jwt:Key), whatever the launch profile / environment
builder.Configuration.AddUserSecrets<Program>(optional: true);
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
if (jwt.Key.Length < 32)
    throw new InvalidOperationException(
        "Jwt:Key is missing or shorter than 32 characters. Run: dotnet user-secrets set \"Jwt:Key\" \"<32+ chars>\" --project DalilAcademy.API");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)      // [Authorize] uses JWT by default
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)                     // default is 5 minutes!
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();   // no per-request state
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

// FluentValidation — one line registers every validator in Core
builder.Services.AddValidatorsFromAssemblyContaining<CreateCourseDtoValidator>();

// Global error handling — exceptions -> ProblemDetails
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Dalil Academy LMS API",
        Version = "v1",
        Description = "Learning Management System API built with ASP.NET Core 10"
    });

    // "Authorize" button in Swagger UI: paste the token from /api/auth/login
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste the token from POST /api/auth/login"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ───────────── 2. Configure the middleware pipeline (order matters) ─────────────
app.UseExceptionHandler();   // FIRST: wraps everything after it

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dalil Academy API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at the root URL
    });
}

app.UseHttpsRedirection();   // JWTs over plain HTTP can be intercepted
app.UseAuthentication();     // who are you?   ALWAYS before...
app.UseAuthorization();      // ...what may you do?
app.MapControllers();

app.Run();
