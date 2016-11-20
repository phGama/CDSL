using ContentDeliverySystem.WEB.Models;
using ContentDeliverySystem.SVC;
using ContentDeliverySystem.WEB.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContentDeliverySystem.WEB.Models
{
    public class CDSControllersFactory:DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            try
            {

                if (controllerType == typeof(HomeController))
                    return new HomeController(CDSDependencyResolver.GetService<IAuthService>(requestContext),
                        CDSDependencyResolver.GetService<IUserService>(requestContext));
                if (controllerType == typeof(MaterialController))
                    return new MaterialController(CDSDependencyResolver.GetService<IContentService>(requestContext), 
                        CDSDependencyResolver.GetService<IGroupService>(requestContext));
                if (controllerType == typeof(GroupController))
                    return new GroupController(CDSDependencyResolver.GetService<IGroupService>(requestContext));
                if (controllerType == typeof(UsersController))
                    return new UsersController(CDSDependencyResolver.GetService<IUserService>(requestContext),
                        CDSDependencyResolver.GetService<IGroupService>(requestContext));
                if (controllerType == typeof(GenreController))
                    return new GenreController(CDSDependencyResolver.GetService<IGenreService>(requestContext));
                else
                    return base.GetControllerInstance(requestContext, controllerType);
            }
            catch
            {
                return null;// base.GetControllerInstance(requestContext, controllerType);
            }
            
        }
    }
}