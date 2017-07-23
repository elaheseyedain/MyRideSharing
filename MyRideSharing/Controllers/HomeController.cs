using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyRideSharing.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "با ما در زمان و هزینه های خودتان صرفه جویی کنید.";

            return View();
        }

        public ActionResult Terms()
        {
            ViewBag.Message = "صرفه جویی در هزینه ها با ما";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "ارتباط با ما";

            return View();
        }
    }
}