using ContentDeliverySystem.SVC.DTO;
using System;
namespace ContentDeliverySystem.SVC
{
    internal interface ITokenService:IDisposable
    {
        string CreateToken(TokenType Type, int IdUser, int? IdContent=null);
        bool IsValidToken(string TokenCode, TokenType Type);
        int GetTokenTypeId(TokenType Type);

        TokenType GetTokenType(string TokenCode);
    }
}
