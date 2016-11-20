using ContentDeliverySystem.SVC.DTO;
using System;
namespace ContentDeliverySystem.SVC
{
    internal interface IAuthServiceInternal:IDisposable
    {
        ContentDeliverySystem.SVC.DTO.AuthResponse AuthUser(string Email, string Password, ContentDeliverySystem.SVC.DTO.TokenType DesiredTokenType);
        byte[] CreateEncryptedPassword(string Source);
        ContentDeliverySystem.SVC.DTO.AuthResponse GetAuthResponse(string Code,TokenType Type);
    }
}
