using ContentDeliverySystem.SVC;
using ContentDeliverySystem.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContentDeliverySystem.WEB.Controllers
{
    public class GenreController : Controller
    {
        //
        // GET: /Genre/

        IGenreService genreService;

        public GenreController(IGenreService genreService)
        {
            this.genreService = genreService;
        }

        public ActionResult Index()
        {
            try
            {
                var genres = genreService.GetAll();
                return View(genres);
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

        public ActionResult Insert()
        {
            ViewBag.Parents = genreService.GetAll();
            return View("insert");
        }

        [HttpPost]
        public ActionResult Create(string name,int? idParent)
        {
            try
            {
                genreService.Create(name, idParent);
                string Message = "Genre Created";
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
                var genre = genreService.Find(Id);
                ViewBag.Parents = genreService.GetAll();
                return View(genre);
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex);
                return RedirectToAction("index");
            }
        }

        public ActionResult Update(DAL.Genres genre, int id)
        {
            try
            {
                genreService.Update(id, genre);
                InternalNotification.Success("Genre updated");
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
                var genre = genreService.Find(Id);
                genreService.Delete(genre);
                InternalNotification.Success("Genre Deleted");
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
                genreService.Dispose();
        }

    }
}
