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
        [AdminAcces]
        public ActionResult DeleteUser(string userName)
        {
            User userToDelete = DB.FindByUserName(userName);
            userToDelete.ToUserView().RemoveAvatar();
            List<Rating> lstRating = DB.Ratings.Where(r => r.UserId == userToDelete.Id).ToList();
            DB.Ratings.RemoveRange(lstRating);
            DB.Users.Remove(userToDelete);
            DB.SaveChanges();
            return RedirectToAction("UsersList"); 
        }
    }
}