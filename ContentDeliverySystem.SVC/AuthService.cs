using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.Helpers;
using ContentDeliverySystem.SVC.DTO;

namespace ContentDeliverySystem.SVC
{
    public class AuthService : ContentDeliverySystem.SVC.IAuthService, IDisposable, ContentDeliverySystem.SVC.IAuthServiceInternal
    {
        private static int SaltSize = 16;
        private static int KeySize = 64;
        private static Encoding DefaultEncoding = UTF8Encoding.UTF8;
        CDSEntities CdDb;
        ITokenService TokenService;
        AuthResponse ErrorResponse;
        internal AuthService(CDSEntities DbEntities,  ITokenService TokenService)
        {
            CdDb = DbEntities;
            this.TokenService = TokenService;
            ErrorResponse = new AuthResponse()
            {
                IsSuccess = false
            };
        }


        public AuthResponse AuthUser(string Email, string Password,TokenType DesiredTokenType)
        {

            try
            {
                var User = CdDb.Users.SingleOrDefault(x => x.Email == Email && x.Active);
                if (User == null)
                    return ErrorResponse;

                var Salt = new byte[SaltSize];
                var Key = new byte[KeySize];
                Buffer.BlockCopy(User.Password, 0, Salt, 0, SaltSize);
                Buffer.BlockCopy(User.Password, SaltSize, Key, 0, KeySize);
                if (!ComparePasswords(Password, Salt, Key))
                    return ErrorResponse;


                return new AuthResponse()
                {
                    Email = Email,
                    Name = User.Name,
                    Token = TokenService.CreateToken(DesiredTokenType, User.Id),
                    IsAdmin = User.IdType == 1,
                    IsSuccess = true
                };
            }
            catch
            {
                return ErrorResponse;
            }
        }

        public AuthResponse GetAuthResponse(string Code,TokenType Type)
        {
            try
            {
                if (!TokenService.IsValidToken(Code, Type))
                    return ErrorResponse;
                var Token = CdDb.Tokens.SingleOrDefault(x => x.Code == Code);
                if (Token == null || Token.IdType > 2)
                    return ErrorResponse;

                var TokenUser = CdDb.Users.SingleOrDefault(x => x.Id == Token.IdUser);

                return new AuthResponse()
                {
                    Email = TokenUser.Email,
                    IsSuccess = true,
                    Token = Code,
                    Name = TokenUser.Name,
                    IsAdmin = TokenUser.IdType == 1
                };
            }
            catch
            {
                return ErrorResponse; throw;
            }


        }

        public void Unregister(AuthResponse Auth)
        {
            
        }
       
        public byte[] CreateEncryptedPassword(string Source)
        {
            using (var DeriveBytes = new Rfc2898DeriveBytes(Source, SaltSize))
            {
                var Key = DeriveBytes.GetBytes(KeySize);
                return DeriveBytes.Salt.Concat(Key).ToArray();
            };
        }

        private bool ComparePasswords(string Password, byte[] Salt, byte[] Key)
        {
            using (var DeriveBytes = new Rfc2898DeriveBytes(Password, Salt))
            {
                var NewKey = DeriveBytes.GetBytes(KeySize);
                return NewKey.SequenceEqual(Key);
            };
        }

        public void Dispose()
        {
            if (CdDb != null)
                CdDb.Dispose();
            if (TokenService != null)
                TokenService.Dispose();
        }
    }
}
