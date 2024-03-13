using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;
using ShradhaBookStore.Models;
using System.Net;

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
        public IActionResult Show_Users()
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            var users = bookStoreContext.Users.ToList(); 
            ViewData["msg"] = TempData["msg"];
            return View(users);
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
            var categories = bookStoreContext.Categories.Where(x => x.Status != 1).ToList(); // 1 Means "Disabled"
            ViewData["msg"] = TempData["msg"];
            return View(categories);
        }
        public IActionResult Update_Category(int id)
        {
            var category = bookStoreContext.Categories.First(x=>x.Id == id);
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
            category.Status = 1 ; // Assuming there's a Status property in your Category model
            bookStoreContext.Categories.Update(category);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Category Disabled Successfully";

            return RedirectToAction("Show_Categories"); // Redirect to the categories list or any other suitable action
        }
        [HttpPost]
        public IActionResult ConfirmDisable(int CategoryId)
        {
            var category = bookStoreContext.Categories.First(x => x.Id == CategoryId);
            var subs = bookStoreContext.Categories.Where(x => x.ParentCategoryId == CategoryId).ToList();
            var products = bookStoreContext.Products.Where(x => x.CategoryId == CategoryId).ToList();

            // Disable the current category
            category.Status = 1;
            bookStoreContext.Categories.Update(category);// Assuming there's a Status property in your Category model
            bookStoreContext.SaveChanges();

            // If there are subcategories or products, disable them as well
            foreach (var subCategory in subs)
            {
                subCategory.Status = 1;
                bookStoreContext.Categories.Update(subCategory);// Assuming there's a Status property in your Category model
            }
            foreach (var product in products)
            {
                product.Status = 1; // Assuming there's a Status property in your Product model
                bookStoreContext.Products.Update(product);// Assuming there's a Status property in your Category model
            }
            bookStoreContext.SaveChanges();

            // Redirect to a suitable action
            return RedirectToAction("Show_Categories"); // Redirect to the categories list or any other suitable action
        }


        public IActionResult Activate_Category(int id)
        {
            var categories = bookStoreContext.Categories.First(x => x.Id == id);
            if (categories == null)
            {
                TempData["msg"] = "Category Not Found";
                return RedirectToAction("Show_De_Categories");
            }
            categories.Status = 0;
            bookStoreContext.Categories.Update(categories);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Category Activated";
            return RedirectToAction("Show_De_Categories");
        }


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

            var manufacturers = bookStoreContext.Manufacturers
                                              
                                               .Select(c => new SelectListItem
                                               {
                                                   Value = c.Id.ToString(),
                                                   Text = c.Name // Assuming there's a property called Name for category
                                               })
                                               .ToList();
            ViewBag.CategoryList = categories;
            ViewBag.ManufacturerList = manufacturers;
            var last_id = bookStoreContext.Products.OrderByDescending(x => x.Id).FirstOrDefault();
            @ViewBag.AcronymsId = "0001";
            if (last_id != null )
            {
                @ViewBag.AcronymsId = last_id.Id +1;

            }
            return View();
        }
        [HttpPost]
        public IActionResult Add_Product(Product product)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = Product_Image(product);
                product.Image = uniquefilename;
                //Get Manufacturers Abbreviation and Concat With its ID
                var manufacturer = bookStoreContext.Manufacturers.Find(product.ManufacturerId);
                product.Acronym = manufacturer.Acronyms + product.Acronym;
                bookStoreContext.Products.Add(product);
                bookStoreContext.SaveChanges();
                return RedirectToAction("Show_Products");
            }
            return RedirectToAction("Add_Product");
        }
        private string Product_Image(Product category)
        {
            string uniquefilename = string.Empty;
            if (category.ImageFile != null)
            {
                string folder = Path.Combine(webHostEnvironment.WebRootPath, "Images", "Products");
                uniquefilename = Guid.NewGuid().ToString() + "_" + category.ImageFile.FileName;
                string filepath = Path.Combine(folder, uniquefilename);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    category.ImageFile.CopyTo(fileStream);
                }
            }
            return uniquefilename;
        }
        public IActionResult Show_Products()
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            ViewData["msg"] = TempData["msg"];
            Allproduct allproduct = new Allproduct();   
            var products = bookStoreContext.Products.Where(x=>x.Status != 1).ToList();
            var categories = bookStoreContext.Categories.ToList();
            var manufacturers = bookStoreContext.Manufacturers.ToList();
            allproduct.Products = products;
            allproduct.Categories = categories;
            allproduct.Manufacturers = manufacturers;
            return View(allproduct);
        }
        public IActionResult Update_Product(int id)
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            var product = bookStoreContext.Products.First(x => x.Id == id);

            var categories = bookStoreContext.Categories
                                               .Where(x => x.ParentCategoryId != null)
                                                .Select(c => new SelectListItem
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.Name // Assuming there's a property called Name for category
                                                })
                                                .ToList();

            var manufacturers = bookStoreContext.Manufacturers

                                               .Select(c => new SelectListItem
                                               {
                                                   Value = c.Id.ToString(),
                                                   Text = c.Name // Assuming there's a property called Name for category
                                               })
                                               .ToList();
            ViewBag.CategoryList = categories;
            ViewBag.ManufacturerList = manufacturers;

            return View(product);
        }
        [HttpPost]
        public IActionResult Update_Product(Product product)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = Product_Image(product);
                product.Image = uniquefilename;
                bookStoreContext.Products.Update(product);
                bookStoreContext.SaveChanges();
                TempData["msg"] = "Product Updated Successfully";
                return RedirectToAction("Show_Products");
            }
            return RedirectToAction("Update_Product");
        }
        public IActionResult Deactivate_Product(int id)
        {
            var products = bookStoreContext.Products.First(x=>x.Id == id);
            if (products == null)
            {
                TempData["msg"] = "Product Not Found";
                return RedirectToAction("Show_Products");
            }
            products.Status = 1;
            bookStoreContext.Products.Update(products);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Product Deactivated";
            return RedirectToAction("Show_Products");
        }
        public IActionResult Activate_Product(int id)
        {
            var products = bookStoreContext.Products.First(x => x.Id == id);
            if (products == null)
            {
                TempData["msg"] = "Product Not Found";
                return RedirectToAction("Show_Products");
            }
            products.Status = 0;
            bookStoreContext.Products.Update(products);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Product Activated";
            return RedirectToAction("Show_De_Products");
        }
        public IActionResult Add_Feature(int id)
        {
            var products = bookStoreContext.Products.First(x => x.Id == id);
            if (products == null)
            {
                TempData["msg"] = "Product Not Found";
                return RedirectToAction("Show_Products");
            }
            products.Status = 2;
            bookStoreContext.Products.Update(products);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Product Added To Featured";
            return RedirectToAction("Show_Products");
        }
        public IActionResult Remove_feature(int id)
        {
            var products = bookStoreContext.Products.First(x => x.Id == id);
            if (products == null)
            {
                TempData["msg"] = "Product Not Found";
                return RedirectToAction("Show_Products");
            }
            products.Status = 0;
            bookStoreContext.Products.Update(products);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Product Removed from Featured";
            return RedirectToAction("Show_Products");
        }
        public IActionResult Show_De_Products()
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            ViewData["msg"] = TempData["msg"];
            Allproduct allproduct = new Allproduct();
            var products = bookStoreContext.Products.Where(x => x.Status == 1).ToList();
            var categories = bookStoreContext.Categories.ToList();
            var manufacturers = bookStoreContext.Manufacturers.ToList();
            allproduct.Products = products;
            allproduct.Categories = categories;
            allproduct.Manufacturers = manufacturers;
            return View(allproduct);
        }
        public IActionResult Show_De_Categories()
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            var categories = bookStoreContext.Categories.Where(x => x.Status == 1).ToList();
            ViewData["msg"] = TempData["msg"];
            return View(categories);
        }
        public IActionResult Add_Faq()
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            ViewBag.faqs = bookStoreContext.Faqs.ToList();
            ViewData["msg"] = TempData["msg"];
            return View();
        }
        [HttpPost]
        public IActionResult Add_Faq(Faq faq)
        {
            if (ModelState.IsValid)
            {
                bookStoreContext.Faqs.Add(faq);
                bookStoreContext.SaveChanges();
                return RedirectToAction("Add_Faq");
            }
            return View();
        }
        public IActionResult Delete_Faq(int id)
        {
            if (HttpContext.Session.GetString("adminsession") == null)
            {
                TempData["Error"] = "Please Login first";
                return RedirectToAction("Login", "User");
            }
            var faq = bookStoreContext.Faqs.Find(id);
            if (faq == null) 
            {
                TempData["msg"] = "No Faq Found";
                return RedirectToAction("Add_Faq");
            }
            bookStoreContext.Faqs.Remove(faq);
            bookStoreContext.SaveChanges();

            TempData["msg"] = "Succesffully Deleted";
            return RedirectToAction("Add_Faq");
        }

    }
}
