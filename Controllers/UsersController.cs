using MoviesManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoviesManager.Controllers
{
    public class UsersController : Controller
    {
        private DBEntities DB = new DBEntities();

        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
        //Creer la vue sbscribe
        public ActionResult Subscribe()
        {
            return View();
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
        public ActionResult Index()
        {
            User currentUser = OnlineUsers.GetSessionUser();
            ViewBag.currentUser = currentUser;
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginView loginView)
        {
            if (ModelState.IsValid)
            {
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
            }
            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            User currentUser = OnlineUsers.GetSessionUser();
            if (currentUser != null)
            {
                ReversiGame game = Games.Find(currentUser.Id);
                OnlineUsers.RemoveSessionUser();

                if (game != null)
                {
                    game.PlayersStillOnLine();
                }
            }
            return RedirectToAction("Login");
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

                
            }
            return RedirectToAction("Index");
        }
       
    }
}