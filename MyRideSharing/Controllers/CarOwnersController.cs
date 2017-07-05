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
            else
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

//                {
//            JobCorp jc = new JobCorp();
//        var u = Int32.Parse(Session["userId"].ToString());
//        jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
//        service.JobCorpId = jc.Id;
//            if (ModelState.IsValid)
//            {
//                db.Services.Add(service);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//    }

            
//            return View(service);
//}

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

        // GET: CarOwners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarOwner carOwner = db.CarOwners.Find(id);
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
