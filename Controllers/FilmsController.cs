using MoviesManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoviesManager.Controllers
{
    public class FilmsController : Controller
    {
        private DBEntities DB = new DBEntities();

        public ActionResult Index()
        {
            return View(DB.FilmsList());
        }
        public ActionResult RatingsList(List<RatingView> ratings)
        {
            return PartialView(ratings);
        }
        public ActionResult FilmsList(List<FilmView> films)
        {
            return PartialView(films);
        }
        [AdminAcces]
        public ActionResult Create()
        {
            ViewBag.SelectedActors = new List<ListItem>();
            ViewBag.Actors = DB.ActorListItems();
            return View();
        }
        [HttpPost]
        public ActionResult Create(FilmView filmView, List<int> SelectedItems /*Liste des Id des acteurs sélectionés*/)
        {
            DB.AddFilm(filmView, SelectedItems);
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Film film = DB.Films.Find(id);
            List<Rating> ratings = DB.Ratings.Where(r => r.FilmId == id).ToList();
            ViewBag.Actors = film.ActorsList();
            var avgRating = 0;
            foreach(Rating  r in ratings)
            {
                avgRating += r.Value;
            }
            avgRating = avgRating / ratings.Count;
            ViewBag.Ratings = ratings.ToRatingViewList();
            ViewBag.Avg = avgRating;
            return View(film.ToFilmView());
        }
        [AdminAcces]
        public ActionResult Edit(int id)
        {
            Film film = DB.Films.Find(id);
            ViewBag.SelectedActors = film.ActorListItems();
            ViewBag.Actors = DB.ActorListItems();
            return View(film.ToFilmView());
        }
        [HttpPost]
        public ActionResult Edit(FilmView filmView, List<int> SelectedItems /*Liste des Id des acteurs sélectionés*/)
        {
            filmView.SaveAvatar();
            DB.UpdateFilm(filmView, SelectedItems);
            return RedirectToAction("Index");
        }
        [AdminAcces]
        public ActionResult Delete(int id)
        {
            return View(DB.Films.Find(id).ToFilmView());
        }
        [HttpPost]
        public ActionResult Delete(FilmView filmView)
        {
            DB.RemoveFilm(filmView);
            return RedirectToAction("Index");
        }
    }
}