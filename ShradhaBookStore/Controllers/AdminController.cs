﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShradhaBookStore.Models;

namespace ShradhaBookStore.Controllers
{
    public class AdminController : Controller
    {
        public readonly ShradhaBookStoreContext bookStoreContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AdminController(ShradhaBookStoreContext bookStoreContext, IWebHostEnvironment webHostEnvironment)
        {
            this.bookStoreContext = bookStoreContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            ViewData["Name"] = HttpContext.Session.GetString("adminsession");
            return View();
        }
        public IActionResult Add_Category()
        {
            var categoriesWithoutParent = bookStoreContext.Categories
                                               .Where(x => x.ParentCategoryId == null)
                                                .Select(c => new SelectListItem
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.Name 
                                                })
                                                .ToList();

            ViewBag.ParentCategoryList = categoriesWithoutParent;

            return View();
        }
        [HttpPost]
        public IActionResult Add_Category(Category category)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = Category_Image(category);
                category.Image = uniquefilename;
                bookStoreContext.Categories.Add(category);
                bookStoreContext.SaveChanges();
                return RedirectToAction("Show_Categories");
            }
            return RedirectToAction("Add_Category");
        }
        private string Category_Image(Category category)
        {
            string uniquefilename = string.Empty;
            if (category.ImageFile != null)
            {
                string folder = Path.Combine(webHostEnvironment.WebRootPath, "Images", "Categories");
                uniquefilename = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
                string filepath = Path.Combine(folder, uniquefilename);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    category.ImageFile.CopyTo(fileStream);
                }
            }
            return uniquefilename;
        }
        public IActionResult Show_Categories()
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            var categories = bookStoreContext.Categories.ToList();
            return View(categories);
        }
        public IActionResult Update_Category(int id)
        {
            var category = bookStoreContext.Categories.Find(id);
            var categoriesWithoutParent = bookStoreContext.Categories
                                               .Where(x => x.ParentCategoryId == null)
                                                .Select(c => new SelectListItem
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.Name // Assuming there's a property called Name for category
                                                })
                                                .ToList();

            ViewBag.ParentCategoryList = categoriesWithoutParent;

            return View(category);
        }
        [HttpPost]
        public IActionResult Update_Category(Category category)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = Category_Image(category);
                category.Image = uniquefilename;
                bookStoreContext.Categories.Update(category);
                bookStoreContext.SaveChanges();
                return RedirectToAction("Show_Categories");
            }
            return RedirectToAction("Update_Category");
        }

        public IActionResult Deactivate_Category(int id)
        {
            var category = bookStoreContext.Categories.FirstOrDefault(x=>x.Id == id);
            var subs = bookStoreContext.Categories.Where(x => x.ParentCategoryId == id).ToList();
            var products = bookStoreContext.Products.Where(x => x.CategoryId == id).ToList();

            if (subs.Count > 0 || products.Count > 0)
            {
                ViewData["CategoryId"] = id;
                if (subs.Count > 0)
                {
                    ViewData["msg"] = "This Category Has Sub Categories. Do you want to disable them all?";
                }
                else // products.Count > 0
                {
                    ViewData["msg"] = "This Category Has Products. Do you want to disable them all?";
                }

                return View();
            }

            // If there are no subcategories or products, directly disable the category
            category.Status = "Disabled"; // Assuming there's a Status property in your Category model
            bookStoreContext.Categories.Update(category);
            bookStoreContext.SaveChanges();

            return RedirectToAction("Show_Categories"); // Redirect to the categories list or any other suitable action
        }
        [HttpPost]
        public IActionResult ConfirmDisable(int CategoryId)
        {
            var category = bookStoreContext.Categories.Find(CategoryId);
            var subs = bookStoreContext.Categories.Where(x => x.ParentCategoryId == CategoryId).ToList();
            var products = bookStoreContext.Products.Where(x => x.CategoryId == CategoryId).ToList();

            // Disable the current category
            category.Status = "Disabled";
            bookStoreContext.Categories.Update(category);// Assuming there's a Status property in your Category model
            bookStoreContext.SaveChanges();

            // If there are subcategories or products, disable them as well
            foreach (var subCategory in subs)
            {
                subCategory.Status = "Disabled";
                bookStoreContext.Categories.Update(subCategory);// Assuming there's a Status property in your Category model
            }
            foreach (var product in products)
            {
                product.Status = "Disabled"; // Assuming there's a Status property in your Product model
                bookStoreContext.Products.Update(product);// Assuming there's a Status property in your Category model
            }
            bookStoreContext.SaveChanges();

            // Redirect to a suitable action
            return RedirectToAction("Index", "Categories"); // Redirect to the categories list or any other suitable action
        }


        //public IActionResult Disable_Category(int id)
        //{
        //    var category = bookStoreContext.Categories.Find(id);
        //    var subs = bookStoreContext.Categories.Where(x=> x.ParentCategoryId == id).ToList();
        //    var products = bookStoreContext.Products.Where(x=> x.CategoryId== id).ToList();
        //    if(subs.Count > 0)
        //    {
        //        ViewData["msg"] = "This Category Has Sub Categories , Do You Wan't to Disable Them All ?";
        //        return RedirectToAction();
        //    }
        //    else if (products.Count > 0)
        //    {
        //        ViewData["msg"] = "This Category Has Products , Do You Wan't to Disable Them All ?";
        //        return RedirectToAction();
        //    }
        //    return View();
        //}


        public IActionResult Add_Product()
        {
            var categories = bookStoreContext.Categories
                                               .Where(x => x.ParentCategoryId != null)
                                                .Select(c => new SelectListItem
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.Name // Assuming there's a property called Name for category
                                                })
                                                .ToList();

            ViewBag.CategoryList = categories;

            return View();
        }
        [HttpPost]
        public IActionResult Add_Product(Product product)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = Product_Image(product);
                product.Image = uniquefilename;
                bookStoreContext.Products.Add(product);
                bookStoreContext.SaveChanges();
                return RedirectToAction("Show_Categories");
            }
            return RedirectToAction("Add_Category");
        }
        private string Product_Image(Product category)
        {
            string uniquefilename = string.Empty;
            if (category.ImageFile != null)
            {
                string folder = Path.Combine(webHostEnvironment.WebRootPath, "Images", "Categories");
                uniquefilename = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
                string filepath = Path.Combine(folder, uniquefilename);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    category.ImageFile.CopyTo(fileStream);
                }
            }
            return uniquefilename;
        }

    }
}
