using proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiect.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var photos = (from ph in db.dbPhotos
                             orderby ph.Date descending
                         select ph).Take(15); ;
            ViewBag.Photos = photos;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            if (TempData.ContainsKey("notFound"))
            {
                ViewBag.notFound = TempData["notFound"].ToString();
            }
            return View();
           
        }
        
    }
}