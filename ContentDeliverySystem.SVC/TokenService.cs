using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.SVC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
    internal class TokenService : IDisposable, ContentDeliverySystem.SVC.ITokenService
    {
        private CDSEntities CdDb;

        public TokenService(CDSEntities DbEntities)
        {
            CdDb = DbEntities;
        }

        public bool IsValidToken(string TokenCode, TokenType Type)
        {
            var IdType = GetTokenTypeId(Type);
            var Token = CdDb.Tokens.SingleOrDefault(x => x.Code == TokenCode);
            if (Token == null)
                return false;

            return Token.IdType == IdType && Token.ExpireDate > DateTime.Now;
        }

        public string CreateToken(TokenType Type, int IdUser, int? IdContent = null)
        {
            DeleteExpiredTokens();
            int IdTokenType = GetTokenTypeId(Type);
            var Token = CdDb.Tokens.FirstOrDefault(x => x.IdType == IdTokenType && x.IdUser == IdUser && x.IdContent == IdContent);
            if (Token == null)
            {
                Token = GenerateNewToken(Type, IdUser, IdContent);
                CdDb.Tokens.Add(Token);
            }
            Token.ExpireDate = GetExpireDate(Type);
            CdDb.SaveChanges();

            return Token.Code;
        }

        public TokenType GetTokenType(string TokenCode)
        {
            var token = CdDb.Tokens.SingleOrDefault(x => x.Code == TokenCode);
            if (token == null)
                throw new InvalidTokenException("Could not find token");
            return (TokenType)token.IdType;
        }

        public int GetTokenTypeId(TokenType Type)
        {
            return (int)Type;
        }

        private void DeleteExpiredTokens()
        {
            CdDb.Tokens.RemoveRange(CdDb.Tokens.Where(x => x.ExpireDate < DateTime.Now));
            CdDb.SaveChanges();
        }
        private Tokens GenerateNewToken(TokenType Type, int IdUser, int? IdContent = null)
        {
            string Code = String.Empty;
            do
            {
                Code = Guid.NewGuid().ToString().Replace("-", "");
            } while (CdDb.Tokens.Any(x => x.Code == Code));
            return new Tokens()
            {
                IdType = GetTokenTypeId(Type),
                IdUser = IdUser,
                IdContent = IdContent,
                Code = Code,
                CreatedAt = DateTime.Now
            };
        }

        private DateTime GetExpireDate(TokenType Type)
        {
            switch (Type)
            {
                case TokenType.WebAuth:
                case TokenType.WebContent:
                    return DateTime.Now.AddHours(4);
                case TokenType.ApiAuth:
                    return DateTime.Now.AddHours(1);
                case TokenType.ApiContent:
                    return DateTime.Now.AddMinutes(20);

                default:
                    throw new Exception("Token Type is undefined");
            }
        }


        public void Dispose()
        {
            if (CdDb != null)
                CdDb.Dispose();
        }



    }
}
