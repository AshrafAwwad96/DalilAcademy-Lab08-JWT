using DalilAcademy.Core.DTOs.Auth;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Security;
using FluentValidation;

namespace DalilAcademy.Core.Service;

// Orchestrates login: find the user, verify the password, issue a token.
// It never sees a hash algorithm or a signing key — those live in Infra.
public class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens,
    IValidator<LoginRequestDto> loginValidator,
    IValidator<RegisterRequestDto> registerValidator) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        await loginValidator.ValidateAndThrowAsync(dto);                           // -> 400
        var user = await users.GetByEmailAsync(dto.Email.Trim());

        // Same message for "no such user" and "wrong password" — never reveal which
        if (user is null || !hasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");        // -> 401

        var (token, expiresAt) = tokens.CreateToken(user);
        return new LoginResponseDto(token, expiresAt);
    }

    // Lab 8 · Task 01 — POST /api/auth/register -> Student role, hashed password
    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        await registerValidator.ValidateAndThrowAsync(dto);                        // -> 400
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await users.EmailExistsAsync(email))
            throw new ConflictException($"The email '{email}' is already registered.");   // -> 409

        var user = new User
        {
            Email = email,
            PasswordHash = hasher.Hash(dto.Password),   // BCrypt — the password itself is never stored
            Role = Roles.Student                        // always Student: the client cannot pick a role
        };
        var profile = new Student { FullName = dto.FullName.Trim(), Email = email };

        await users.RegisterStudentAsync(user, profile);   // user + linked student, one transaction

        var (token, expiresAt) = tokens.CreateToken(user);
        return new LoginResponseDto(token, expiresAt);
    }
}
