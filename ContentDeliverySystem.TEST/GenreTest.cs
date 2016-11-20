using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentDeliverySystem.SVC;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.DAL;
using System.Linq;

namespace ContentDeliverySystem.TEST
{
    [TestClass]
    public class GenreTest
    {
        IGenreService genreService;
        string tokenCode;
        TokenType tokenType;
        CDSEntities cdDb;
        string testName;


        [TestInitialize]
        public void SetUp()
        {
            tokenType = TokenType.WebAuth;
            cdDb = new CDSEntities();
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                tokenCode = TokenService.CreateToken(tokenType, 1);
            }
            genreService = ServiceFinder.GetGenreService(tokenCode, tokenType);
            testName = "TESTENAME";
        }

        [TestMethod]
        public void createTest()
        {
            var id = genreService.Create(testName);

            var dbEntity = cdDb.Genres.SingleOrDefault(x => x.Id == id);

            Assert.IsNotNull(dbEntity);

            cdDb.Genres.Remove(dbEntity);
            cdDb.SaveChanges();
        }

        [TestMethod]
        public void FindTest()
        {
            var id = genreService.Create(testName);
            var entity = genreService.Find(id);
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);

            var dbEntity = cdDb.Genres.SingleOrDefault(x => x.Id == id);
            cdDb.Genres.Remove(dbEntity);
            cdDb.SaveChanges();
        }

        [TestMethod]

        public void DeleteTest()
        {
            var id = genreService.Create(testName);
            try
            {
                var entity = genreService.Find(id);
                Assert.IsNotNull(entity);

                var success = genreService.Delete(entity);
                var entityDeleted = genreService.Find(id);

                Assert.IsNull(entityDeleted);
                Assert.IsTrue(success);
                
            }
            catch (Exception)
            {
                var dbEntity = cdDb.Genres.SingleOrDefault(x => x.Id == id);
                cdDb.Genres.Remove(dbEntity);
                cdDb.SaveChanges();
            }
        }

        [TestMethod]
        public void UpdateTest()
        {
            var id = genreService.Create(testName);
            try
            {
                var entity = genreService.Find(id);
                Assert.IsNotNull(entity);
                string nameUpdate = "testupdate";
                entity.Name = nameUpdate;

                genreService.Update(id, entity);

                var entityUpdate = genreService.Find(id);

                Assert.AreEqual(entityUpdate.Name, nameUpdate);

                genreService.Delete(entityUpdate);
            }
            catch (Exception)
            {
                var dbEntity = cdDb.Genres.SingleOrDefault(x => x.Id == id);
                cdDb.Genres.Remove(dbEntity);
                cdDb.SaveChanges();
            }
        }
    }
}
