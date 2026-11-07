namespace DalilAcademy.Core.Security;

// Contract only — HOW hashing works (BCrypt) is an Infra detail.
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
