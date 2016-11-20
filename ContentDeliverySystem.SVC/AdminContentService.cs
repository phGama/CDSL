using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
    public class AdminContentService : ContentService
    {
        internal AdminContentService(CDSEntities DbEntities, ITokenService TokenService, AuthResponse Response, string UploadsFolder) :
            base(DbEntities, TokenService, Response, UploadsFolder) { }

        public override int CreateContent(IEnumerable<int> IdGroup, IEnumerable<int> IdsGenre, int IdType, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile = null)
        {
            if (EndDate < DateTime.Now.AddMinutes(1))
                throw new ArgumentException("End Date must be in the future");
            string FileName = SaveFiles(Name, Files);
            var Content = new Contents()
            {
                FileName = FileName,
                Name = Name,
                Description = Description,
                BeginDeliveryDate = BeginDate,
                EndDeliveryDate = EndDate,
                CreatedAt = DateTime.Now,
                IsBroadcast = IsBroadcast,
                IdType = IdType
            };
            if (imageFile != null)Content.ImageName = SaveImage(imageFile);
            if (IdsGenre != null)
            {
                foreach (var idGenre in IdsGenre)
                {
                    var genre = CdDb.Genres.FirstOrDefault(x => x.Id == idGenre);
                    if (genre != null) Content.Genres.Add(genre);
                }
            }

            CdDb.Contents.Add(Content);
            CdDb.SaveChanges();

            SaveGroupContents(IdGroup, Content);

            return Content.Id;
        }

        public override void UpdateContent(int Id, IEnumerable<int> IdGroup, IEnumerable<int> IdsGenre, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile = null)
        {
            if (EndDate < DateTime.Now.AddMinutes(1))
                throw new ArgumentException("End Date must be in the future");
            var Content = CdDb.Contents.SingleOrDefault(x => x.Id == Id);
            if (Content == null)
                throw new ArgumentException("Id Couldn't be find");
            DeleteOldFile(Content);
            ClearContentGroup(Content);

            Content.FileName = SaveFiles(Name, Files);
            if (imageFile != null)
                Content.ImageName = SaveImage(imageFile);
            Content.Name = Name;
            Content.Description = Description;
            Content.BeginDeliveryDate = BeginDate;
            Content.EndDeliveryDate = EndDate;
            Content.IsBroadcast = IsBroadcast;

            var genreIdsToAdd = IdsGenre.Where(x => !Content.Genres.Any(y => y.Id == x));
            var genresToDelete = Content.Genres.Where(x => !IdsGenre.Contains(x.Id));

            foreach (var genre in genresToDelete)
            {
                Content.Genres.Remove(genre);
            }

            foreach (var id in genreIdsToAdd)
            {
                var genre = CdDb.Genres.FirstOrDefault(x => x.Id == id);
                if (genre != null) Content.Genres.Add(genre);
            }

            CdDb.SaveChanges();

            SaveGroupContents(IdGroup, Content);
        }

        public override List<ContentListItem> GetContents(string BaseUrl)
        {
            var ActiveContent = CdDb.Contents.Where(x => x.BeginDeliveryDate < DateTime.Now &&
                                                x.EndDeliveryDate > DateTime.Now);
            
            var ReturnContents = new List<ContentListItem>();
            foreach (var GroupContent in ActiveContent)
            {
                ReturnContents.Add(new ContentListItem()
                {
                    Description = GroupContent.Description,
                    Id = GroupContent.Id,
                    Name = GroupContent.Name,
                    Type = GroupContent.ContentTypes.Name,
                    DownloadLink = GenerateDownloadLink(GroupContent, BaseUrl),
                    Image = GetImage(GroupContent),
                });
            }
            return BuildContentList(BaseUrl,ActiveContent);
        }

        public override Contents Find(int Id)
        {
            var DbContent = CdDb.Contents.SingleOrDefault(x => x.Id == Id);
            if (DbContent == null)
                throw new ArgumentException("Couldn't find resource with specified ID");
            return DbContent;
        }

        public override void Delete(int Id)
        {
            var Content = this.Find(Id);
            CdDb.GroupContents.RemoveRange(CdDb.GroupContents.Where(x => x.IdContent == Id));
            CdDb.Tokens.RemoveRange(CdDb.Tokens.Where(x => x.IdContent == Id));
            CdDb.SaveChanges();

            DeleteOldFile(Content);

            CdDb.Contents.Remove(this.Find(Id));
            CdDb.SaveChanges();


        }

        private string SaveFiles(string Name, IEnumerable<HttpFileStream> Files)
        {
            string FileName = string.Empty;

            if (!Directory.Exists(UploadsFolderPath))
                Directory.CreateDirectory(UploadsFolderPath);
            ZipFile zip = new ZipFile();
            foreach (var File in Files)
            {
                zip.AddEntry(File.Name,File.InputStream);
            }
            FileName = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + Name.Replace(" ", "") + ".zip";
            zip.Save(UploadsFolderPath + FileName);
            return FileName;
        }

        private string SaveImage(HttpFileStream file)
        {
            var imageDir = Path.Combine(UploadsFolderPath,"banners");
            if (!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);
            var fileNameTimestamp = DateTime.Now.ToString("yyyymmdd") + file.Name;
            using (var diskFile = File.Create(imageDir + fileNameTimestamp))
            {
                file.InputStream.Seek(0, SeekOrigin.Begin);
                file.InputStream.CopyTo(diskFile);
            }
            return fileNameTimestamp;
            
        }

        private void SaveGroupContents(IEnumerable<int> IdGroup, Contents Content)
        {
            foreach (var Id in IdGroup)
            {
                CdDb.GroupContents.Add(new GroupContents()
                {
                    IdContent = Content.Id,
                    IdGroup = Id,
                    CreatedAt = DateTime.Now
                });

                CdDb.SaveChanges();
            }
        }

        private void DeleteOldFile(Contents Content)
        {
            var FullFileName = UploadsFolderPath + Content.FileName;
            if (File.Exists(FullFileName))
                File.Delete(FullFileName);
        }

        private void ClearContentGroup(Contents Content)
        {
            CdDb.GroupContents.RemoveRange(CdDb.GroupContents.Where(x => x.IdContent == Content.Id));
            CdDb.SaveChanges();
        }

    }
}
