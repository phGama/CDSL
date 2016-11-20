using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentDeliverySystem.SVC;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.DAL;
using System.Linq;

namespace ContentDeliverySystem.TEST
{
    [TestClass]
    public class GroupTest
    {
        [TestMethod]
        public void CreteTest()
        {
            string Code = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                Code = TokenService.CreateToken(TokenType.WebAuth, 1);
            }

            var GroupService = ServiceFinder.GetGroupService(Code, TokenType.WebAuth);

            using (var CdDb = new CDSEntities())
            {
                var LastGroupId = CdDb.Groups.Max(x => x.Id);

                try
                {
                    GroupService.Create("default");
                    Assert.Fail("Exception should by raised");
                }
                catch { }

                var CreatedID = GroupService.Create("TEST" + LastGroupId);
                Assert.IsTrue(CreatedID != 0);
                Assert.IsTrue(LastGroupId < CreatedID);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateEmptyNameErrorTest()
        {
            string Code = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                Code = TokenService.CreateToken(TokenType.WebAuth, 1);
            }
            var GroupService = ServiceFinder.GetGroupService(Code, TokenType.WebAuth);
            GroupService.Create("");
        }
        [TestMethod]
        public void UpdateTest()
        {
            string Code = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                Code = TokenService.CreateToken(TokenType.WebAuth, 1);
            }

            var GroupService = ServiceFinder.GetGroupService(Code, TokenType.WebAuth);
            var NewName = "testname";
            var TestActive = false;
            var TestId = 1;
            GroupService.Update(TestId, NewName, TestActive);

            using (CDSEntities CdDb = new CDSEntities())
            {
                var DefaultGroup = CdDb.Groups.SingleOrDefault(x => x.Id == TestId);
                Assert.IsTrue(DefaultGroup.Name == NewName);
                Assert.IsTrue(DefaultGroup.Active == TestActive);
            }
            GroupService.Update(TestId, "default", true);
        }

        [TestMethod]
        public void DeleteTest()
        {
            string Code = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                Code = TokenService.CreateToken(TokenType.WebAuth, 1);
            }

            var GroupService = ServiceFinder.GetGroupService(Code, TokenType.WebAuth);

            var CreatedId = GroupService.Create("DeleteTestGroup");
            using (var CdDb = new CDSEntities())
            {
                Assert.IsTrue(GroupService.Delete(CreatedId));
                Assert.IsFalse(CdDb.Groups.Any(x => x.Id == CreatedId));
            }


        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteTestFailWithContents()
        {
            string Code = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                Code = TokenService.CreateToken(TokenType.WebAuth, 1);
            }

            var GroupService = ServiceFinder.GetGroupService(Code, TokenType.WebAuth);

            var CreatedId = GroupService.Create("DeleteTestGroup");
            using (var CdDb = new CDSEntities())
            {
                CdDb.GroupContents.Add(new GroupContents()
                {
                    IdContent = 24,
                    IdGroup = CreatedId,
                    CreatedAt = DateTime.Now
                });
                CdDb.SaveChanges();
                try
                {
                    GroupService.Delete(CreatedId);
                }
                catch (Exception)
                {
                    CdDb.GroupContents.RemoveRange(CdDb.GroupContents.Where(x => x.IdGroup == CreatedId));
                    CdDb.SaveChanges();
                    CdDb.Groups.Remove(CdDb.Groups.SingleOrDefault(x => x.Id == CreatedId));
                    CdDb.SaveChanges();
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteTestFailWithUsers()
        {
            string Code = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                Code = TokenService.CreateToken(TokenType.WebAuth, 1);
            }

            var GroupService = ServiceFinder.GetGroupService(Code, TokenType.WebAuth);

            var CreatedId = GroupService.Create("DeleteTestGroup");
            using (var CdDb = new CDSEntities())
            {
                var User = CdDb.Users.SingleOrDefault(x => x.Id == 1);
                User.IdGroup = CreatedId;
                CdDb.SaveChanges();

                try
                {
                    GroupService.Delete(CreatedId);
                }
                catch (Exception)
                {
                    User.IdGroup = null;
                    CdDb.SaveChanges();
                    CdDb.Groups.Remove(CdDb.Groups.SingleOrDefault(x => x.Id == CreatedId));
                    CdDb.SaveChanges();
                    throw;
                }
            }

        }

        [TestMethod]
        public void FindTest()
        {
            string Code = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                Code = TokenService.CreateToken(TokenType.WebAuth, 1);
            }
            var GroupService = ServiceFinder.GetGroupService(Code, TokenType.WebAuth);

            var DGroup = GroupService.Find(1);
            Assert.IsTrue(DGroup != null);
            Assert.IsTrue(DGroup.Name == "default");
        }

    }
}
