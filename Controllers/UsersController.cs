using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoviesManager.Models;

namespace MoviesManager.Controllers
{
    public class UsersController : Controller
    {
        private DBEntities DB = new DBEntities();

        public ActionResult Subscribe()
        {
            return View(new UserView());
        }
        [HttpPost]
        public ActionResult Subscribe(UserView userView)
        {
            if (ModelState.IsValid)
            {
                if (DB.UserExists(userView.UserName))
                {
                    ModelState.AddModelError("UserName", "Ce nom d'usager existe deja");
                    return View(userView);
                }
                userView.Password = userView.NewPassword;
                DB.AddUser(userView);

                return RedirectToAction("Login");
            }
            return View(userView);
        }
        public ActionResult Login()
        {
            return View(new LoginView());
        }
        [HttpPost]
        public ActionResult Login(LoginView loginView)
        {
            //if (ModelState.IsValid)
            //{
                User userFound = DB.FindByUserName(loginView.UserName);
                if (userFound != null)
                {
                    if (userFound.Password != loginView.Password)
                    {
                        ModelState.AddModelError("Password", "Mot de passe incorrect");
                        return View(loginView);
                    }
                }
                else
                {
                    ModelState.AddModelError("UserName", "ce nom d'usager n'existe pas");
                    return View(loginView);
                }
                OnlineUsers.AddSessionUser(userFound);
            //}
            return RedirectToAction("Profil");
        }
        [UserAcces]
        public ActionResult Profil()
        {
            UserView userView = OnlineUsers.GetSessionUser().ToUserView();
            ViewBag.PasswordChangeToken = Guid.NewGuid().ToString();
            return View(userView);
        }
        [HttpPost]
        public ActionResult Profil(UserView userView)
        {
            if (ModelState.IsValid)
            {

                string PasswordChangeToken = (string)Request["PasswordChangeToken"];
                User user = DB.FindByUserName(OnlineUsers.Find(userView.Id).UserName);
                if (userView.NewPassword.Equals(PasswordChangeToken))
                {
                    userView.Password = user.Password;
                }
                else
                {
                    userView.Password = userView.NewPassword;
                }
                DB.UpdateUser(userView);
                userView.CopyToUser(OnlineUsers.GetSessionUser());
            }
            return RedirectToAction("Profil");
        }
        public ActionResult Logout()
        {
            User currentUser = OnlineUsers.GetSessionUser();
            if (currentUser != null)
            {
                OnlineUsers.RemoveSessionUser();
            }
            return RedirectToAction("Login");
        }
    }
}