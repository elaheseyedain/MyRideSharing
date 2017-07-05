using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using MyRideSharing.Models;
using System.Data.Entity;
using MyRideSharing.Security;

namespace MyRideSharing.Controllers
{


    public class AccountController : Controller
    {


        private RideSharingEntities db = new RideSharingEntities();
        private AccountModel um = new AccountModel();
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            if (SessionPersister.UserId < 0)
            {

                ViewBag.genders = um.GetGenderList();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {

            if (string.IsNullOrEmpty(user.NationalId) || string.IsNullOrEmpty(user.StudentId) || um.signup(user) == null)
            {
                ViewBag.Error = "Signup was not successful";
                return View();
            }

            SessionPersister.UserId = user.Id;
            SessionPersister.NationalId = user.NationalId;
            SessionPersister.StudentId = user.StudentId;
            SessionPersister.Email = user.Email;
            SessionPersister.FirstName = user.LastName;
            SessionPersister.FirstName = user.FirstName;
            
            return RedirectToAction("Index", "Home");
            
        }


        [HttpGet]
        public ActionResult SignIn()
        {
            if (SessionPersister.UserId < 0)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
                
            }
        }



        [HttpPost]
        public ActionResult SignIn(User user)
        {

            user = um.signIn(user.StudentId, user.NationalId);
            if (user == null)
            {
                ViewBag.Error = "شماره دانشجویی یا کد ملی درست وارد کنید";
                return View();
            }

            SessionPersister.UserId = user.Id;
            SessionPersister.Email = user.Email;

            SessionPersister.FirstName = user.FirstName;
            SessionPersister.LastName = user.LastName;
            SessionPersister.NationalId = user.NationalId;
            SessionPersister.StudentId = user.StudentId;
            return RedirectToAction("Index", "Home");
            
        }

        [HttpGet]
        public ActionResult Edit()
        {
            User user = um.findById(SessionPersister.UserId);
            ViewBag.genders = um.GetGenderList();

            if (user == null)
            {
                ViewBag.Error = "user Not Found";
                return RedirectToAction("Index" , "Home");
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {

            um.EditUser(user);
            SessionPersister.UserId = user.Id;
            SessionPersister.Email = user.Email;
            SessionPersister.FirstName = user.FirstName;
            SessionPersister.LastName = user.LastName;
            SessionPersister.NationalId = user.NationalId;
            SessionPersister.StudentId = user.StudentId;


            return RedirectToAction("Index", "Home");
            
        }


    }
}