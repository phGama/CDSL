using ContentDeliverySystem.SVC;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContentDeliverySystem.WEB.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        IAuthService AuthService;
        IUserService UserService;
        public HomeController(IAuthService AuthService,IUserService UserService)
        {
            this.AuthService = AuthService;
            this.UserService = UserService;
        }

        

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserCreated()
        {
            return View();
        }

        public ActionResult Authenticate(string Email, string Password)
        {
            var AuthResult = AuthService.AuthUser(Email, Password, GlobalParams.ProjectType);
            if (AuthResult.IsSuccess)
            {
                Session[GlobalParams.UserSessionKey] = AuthResult;
                return RedirectToAction("Index", "Material");
            }
            else
            {
                InternalNotification.Subscribe(new AlertMessage()
                {
                    Message = "Email or Password incorrect",
                    MessageType = InternalNotificationType.error
                });
                return View("index");
            }

        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("index");
        }

        public ActionResult CreateUser(string cpf)
        {
            if(string.IsNullOrEmpty(cpf))
            {
                InternalNotification.Subscribe(new AlertMessage()
                {
                    Message = "CPF is requered",
                    MessageType = InternalNotificationType.error
                });
                return View("index");
            }

            if (UserService.Exists(cpf))
            {
                InternalNotification.Subscribe(new AlertMessage()
                {
                    Message = "There is already a user for this cpf",
                    MessageType = InternalNotificationType.error
                });
                return View("index");
            }
            return RedirectToAction("Insert","Users");
        }
    }
}
