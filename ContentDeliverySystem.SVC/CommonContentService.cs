using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
    public class CommonContentService : ContentService, IContentService, IDisposable
    {
        internal CommonContentService(CDSEntities DbEntities, ITokenService TokenService, AuthResponse Response, string UploadsFolder) :
            base(DbEntities, TokenService, Response, UploadsFolder) { }

        public override int CreateContent(IEnumerable<int> IdGroup, IEnumerable<int> IdsGenre, int IdType, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile)
        {
            throw new InvalidTokenException();
        }

        public override void UpdateContent(int Id, IEnumerable<int> IdGroup, IEnumerable<int> IdsGenre, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile)
        {
            if (!AuthResult.IsAdmin)
                throw new InvalidTokenException();
        }

        public override List<ContentListItem> GetContents(string BaseUrl)
        {
            if (!AuthResult.IsSuccess)
                throw new InvalidTokenException("Expired Token");
            var User = GetUser();
            var UserGroupActiveContent = CdDb.GroupContents.Where(x => x.IdGroup == User.IdGroup &&
                                                x.Contents.BeginDeliveryDate < DateTime.Now &&
                                                x.Contents.EndDeliveryDate > DateTime.Now).Select(x => x.Contents).ToList();

            UserGroupActiveContent.AddRange(CdDb.Contents.Where(x => x.IsBroadcast &&
                 x.BeginDeliveryDate < DateTime.Now && x.EndDeliveryDate > DateTime.Now));

            return BuildContentList(BaseUrl, UserGroupActiveContent);
        }



        public override Contents Find(int Id)
        {
            throw new InvalidTokenException();
        }
        public override void Delete(int Id)
        {
            throw new InvalidTokenException();
        } 
    }
}
