using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using Project_2_Ecomm_116.Models.ViewModels;
using Project_2_Ecomm_116.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace Project_2_Ecomm_116.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string searchBy, string search)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }

            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");           
            if (!string.IsNullOrEmpty(searchBy))
            {
                searchBy = searchBy.ToLower();
                if(search=="Title")
                {
                    productList = productList.Where(s => s.Title.ToLower().Contains(searchBy));
                }
                else if(search=="Author")
                {
                    productList = productList.Where(s => s.Author.ToLower().Contains(searchBy));
                }
                else
                {
                    productList = productList.Where(m => m.Title.ToLower().Contains(searchBy) || m.Author.ToLower().Contains(searchBy));
                }
            }           
            return View(productList);
        }
        public ActionResult Details(int id)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            var productInDb = _unitOfWork.Product.FirstOrDefault(x => x.Id == id,
                includeProperties: "Category,CoverType");
            if (productInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                ProductId = id,
                Product = productInDb
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return NotFound();
                shoppingCart.ApplicationUserId = claim.Value;

                var shoppingCartFromDb = _unitOfWork.ShoppingCart.FirstOrDefault
                    (sc => sc.ApplicationUserId == claim.Value && sc.ProductId == shoppingCart.ProductId);
                if (shoppingCartFromDb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartFromDb.Count += shoppingCart.Count;
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefault(x => x.Id == shoppingCart.Id,
                                             includeProperties: "Category,CoverType");
                if (productInDb == null) return NotFound();
                var shoppingCartEdit = new ShoppingCart()
                {
                    ProductId = shoppingCart.Id,
                    Product = productInDb
                };
                return View(shoppingCartEdit);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}