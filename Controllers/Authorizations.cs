using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoviesManager.Models;

namespace MoviesManager.Controllers
{
    public class UserAcces : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (OnlineUsers.GetSessionUser() != null)
                return true;
            else
                httpContext.Response.Redirect("/Users/Login");
            return base.AuthorizeCore(httpContext);
        }
    }
    public class AdminAcces : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            User sessionUser = OnlineUsers.GetSessionUser();
            if (sessionUser != null)
                if(sessionUser.Admin)
                    return true;
            else
                httpContext.Response.Redirect("/Users/Login");
            return base.AuthorizeCore(httpContext);
        }
    }
}