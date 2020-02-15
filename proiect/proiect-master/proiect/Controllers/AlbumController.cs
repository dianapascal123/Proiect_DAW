using Microsoft.AspNet.Identity;
using proiect.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiect.Controllers
{
    public class AlbumController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        [Authorize(Roles = "Administrator, User")]
        public ActionResult Index()
        {
            //var articles = from article in db.Articles   
            //select article;   
            var userId = User.Identity.GetUserId();
            var albums = from album in db.dbAlbums
                         where album.UserId == userId
                         orderby album.AlbumTitle
                         select album;
            ViewBag.Albums = albums;
            ViewBag.UserId = userId;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }


            return View();
        }

       

        public ActionResult Show(int id)
        {
            try
            {
                Album album = db.dbAlbums.Find(id);
                if (album == null)
                {
                    TempData["notFound"] = "Album not found";
                    return Redirect("~/");
                }
                var photos = from ph in db.dbPhotos
                             where ph.AlbumId == id
                             select ph;
                ViewBag.Photos = photos;

                ViewBag.afisareButoane = false;
                if (User.IsInRole("User") || User.IsInRole("Administrator"))
                {
                    ViewBag.afisareButoane = true;
                }

                ViewBag.esteAdmin = User.IsInRole("Administrator");
                ViewBag.utilizatorCurent = User.Identity.GetUserId();

                return View(album);
            }

            catch(Exception e)
            {
                return View();
            }
            
        }

        [Authorize(Roles = "Administrator, User")]
        public ActionResult New()
        {
            Album album = new Album();
            album.UserId = User.Identity.GetUserId();

            return View(album);

        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public ActionResult New(Album album)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.dbAlbums.Add(album); //insert
                    db.SaveChanges(); //commit
                    TempData["message"] = "Album was succesfully added.";
                    return RedirectToAction("ShowOwn", "Profile");
                }
                else
                {
                    return View(album);
                }
            }
            catch (Exception e)
            {
                return View(album);
            }

        }

        [Authorize(Roles = "Administrator, User")]
        public ActionResult Edit(int id)
        {

            Album album = db.dbAlbums.Find(id);
            if (album == null)
            {
                TempData["notFound"] = "Album not found";
                return RedirectToAction("ShowOwn", "Profile");
            }

            if (album.UserId == User.Identity.GetUserId())
            {
                return View(album);
            }
            else
            {
                TempData["message"] = "You do not have the right to modify an album that is not yours!";
                return RedirectToAction("ShowOwn", "Profile");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, User")]
        public ActionResult Edit(int id, Album requestAlbum)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Album album = db.dbAlbums.Find(id);

                    if (album == null)
                    {
                        TempData["notFound"] = "Album not found";
                        return Index();
                    }
                    if (album.UserId == User.Identity.GetUserId())
                    {
                        if (TryUpdateModel(album))
                        {
                            album.AlbumTitle = requestAlbum.AlbumTitle;

                            db.SaveChanges();
                        }
                        TempData["message"] = "Album was succesfully modified";
                        return RedirectToAction("ShowOwn", "Profile");
                    }
                    else
                    {
                        TempData["message"] = "You do not have the right to modify an album that is not yours!";
                        return RedirectToAction("ShowOwn", "Profile");
                    }
                        
                }
                else
                {
                    return View(requestAlbum);
                }
            }

            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator, User")]
        public ActionResult Delete(int id)
        {
            try
            {
                Album album = db.dbAlbums.Find(id);
                if (album == null)
                {
                    TempData["notFound"] = "Album not found";
                    return RedirectToAction("ShowOwn", "Profile");
                }
                if (album.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                {
                    db.dbAlbums.Remove(album);
                    db.SaveChanges();
                    TempData["message"] = "Album was succesfully deleted";
                }
                else
                {
                    TempData["message"] = "You do not have the right to delete an album that is not yours!";
                    return RedirectToAction("ShowOwn", "Profile");
                }


                return RedirectToAction("ShowOwn", "Profile");
            }

            catch(Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}