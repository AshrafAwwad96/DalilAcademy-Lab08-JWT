namespace DalilAcademy.Core.DTOs.Auth;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(string Token, DateTime ExpiresAt);

// Lab 8 · Task 01 — no Role field: the client can never choose its own role
public record RegisterRequestDto(string FullName, string Email, string Password);
