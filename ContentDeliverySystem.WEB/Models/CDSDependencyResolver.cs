using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContentDeliverySystem.SVC;

using System.Net.Http;
using System.Web.Routing;
using ContentDeliverySystem.SVC.DTO;

namespace ContentDeliverySystem.WEB.Models
{
    public class CDSDependencyResolver
    {


        public static T GetService<T>(RequestContext Request) where T : class
        {

            string Token = string.Empty;
            var SessionAuth = Request.HttpContext.Session[GlobalParams.UserSessionKey];
            var TokenType = SVC.DTO.TokenType.WebAuth;

            if (SessionAuth != null)
            {
                var AuthObject = (AuthResponse)SessionAuth;
                Token = AuthObject.Token;
            }
            
            if (typeof(T) == typeof(IAuthService))
                return (T)ServiceFinder.GetAuthService();
            else if (typeof(T) == typeof(IContentService))
                return (T)ServiceFinder.GetContentService(Token, TokenType, Request.HttpContext.Server.MapPath("~/Uploads/"));
            else if (typeof(T) == typeof(IUserService))
                return (T)ServiceFinder.GetUserService(Token, TokenType);
            else if (typeof(T) == typeof(IGroupService))
                return (T)ServiceFinder.GetGroupService(Token, TokenType);
            else if (typeof(T) == typeof(IGenreService))
                return (T)ServiceFinder.GetGenreService(Token, TokenType);

            return null;
        }
    }
}