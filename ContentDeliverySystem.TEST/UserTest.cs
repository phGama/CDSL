using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC;
using ContentDeliverySystem.SVC.DTO;
using System.Linq;

namespace ContentDeliverySystem.TEST
{
    [TestClass]
    public class UserTest
    {
        string mockPwd = "h264aac";
        Users MockAddUser = new Users()
        {
            Adress = "userAdress",
            BirthDate = new DateTime(1980, 10, 15),
            Cellphone = "900000000",
            CEP = "04830260",
            CPF = "20947354433",
            Email = "john@gmail.com",
            Gender = 1,
            IdGroup = 1,
            Name = "Peter",
            Phone = "12345678",
            State = "SP",
        };

        Users MockUpdateUser = new Users()
        {
            Adress = "userAdress2",
            BirthDate = new DateTime(1980, 10, 14),
            Cellphone = "900000000",
            CEP = "04830270",
            Gender = 2,
            IdGroup = 1,
            Name = "Toyota",
            Phone = "32165498",
            State = "RJ"
        };


        [TestMethod]
        public void CreateTest()
        {
            var UserService = GetService();
            int CreatedId = UserService.Create(MockAddUser, mockPwd, "");
            using (CDSEntities CdDb = new CDSEntities())
            {
                var DbUser = CdDb.Users.SingleOrDefault(x => x.Id == CreatedId);
                Assert.IsNotNull(DbUser);
                Assert.IsTrue(DbUser.IdType != 1);
                CdDb.Users.Remove(DbUser);
                CdDb.SaveChanges();
            }
        }
        [TestMethod]
        public void DeleteTest()
        {
            var UserService = GetService();
            int CreatedId = UserService.Create(MockAddUser, "testcom01", "");
            using (CDSEntities CdDb = new CDSEntities())
            {
                var DbUser = CdDb.Users.SingleOrDefault(x => x.Id == CreatedId);
                Assert.IsNotNull(DbUser);

                UserService.Delete(CreatedId);
                Assert.IsFalse(CdDb.Users.Any(x => x.Id == CreatedId));
            }

        }

        [TestMethod]
        public void UpdateTest()
        {
            var UserService = GetService();
            int CreatedId = UserService.Create(MockAddUser, "testcom01", "");
            MockUpdateUser.Id = CreatedId;
            UserService.Update(MockUpdateUser);

            using (var CdDb = new CDSEntities())
            {
                var DbUser = CdDb.Users.SingleOrDefault(x => x.Id == CreatedId);
                Assert.IsNotNull(DbUser);

                Assert.AreEqual(DbUser.Adress, MockUpdateUser.Adress);
                Assert.AreEqual(DbUser.BirthDate, MockUpdateUser.BirthDate);
                Assert.AreEqual(DbUser.Cellphone, MockUpdateUser.Cellphone);
                Assert.AreEqual(DbUser.CEP, MockUpdateUser.CEP);
                Assert.AreEqual(DbUser.Gender, MockUpdateUser.Gender);
                Assert.AreEqual(DbUser.IdGroup, MockUpdateUser.IdGroup);
                Assert.AreEqual(DbUser.Name, MockUpdateUser.Name);
                Assert.AreEqual(DbUser.Phone, MockUpdateUser.Phone);
                Assert.AreEqual(DbUser.State, MockUpdateUser.State);

                CdDb.Users.Remove(DbUser);
                CdDb.SaveChanges();
            }

        }

        [TestMethod]
        public void CPFValidationTest()
        {
            var UserService = GetService();
            var User = new Users()
            {
                CPF = "12345678901",
                Email = "name@domain.com"
            };
            Assert.IsFalse(UserService.Validate(User));
            User.CPF = "0";
            Assert.IsFalse(UserService.Validate(User));
            User.CPF = "11111111111";
            Assert.IsFalse(UserService.Validate(User));
            User.CPF = "480.237.112-88";
            Assert.IsFalse(UserService.Validate(User));
            User.CPF = "48023711288";
            Assert.IsTrue(UserService.Validate(User));
        }

        [TestMethod]
        public void EmailValidateTest()
        {
            var UserService = GetService();
            var User = new Users()
            {
                CPF = "48023711288",
                Email = "notvalid"
            };
            Assert.IsFalse(UserService.Validate(User));
            User.Email = "name@";
            Assert.IsFalse(UserService.Validate(User));
            User.Email = "name@domain";
            Assert.IsTrue(UserService.Validate(User));
        }

        private IUserService GetService()
        {
            string CodeToken = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                CodeToken = TokenService.CreateToken(TokenType.WebAuth, 1);
            }
            var ContentService = ServiceFinder.GetUserService(CodeToken, TokenType.WebAuth);
            return ContentService;
        }
    }
}
