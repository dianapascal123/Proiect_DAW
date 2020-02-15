using proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proiect.Controllers
{
    
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index()
        {
            //var articles = from article in db.Articles   
            //select article;   

            var categories = from categ in db.dbCategories
                             orderby categ.CategoryName
                             select categ;
            ViewBag.Categories = categories;

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

        public ActionResult Show(int id)
        {
            try
            {
                Category category = db.dbCategories.Find(id);
                if (category == null)
                {
                    TempData["notFound"] = "Category not found";
                    return RedirectToAction("Index");
                }

                var photos = from photo in db.dbPhotos
                             where photo.CategoryId == id
                             orderby photo.Date descending
                             select photo;
                ViewBag.Photos = photos;

                ViewBag.afisareButoane = false;
                if (User.IsInRole("Administrator"))
                {
                    ViewBag.afisareButoane = true;
                }

                ViewBag.esteAdmin = User.IsInRole("Administrator");

                return View(category);
            }
            catch(Exception e)
            {
                return View();
            }
            
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult New()
        {
            Category categorie = new Category();


            return View(categorie);

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult New(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.dbCategories.Add(category); //insert
                    db.SaveChanges(); //commit
                    TempData["message"] = "Category was succesfully added.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(category);
                }
            }
            catch (Exception e)
            {
                return View();
            }

        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {

            Category category = db.dbCategories.Find(id);
            if (category == null)
            {
                TempData["notFound"] = "Category not found";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.dbCategories.Find(id);
                    if (category == null)
                    {
                        TempData["notFound"] = "Category not found";
                        return RedirectToAction("Index");
                    }
                    if (TryUpdateModel(category))
                    {
                        category.CategoryName = requestCategory.CategoryName;

                        db.SaveChanges();
                    }
                    TempData["message"] = "Category was succesfully modified";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }
            }

            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            try
            {
                Category category = db.dbCategories.Find(id);
                if (category == null)
                {
                    TempData["notFound"] = "Category not found";
                    return RedirectToAction("Index");
                }
                db.dbCategories.Remove(category);
                db.SaveChanges();
                TempData["message"] = "Category was succesfully deleted";

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

        }


    }
}