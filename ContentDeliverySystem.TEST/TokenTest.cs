using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentDeliverySystem.SVC;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.DAL;
using System.Linq;


namespace ContentDeliverySystem.TEST
{
    [TestClass]
    public class TokenTest
    {
        TokenType MockType = TokenType.WebAuth;
        [TestMethod]
        public void CreateTokenTest()
        {
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                int IdResource = 1;
                var Code = TokenService.CreateToken(MockType, IdResource);
                Assert.IsTrue(Code.Length == 32);
                var TokenTypeId = TokenService.GetTokenTypeId(MockType);
                using (var CdDb = new CDSEntities())
                {
                    var Count = CdDb.Tokens.Count(x => x.IdUser == IdResource &&
                        x.IdType == TokenTypeId);
                    Assert.IsTrue(Count == 1);
                }
            }
        }

        [TestMethod]
        public void ValidationTest()
        {
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                int IdResource = 1;
                var Code = TokenService.CreateToken(MockType, IdResource);
                Assert.IsTrue(TokenService.IsValidToken(Code, MockType));
                Assert.IsFalse(TokenService.IsValidToken(Code, TokenType.ApiAuth));
                Assert.IsFalse(TokenService.IsValidToken(Code, TokenType.ApiContent));
            }
        }
    }
}
