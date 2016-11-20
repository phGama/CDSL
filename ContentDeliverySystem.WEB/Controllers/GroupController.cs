using ContentDeliverySystem.SVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContentDeliverySystem.WEB.Models;

namespace ContentDeliverySystem.WEB.Controllers
{
    public class GroupController : Controller
    {
        //
        // GET: /Group/

        IGroupService GroupService;

        public GroupController(IGroupService Service)
        {
            GroupService = Service;
        }

        public ActionResult Index()
        {
            try
            {
                var Groups = GroupService.GetAll();
                return View(Groups);
            }
            catch (InvalidTokenException)
            {
                return RedirectToAction("index","home");
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Insert()
        {
            return View("insert");
        }
        [HttpPost]
        public ActionResult Create(string Name)
        {
            try
            {
                GroupService.Create(Name);
                string Message  = "Group Created";
                InternalNotification.Success(Message);
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
            }
            return RedirectToAction("index");
        }
        
        public ActionResult Edit(int Id)
        {
            try
            {
                var Group = GroupService.Find(Id);
                return View(Group);
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
                return RedirectToAction("index");
            }
        }

        public ActionResult Update(int Id, string Name, bool IsActive=false)
        {
            try
            {
                GroupService.Update(Id, Name, IsActive);
                InternalNotification.Success("Group updated");
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
            }
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            try
            {
                GroupService.Delete(Id);
                InternalNotification.Success("Group Deleted");
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
            }
            return RedirectToAction("index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                GroupService.Dispose();
        }

    }
}
