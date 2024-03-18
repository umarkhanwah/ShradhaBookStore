using Azure.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using ShradhaBookStore.Models;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShradhaBookStore.Controllers
{
    public class UserController : Controller
    {
        public readonly ShradhaBookStoreContext bookStoreContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        public UserController(ShradhaBookStoreContext bookStoreContext , IWebHostEnvironment webHostEnvironment)
        {
            this.bookStoreContext = bookStoreContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["Name"] = HttpContext.Session.GetString("usersession");
            if (ViewData["Name"] != null)
            {

                var user_id = HttpContext.Session.GetInt32("usersession");
                var User = bookStoreContext.Users.Find(user_id);
                ViewData["count_cart"] = bookStoreContext.Carts.Where(x => x.UserId == user_id).Count();
                ViewData["Image"] = User.Image;
                ViewBag.orders = bookStoreContext.Orders.Where(x=>x.UserId==user_id).ToList();
            
            }
            Allproduct alldata = new Allproduct();
            var categories = bookStoreContext.Categories.Where(x=>x.ParentCategoryId == null).ToList();
            var products = bookStoreContext.Products.ToList();
            var manufacturers = bookStoreContext.Manufacturers.ToList();
            alldata.Products = products;
            alldata.Manufacturers = manufacturers;
            alldata.Categories = categories;
            return View(alldata);
        }
        public IActionResult Faqs()
        {
            ViewData["Name"] = HttpContext.Session.GetString("usersession");
            if (ViewData["Name"] != null)
            {

                var user_id = HttpContext.Session.GetInt32("usersession");
                var User = bookStoreContext.Users.Find(user_id);
                ViewData["count_cart"] = bookStoreContext.Carts.Where(x => x.UserId == user_id).Count();

                ViewData["Image"] = User.Image;
                ViewBag.orders = bookStoreContext.Orders.Where(x => x.UserId == user_id).ToList();

            }
            var faq = bookStoreContext.Faqs.ToList();
            return View(faq);
        }
        public IActionResult Sub_Categories(int id ,  string heading)
        {

            ViewData["Name"] = HttpContext.Session.GetString("usersession");
            if (ViewData["Name"] != null)
            {

                var user_id = HttpContext.Session.GetInt32("usersession");
                var User = bookStoreContext.Users.Find(user_id);
                ViewData["count_cart"] = bookStoreContext.Carts.Where(x => x.UserId == user_id).Count();

                ViewData["Image"] = User.Image;
                ViewBag.orders = bookStoreContext.Orders.Where(x => x.UserId == user_id).ToList();

            }
            var categories = bookStoreContext.Categories.Where(x => x.ParentCategoryId == id).ToList();
            var category = bookStoreContext.Categories.FirstOrDefault(x => x.Id == id);
            //This Method Returns Category And Products Data, If the User Select a category which has sub categories so show its subs, otherwise show its products
            var viewModel = new CategoryProductViewModel
            {

                Categories = categories,
                Products = bookStoreContext.Products.Where(x => x.CategoryId == id).ToList()

            };

            ViewData["Heading"] = categories.Count == 0 ? heading + "/" + category.Name : category.Name ;
            return View(viewModel);

        }
       
        public IActionResult Product_Info(int id , string heading)
        {
            ViewData["Name"] = HttpContext.Session.GetString("usersession");
            if (ViewData["Name"] != null)
            {

                var user_id = HttpContext.Session.GetInt32("usersession");
                var User = bookStoreContext.Users.Find(user_id);
                ViewData["count_cart"] = bookStoreContext.Carts.Where(x => x.UserId == user_id).Count();

                ViewData["Image"] = User.Image;
                ViewBag.orders = bookStoreContext.Orders.Where(x => x.UserId == user_id).ToList();

            }
            // Retrieve the product with the specified id from the database
            var product = bookStoreContext.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                // If the product with the specified id is not found, you may want to handle this case accordingly.
                return NotFound(); // Return a 404 Not Found response or handle the situation as per your application logic.
            }
            
            // You may also want to retrieve other data needed for your view.
            var categories = bookStoreContext.Categories.ToList();
            var manufacturer = bookStoreContext.Manufacturers.ToList();

            Allproduct allproducts = new Allproduct();
            allproducts.Products = new List<Product>() { product }; // Place the selected product in the list.
            allproducts.Manufacturers = manufacturer;
            allproducts.Categories = categories;
            ViewData["Heading"] = new { 
                id = id,
                Name = heading 
            };
            ViewData["PName"] = product.Name;
            //Check If Product is Added To wishlist for this user
            var userid = HttpContext.Session.GetInt32("usersession");
            if(userid!= null)
            {
                ViewData["User_id"] = userid;
                var wishlist = bookStoreContext.Wishlists.FirstOrDefault(x => x.ProductId == id && x.UserId == userid);
                if (wishlist != null)
                {
                    ViewData["Wishlist"] = wishlist.Id;

                }
                var cart = bookStoreContext.Carts.FirstOrDefault(x => x.ProductId == id && x.UserId == userid);
                if (cart != null)
                {
                    ViewData["Cart"] = cart.Id;
                }
            }
            ViewData["msg"] = TempData["Msg"];
            
            return View(allproducts);
        }

        public IActionResult Add_to_Wishlist(int product)
        {
            var user_id = HttpContext.Session.GetInt32("usersession");
            if (user_id == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }
            Wishlist wishlist = new Wishlist();
            wishlist.ProductId = product;
            wishlist.UserId = user_id;
            bookStoreContext.Wishlists.Add(wishlist);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Product Added to Wishlist";
            return RedirectToAction("Product_info", new { id = product });
        }
        public IActionResult Add_to_Cart(int product,int price, int quantity)
        {
            var user_id = HttpContext.Session.GetInt32("usersession");
            if (user_id == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }
            Cart cart = new Cart();
            cart.ProductId = product;
            cart.UserId = user_id;
            cart.Quantity = quantity;
            cart.Total = price * quantity;
            bookStoreContext.Carts.Add(cart);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Product Added to Cart";
            return RedirectToAction("Product_info", new { id = product });
        }
        public IActionResult Remove_Wishlist(int wishid)
        {
            var user_id = HttpContext.Session.GetInt32("usersession");
            if (user_id == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }
            var wishlist = bookStoreContext.Wishlists.Find(wishid);
            if (wishlist == null)
            {
                TempData["msg"] = "No Wishlist found";
                return RedirectToAction("Login", "User");
            }
            var product = wishlist.ProductId;
            bookStoreContext.Wishlists.Remove(wishlist);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Removed From Wishlist Succesfully";
            return RedirectToAction("Product_info", new { id = product });
        }

        public IActionResult Show_Cart()
        {

            //We will Not get User id from url, so no user can see another user's cart by address,
            //We will Get User id in Backend
            var user_id = HttpContext.Session.GetInt32("usersession");
            if (user_id == null)
            {
                TempData["Error"] = "Please Login First";
                var User = bookStoreContext.Users.Find(user_id);
                ViewData["count_cart"] = bookStoreContext.Carts.Where(x => x.UserId == user_id).Count();

                ViewData["Image"] = User.Image;
                ViewBag.orders = bookStoreContext.Orders.Where(x => x.UserId == user_id).ToList();
                return RedirectToAction("Login", "User");
            }
            
            CartProducts cartProducts = new CartProducts();
            var carts = bookStoreContext.Carts.Where(x=>x.UserId == user_id).OrderByDescending(x => x.Id).ToList();
            var products = bookStoreContext.Products.ToList();
            cartProducts.Carts = carts;
            cartProducts.Products = products;
            ViewData["msg"] = TempData["msg"];
            return View(cartProducts);
        }

        public IActionResult Increment_Quantity(int cartid)
        {
            var cart = bookStoreContext.Carts.Find(cartid);

            if (cart == null)
            {
                TempData["msg"] = "No Cart found";
                return RedirectToAction("Show_Cart");
            }
            cart.Quantity += 1;
            bookStoreContext.Carts.Update(cart);
            bookStoreContext.SaveChanges();
            //TempData["msg"] = "Quantity Incremented";
            return RedirectToAction("Show_Cart");
        }
        public IActionResult Decrement_Quantity(int cartid)
        {
            var cart = bookStoreContext.Carts.Find(cartid);

            if (cart == null)
            {
                TempData["msg"] = "No Cart found";
                return RedirectToAction("Show_Cart");
            }
            cart.Quantity -= 1;
            bookStoreContext.Carts.Update(cart);
            bookStoreContext.SaveChanges();
            //TempData["msg"] = "Quantity Incremented";
            return RedirectToAction("Show_Cart");
        }
        public IActionResult Update_Cart(int cartid , int quantity)
        {
            var cart = bookStoreContext.Carts.Find(cartid);

            if (cart == null)
            {
                TempData["msg"] = "No Cart found";
                return RedirectToAction("Show_Cart");
            }
            cart.Quantity = quantity;
            bookStoreContext.Carts.Update(cart);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Quantity Updated";
            return RedirectToAction("Show_Cart");
        }
        public IActionResult Remove_Cart(int cartid) 
        {
            var user_id = HttpContext.Session.GetInt32("usersession");
            if (user_id == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }

            var cart = bookStoreContext.Carts.Find(cartid);
  
            if (cart == null)
            {
                TempData["msg"] = "No Cart found";
                return RedirectToAction("Show_Cart");
            }

            bookStoreContext.Carts.Remove(cart);
            bookStoreContext.SaveChanges();
            TempData["msg"] = "Removed From Cart Succesfully";
            return RedirectToAction("Show_Cart");
        }

        public IActionResult Checkout(int total)
        {
            var user_id = HttpContext.Session.GetInt32("usersession");
            if (user_id == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }
  
            var User = bookStoreContext.Users.Find(user_id);
            ViewData["count_cart"] = bookStoreContext.Carts.Where(x => x.UserId == user_id).Count();

            ViewData["Image"] = User.Image;
            ViewBag.orders = bookStoreContext.Orders.Where(x => x.UserId == user_id).ToList();

            
            CartProducts cartProducts = new CartProducts();
            var carts = bookStoreContext.Carts.Where(x=>x.UserId == user_id).ToList();
            var products = bookStoreContext.Products.ToList();
            cartProducts.Carts = carts;
            cartProducts.Products = products;
            
            var user = bookStoreContext.Users.Find(user_id);
            
            ViewData["UserDetails"] = new
            {
               Name = user.Username,
               Address = user.Address,
               Phone = user.Phone , 
               Total = total,
                // We Will Pass only Provided Data Of User Because of Data Security
            };
            ViewData["error"] = TempData["error"];
            return View(cartProducts);
        }
        [HttpPost]
        public IActionResult Checkout()
        {
            var user_id = HttpContext.Session.GetInt32("usersession");
            if (user_id == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }
            var name = Request.Form["name"];
            var phone = Request.Form["phone"];
            var address = Request.Form["address"];
            var postal_code = Request.Form["postal_code"];
            var total1 = Request.Form["total"];
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(postal_code))
            {
                TempData["error"] = "Fill All Credentials";
                return RedirectToAction("Checkout");
            }
            var total = int.Parse(total1);
            var cart = bookStoreContext.Carts.Where(x=>x.UserId == user_id).ToList();
            Order order = new Order();
            foreach (var item in cart)
            {
                
                order.ProductId = item.ProductId;
                order.UserId = item.UserId;
                order.PaidAmount = 0;
                order.Payment = total;
                order.Status = "Placed";
                order.Quantity = item.Quantity;
                order.Location = address;
                order.ReceiverName = name;
                order.PostalCode = postal_code;
                order.Phone = phone;
                bookStoreContext.Orders.Add(order);
                bookStoreContext.Carts.Remove(item);
            }
            bookStoreContext.SaveChanges();
            return RedirectToAction("Index");
        }
            public IActionResult Dashboard()
        {
            ViewData["Name"] = HttpContext.Session.GetString("usersession");
            if (ViewData["Name"] == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                string uniquefilename = User_Image(user);
                user.Image = uniquefilename;
                bookStoreContext.Users.Add(user);
                bookStoreContext.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }
        private string User_Image(User user)
        {
            string uniquefilename = string.Empty;
            if (user.ImageFile != null)
            {
                string folder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Users");
                uniquefilename = Guid.NewGuid().ToString() + "_" + user.ImageFile.FileName;
                string filepath = Path.Combine(folder, uniquefilename);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    user.ImageFile.CopyTo(fileStream);
                }
            }
            return uniquefilename;
        }

        public IActionResult Add_Review(int product_id)
        {
            ViewData["Name"] = HttpContext.Session.GetString("usersession");
            if (ViewData["Name"] == null)
            {
                TempData["Error"] = "Please Login First";
                return RedirectToAction("Login", "User");
            }
            var user_id = HttpContext.Session.GetInt32("usersession");
            var User = bookStoreContext.Users.Find(user_id);
            ViewData["count_cart"] = bookStoreContext.Carts.Where(x => x.UserId == user_id).Count();

            ViewData["Image"] = User.Image;
            ViewBag.orders = bookStoreContext.Orders.Where(x => x.UserId == user_id).ToList();
            ViewData["Product_id"] = product_id;
            return View();
        }
        [HttpPost]
        public IActionResult Add_Review(Review review)
        {
            if (ModelState.IsValid)
            {
                review.UserId = HttpContext.Session.GetInt32("usersession");
                bookStoreContext.Reviews.Add(review);
                bookStoreContext.SaveChanges();
                TempData["msg"] = "Successfully Added";
                return RedirectToAction("Show_Products");
            }
            TempData["msg"] = "Model State not valid";
            return RedirectToAction("Show_Products");
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("adminsession") != null)
            {   
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (HttpContext.Session.GetString("usersession") != null)
            {
                return RedirectToAction("Index");
            }

            ViewData["Error"] = TempData["Error"];
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            var myuser = bookStoreContext.Users.Where(x=>x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
           
            if (myuser != null)
            {
                if (myuser.Usertype  == "admin")
                {
                    HttpContext.Session.SetString("adminsession" , myuser.Username);
                    HttpContext.Session.SetInt32("usersession", myuser.Id);
                    return RedirectToAction("Dashboard" , "Admin");
                }
                else if (myuser.Usertype == "user")
                {
                    HttpContext.Session.SetString("usersession", myuser.Username);
                    HttpContext.Session.SetInt32("usersession", myuser.Id);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["Error"] = "InValid UserType";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                TempData["Error"] = "InValid Credentials";
                return RedirectToAction("Login");

            }
        }

        //public IActionResult Logout()
        //{
        //    if (HttpContext.Session.GetString("usersession") != null)
        //    {
        //        HttpContext.Session.Remove("usersession");
        //        return RedirectToAction("Login");
        //    }
        //    if (HttpContext.Session.GetString("adminsession") != null)
        //    {
        //        HttpContext.Session.Remove("adminsession");
        //        return RedirectToAction("Login");
        //    }
        //    else
        //    {
        //        TempData["Error"] = "No User Found To Logout";
        //        return RedirectToAction("Login");
        //    }
        //}
        public IActionResult Logout()
        {
            // Remove session data
            HttpContext.Session.Remove("adminsession");
            HttpContext.Session.Remove("usersession");

            // Redirect to login page
            return RedirectToAction("Login");
        }




        //Functions End
    }
}
