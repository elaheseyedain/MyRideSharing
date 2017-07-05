using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;


namespace MyRideSharing.Models
{
    public class AccountModel
    {

        private RideSharingEntities db = new RideSharingEntities();
        private User _user = new User();

        public AccountModel()
        {
        }

        public User signIn(string studentid, string nationalid)
        {
            return db.Users.Where(acc => acc.StudentId.Equals(studentid) && acc.NationalId.Equals(nationalid)).FirstOrDefault();
        }

        public User signup(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

   

        public User EditUser(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return user;
        }

        public User DeleteUser(User user)
        {
            db.Entry(user).State = EntityState.Deleted;
            db.SaveChanges();
            return user;
        }

        public List<Gender> GetGenderList()
        {
            return db.Genders.ToList();
        }

        public User find(string email)
        {
            return db.Users.Where(acc => acc.Email.Equals(email)).FirstOrDefault();
        }

        public User findById(int id)
        {
            return db.Users.Where(acc => acc.Id.Equals(id)).FirstOrDefault();
        }

        public List<User> findAll()
        {
            return db.Users.ToList();
        }




    }

}

