using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoviesManager.Models;

namespace MoviesManager.Controllers
{
    public class AdminController : Controller
    {
        private DBEntities DB = new DBEntities();


        public DateTime LastOnlineUsersUpdate
        {
            get
            {
                if (Session["LastOnlineUsersUpdate"] == null)
                    Session["LastOnlineUsersUpdate"] = new DateTime(0);
                return (DateTime)Session["LastOnlineUsersUpdate"];
            }
            set
            {
                Session["LastOnlineUsersUpdate"] = value;
            }
        }

        [AdminAcces]
        public ActionResult UsersList()
        {
            User[] allUsers = Extension.getAllUsers(DB);
            User currentUser = OnlineUsers.GetSessionUser();
            if (currentUser != null)
            {
                ViewBag.CurrentUserId = currentUser.Id;
                return View(allUsers);
            }
            return null;
        }
    }
}