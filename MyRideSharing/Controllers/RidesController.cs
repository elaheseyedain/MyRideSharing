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
    public class RidesController : Controller
    {
        private RideSharingEntities db = new RideSharingEntities();

        // GET: Rides
        public ActionResult Index()
        {
            return View(db.Rides.ToList());
        }

        public ActionResult MyRides()
        {
            User u = db.Users.Find(SessionPersister.UserId);
            //User u = new User();
            //var uid = Int32.Parse(Session["userId"].ToString());
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            //var ownsCar = db.CarOwners.Any(p => (p.UserId == uid));
            var uid = u.Id;
            CarOwner co = new CarOwner();
            co = db.CarOwners.Where(l => l.UserId.Equals(uid)).FirstOrDefault();
            //if (u == null)
            //{
            //    return RedirectToAction("SignIn", "Account");
            //}
            if (co == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var mr = db.Rides.Where(a => a.CarOwnerId == co.Id).ToList();
            return View(mr.ToList());

            /*
            //DateTime dt = DateTime.Now.AddDays(2);
            //&& a.StartTime >= DateTime.Now  && a.StartTime <= dt
            //var jobs = db.Jobs.Include(j => j.User).Include(j => j.JobType);
            var Appointments = db.Appointments.Where(a => a.UserId == user.Id && a.StartTime >= DateTime.Now).ToList();
            return View(Appointments.ToList());*/
            //ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime > DateTime.Now)).ToList();

            //return View(db.Rides.ToList());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]//todo
        public ActionResult GoToUni(DateTime myDate, String myTime, string source)
        {
            Ride r = new Ride();
            IEnumerable<Ride> query = db.Rides.ToList();
            TimeSpan ts = TimeSpan.Parse(myTime);
            r.StartTime = r.myDate.Date + ts;

            query = db.Rides.Where(m => (string.IsNullOrEmpty(source) ? true : m.SourcePlace.Contains(source)) && (m.StartTime.Equals(myDate) && (m.DestinationPlace.Equals("دانشگاه زنجان"))));
            //if (query == null)
            //{

            //    ViewBag.cities = db.Cities.ToList();
            //    ViewBag.jt = db.JobTypes.ToList();
            //    var jobs = db.Jobs.ToList();
            //    return View(jobs.ToList());
            //}
            //ViewBag.cities = db.Cities.ToList();
            //ViewBag.jt = db.JobTypes.ToList();
            //var jb = query.ToList();
            return View();

        }


        // GET: Rides/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            return View(ride);
        }

        // GET: Rides/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ride ride)
        {

            TimeSpan ts = TimeSpan.Parse(ride.myStartTime);
            ride.StartTime = ride.myDate.Date + ts;
            ride.EndTime = ride.StartTime;
            ride.EndTime = ride.EndTime.AddMinutes(ride.Duration);
            User u = new User();
            var uid = Int32.Parse(Session["userId"].ToString());
            
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(uid)).FirstOrDefault();
            ride.CarOwnerId = co.Id;
            if (ModelState.IsValid)
            {
                db.Rides.Add(ride);
                db.SaveChanges();
                Seat s = new Seat();
                s.UserId = uid;
                s.RideId = ride.Id;
                db.Seats.Add(s);//add the carOwner to the Seats
                db.SaveChanges();
                return RedirectToAction("MyRides");
            }
            //////
            return View(ride);
        }

        // GET: Rides/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            return View(ride);
        }

        // POST: Rides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SourceLatitude,SourceLongitude,DestinationLatitude,DestinationLongitude,StartTime,EndTime,Price,Description,SourcePlace,DestinationPlace")] Ride ride)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ride).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ride);
        }

        // GET: Rides/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            return View(ride);
        }

        // POST: Rides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ride ride = db.Rides.Find(id);
            db.Rides.Remove(ride);
            db.SaveChanges();
            return RedirectToAction("Index");
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
