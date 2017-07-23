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


        public ActionResult DriverRides()//as a CarOwner
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
            var mr = db.Rides.Where(a => a.CarOwnerId == co.Id).OrderByDescending(p => p.StartTime).ToList();
            ViewBag.CarOwnerId = co.Id;
            return View(mr.ToList());


        }

        public ActionResult DriverFutureRides()//as a CarOwner
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
            ViewBag.CarOwnerId = co.Id;
            return View(mr.ToList());


        }

        public ActionResult DriverPastRides()//as a CarOwner
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
            ViewBag.CarOwnerId = co.Id;
            return View(mr.ToList());


        }


        public ActionResult PassengerRides()//as a Passenger
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var UserRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.UserId == u.Id
                select Ride;

            var CarOwnerRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.Ride.CarOwner.UserId == u.Id
                select Ride;

            IEnumerable<Ride> exceptQuery =
                UserRidesQuery.Except(CarOwnerRidesQuery).OrderBy(a => a.StartTime);
            if (exceptQuery == null)
            {
                return RedirectToAction("AvailableRides");
            }
            return View(exceptQuery.ToList());


        }

        public ActionResult FuturePassengerRides()//as a Passenger
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var UserRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.UserId == u.Id
                select Ride;

            var CarOwnerRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.Ride.CarOwner.UserId == u.Id
                select Ride;

            IEnumerable<Ride> exceptQuery =
                UserRidesQuery.Except(CarOwnerRidesQuery).Where(a => a.StartTime >= DateTime.Now).OrderBy(a => a.StartTime);
            if (exceptQuery == null)
            {
                return RedirectToAction("AvailableRides");
            }
            return View(exceptQuery.ToList());
        }

        public ActionResult PastPassengerRides()//as a Passenger
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var UserRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.UserId == u.Id
                select Ride;

            var CarOwnerRidesQuery =
                from Seat in db.Seats
                join Ride in db.Rides on Seat.RideId equals Ride.Id
                where Seat.Ride.CarOwner.UserId == u.Id
                select Ride;

            IEnumerable<Ride> exceptQuery =
                UserRidesQuery.Except(CarOwnerRidesQuery).Where(a => a.StartTime <= DateTime.Now).OrderByDescending(a => a.StartTime);
            if (exceptQuery == null)
            {
                return RedirectToAction("AvailableRides");
            }
            return View(exceptQuery.ToList());
        }

        public ActionResult GoToUni()
        {
            var q = db.Rides.Where(a => (a.DestinationPlace.Contains("دانشگاه زنجان")) && (a.StartTime >= DateTime.Now)).OrderBy(a => a.StartTime);
            return View(q.ToList());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
            ViewBag.RideId = id.ToString();
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
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }


            CarOwner carOwner = db.CarOwners.Where(p => p.UserId == u.Id).FirstOrDefault();
            if (carOwner == null)
            {
                ViewBag.Error = "ابتدا باید ماشین خود را تعریف کنید";
                return RedirectToAction("Create", "CarOwners");
            }
            return View();
        }

        // POST: Rides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ride ride)
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            TimeSpan ts = TimeSpan.Parse(ride.myStartTime);
            ride.StartTime = ride.myDate.Date + ts;
            ride.EndTime = ride.StartTime;
            ride.EndTime = ride.EndTime.AddMinutes(ride.Duration);
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(u.Id)).FirstOrDefault();
            if (co == null)
            {
                ViewBag.Error = "ابتدا باید ماشین خود را تعریف کنید";
                return RedirectToAction("Create", "CarOwners");
            }
            ride.CarOwnerId = co.Id;
            if (ModelState.IsValid)
            {
                db.Rides.Add(ride);
                db.SaveChanges();
                Seat s = new Seat();
                s.UserId = u.Id;
                s.RideId = ride.Id;
                db.Seats.Add(s);//add the carOwner to the Seats
                db.SaveChanges();
                return RedirectToAction("DriverRides");
            }
            //////
            return View(ride);
        }

        // GET: Rides/Edit/5
        public ActionResult Edit(int? id)
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
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(u.Id)).FirstOrDefault();
            if (co == null)
            {
                ViewBag.Error = "ابتدا باید ماشین خود را تعریف کنید";
                return RedirectToAction("Create", "CarOwners");
            }
            Ride ride = db.Rides.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            if (ride.StartTime < DateTime.Now)
            {
                ViewBag.Error = "باید زمان برای آینده باشد تا بتوانید سفر را ویرایش کنید";
                return View(ride);
            }
            return View(ride);
        }

        // POST: Rides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ride ride)
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            TimeSpan ts = TimeSpan.Parse(ride.myStartTime);
            ride.StartTime = ride.myDate.Date + ts;
            ride.EndTime = ride.StartTime;
            ride.EndTime = ride.EndTime.AddMinutes(ride.Duration);
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(u.Id)).FirstOrDefault();
            if (co == null)
            {
                ViewBag.Error = "ابتدا باید ماشین خود را تعریف کنید";
                return RedirectToAction("Create", "CarOwners");
            }
            ride.CarOwnerId = co.Id;
            if (ModelState.IsValid)
            {
                db.Entry(ride).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DriverRides");
            }
            if (ride.StartTime < DateTime.Now)
            {
                ViewBag.Error = "باید زمان برای آینده باشد تا بتوانید سفر را ویرایش کنید";
                return View(ride);
            }
            return View(ride);
        }


        public ActionResult PassengersList(int? id)
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            Ride r = db.Rides.Find(id);
            ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();

            if (r == null)
            {
                return RedirectToAction("DriverRides");
            }
            return View();
        }

        public ActionResult Reserve(int? id)
        {

            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (id == null)
            {
                return RedirectToAction("AvailableRides");
            }


            Ride ride = db.Rides.Find(id);
            ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
            if (ride == null)
            {
                return HttpNotFound();
            }
            //
            var alreadyReserved = db.Seats.Any(p => (p.UserId == u.Id) && (p.RideId == ride.Id));
            if (alreadyReserved)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "شما قبلا این سفر را انتخاب کرده اید و نمی توانید دوباره انتخابش کنید ";
                //return RedirectToAction("Index");
                return View(ride);
            }
            if (ride.StartTime < DateTime.Now)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "باید سفری که رزرو می کنید برای آینده باشد! ";
                return View(ride);
            }
            if (ride.EmptySeats <= 0)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "ظرفیت خودرو به پایان رسیده است. ";
                return View(ride);
            }

            //if (ride.CarOwner.UserId == u.Id)
            //{
            //    ViewBag.Error = "شما نمی توانید در سفر خود صندلی رزرو کنید ";
            //    //return RedirectToAction("Index");
            //    return View(ride);
            //}
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

            var alreadyReserved = db.Seats.Any(p => (p.UserId == u.Id) && (p.RideId == r.Id));
            if (alreadyReserved)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "شما قبلا این سفر را انتخاب کرده اید و نمی توانید دوباره انتخابش کنید ";
                //return RedirectToAction("Index");
                return View(r);
            }
            if (r.StartTime < DateTime.Now)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "باید سفری که رزرو می کنید برای آینده باشد! ";
                return View(r);
            }
            if (r.EmptySeats <= 0)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "ظرفیت خودرو به پایان رسیده است. ";
                return View(r);
            }
            if (ModelState.IsValid)
            {

                db.Entry(r).State = EntityState.Modified;
                db.Seats.Add(s);//add the carOwner to the Seats
                db.SaveChanges();
                return RedirectToAction("FuturePassengerRides");
            }

            return RedirectToAction("AvailableRides");
        }


        public ActionResult CancelReserve(int? id)
        {

            // in table Seats if there is no row error

            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (id == null)
            {
                return RedirectToAction("AvailableRides");
            }
            Ride ride = db.Rides.Find(id);
            ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();

            var Reserved = db.Seats.Any(p => (p.UserId == u.Id) && (p.RideId == ride.Id));
            if (ride == null)
            {
                return HttpNotFound();
            }
            if (Reserved)
            {

                return View(ride);
            }
            if (ride.StartTime < DateTime.Now)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "باید سفری که کنسل می کنید برای آینده باشد! ";
                return View(ride);
            }
            else
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "شما قبلا این سفر را انتخاب نکرده اید که بتوانید کنسلش کنید. ";
                return View(ride);
            }
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
            if (r.StartTime < DateTime.Now)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "باید سفری که کنسل می کنید برای آینده باشد! ";
                return View(r);
            }
            var Reserved = db.Seats.Any(p => (p.UserId == u.Id) && (p.RideId == r.Id));
            if (Reserved)
            {
                if (ModelState.IsValid)
                {

                    db.Entry(r).State = EntityState.Modified;
                    db.Seats.Remove(s);//delete that row
                    db.SaveChanges();
                    return RedirectToAction("PassengerRides");
                }
            }
            else
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "شما قبلا این سفر را انتخاب نکرده اید که بتوانید کنسلش کنید. ";
                return View(r);
            }


            return RedirectToAction("AvailableRides");
            //db.Appointments.Remove(ap);

        }



        // GET: Rides/Delete/5
        public ActionResult Delete(int? id)
        {
            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            Ride r = db.Rides.Find(id);
            ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(u.Id)).FirstOrDefault();
            if (co == null)
            {
                ViewBag.Error = "ابتدا باید ماشین خود را تعریف کنید";
                return RedirectToAction("Create", "CarOwners");
            }
            if (r.StartTime < DateTime.Now)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "باید سفری که کنسل می کنید برای آینده باشد! ";
                return View(r);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (r == null)
            {
                return HttpNotFound();
            }
            return View(r);
        }

        // POST: Rides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            User u = db.Users.Find(SessionPersister.UserId);
            if (u == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            Ride r = db.Rides.Find(id);
            var DeleteSeats = new List<Seat>();
            CarOwner co = db.CarOwners.Where(l => l.UserId.Equals(u.Id)).FirstOrDefault();
            if (co == null)
            {
                ViewBag.Error = "ابتدا باید ماشین خود را تعریف کنید";
                return RedirectToAction("Create", "CarOwners");
            }
            if (r.StartTime < DateTime.Now)
            {

                ViewBag.Passengers = db.Seats.Where(acc => (acc.RideId == id)).ToList();
                ViewBag.Error = "باید سفری که کنسل می کنید برای آینده باشد! ";
                return View(r);
            }
            DeleteSeats = db.Seats.Where(a => a.RideId == id).ToList();
            db.Seats.RemoveRange(DeleteSeats);

            //Seat s = new Seat();
            //IEnumerable<Seat> DeleteSeats = db.Seats.Where(a => a.RideId == id);

            db.SaveChanges();
            db.Rides.Remove(r);
            db.SaveChanges();
            return RedirectToAction("DriverFutureRides");
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
