using ContentDeliverySystem.SVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContentDeliverySystem.WEB.Models;

namespace ContentDeliverySystem.WEB.Controllers
{
    public class UsersController : Controller
    {

        IUserService UserService;
        IGroupService GroupService;
        public UsersController(IUserService Service, IGroupService GroupService)
        {
            UserService = Service;
            this.GroupService = GroupService;
        }

        public ActionResult Index()
        {
            return View(UserService.GetUsers());
        }

        public ActionResult Active(int ticket)
        {
            try
            {
                UserService.ActivateUser(ticket);
                return View("Activated");
            }
            catch
            {
                return View("ActivationError");
            }
        }

        public ActionResult Insert()
        {
            try
            {
                ViewBag.Groups = GroupService.GetAll().Where(x => x.Active);
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
            ViewBag.Groups = GroupService.GetAll().Where(x => x.Active);
            var User = UserService.Find(Id);
            return View(User);
        }

        public ActionResult ResendActivationEmail(string Email)
        {
            try
            {
                UserService.ResendActiveEmail(Email, GetActiveEndPoint());
                InternalNotification.Success("The email was sent");
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex.Message);
            }
            return RedirectToAction("index");
        }

        public ActionResult Create(string Adress, string Name, DateTime BirthDate, string Cellphone, string CEP, string CPF, string Email, byte? Gender, int IdGroup, string Phone, string Password, string PasswordConfirmation)
        {

            if (Password != PasswordConfirmation)
            {
                InternalNotification.Error("Password does not match");
                RedirectToAction("insert");
            }

            try
            {
                var CreatedId = UserService.Create(new DAL.Users()
                {
                    Name = Name,
                    Adress = Adress,
                    BirthDate = BirthDate,
                    Cellphone = Cellphone,
                    CEP = CEP,
                    CPF = CPF,
                    Email = Email,
                    Gender = Gender,
                    IdGroup = IdGroup,
                    Phone = Phone
                }, Password, GetActiveEndPoint());

                if (CreatedId < 1)
                    throw new Exception("Could not Create User");

                //InternalNotification.Success("User created");
                return RedirectToAction("UserCreated", "Home");
            }
            catch (Exception)
            {
                InternalNotification.Error("Ocorreu um erro inesperado, por favor tente mais tarde");
                return RedirectToAction("insert");
            }
        }

        public ActionResult Update(int Id, string Adress, string Name, DateTime BirthDate, string Cellphone, string CEP, byte? Gender, int IdGroup, string Phone)
        {
            try
            {
                UserService.Update(new DAL.Users()
               {
                   Id = Id,
                   Adress = Adress,
                   Name = Name,
                   BirthDate = BirthDate,
                   Cellphone = Cellphone,
                   CEP = CEP,
                   Gender = Gender,
                   IdGroup = IdGroup,
                   Phone = Phone
               });


                InternalNotification.Success("User updated");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex.Message);
                RedirectToAction("insert");
            }
            return RedirectToAction("index");
        }

        public ActionResult Delete(int Id)
        {
            try
            {
                var Deleted = UserService.Delete(Id);
                InternalNotification.Success("User deleted");
            }
            catch (Exception ex)
            {
                InternalNotification.Error(ex.Message);
            }
            return RedirectToAction("index");
        }
        private string GetActiveEndPoint()
        {
            return Url.Action("active", "users",
               routeValues: null,
               protocol: Request.Url.Scheme);
        }
    }
}
