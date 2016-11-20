using ContentDeliverySystem.SVC.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContentDeliverySystem.WEB.Models
{
    public static class GlobalParams
    {
        public static TokenType ProjectType = TokenType.WebAuth;
        public static string UserSessionKey = "AUTH";
    }
}