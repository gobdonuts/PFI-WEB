using MoviesManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoviesManager.Controllers
{
    public class ActorsController : Controller
    {
        private DBEntities DB = new DBEntities();

        public ActionResult Index()
        {
            return View(DB.ActorsList());
        }

        public ActionResult ActorsList(List<ActorView> actors)
        {
            return PartialView(actors);
        }
        [AdminAcces]
        public ActionResult Create()
        {
            ViewBag.SelectedFilms = new List<ListItem>();
            ViewBag.Films = DB.FilmListItems();
            SelectList countries = new SelectList(DB.CountryListItems(),"Id","Text",0,new[] { -1 });
            ViewBag.Countries = countries;
            return View(new ActorView());
        }

        [HttpPost]
        public ActionResult Create(ActorView actorView, List<int> SelectedItems/*Liste des Id des films sélectionés*/)
        {
            if (ModelState.IsValid)
            {
                if (actorView.Name == null || actorView.Name.Trim() == "")
                {
                    ModelState.AddModelError("Name", "");
                    return View(actorView);
                }
                if (actorView.Sexe < 0 || actorView.Sexe > 2)
                {
                    ModelState.AddModelError("Sexe", "");
                }
                DB.AddActor(actorView, SelectedItems);
                return RedirectToAction("Index");
            }
            return View(actorView);
           
        }
        [UserAcces]
        public ActionResult Details(int id)
        {
            Actor actor = DB.Actors.Find(id);
            ViewBag.Films = actor.FilmsList();
            Models.Country pays = DB.Countries.Find(actor.CountryId);
            if (pays != null)
                ViewBag.Country = pays;
            else
            {
                ViewBag.Country = new Models.Country();
                ViewBag.Country.Name = "Inconnu";
            }
            return View(actor.ToActorView());
        }
        [AdminAcces]
        public ActionResult Edit(int id)
        {
            Actor actor = DB.Actors.Find(id);
            ViewBag.SelectedFilms = actor.FilmListItems();
            ViewBag.Films = DB.FilmListItems();
            SelectList countries = new SelectList(DB.CountryListItems(), "Id", "Text", actor.CountryId, new[] { -1 });
            ViewBag.Countries = countries;
            return View(actor.ToActorView());
        }

        [HttpPost]
        public ActionResult Edit(ActorView actorView, List<int> SelectedItems/*Liste des Id des films sélectionés*/)
        {
            DB.UpdateActor(actorView, SelectedItems);
            return RedirectToAction("Index");
        }
        [AdminAcces]
        public ActionResult Delete(int id)
        {
            return View(DB.Actors.Find(id).ToActorView());
        }

        [HttpPost]
        public ActionResult Delete(ActorView actorView)
        {
            DB.RemoveActor(actorView);
            return RedirectToAction("Index");
        }
    }
}
