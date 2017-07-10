using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyRideSharing.Models;
using MyRideSharing.Security;

namespace MyRideSharing.Controllers
{
    public class CarOwnersController : Controller
    {
        private RideSharingEntities db = new RideSharingEntities();

        // GET: CarOwners
        public ActionResult Index()
        {
            var carOwners = db.CarOwners.Include(c => c.User);
            return View(carOwners.ToList());
        }

        // GET: CarOwners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarOwner carOwner = db.CarOwners.Find(id);
            //var avgRate = db.Ratings
            //      .Where(c => c.CarOwnerId == id)
            //      .GroupBy(g => g.CarOwnerId, c => c.Rate)
            //      .Select(g => new
            //      {
            //          Average = g.Average()
            //      });
            double avg = 0;
            var hasAverage = db.Ratings.Any(p => p.CarOwnerId == carOwner.Id);
            if (hasAverage)
            {
                avg = db.Ratings.Where(x => x.CarOwnerId == carOwner.Id).Average(z => z.Rate);
            }

            //var RateQuery =
            //    from Seat in db.Seats
            //    join Ride in db.Rides on Seat.RideId equals Ride.Id
            //    where Seat.UserId == u.Id && Ride.StartTime >= DateTime.Now
            //    select Ride;
            ViewBag.Rate = avg.ToString();
            if (carOwner == null)
            {
                return HttpNotFound();
            }
            return View(carOwner);
        }

        // GET: CarOwners/Create
        public ActionResult Create()
        {

            User u = new User();
            var uid = Int32.Parse(Session["userId"].ToString());
            var ownsCar = db.CarOwners.Any(p => (p.UserId == uid));
            if (!ownsCar)
            {
                ViewBag.UserId = new SelectList(db.Users, "Id", "StudentId");
                return View();
            }
            else //if user alreay defined a car
            {
                return RedirectToAction("Edit");
            }

        }

        // POST: CarOwners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CarOwner carOwner)
        {

            User u = new User();
            var uid = Int32.Parse(Session["userId"].ToString());
            u = db.Users.Where(acc => acc.Id.Equals(uid)).FirstOrDefault();
            carOwner.UserId = u.Id;

            if (ModelState.IsValid)
            {
                db.CarOwners.Add(carOwner);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "StudentId", carOwner.UserId);
            return View(carOwner);
        }

        // GET: CarOwners/Edit/5
        public ActionResult Edit()
        {

            CarOwner co = new CarOwner();
            var uid = Int32.Parse(Session["userId"].ToString());
            co = db.CarOwners.Where(acc => acc.UserId.Equals(uid)).FirstOrDefault();



            if (co == null)
            {
                return RedirectToAction("Create");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "StudentId", co.UserId);
            return View(co);
        }

        // POST: CarOwners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CarOwner carOwner)
        {

            carOwner.UserId = Int32.Parse(SessionPersister.UserId.ToString());
            if (ModelState.IsValid)
            {
                db.Entry(carOwner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "StudentId", carOwner.UserId);
            return View(carOwner);
        }


        public ActionResult CarOwnerRides(int? id)//CarOwner's id to list of rides
        {
            User u = db.Users.Find(SessionPersister.UserId);

            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            CarOwner co = db.CarOwners.Find(id);
            ViewBag.Name = co.User.FirstName + " " + co.User.LastName;
            ViewBag.CarOwnerId = co.Id;
            if (co == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //co = db.CarOwners.Where(l => l.UserId.Equals(uid)).FirstOrDefault();


            var dr = db.Rides.Where(a => a.CarOwnerId == co.Id).OrderByDescending(p => p.StartTime).ToList();
            return View(dr.ToList());

        }


        // GET: CarOwners/Delete/5
        public ActionResult RateDriver(int? id)
        {

            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarOwner carOwner = db.CarOwners.Find(id);
            if (carOwner == null)
            {
                return HttpNotFound();
            }

            if (carOwner.UserId == u.Id)
            {
                ViewBag.Error = "شما نمی توانید به خودتان امتیاز دهید";
                return View(carOwner);
            }

            //var alreadyRated = db.Ratings.Any(p => (p.UserId == u.Id) && (p.CarOwnerId == carOwner.Id));
            //if (alreadyRated)
            //{
            //    ViewBag.Error = "شما نمی توانید دوباره به این راننده امتیاز دهید ";
            //    return View(carOwner);
            //}
            return View(carOwner);
        }

        // POST: CarOwners/Delete/5
        [HttpPost, ActionName("RateDriver")]
        [ValidateAntiForgeryToken]
        public ActionResult RateDriverConfirmed(int id, string rate)
        {
            User u = db.Users.Find(SessionPersister.UserId);
            CarOwner carOwner = db.CarOwners.Find(id);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (carOwner.UserId == u.Id)
            {
                ViewBag.Error = "شما نمی توانید به خودتان امتیاز دهید";
                return View(carOwner);
            }

            var alreadyRated = db.Ratings.Any(p => (p.UserId == u.Id) && (p.CarOwnerId == carOwner.Id));

            if (int.Parse(rate) <= 0 || int.Parse(rate) > 5)
            {

                ViewBag.Error = "امتیاز شما باید عددی صحیح بین 1 تا 5 باشد";
                return View(carOwner);
            }
            if (alreadyRated)
            {

                Rating ar = db.Ratings.Where(a => (a.UserId == u.Id) && (a.CarOwnerId == carOwner.Id)).FirstOrDefault();
                ar.Rate = int.Parse(rate);
                db.Entry(ar).State = EntityState.Modified;
                db.SaveChanges();
                //ViewBag.Error = "شما نمی توانید دوباره به این راننده امتیاز دهید ";
                return RedirectToAction("Index", "CarOwners");
            }

            Rating r = new Rating();
            r.CarOwnerId = carOwner.Id;
            r.UserId = u.Id;
            r.Rate = int.Parse(rate);
            //if (ModelState.IsValid)
            //{
            db.Ratings.Add(r);
            db.SaveChanges();
            return RedirectToAction("Index", "CarOwners");
            //}
        }

        // GET: CarOwners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User u = db.Users.Find(SessionPersister.UserId);
            CarOwner carOwner = db.CarOwners.Find(id);
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(u.Id)).FirstOrDefault();
            if (co == null)
            {
                ViewBag.Error = "شما قادر به حذف ماشین کس دیگر نیستید";
                return RedirectToAction("Create", "CarOwners");
            }
            if (carOwner == null)
            {
                return HttpNotFound();
            }
            return View(carOwner);
        }

        // POST: CarOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CarOwner carOwner = db.CarOwners.Find(id);
            User u = db.Users.Find(SessionPersister.UserId);
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(u.Id)).FirstOrDefault();
            if (co == null)
            {
                ViewBag.Error = "شما قادر به حذف ماشین کس دیگر نیستید";
                return RedirectToAction("Create", "CarOwners");
            }
            db.CarOwners.Remove(carOwner);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
