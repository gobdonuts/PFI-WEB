﻿using MoviesManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MoviesManager.Controllers
{
    public class FilmsController : Controller
    {
        private DBEntities DB = new DBEntities();

        public ActionResult Index()
        {
            return View(DB.FilmsList());
        }

        public ActionResult FilmsList(List<FilmView> films)
        {
            return PartialView(films);
        }

        public ActionResult Create()
        {
            ViewBag.SelectedActors = new List<MoviesManager.Models.ListItem>();
            ViewBag.Actors = new SelectList(DB.ActorListItems());
            ViewBag.Audiences = new SelectList(DB.Audiences);
            ViewBag.Styles = new SelectList(DB.Styles);
            return View(new FilmView());
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
            ViewBag.Actors = film.ActorsList();
            return View(film.ToFilmView());
        }

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
            DB.UpdateFilm(filmView, SelectedItems);
            return RedirectToAction("Index");
        }

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