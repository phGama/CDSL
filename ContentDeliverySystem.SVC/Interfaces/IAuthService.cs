using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System;
namespace ContentDeliverySystem.SVC
{
   public interface IAuthService:IDisposable
    {
       AuthResponse GetAuthResponse(string Code, TokenType Type);
       AuthResponse AuthUser(string Email, string Password, TokenType DesiredTokenType);

    }
}
