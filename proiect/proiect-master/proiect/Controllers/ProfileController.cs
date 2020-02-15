using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using proiect.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace proiect.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Administrator, User")]
        public ActionResult Edit()
        {
            var userId = User.Identity.GetUserId();
            var profile = (from dbProfile in db.dbProfiles
                           where dbProfile.ApplicationUserId == userId
                           select dbProfile).FirstOrDefault();
           
            if (profile == null)
            {
                TempData["notFound"] = "Not found";
                return Redirect("~/");
            }
            ViewBag.Profile = profile;
            if (profile.ApplicationUserId == User.Identity.GetUserId())
            {
                return View(profile);
            }
            else
            {
                TempData["message"] = "You do not  have the right to edit this profile";
                return Redirect("~/");
            }
           
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, User")]
        public ActionResult Edit(Profile requestProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId();
                    var profile = (from dbProfile in db.dbProfiles
                                      where dbProfile.ApplicationUserId == userId
                                      select dbProfile).FirstOrDefault();
                   
                    if (profile == null)
                    {
                        TempData["notFound"] = "Profile not found";
                        return Redirect("~/");
                    }
                    if (profile.ApplicationUserId == User.Identity.GetUserId())
                    {
                        if (TryUpdateModel(profile))
                        {
                            profile.Description = requestProfile.Description;
                            profile.Name = requestProfile.Name;
                            db.SaveChanges();
                        }
                        TempData["message"] = "Profile was succesfully modified";
                        return RedirectToAction("ShowOwn");
                    }
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return View(requestProfile);
                }
            }

            catch (Exception e)
            {
                return View();
            }
        }

        

        [Authorize(Roles = "Administrator, User")]
        public ActionResult ShowOwn()
        {
            var userId = User.Identity.GetUserId();
            var albums = from album in db.dbAlbums
                         where album.UserId == userId
                         orderby album.AlbumTitle
                         select album;
            var profile = (from dbProfile in db.dbProfiles
                           where dbProfile.ApplicationUserId == userId
                           select dbProfile).FirstOrDefault();

            ViewBag.profile = profile;
            ViewBag.albums = albums;
            //ViewBag.afisareButoane = true;
            ViewBag.utilizatorCurent = userId;
            
            //ca sa afiseze mesajul TempData primit din alta functie
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

        public ActionResult Show(string id)
        {

            try
            {
                var profile = (from dbProfile in db.dbProfiles
                               where dbProfile.ApplicationUserId == id
                               select dbProfile).FirstOrDefault();
                if(profile == null)
                {
                    RedirectToAction("Index", "Home");
                }
                var albums = from album in db.dbAlbums
                             where album.UserId == id
                             orderby album.AlbumTitle
                             select album;

                ViewBag.albums = albums;
                ViewBag.profile = profile;

                //ca sa afiseze mesajul TempData primit din alta functie
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
            catch(Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteUser(string id)
        {

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = await userManager.FindByIdAsync(id);


            if (User.IsInRole("Administrator"))
            {
                var profiles1 = from profile in db.dbProfiles
                                where profile.ApplicationUserId == id
                                select profile;

                foreach (var profile in profiles1)
                {
                    //sterge albumurile
                    var albums = from alb in db.dbAlbums
                                 where alb.UserId == id
                                 select alb;
                    foreach (var a in albums)
                    {
                        db.dbAlbums.Remove(a);
                        db.SaveChangesAsync();
                    }

                    //sterge comentariile
                    var coms = from com in db.dbComments
                               where com.UserId == id
                               select com;
                    foreach (var c in coms)
                    {
                        db.dbComments.Remove(c);
                        db.SaveChangesAsync();
                    }

                    db.dbProfiles.Remove(profile);
                    db.SaveChangesAsync();

                }

                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["message"] = "The account has been deleted";

                    return RedirectToAction("Index", "Home");

                }
                else
                    return RedirectToAction("Index", "Home");



            }

            else
            {
                TempData["message"] = "You can`t delete";
                return RedirectToAction("Index", "Home");
            }

        }

        [Authorize(Roles = "Administrator")]
        public ActionResult IndexUsers()
        {
          

            var users = from user in db.Users
                        where user.UserName != "admin@admin.com"
                        orderby user.UserName
                        select user;
            ViewBag.Users = users;

            //ca sa afiseze mesajul TempData primit din alta functie
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            if (TempData.ContainsKey("notFound"))
            {
                ViewBag.notFound = TempData["notFound"].ToString();
            }
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            return View();
        }

    }
}