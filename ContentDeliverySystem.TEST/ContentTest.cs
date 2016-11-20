using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContentDeliverySystem.SVC;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.DAL;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ContentDeliverySystem.TEST
{
    [TestClass]
    public class ContentTest
    {
        int[] GroupIds = { 1 };
        int[] IdsGenre = { 1 };
        string FileName = "Important Stuff";
        string Description = "This is what is important, because reasons";
        int IdType = 1;
        List<HttpFileStream> UploadedFiles = new List<HttpFileStream>();
        DateTime BeginDate = DateTime.Now;
        DateTime EndDate = DateTime.Now.AddDays(5);
        string BaseUploadPath = AppDomain.CurrentDomain.BaseDirectory + "\\Content\\";

        [TestMethod]
        public void CreateTest()
        {
            int LastContentId = 0;
            var ContentService = GetAdminService();
            using (CDSEntities CdDb = new CDSEntities())
            {
                if (CdDb.Contents.Any())
                    LastContentId = CdDb.Contents.Max(x => x.Id);
                FileName = FileName + LastContentId;

                
                var CreatedId = ContentService.CreateContent(GroupIds,IdsGenre,IdType, FileName, Description, UploadedFiles, BeginDate, EndDate,false,null);
                Assert.IsTrue(CreatedId > LastContentId);
                Assert.IsTrue(CreatedId > 1);
                Assert.IsTrue(CdDb.GroupContents.Count(x => x.IdContent == CreatedId) == GroupIds.Count());

                var ResultFileName = CdDb.Contents.SingleOrDefault(x => x.Id == CreatedId).FileName;
                Assert.IsTrue(File.Exists(BaseUploadPath + ResultFileName));

            }

        }

        

        [TestMethod]
        [ExpectedException(typeof(InvalidTokenException))]
        public void CreateCommonExceptionTest()
        {
            var ContentService = GetCommonService();
            ContentService.CreateContent(GroupIds,IdsGenre, IdType, FileName, Description, UploadedFiles, BeginDate, EndDate,false);
        }

      

        [TestMethod]
        [ExpectedException(typeof(InvalidTokenException))]
        public void UpdateContentExceptionTest()
        {
            var ContentService = GetCommonService();
            ContentService.UpdateContent(1, GroupIds, IdsGenre, FileName, Description, UploadedFiles, BeginDate, EndDate, false);
        }

        [TestMethod]
        public void UpdateTest()
        {
            
            int LastContentId = 0;
            var ContentService = GetAdminService();

            using (CDSEntities CdDb = new CDSEntities())
            {
                if (CdDb.Contents.Any())
                    LastContentId = CdDb.Contents.Max(x => x.Id);
                FileName = FileName + LastContentId;
                var CreatedId = ContentService.CreateContent(GroupIds, IdsGenre, IdType, FileName, Description, UploadedFiles, BeginDate, EndDate, false);

                ContentService.UpdateContent(CreatedId, GroupIds, IdsGenre, FileName + "D", Description + "D", UploadedFiles, DateTime.Today, DateTime.Today.AddDays(1), true);

                var DbContent = CdDb.Contents.SingleOrDefault(x => x.Id == CreatedId);

                Assert.IsTrue(DbContent.Name == FileName + "D");
                Assert.IsTrue(DbContent.Description == Description + "D");
                Assert.IsTrue(DbContent.BeginDeliveryDate == DateTime.Today);
                Assert.IsTrue(DbContent.EndDeliveryDate == DateTime.Today.AddDays(1));
                Assert.IsTrue(DbContent.IsBroadcast == true);


                CdDb.GroupContents.RemoveRange(CdDb.GroupContents.Where(x => x.IdContent == CreatedId));
                CdDb.SaveChanges();
                DbContent.Genres.Clear();
                CdDb.Contents.Remove(CdDb.Contents.SingleOrDefault(x => x.Id == CreatedId));
                CdDb.SaveChanges();
            }
        }

        [TestMethod]
        public void GetCommunContents()
        {
            var ContentService = GetCommonService();
            var UserContents = ContentService.GetContents("");
            using (CDSEntities CdDb = new CDSEntities())
            {
                var User = CdDb.Users.First(x=> x.IdType == 2);
                var DbUserContents = CdDb.GroupContents.Where(x => x.IdGroup == User.IdGroup &&
                                                x.Contents.BeginDeliveryDate < DateTime.Now &&
                                                x.Contents.EndDeliveryDate > DateTime.Now).Select(x => x.Contents).ToList();
                DbUserContents.AddRange(CdDb.Contents.Where(x => x.IsBroadcast &&
                 x.BeginDeliveryDate < DateTime.Now && x.EndDeliveryDate > DateTime.Now));
                Assert.IsTrue(UserContents.Count == DbUserContents.Count());
                Assert.IsFalse(UserContents.Any(x => string.IsNullOrEmpty(x.DownloadLink)));
            }
        }

        [TestMethod]
        public void GetAdminContents()
        {
            var ContentService = GetAdminService();
            var UserContents = ContentService.GetContents("");
            using (CDSEntities CdDb = new CDSEntities())
            {
                var DbUserContents = CdDb.Contents.Where(x => x.BeginDeliveryDate < DateTime.Now &&
                    x.EndDeliveryDate > DateTime.Now);

                Assert.IsTrue(UserContents.Count == DbUserContents.Count());
                Assert.IsFalse(UserContents.Any(x => string.IsNullOrEmpty(x.DownloadLink)));
            }
        }
        private IContentService GetAdminService()
        {
            string CodeAdmin = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {

                CodeAdmin = TokenService.CreateToken(TokenType.WebAuth, 1);
            }
            var ContentService = ServiceFinder.GetContentService(CodeAdmin, TokenType.WebAuth, BaseUploadPath);
            return ContentService;
        }

        private IContentService GetCommonService()
        {
            int Id = 0;
            using (CDSEntities db = new CDSEntities()){
                Id = db.Users.First(x => x.IdType == 2).Id;
            }
            string CodeCommon = string.Empty;
            using (var TokenService = ServiceFinder.GetTokenService())
            {
                CodeCommon = TokenService.CreateToken(TokenType.WebAuth, Id);

            }
            var ContentService = ServiceFinder.GetContentService(CodeCommon, TokenType.WebAuth, BaseUploadPath);
            return ContentService;
        }
    }
}
