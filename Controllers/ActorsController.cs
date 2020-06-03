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

        public ActionResult Create()
        {
            ViewBag.SelectedFilms = new List<ListItem>();
            ViewBag.Films = DB.FilmListItems();
            return View(new ActorView());
        }

        [HttpPost]
        public ActionResult Create(ActorView actorView, List<int> SelectedItems/*Liste des Id des films sélectionés*/)
        {
            DB.AddActor(actorView, SelectedItems);
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Actor actor = DB.Actors.Find(id);
            ViewBag.Films = actor.FilmsList();
            return View(actor.ToActorView());
        }

        public ActionResult Edit(int id)
        {
            Actor actor = DB.Actors.Find(id);
            ViewBag.SelectedFilms = actor.FilmListItems();
            ViewBag.Films = DB.FilmListItems();
            return View(actor.ToActorView());
        }

        [HttpPost]
        public ActionResult Edit(ActorView actorView, List<int> SelectedItems/*Liste des Id des films sélectionés*/)
        {
            DB.UpdateActor(actorView, SelectedItems);
            return RedirectToAction("Index");
        }

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
