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
using MyRideSharing.Utilities;
using System.Globalization;
using System.Data.Entity.Core.Objects;

namespace MyRideSharing.Controllers
{
    public class RidesController : Controller
    {
        private RideSharingEntities db = new RideSharingEntities();

        // GET: Rides
        public ActionResult Index()//AllRides
        {
            /*IEnumerable<Job> query = db.Jobs.ToList();
            query = db.Jobs.Where(m => (string.IsNullOrEmpty(title) ? true : m.Title.Contains(title)) && (string.IsNullOrEmpty(city) ? true : m.City.Name == city) && (string.IsNullOrEmpty(jobtype) ? true : m.JobType.Title == jobtype));
            */

            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            //var uid = u.Id;
            var UserRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.UserId == u.Id
                select Ride;

            var ur = UserRidesQuery.ToList();
            return View(ur.ToList());



            //return View(db.Rides.ToList());
        }

        public ActionResult AllFutureRides()
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var UserRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.UserId == u.Id && Ride.StartTime >= DateTime.Now
                select Ride;
            //ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now)).OrderBy(p => p.StartTime).ToList();

            var ur = UserRidesQuery.OrderBy(p => p.StartTime).ToList();
            return View(ur.ToList());

        }

        public ActionResult AllPastRides()
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var UserRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.UserId == u.Id && Ride.StartTime <= DateTime.Now
                select Ride;

            var ur = UserRidesQuery.OrderByDescending(p => p.StartTime).ToList();
            return View(ur.ToList());

        }

        public ActionResult MyRides()
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var uid = u.Id;
            CarOwner co = new CarOwner();
            co = db.CarOwners.Where(l => l.UserId.Equals(uid)).FirstOrDefault();

            if (co == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var mr = db.Rides.Where(a => a.CarOwnerId == co.Id).OrderBy(p => p.StartTime).ToList();
            return View(mr.ToList());


        }

        public ActionResult MyFutureRides()
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var uid = u.Id;
            CarOwner co = new CarOwner();
            co = db.CarOwners.Where(l => l.UserId.Equals(uid)).FirstOrDefault();

            if (co == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var mr = db.Rides.Where(a => (a.CarOwnerId == co.Id) && (a.StartTime >= DateTime.Now)).OrderBy(p => p.StartTime).ToList();
            return View(mr.ToList());


        }

        public ActionResult MyPastRides()
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var uid = u.Id;
            CarOwner co = new CarOwner();
            co = db.CarOwners.Where(l => l.UserId.Equals(uid)).FirstOrDefault();

            if (co == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var mr = db.Rides.Where(a => (a.CarOwnerId == co.Id) && (a.StartTime <= DateTime.Now)).OrderByDescending(p => p.StartTime).ToList();
            return View(mr.ToList());


        }

        public ActionResult GoToUni()
        {
            var q = db.Rides.Where(a => (a.DestinationPlace.Contains("دانشگاه زنجان"))  && (a.StartTime >= DateTime.Now)).OrderBy(a => a.StartTime);
            return View(q.ToList());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]//todo
        public ActionResult GoToUni(string myDate, string SourcePlace)//(string myDate, string myStartTime, string SourcePlace)
        {
            if (myDate != "")
            {
                DateTime pDate = Convert.ToDateTime(myDate);
                DateTime justDate = Utility.ToMiladiDateTime(pDate);
                var query = db.Rides.Where(a => (a.DestinationPlace.Contains("دانشگاه زنجان")) && (string.IsNullOrEmpty(SourcePlace) ? true : a.SourcePlace.Contains(SourcePlace)) && (a.StartTime >= DateTime.Now) && (string.IsNullOrEmpty(myDate) ? true : DbFunctions.TruncateTime(a.StartTime) == justDate)).OrderBy(a => a.StartTime);
                return View(query.ToList());
            }
            else
            {
                var q = db.Rides.Where(a => (a.DestinationPlace.Contains("دانشگاه زنجان")) && (string.IsNullOrEmpty(SourcePlace) ? true : a.SourcePlace.Contains(SourcePlace)) && (a.StartTime >= DateTime.Now)).OrderBy(a => a.StartTime);
                return View(q.ToList());
            }

        }

        public ActionResult ReturnFromUni()
        {
            var q = db.Rides.Where(a => (a.SourcePlace.Contains("دانشگاه زنجان")) && (a.StartTime >= DateTime.Now)).OrderBy(a => a.StartTime);
            return View(q.ToList());

        }


        [HttpPost]
        [ValidateAntiForgeryToken]//todo
        public ActionResult ReturnFromUni(string myDate, string DestinationPlace)
        {
            if (myDate != "")
            {
                DateTime pDate = Convert.ToDateTime(myDate);
                DateTime justDate = Utility.ToMiladiDateTime(pDate);
                var query = db.Rides.Where(a => (a.SourcePlace.Contains("دانشگاه زنجان")) && (string.IsNullOrEmpty(DestinationPlace) ? true : a.DestinationPlace.Contains(DestinationPlace)) && (a.StartTime >= DateTime.Now) && (string.IsNullOrEmpty(myDate) ? true : DbFunctions.TruncateTime(a.StartTime) == justDate)).OrderBy(a => a.StartTime);
                return View(query.ToList());
            }
            else
            {
                var q = db.Rides.Where(a => (a.SourcePlace.Contains("دانشگاه زنجان")) && (string.IsNullOrEmpty(DestinationPlace) ? true : a.DestinationPlace.Contains(DestinationPlace)) && (a.StartTime >= DateTime.Now)).OrderBy(a => a.StartTime);
                return View(q.ToList());
            }
        }


        public ActionResult AvailableRides()
        {
            var q = db.Rides.Where(a => (a.StartTime >= DateTime.Now)).OrderBy(a => a.StartTime);
            return View(q.ToList());

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AvailableRides(string myDate, string SourcePlace, string DestinationPlace)
        {
            if (myDate != "")
            {
                DateTime pDate = Convert.ToDateTime(myDate);
                DateTime justDate = Utility.ToMiladiDateTime(pDate);
                var query = db.Rides.Where(a => (string.IsNullOrEmpty(SourcePlace) ? true : a.SourcePlace.Contains(SourcePlace)) && (string.IsNullOrEmpty(DestinationPlace) ? true : a.DestinationPlace.Contains(DestinationPlace)) && (a.StartTime >= DateTime.Now) && (string.IsNullOrEmpty(myDate) ? true : DbFunctions.TruncateTime(a.StartTime) == justDate)).OrderBy(a => a.StartTime);
                return View(query.ToList());
            }
            else
            {
                var q = db.Rides.Where(a => (string.IsNullOrEmpty(SourcePlace) ? true : a.SourcePlace.Contains(SourcePlace)) && (string.IsNullOrEmpty(DestinationPlace) ? true : a.DestinationPlace.Contains(DestinationPlace)) && (a.StartTime >= DateTime.Now)).OrderBy(a => a.StartTime);
                return View(q.ToList());
            }
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

        public ActionResult Reserve(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AvailableRides");
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            return View(ride);
        }

        [HttpPost, ActionName("Reserve")]
        [ValidateAntiForgeryToken]
        public ActionResult ReserveConfirmed(int id)
        {
            Ride r = db.Rides.Find(id);
            Seat s = new Seat();
            User u = db.Users.Find(SessionPersister.UserId);
            s.RideId = r.Id;
            s.UserId = u.Id;
            r.EmptySeats -= 1;
           
            if (ModelState.IsValid)
            {

                db.Entry(r).State = EntityState.Modified;
                db.Seats.Add(s);//add the carOwner to the Seats
                db.SaveChanges();
                return RedirectToAction("MyRides");
            }

            return RedirectToAction("AvailableRides");
        }


        //CancelReserve for CarOwner should be manipulated
        public ActionResult CancelReserve(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AvailableRides");
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            return View(ride);
        }

        [HttpPost, ActionName("CancelReserve")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelReserveConfirmed(int id)
        {
            Ride r = db.Rides.Find(id);
            User u = db.Users.Find(SessionPersister.UserId);
            Seat s = db.Seats.Where(a => (a.UserId.Equals(u.Id)) && (a.RideId.Equals(r.Id))).FirstOrDefault();
            
            //s.RideId = r.Id;
            //s.UserId = u.Id;
            r.EmptySeats += 1;

            if (ModelState.IsValid)
            {

                db.Entry(r).State = EntityState.Modified;
                db.Seats.Remove(s);//delete that row
                db.SaveChanges();
                return RedirectToAction("MyRides");
            }

            return RedirectToAction("AvailableRides");
            //db.Appointments.Remove(ap);

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
