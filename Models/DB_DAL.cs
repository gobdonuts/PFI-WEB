using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;
using MoviesManager.Models;

namespace MoviesManager.Models
{
    public class FilmView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Titre")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Titre")]
        public string Synopsis { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Auteur")]
        public string Author { get; set; }
        public int AudienceId { get; set; }
        public int StyleId { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Année")]
        public int Year { get; set; }
        public string PosterId { get; set; }
        public double RatingsAverage { get; set; }
        public int NbRatings { get; set; }


        public void ToFilm(Film film)
        {
            film.Id = Id;
            film.Title = Title;
            film.Synopsis = Synopsis;
            film.Author = Author;
            film.AudienceId = AudienceId;
            film.StyleId = StyleId;
            film.Year = Year;
            film.PosterId = PosterId;
            film.RatingsAverage = RatingsAverage;
            film.NbRatings = NbRatings;
        }
    }
    
   
    public class ActorView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public void ToActor(Actor actor)
        {
            actor.Id = Id;
            actor.Name = Name;
            actor.BirthDate = BirthDate;
        }
    }
    public static class OnlineUsers
    {
        public static List<User> Users
        {
            get
            {
                if (HttpRuntime.Cache["OnLineUsers"] == null)
                {
                    HttpRuntime.Cache["OnLineUsers"] = new List<User>();
                }
                return (List<User>)HttpRuntime.Cache["OnLineUsers"];
            }
        }
        public static int IdProvider
        { // Cet accesseur ne sera plus utile lorsque vous aurez implanter la nouvelle approche d'authentification
            get
            {
                if (HttpRuntime.Cache["IdProvider"] == null)
                {
                    HttpRuntime.Cache["IdProvider"] = 0;
                }
                HttpRuntime.Cache["IdProvider"] = (int)HttpRuntime.Cache["IdProvider"] + 1;
                return (int)HttpRuntime.Cache["IdProvider"];
            }
        }
        public static DateTime LastUpdate
        {
            get
            {
                if (HttpRuntime.Cache["LastOnLineUsersUpdate"] == null)
                {
                    HttpRuntime.Cache["LastOnLineUsersUpdate"] = new DateTime(0);
                }
                return (DateTime)HttpRuntime.Cache["LastOnLineUsersUpdate"];
            }
            set { HttpRuntime.Cache["LastOnLineUsersUpdate"] = value; }
        }
        public static void AddSessionUser(User user)
        {
            user.Id = IdProvider;
            LastUpdate = DateTime.Now;
            Users.Add(user);
            HttpContext.Current.Session["User"] = user;
            HttpContext.Current.Session.Timeout = 15; // minutes
        }
        public static List<User> ToList()
        {
            return Users;
        }
        public static User Find(int userId)
        {

            return OnlineUsers.ToList().Where(u => u.Id == userId).FirstOrDefault();
        }
        public static UserView FindId(string userName)
        {
            UserView user = OnlineUsers.ToList().Where(u => u.FullName == userName).FirstOrDefault().ToUserView();
            return user;
        }
        public static bool NeedUpdate(DateTime date)
        {
            return DateTime.Compare(date, LastUpdate) < 0;
        }
        public static void RemoveSessionUser()
        {
            User user = GetSessionUser();
            if (user != null)
            {
                Users.Remove(user);
                LastUpdate = DateTime.Now;
                HttpContext.Current.Session.Abandon();
            }
        }
        public static User GetSessionUser()
        {
            if (HttpContext.Current != null)
                return (User)HttpContext.Current.Session["User"];
            return null;
        }
    }
    
    public static class DB_DAL_ExtensionsMethods
    {
        private static DbContextTransaction Transaction
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return (DbContextTransaction)HttpContext.Current.Session["Transaction"];
                }
                return null;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Session["Transaction"] = value;
                }
            }
        }
        private static void BeginTransaction(DBEntities DB)
        {
            if (Transaction != null)
                throw new Exception("Transaction en cours! Impossible d'en démarrer une nouvelle!");
            Transaction = DB.Database.BeginTransaction();
        }
        private static void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
            else
                throw new Exception("Aucune ransaction en cours! Impossible de mettre à jour la base de ddonnées!");
        }

        public static FilmView ToFilmView(this Film film)
        {
            return new FilmView()
            {
                Id = film.Id,
                Title = film.Title,
                Synopsis = film.Synopsis,
                Author = film.Author,
                AudienceId = film.AudienceId,
                StyleId = film.StyleId,
                Year = film.Year,
                PosterId = film.PosterId,
                RatingsAverage = film.RatingsAverage,
                NbRatings = film.NbRatings
            };
        }
        public static ActorView ToActorView(this Actor actor)
        {
            return new ActorView()
            {
                Id = actor.Id,
                Name = actor.Name,
                BirthDate = actor.BirthDate
            };
        }
        public static List<FilmView> FilmsList(this Actor actor)
        {
            List<FilmView> films = new List<FilmView>();
            if (actor.Castings != null)
            {
                foreach (Casting casting in actor.Castings)
                {
                    films.Add(casting.Film.ToFilmView());
                }
            }
            return films.OrderBy(f => f.Title).ToList();
        }
        public static List<FilmView> FilmsList(this DBEntities DB)
        {
            List<FilmView> films = new List<FilmView>();
            if (DB.Films != null)
            {
                foreach (Film film in DB.Films)
                {
                    films.Add(film.ToFilmView());
                }
            }
            return films.OrderBy(f => f.Title).ToList();
        }
        public static List<ActorView> ActorsList(this Film film)
        {
            List<ActorView> actors = new List<ActorView>();
            if (film.Castings != null)
            {
                foreach (Casting casting in film.Castings)
                {
                    actors.Add(casting.Actor.ToActorView());
                }
            }
            return actors.OrderBy(f => f.Name).ToList();
        }
        public static List<ActorView> ActorsList(this DBEntities DB)
        {
            List<ActorView> actors = new List<ActorView>();
            if (DB.Actors != null)
            {
                foreach (Actor actor in DB.Actors)
                {
                    actors.Add(actor.ToActorView());
                }
            }
            return actors.OrderBy(f => f.Name).ToList();
        }
 
        public static List<ListItem> FilmListItems(this Actor actor)
        {
            List<ListItem> films = new List<ListItem>();
            if (actor.Castings != null)
            {
                foreach (Casting casting in actor.Castings)
                {
                    films.Add(new ListItem() { Id = casting.Film.Id, Text = casting.Film.Title });
                }
            }
            return films;
        }
        public static List<ListItem> FilmListItems(this DBEntities DB)
        {
            List<ListItem> films = new List<ListItem>();
            if (DB.Films != null)
            {
                foreach (Film film in DB.Films)
                {
                    films.Add(new ListItem() { Id = film.Id, Text = film.Title });
                }
            }
            return films;
        }
        public static List<ListItem> ActorListItems(this Film film)
        {
            List<ListItem> actors = new List<ListItem>();
            if (film.Castings != null)
            {
                foreach (Casting casting in film.Castings)
                {
                    actors.Add(new ListItem() { Id = casting.Actor.Id, Text = casting.Actor.Name });
                }
            }
            return actors;
        }
        public static List<ListItem> ActorListItems(this DBEntities DB)
        {
            List<ListItem> actors = new List<ListItem>();
            if (DB.Actors != null)
            {
                foreach (Actor actor in DB.Actors)
                {
                    actors.Add(new ListItem() { Id = actor.Id, Text = actor.Name });
                }
            }
            return actors;
        }

        public static FilmView AddFilm(this DBEntities DB, FilmView filmView, List<int> actorsIdList)
        {
            Film film = new Film();
            filmView.ToFilm(film);
            BeginTransaction(DB);
            film = DB.Films.Add(film);
            DB.SaveChanges();
            SetFilmCastings(DB, film.Id, actorsIdList);
            Commit();
            return film.ToFilmView();
        }
        public static bool UpdateFilm(this DBEntities DB, FilmView filmView, List<int> actorsIdList)
        {
            Film film = DB.Films.Find(filmView.Id);
            filmView.ToFilm(film);
            BeginTransaction(DB);
            DB.Entry(film).State = EntityState.Modified;
            DB.SaveChanges();
            SetFilmCastings(DB, film.Id, actorsIdList);
            Commit();
            return true;
        }
        public static bool RemoveFilm(this DBEntities DB, FilmView filmView)
        {
            Film film = DB.Films.Find(filmView.Id);
            BeginTransaction(DB);
            SetFilmCastings(DB, film.Id, null);
            DB.Films.Remove(film);
            DB.SaveChanges();
            Commit();
            return true;
        }
        private static bool SetFilmCastings(DBEntities DB, int film_Id, List<int> actorsIdList)
        {
            DB.Castings.RemoveRange(DB.Castings.Where(c => c.FilmId == film_Id));
            if (actorsIdList != null)
            {
                foreach (int actor_Id in actorsIdList)
                {
                    Casting casting = new Casting() { ActorId = actor_Id, FilmId = film_Id };
                    DB.Castings.Add(casting);
                }
            }
            DB.SaveChanges();
            return true;
        }

        public static ActorView AddActor(this DBEntities DB, ActorView actorView, List<int> filmsIdList)
        {
            Actor actor = new Actor();
            actorView.ToActor(actor);
            BeginTransaction(DB);
            actor = DB.Actors.Add(actor);
            DB.SaveChanges();
            SetActorCastings(DB, actor.Id, filmsIdList);
            Commit();
            return actor.ToActorView();
        }
        public static bool UpdateActor(this DBEntities DB, ActorView actorView, List<int> filmsIdList)
        {
            Actor actor = DB.Actors.Find(actorView.Id);
            actorView.ToActor(actor);
            BeginTransaction(DB);
            DB.Entry(actor).State = EntityState.Modified;
            DB.SaveChanges();
            SetActorCastings(DB, actor.Id, filmsIdList);
            Commit();
            return true;
        }
        public static bool RemoveActor(this DBEntities DB, ActorView actorView)
        {
            Actor actor = DB.Actors.Find(actorView.Id);
            BeginTransaction(DB);
            SetActorCastings(DB, actor.Id, null);
            DB.Actors.Remove(actor);
            DB.SaveChanges();
            Commit();
            return true;
        }
        private static bool SetActorCastings(DBEntities DB, int actor_Id, List<int> filmsIdList)
        {
            DB.Castings.RemoveRange(DB.Castings.Where(c => c.ActorId == actor_Id));
            if (filmsIdList != null)
            {
                foreach (int film_Id in filmsIdList)
                {
                    Casting casting = new Casting() { ActorId = actor_Id, FilmId = film_Id };
                    DB.Castings.Add(casting);
                }
            }
            DB.SaveChanges();
            return true;
        }
    }
}
