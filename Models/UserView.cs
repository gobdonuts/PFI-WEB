using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace MoviesManager.Models
{
    public class UserView
    {
        
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nom d'usager")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nom")]
        public string FullName { get; set; }

        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        public bool Admin { get; set; }

        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nouveau mot de passe")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmation")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage ="La confirmation ne correspond pas")]
        public string Confirmation { get; set; }

        public string AvatarId { get; set; }


        private ImageGUIDReference AvatarReference { get; set; }
        public string AvatarImageData { get; set; }

        public void InitAvatarManagement()
        {
            AvatarReference = new ImageGUIDReference(@"/Avatars/", @"no_avatar.png");
            AvatarReference.MaxSize = 512;
            AvatarReference.HasThumbnail = false;
        }
        public UserView()
        {
            InitAvatarManagement();
        }
        public String GetAvatarURL()
        {
            return AvatarReference.GetURL(AvatarId, false);
        }
        public void SaveAvatar()
        {
            AvatarId = AvatarReference.SaveImage(AvatarImageData, AvatarId);
        }
        public void RemoveAvatar()
        {
            AvatarReference.Remove(AvatarId);
        }
        public User ToUser()
        {
            return new User()
            {
                Id = this.Id,
                AvatarId = this.AvatarId,
                UserName = this.UserName,
                FullName = this.FullName,
                Password = this.Password,
                Admin = this.Admin
            };
        }
        public void CopyToUser(User user)
        {
            user.Id = Id;
            user.UserName = UserName;
            user.FullName = FullName;
            user.Password = Password;
            user.Admin = Admin;
            user.AvatarId = AvatarId;
        }
        public void CopyToUserView(User user)
        {
            user.Id = Id;
            user.UserName = UserName;
            user.FullName = FullName;
            user.Password = Password;
            user.Admin = Admin;
            user.AvatarId = AvatarId;
        }
    }
    public class LoginView
    {
        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Nom d'usager")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Requis")]
        [Display(Name = "Mot de passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public static class Extension
    {
        public static UserView ToUserView(this User user)
        {
            return new UserView()
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Password = user.Password,
                AvatarId = user.AvatarId,
                Admin = user.Admin,
            };
        }
        public static bool UserExists(this DBEntities DB, string userName)
        {
            User user = DB.Users.Where(u => u.UserName == userName).FirstOrDefault();
            return (user != null);
        }
        public static UserView AddUser(this DBEntities DB, UserView userView)
        {
            userView.SaveAvatar();
            User user = userView.ToUser();
            user = DB.Users.Add(user);
            DB.SaveChanges();
            return user.ToUserView();
        }
        public static bool UpdateUser(this DBEntities DB, UserView userView)
        {
            userView.SaveAvatar();
            User userToUpdate = DB.Users.Find(userView.Id);
            userView.CopyToUser(userToUpdate);
            DB.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
            DB.SaveChanges();
            return true;
        }
        public static bool RemoveUser(this DBEntities DB, UserView userView)
        {
            User userToDelete = userView.ToUser();
            userView.RemoveAvatar();
            DB.Users.Remove(userToDelete);
            DB.SaveChanges();
            return true;
        }
        public static User FindByUserName(this DBEntities DB,string userName)
        {
            User user = DB.Users.Where(u => u.UserName == userName).FirstOrDefault();
            return user;
        }
        public static User[] getAllUsers(this DBEntities DB)
        {
            User[] users = new User[DB.Users.Count()];
            users = DB.Users.Where(u => u.Id != 0).ToArray();
            return users;
        }
    }
}