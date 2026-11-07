using DalilAcademy.Core.DTOs.Auth;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers;

// Thin, as always: a wrong password becomes UnauthorizedException -> GlobalExceptionHandler -> 401.
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService auth) : ControllerBase
{
    // POST /api/auth/login -> 200 { token, expiresAt } / 401
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
        => Ok(await auth.LoginAsync(dto));

    // Lab 8 · Task 01 — POST /api/auth/register -> 201 { token, expiresAt } (Student role) / 400 / 409
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Register(RegisterRequestDto dto)
        => StatusCode(StatusCodes.Status201Created, await auth.RegisterAsync(dto));
}
