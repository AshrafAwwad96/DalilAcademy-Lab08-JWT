using DalilAcademy.Core.Security;

namespace DalilAcademy.Infra.Security;

// package: BCrypt.Net-Next — one-way, salted. "Admin@123" -> "$2a$11$..."
public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
