using Microsoft.AspNet.Identity;
using proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace proiect.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Add(int id)
        {
            Photo photo = db.dbPhotos.Find(id);
            if (photo == null)
            {
                TempData["notFound"] = "Photo not found";
                return Redirect("~/");
            }
            Comment com = new Comment();

            // Preluam ID-ul utilizatorului curent 
            com.UserId = User.Identity.GetUserId();

            // Adaugam id-ul pozei
            com.PhotoId = id;

            return View(com);

        }

        [HttpPost]
        public ActionResult Add(Comment com)
        {
            
            try
            {
                if (ModelState.IsValid)
                {

                    db.dbComments.Add(com); //insert
                    db.SaveChanges(); //commit
                    TempData["message"] = "Comment was added.";
                    //return RedirectToAction("Show","Photo",new { PhotoId=com.PhotoId});
                    return RedirectToAction("Show", new RouteValueDictionary(new { controller = "Photo", action = "Show", id = com.PhotoId }));
                    //return Redirect("photo/show/com.PhotoId");
                }
                else
                {
                    return View(com);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult Edit(int id)
        {
            try
            {
                Comment com = db.dbComments.Find(id);
                Photo photo = db.dbPhotos.Find(com.PhotoId);
                if (com == null)
                {
                    TempData["notFound"] = "Comment not found";
                    return Redirect("~/");
                }
                ViewBag.Comment = com;

                //verifica daca userul logat poate face aceste operatii
                if (com.UserId == User.Identity.GetUserId() || photo.UserId == User.Identity.GetUserId())
                {
                    return View(com);
                }
                else
                {
                    TempData["message"] = "You don`t have the right to do edit this comment!";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }


        }


        [HttpPut]
        
        public ActionResult Edit(int id, Comment requestComment)
        {

            try
            {
                Comment com = db.dbComments.Find(id);
                Photo photo = db.dbPhotos.Find(com.PhotoId);

                if (ModelState.IsValid)
                {

                    if (com.UserId == User.Identity.GetUserId() || photo.UserId == User.Identity.GetUserId())
                    {
                        if (TryUpdateModel(com))
                        {

                            com.Content = requestComment.Content;
                            com.Date = requestComment.Date;
                            db.SaveChanges();
                            TempData["message"] = "Comment was modified.";
                        }

                        return RedirectToAction("Show", "Photo", new { id = com.PhotoId });
                    }
                    else
                    {
                        TempData["message"] = "You don`t have the right to edit this comment!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return View(requestComment);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            } 
                
        }

        //[HttpDelete]
   
        public ActionResult Delete(int id)
        {
            try
            {
                Comment com = db.dbComments.Find(id);
                Photo photo = db.dbPhotos.Find(com.PhotoId);
                if (com.UserId == User.Identity.GetUserId() || photo.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
                {
                    db.dbComments.Remove(com);
                    db.SaveChanges();
                    TempData["message"] = "Comments was deleted";
                    return RedirectToAction("Show", "Photo", new { id = com.PhotoId });
                }
                else
                {
                    TempData["message"] = "You don`t have the right";
                    return RedirectToAction("Show", "Photo", new { id = com.PhotoId });
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

        }

    }
}