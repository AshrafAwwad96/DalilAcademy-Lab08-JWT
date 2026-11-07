using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Security;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
