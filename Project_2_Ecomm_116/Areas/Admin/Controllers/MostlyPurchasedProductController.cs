using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_2_Ecomm_116.DataAccess.Data;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models.ViewModels;
using System.Globalization;
using System.Security.Claims;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MostlyPurchasedProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public MostlyPurchasedProductController(IUnitOfWork unitOfWork,ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public IActionResult Index(DateTime startDate, DateTime endDate,string viewOption)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser?.FirstOrDefault(x => x.Id == claim.Value);

            //*** Show orders monthly after choosing date in calender***
            if (viewOption == "Monthly")
            {
                startDate = new DateTime(startDate.Year, startDate.Month, 1); // Start of the month
                endDate = startDate.AddMonths(1).AddDays(-1); // End of the month
            }
            else if (viewOption == "Yearly")
            {
                startDate = new DateTime(startDate.Year, 1, 1); // Start of the year
                endDate = startDate.AddYears(1).AddDays(-1); // End of the year
            }
            //***

            var productCounts = _unitOfWork.OrderDetail.GetAll(includeProperties: "OrderHeader,Product")
                .Where(od => od.OrderHeader.ApplicationUserId == user.Id && od.OrderHeader.OrderDate >= startDate && od.OrderHeader.OrderDate <= endDate)
                .GroupBy(od => od.Product.Id)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Count = group.Sum(e => e.Count)
                }).OrderByDescending(p => p.Count)
                .ToList();

            var uniqueProductCounts = productCounts
                .GroupBy(pc => _unitOfWork.Product.FirstOrDefault(p => p.Id == pc.ProductId)?.Title)
                .Select(group => new ProductCountVM
                {
                    ProductName = group.Key,
                    Count = group.Sum(pc => pc.Count)
                })
                .ToList();

            return View(new ProductsWithCountsViewModel
            {
                ProductCounts = uniqueProductCounts   // Count of single single products.....
            });

        }























        //*********
        public IActionResult FavProductByDropDownList()
        {
            List<PendingProducts> products = new List<PendingProducts>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(x => x.Id == claim.Value);
            var userPurchasedProducts = _unitOfWork.OrderDetail
                .GetAll(includeProperties: "Product,OrderHeader")
                .Where(od => od.OrderHeader.ApplicationUserId == user.Id)
                .GroupBy(od => od.Product.Id)
                .Select(g => new { ProductId = g.Key, Count = g.Count() });

            foreach (var purchasedProduct in userPurchasedProducts)
            {
                var product = _unitOfWork.Product.FirstOrDefault(x => x.Id == purchasedProduct.ProductId);

                if (product != null)
                {
                    products.Add(new PendingProducts
                    {
                        ProductId = product.Id,
                        ProductName = product.Title
                    });
                }
            }
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName");
            return View();
        }
        [HttpPost]
        [ActionName("FavProductByDropDownList")]
        public IActionResult FavProduct(int productId)
        {
            List<PendingProducts> products = new List<PendingProducts>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(x => x.Id == claim.Value);
            var userPurchasedProducts = _unitOfWork.OrderDetail
                .GetAll(includeProperties: "Product,OrderHeader")
                .Where(od => od.OrderHeader.ApplicationUserId == user.Id)
                .GroupBy(od => od.Product.Id)
                .Select(g => new { ProductId = g.Key, Count = g.Count() });

            foreach (var purchasedProduct in userPurchasedProducts)
            {
                var product = _unitOfWork.Product.FirstOrDefault(x => x.Id == purchasedProduct.ProductId);

                if (product != null)
                {
                    products.Add(new PendingProducts
                    {
                        ProductId = product.Id,
                        ProductName = product.Title
                    });
                }
            }

            ViewBag.Products = new SelectList(products, "ProductId", "ProductName");
            //Find product By Id
            var selectedProduct = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").FirstOrDefault(x => x.Id == productId);
            ViewBag.SelectedProduct = selectedProduct;

            var productCounts = _unitOfWork.OrderDetail.GetAll(includeProperties: "OrderHeader,Product")
             .Where(od => od.OrderHeader.ApplicationUserId == user.Id && od.Product.Id == productId)
             .GroupBy(od => od.Product.Id)
             .Select(group => new
             {
                 Count = group.Sum(d => d.Count)
             }).OrderByDescending(p => p.Count)
             .ToList();
            ViewBag.ProductCounts = productCounts.Select(pc => pc.Count).ToList();
            return View();
        }
    }
}