using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System;
using System.Collections.Generic;
using System.IO;
namespace ContentDeliverySystem.SVC
{
    public interface IContentService:IDisposable
    {
        int CreateContent(IEnumerable<int> IdGroup, IEnumerable<int> IdsGenre, int IdType, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile = null);
        List<ContentListItem> GetContents(string BaseUrl);

        List<ContentListItem> GetPublicContents(string BaseUrl, int IdType);
        System.IO.FileStream GetFile(string Token);
        void UpdateContent(int Id, IEnumerable<int> IdGroup, IEnumerable<int> IdsGenre, string Name, string Description, IEnumerable<HttpFileStream> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast, HttpFileStream imageFile = null);

        Contents Find(int Id);

        void Delete(int Id);
    }
}
