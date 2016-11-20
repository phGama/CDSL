using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System.Globalization;

namespace ContentDeliverySystem.SVC
{
    public abstract class ContentService : IContentService, IDisposable
    {
        protected string UploadsFolderPath;
        private ITokenService TokenService;
        protected AuthResponse AuthResult;
        protected CDSEntities CdDb;

        internal ContentService(CDSEntities DbEntities, ITokenService TokenService, AuthResponse Response, string UploadsFolder)
        {
            this.TokenService = TokenService;
            this.CdDb = DbEntities;
            this.AuthResult = Response;
            this.UploadsFolderPath = UploadsFolder;
        }

        internal static IContentService Create(CDSEntities DbEntities, ITokenService TokenService, AuthResponse Response, string UploadsFolder)
        {
            if (Response.IsAdmin)
            {
                return new AdminContentService(DbEntities, TokenService, Response, UploadsFolder);
            }
            else
            {
                return new CommonContentService(DbEntities, TokenService, Response, UploadsFolder);
            }
        }

        public abstract int CreateContent(IEnumerable<int> IdGroup,IEnumerable<int> IdsGenre,int IdType, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile = null);

        public abstract void UpdateContent(int Id, IEnumerable<int> IdGroup, IEnumerable<int> IdsGenre, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile = null);

        public abstract List<ContentListItem> GetContents(string BaseUrl);

        public List<ContentListItem> GetPublicContents(string BaseUrl, int IdType)
        {
            if (!AuthResult.IsSuccess)
                throw new InvalidTokenException("Expired Token");

            var UserContent = CdDb.Contents.Where(x => x.IdType == IdType &&
                                                x.BeginDeliveryDate < DateTime.Now &&
                                                x.EndDeliveryDate > DateTime.Now).ToList();

            return BuildContentList(BaseUrl, UserContent);
        }

       
        protected List<ContentListItem> BuildContentList(string BaseUrl, IEnumerable<Contents> UserGroupActiveContent)
        {
            var UserContents = new List<ContentListItem>();
            foreach (var GroupContent in UserGroupActiveContent)
            {
                UserContents.Add(new ContentListItem()
                {
                    Description = GroupContent.Description,
                    Id = GroupContent.Id,
                    Name = GroupContent.Name,
                    DownloadLink = GenerateDownloadLink(GroupContent, BaseUrl),
                    Image = GetImage(GroupContent),
                    Type = GroupContent.ContentTypes.Name
                });
            }
            return UserContents;
        }

        protected Users GetUser()
        {
            return CdDb.Users.SingleOrDefault(x => x.Email == AuthResult.Email);
        }
        protected bool HasPermission(int IdUser, int IdContent)
        {
            var User = CdDb.Users.SingleOrDefault(x => x.Id == IdUser);
            var IdGroup = User.IdGroup;
            var HasGroupPermission = false;
            var Content = CdDb.Contents.SingleOrDefault(x => x.Id == IdContent);
            if (IdGroup.HasValue)
                HasGroupPermission = CdDb.GroupContents.Any(x => x.IdGroup == IdGroup && x.IdContent == IdContent);

            return User.IdType == 1 || HasGroupPermission || Content.IsBroadcast;
        }

        public abstract Contents Find(int Id);
        public abstract void Delete(int Id);

        public FileStream GetFile(string Token)
        {
            var PureToken = Token.Substring(0, 32);
            var UserId = int.Parse(Token.Substring(32), NumberStyles.HexNumber);
            if (!TokenService.IsValidToken(PureToken, DTO.TokenType.ApiContent) &&
                !TokenService.IsValidToken(PureToken, DTO.TokenType.WebContent))
                throw new InvalidOperationException("Invalid or expired Token");
            var TokenObject = CdDb.Tokens.SingleOrDefault(x => x.Code == PureToken);
            var Content = CdDb.Contents.SingleOrDefault(x => x.Id == TokenObject.IdContent);

            //if (!HasPermission(UserId, Content.Id))
            //    throw new InvalidOperationException("You doesn't have permission to access this file");

            var FullFileName = UploadsFolderPath + Content.FileName;

            if (!File.Exists(FullFileName))
                throw new FileNotFoundException();

            return File.OpenRead(FullFileName);

        }


        protected string GenerateDownloadLink(Contents Content, string BaseUrl)
        {
            var IdUser = GetUser().Id;
            var ResponseTokenType = GetContentTokenType();
            var Ticket = TokenService.CreateToken(ResponseTokenType, IdUser, Content.Id);
            return BaseUrl + "?ticket=" + Ticket + IdUser.ToString("x");
        }

        public void Dispose()
        {
            if (CdDb != null)
                CdDb.Dispose();
            if (TokenService != null)
                TokenService.Dispose();
        }

        private TokenType GetContentTokenType()
        {
            var AccesTokenType = TokenService.GetTokenType(AuthResult.Token);
            var ResponseTokenType = AccesTokenType == TokenType.WebAuth ? TokenType.WebContent : TokenType.ApiContent;
            return ResponseTokenType;
        }
        private List<ContentListItem> BuildContentList(string BaseUrl, List<Contents> UserGroupActiveContent)
        {
            var UserContents = new List<ContentListItem>();
            foreach (var GroupContent in UserGroupActiveContent)
            {
                UserContents.Add(new ContentListItem()
                {
                    Description = GroupContent.Description,
                    Id = GroupContent.Id,
                    Name = GroupContent.Name,
                    Type = GroupContent.ContentTypes.Name,
                    DownloadLink = GenerateDownloadLink(GroupContent, BaseUrl),
                    Image = GetImage(GroupContent)
                });
            }
            return UserContents;
        }

        protected string GetImage(Contents content)
        {
            if(string.IsNullOrEmpty(content.ImageName))
                return "nocover.jpg";
            return content.ImageName;
        }
    }
}
