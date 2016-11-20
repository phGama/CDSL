using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
   public static class ServiceFinder
    {
       public static IAuthService GetAuthService()
       {
           return new AuthService(new CDSEntities(), GetTokenService());
       }

       internal static IAuthServiceInternal GetAuthUserWithPrivilege()
       {
           return new AuthService(new CDSEntities(), GetTokenService());
       }

       public static IGroupService GetGroupService(string tokenCode, TokenType type)
       {
           
           return new GroupService(new CDSEntities(), GetAuthService(),tokenCode, type);
       }

       public static IGenreService GetGenreService(string tokenCode, TokenType type)
       {
           return new GenreService(new CDSEntities(), GetAuthService(), tokenCode, type);
       }

       public static IContentService GetContentService(string tokenCode, TokenType type,string uploaderFolderPath)
       {

           return ContentService.Create(new CDSEntities(), GetTokenService(), GetAuthResponse(tokenCode, type), uploaderFolderPath);
       }

       internal static ITokenService GetTokenService()
       {
           return new TokenService(new CDSEntities());
       }

       public static IUserService GetUserService(string tokenCode, TokenType type)
       {
           
           var AuthService = GetAuthUserWithPrivilege();
           var AuthResponse = AuthService.GetAuthResponse(tokenCode,type);

           
           return new UserService(new CDSEntities(), AuthResponse,new Helpers.BatchMail(), AuthService);
       }


       private static AuthResponse GetAuthResponse(string tokenCode, TokenType type)
       {
           using (var AuthService = GetAuthService())
           {
               var AuthResponse = AuthService.GetAuthResponse(tokenCode, type);    
               return AuthResponse;
           }
       }
       
    }
}
