using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentDeliverySystem.SVC;
using System.Linq;
using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System.Configuration;

namespace ContentDeliverySystem.TEST
{
    [TestClass]
    public class AuthTest
    {
        private string MockEmail = "pedro.gama@ymail.com";
        private string MockPassword = "h264aac";

                
        [TestMethod]
        public void AuthUserTestEmpty()
        {
            var AuthService = ServiceFinder.GetAuthService();
            Assert.IsFalse(AuthService.AuthUser("", "", SVC.DTO.TokenType.WebAuth).IsSuccess);
        }

        [TestMethod]
        public void AuthUserTest()
        {
            var AuthService = ServiceFinder.GetAuthService();
            Assert.IsFalse(AuthService.AuthUser(MockEmail, "",SVC.DTO.TokenType.WebAuth).IsSuccess);
            Assert.IsFalse(AuthService.AuthUser(MockEmail, "h"+MockPassword,TokenType.WebAuth).IsSuccess);
            Assert.IsTrue(AuthService.AuthUser(MockEmail, MockPassword, TokenType.WebAuth).IsSuccess);
        }

        [TestMethod]
        public void GetAuthResponseTest()
        {
            var TokenService = ServiceFinder.GetTokenService();
            var AuthService = ServiceFinder.GetAuthService();
            var CodeToken = TokenService.CreateToken(TokenType.WebAuth, 1);
            var Auth = AuthService.GetAuthResponse(CodeToken, TokenType.WebAuth);
            Assert.IsTrue(Auth.Token.Length == 32);
            Assert.IsFalse(string.IsNullOrEmpty(Auth.Name));
            Assert.IsFalse(string.IsNullOrEmpty(Auth.Email));
        }

        [TestMethod]
        public void ActivationEmailTest()
        {
            var EmailHtml = "Bom dia {0}<br/>Por favor click <a href='{1}'>aqui</a><br/> para ativa a sua conta. <br/><br/> Caso não tenho se conhecimento do assunto por favor desconsidere esse email";

            var ActivationUrl = "";


            var SmtpService = new ContentDeliverySystem.SVC.Helpers.BatchMail()
            {
                EnableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["smtpSSL"]),
                isHtml = true,
                Message = string.Format(EmailHtml, "Pedro", ActivationUrl),
                SmtpAdress = ConfigurationManager.AppSettings["smtpHost"],
                From = ConfigurationManager.AppSettings["smtpEmail"],
                SmtpMailUser = ConfigurationManager.AppSettings["smtpEmail"],
                SmtpPassword = ConfigurationManager.AppSettings["smtpPassword"],
                Subject = "Ativação de conta CDS",
                To = MockEmail
            };

            SmtpService.Send();

        }
    
    }
}
