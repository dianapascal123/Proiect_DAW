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
    public class PhotoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        [NonAction]
        public IEnumerable<Comment> GetAllComments(int PhotoId)
        {
            // generam o lista goala             
            var commentList = new List<Comment>();

            // Extragem toate categoriile din baza de date            
            var comments = from com in db.dbComments
                           where com.PhotoId==PhotoId
                           select com;

            commentList = comments.ToList();

            

            // returnam lista de categorii            
            return commentList;
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala             
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date            
            var categories = from cat in db.dbCategories
                             select cat;

            // iteram prin categorii             
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown                 
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                }
                                );
            }

            // returnam lista de categorii            
            return selectList;
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllAlbums()
        {
            // generam o lista goala             
            var selectList = new List<SelectListItem>();
            var idlogat = User.Identity.GetUserId();

            // Extragem toate categoriile din baza de date            
            var albums = from alb in db.dbAlbums
                            where alb.UserId== idlogat
                         select alb;

            // iteram prin categorii             
            foreach (var alb in albums)
            {
                // Adaugam in lista elementele necesare pentru dropdown                 
                selectList.Add(new SelectListItem
                {
                    Value = alb.AlbumId.ToString(),
                    Text = alb.AlbumTitle.ToString()
                }
                                );
            }

            // returnam lista de categorii            
            return selectList;
        }

        public ActionResult Show(int id)
        {
           
            Photo photo = db.dbPhotos.Find(id);
            if (photo == null)
            {
                TempData["notFound"] = "Photo not found";
                return Redirect("~/");
            }
           
            ViewBag.Comments = GetAllComments(id);

        
            ViewBag.afisareButoane = false;
            if (User.IsInRole("User") || User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");

            //ca sa afiseze mesajul TempData primit din alta functie
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(photo);
            
            
        }

        [Authorize(Roles = "Administrator, User")]
        public ActionResult AddPhoto()
        {
            Photo photo = new Photo();

            photo.Categories = GetAllCategories();
            photo.Albums = GetAllAlbums();

            photo.UserId = User.Identity.GetUserId();
            return View(photo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, User")]
        public ActionResult AddPhoto(Photo ph, string avatarCropped)
        {
            try
            {
                string filePath = ProcessImage(avatarCropped);
                ph.PhotoFile = filePath;

                ph.Categories = GetAllCategories();
                ph.Albums = GetAllAlbums();
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.dbPhotos.Add(ph);
                        db.SaveChanges();

                        TempData["message"] = "Photo was added";
                        //return RedirectToAction("Index");
                        return RedirectToAction("Show", "Album", new { id = ph.AlbumId });
                    }
                    else
                    {
                        return View(ph);
                    }
                }
                catch (Exception e)
                {
                    return View(ph);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }


        }

        private string ProcessImage(string croppedImage)
        {
            string filePath = String.Empty;
            try
            {
                string base64 = croppedImage;
                byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);
                filePath = "/Images/Photo/Ph-" + Guid.NewGuid() + ".png";
                using (FileStream stream = new FileStream(Server.MapPath(filePath), FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                throw ex;
            }

            return filePath;
        }

        [Authorize(Roles = "Administrator, User")]
        public ActionResult Edit(int id)
        {
            Photo photo = db.dbPhotos.Find(id);
            if (photo == null)
            {
                TempData["notFound"] = "Photo not found";
                return Redirect("~/");
            }

            photo.Categories = GetAllCategories();
            photo.Albums = GetAllAlbums();

            ViewBag.Photo = photo;

            //verifica daca userul logat poate face aceste operatii
            if (photo.UserId == User.Identity.GetUserId())
            {
                return View(photo);
            }
            else
            {
                TempData["message"] = "You do not  have the right to modify this photo";
                return RedirectToAction("ShowOwn","Profile");
            }


        }

        [HttpPut]
        [Authorize(Roles = "Administrator, User")]
        public ActionResult Edit(int id, Photo requestPhoto)
        {
            try
            {
                requestPhoto.Categories = GetAllCategories();
                requestPhoto.Albums = GetAllAlbums();
                try
                {
                    if (ModelState.IsValid)
                    {
                        Photo photo = db.dbPhotos.Find(id);
                        if (photo == null)
                        {
                            TempData["notFound"] = "Photo not found";
                            return Redirect("~/");
                        }
                        if (photo.UserId == User.Identity.GetUserId())
                        {
                            if (TryUpdateModel(photo))
                            {
                                photo.Description = requestPhoto.Description;
                                photo.CategoryId = requestPhoto.CategoryId;
                                photo.AlbumId = requestPhoto.AlbumId;

                                db.SaveChanges();

                            }
                            TempData["message"] = "Photo was succesfully modified";
                            return RedirectToAction("ShowOwn", "Profile");

                        }



                        else
                        {
                            TempData["message"] = "You do not have the right to modify this photo";
                            return RedirectToAction("Show", "Album", new { id = photo.AlbumId });
                        }
                    }
                    else
                    {
                        return View(requestPhoto);
                    }
                }
                catch (Exception e)
                {
                    return View(requestPhoto);
                }
            }
            catch(Exception e)
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
                Photo photo = db.dbPhotos.Find(id);
                if (photo == null)
                {
                    TempData["notFound"] = "Photo not found";
                    return Redirect("~/");
                }
                if (photo.UserId == User.Identity.GetUserId() ||
                    User.IsInRole("Administrator"))
                {
                    db.dbPhotos.Remove(photo);
                    db.SaveChanges();
                    TempData["message"] = "Photo was succesfully deleted";
                    return RedirectToAction("Show", "Album", new { id = photo.AlbumId });
                }
                else
                {
                    TempData["message"] = "You do not have the right to delete this photo";
                    return RedirectToAction("ShowOwn", "Profile");
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
           
        }

        public ActionResult IndexSearchedPhotos(string SearchText)
        {
            
                var model = from ph in db.dbPhotos
                            select ph;
            
            if (!string.IsNullOrEmpty(SearchText))
            {
                var result = model.Where(s => s.Description.Contains(SearchText));
                return View(result.ToList());
            }
            return View(model);
        }

    }
}