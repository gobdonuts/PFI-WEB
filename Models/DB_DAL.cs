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
using System.Web.Mvc;
using System.ComponentModel;

namespace MoviesManager.Models
{
    public class FilmView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Titre")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Synopsis")]
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

        private ImageGUIDReference PosterReference { get; set; }
        public string PosterImageData { get; set; }

        public void InitAvatarManagement()
        {
            PosterReference = new ImageGUIDReference(@"/Posters/", @"no_avatar.png");
            PosterReference.MaxSize = 512;
            PosterReference.HasThumbnail = false;
        }
        public FilmView()
        {
            InitAvatarManagement();
        }
        public String GetPosterURL()
        {
            return PosterReference.GetURL(PosterId, false);
        }
        public void SaveAvatar()
        {
            PosterId = PosterReference.SaveImage(PosterImageData, PosterId);
        }
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

    public class RatingView
    {
        public int Id { get; set; }

       
        public int FilmId { get; set; }

        public int UserId { get; set; }

        
        [Display(Name = "Note")]
        [Required(ErrorMessage = "La note doit etre entre 1 et 5")]
        [Range(1,5)]
        public int Value { get; set; }

        [Display(Name = "Commentaire")]
        [Required(ErrorMessage = "Requis")]
        public string Comment { get; set; }

        public RatingView()
        {

        }
        public RatingView(int idFilm,int userId)
        {
            UserId = userId;
            FilmId = idFilm;
        }
        public void toRating(Rating rating)
        {
            rating.Id = Id;
            rating.FilmId = FilmId;
            rating.UserId = UserId;
            rating.Value = Value;
            rating.Comment = Comment;
        }
    }

    public class ActorView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Sexe")]
        public int Sexe { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Pays")]
        public int CountryId { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string PhotoId { get; set; }

        public ICollection<Casting> Castings { get; set; }

        private ImageGUIDReference AvatarReference { get; set; }
        public string AvatarImageData { get; set; }

        public void InitAvatarManagement()
        {
            AvatarReference = new ImageGUIDReference(@"/Avatars/", @"no_avatar.png");
            AvatarReference.MaxSize = 512;
            AvatarReference.HasThumbnail = false;
        }
        public ActorView()
        {
            InitAvatarManagement();
        }
        public String GetAvatarUrl()
        {
            return AvatarReference.GetURL(PhotoId, false);
        }
        public void SaveAvatar()
        {
            PhotoId = AvatarReference.SaveImage(AvatarImageData, PhotoId);
        }
        public void RemoveAvatar()
        {
            AvatarReference.Remove(PhotoId);
        }
        public void ToActor(Actor actor)
        {
            actor.Id = Id;
            actor.Name = Name;
            actor.CountryId = CountryId;
            actor.BirthDate = BirthDate;
            actor.Sexe = Sexe;
            actor.PhotoId = PhotoId;
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
                BirthDate = actor.BirthDate,
                PhotoId = actor.PhotoId,
                Sexe = actor.Sexe,
                CountryId = actor.CountryId,
                Castings = actor.Castings
            };
        }
        public static RatingView ToRatingView(this Rating rating)
        {
            return new RatingView()
            {
                Id = rating.Id,
                FilmId = rating.FilmId,
                UserId = rating.UserId,
                Value = rating.Value,
                Comment = rating.Comment
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
        
        public static List<RatingView> ToRatingViewList(this List<Rating> ratings)
        {
            List<RatingView> ratingsView = new List<RatingView>();
            if (ratings != null)
            {
                foreach (Rating rating in ratings)
                {
                    ratingsView.Add(rating.ToRatingView());
                }
            }
            return ratingsView;
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
        public static List<ListItem> AudienceListItems(this DBEntities DB)
        {
            List<ListItem> audiences = new List<ListItem>();
            if (DB.Audiences != null)
            {
                foreach (Audience audience in DB.Audiences)
                {
                    audiences.Add(new ListItem() { Id = audience.Id, Text = audience.Name });
                }
            }
            return audiences;
        }

        public static List<ListItem> AudienceListItem(this Film film)
        {
            List<ListItem> audiences = new List<ListItem>();
            audiences.Add(new ListItem() { Id = film.AudienceId });
            return audiences;
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
        public static RatingView AddRating(this DBEntities DB, RatingView ratingView)
        {
            Rating rating = new Rating();
            ratingView.toRating(rating);
            BeginTransaction(DB);
            rating = DB.Ratings.Add(rating);
            DB.SaveChanges();
            Commit();
            return rating.ToRatingView();
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
            actorView.SaveAvatar();
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
            actorView.SaveAvatar();
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
            actorView.RemoveAvatar();
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
