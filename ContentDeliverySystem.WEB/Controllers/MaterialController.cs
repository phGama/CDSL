using ContentDeliverySystem.SVC;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.WEB.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContentDeliverySystem.WEB.Controllers
{
    public class MaterialController : Controller
    {
        //
        // GET: /Material/
        IContentService ContentService;
        IGroupService GroupService;

        public MaterialController(IContentService Service, IGroupService GroupService)
        {
            ContentService = Service;
            this.GroupService = GroupService;
        }

        public ActionResult Index()
        {
            try
            {
                var FileBaseUrl = Url.Content("~/Material/File/");
                var Contents = ContentService.GetContents(FileBaseUrl);
                ViewBag.SubTitle = "Materiais Sugeridos";
                ViewBag.IsAdmin = ((SVC.DTO.AuthResponse)Session[WEB.Models.GlobalParams.UserSessionKey]).IsAdmin;
                return View(Contents);
            }
            catch (InvalidTokenException)
            {
                return RedirectToAction("index", "home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Books()
        {
            int IdType = 1;
            ViewBag.SubTitle = "Livros";
            return GetPublicContents(IdType);
        }

        public ActionResult Articles()
        {
            int IdType = 2;
            ViewBag.SubTitle = "Artigos";
            return GetPublicContents(IdType);
        }

        public ActionResult Podcasts()
        {
            int IdType = 3;
            ViewBag.SubTitle = "Podcasts";
            return GetPublicContents(IdType);
        }

        private ActionResult GetPublicContents(int IdType)
        {
            try
            {
                var FileBaseUrl = Url.Content("~/Material/File/");
                var Contents = ContentService.GetPublicContents(FileBaseUrl, IdType);
                ViewBag.IsAdmin = ((SVC.DTO.AuthResponse)Session[WEB.Models.GlobalParams.UserSessionKey]).IsAdmin;
                return View("Index",Contents);
            }
            catch (InvalidTokenException)
            {
                return RedirectToAction("index", "home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileResult File(string ticket)
        {
            var ResourceFile = ContentService.GetFile(ticket);
            return new FileStreamResult(ResourceFile, "application/zip");
        }

        public ActionResult Insert()
        {
            try
            {
                var Groups = GroupService.GetAll().Where(x => x.Active == true);
                var JsonSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                ViewBag.Groups = JsonConvert.SerializeObject(Groups, JsonSettings);
                return View();
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
                return RedirectToAction("index");
            }
        }
        public ActionResult Edit(int Id)
        {
            try
            {
                var Content = ContentService.Find(Id);
                var Groups = GroupService.GetAll().Where(x => x.Active == true);
                var SelectedGroupIds = Content.GroupContents.Select(x => x.IdGroup);
                var JsonSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                ViewBag.Groups = JsonConvert.SerializeObject(Groups, JsonSettings);
                ViewBag.SelectedGroupIds = JsonConvert.SerializeObject(SelectedGroupIds, JsonSettings);
                return View(Content);
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
                return RedirectToAction("index");
            }
        }



        public ActionResult Delete(int Id)
        {
            try
            {
                ContentService.Delete(Id);
                string Message = "Material deleted";
                InternalNotification.Success(Message);

            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
            }
            return RedirectToAction("index");
        }

        public ActionResult Create(string Groups,int Type, string Name, string Description, IEnumerable<HttpPostedFileBase> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast = false, HttpPostedFileBase imageFile = null)
        {
            try
            {
                var IdsGroup = GetIdsFromString(Groups);

                var FilesDTO = GetFiles(Files);
                HttpFileStream imageFileStream = null;
                if (imageFile != null)
                {
                    imageFileStream = new HttpFileStream()
                 {
                     InputStream = imageFile.InputStream,
                     Name = imageFile.FileName
                 };
                }
                ContentService.CreateContent(IdsGroup, null, Type, Name, Description, FilesDTO, BeginDate, EndDate, IsBroadcast, imageFileStream);
                InternalNotification.Success("Material Created");

            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
            }
            return RedirectToAction("index");
        }

        public ActionResult Update(int Id,string Groups, string Name, string Description, IEnumerable<HttpPostedFileBase> Files, DateTime BeginDate, DateTime EndDate, bool IsBroadcast = false)
        {
            try
            {
                var IdsGroup = GetIdsFromString(Groups);
                var idsTypes = GetIdsFromString(Groups);
                var FilesDTO = GetFiles(Files);
                ContentService.UpdateContent(Id,IdsGroup,idsTypes, Name, Description, FilesDTO, BeginDate, EndDate, IsBroadcast);
                InternalNotification.Success("Material Updated");

            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
            }
            return RedirectToAction("index");
        }


        private static List<int> GetIdsFromString(string Groups)
        {
            var IdsGroup = new List<int>();
            var GroupsSplit = Groups.Split(',');

            foreach (var id in GroupsSplit) {
                int Id = 0;
                if(int.TryParse(id,out Id))
                    IdsGroup.Add(Convert.ToInt32(Id));
            }
            return IdsGroup;
        }
        private static List<HttpFileStream> GetFiles(IEnumerable<HttpPostedFileBase> Files)
        {
            var FilesDTO = new List<HttpFileStream>();
            foreach (var item in Files)
            {
                if (item == null)
                    continue;
                FilesDTO.Add(new HttpFileStream()
                {
                    Name = item.FileName,
                    InputStream = item.InputStream
                });
            }
            return FilesDTO;
        }


    }
}
