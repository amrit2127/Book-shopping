using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_2_Ecomm_116.DataAccess.Data;
using Project_2_Ecomm_116.DataAccess.Repository;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using Project_2_Ecomm_116.Models.ViewModels;
using System.Security.Claims;
using Twilio.Rest.Trunking.V1;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SimilarProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public SimilarProductController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
           
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();

            // Create a SelectList for the dropdown
            var productList = new SelectList(products, "Id", "Title");

            // Pass the SelectList to the view
            ViewBag.ProductList = productList;
            return View();
        }

        public IActionResult Details(int id)
        {
            // Retrieve a single product by ID
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            

            //***
            //List<Product> GetProducts(ApplicationDbContext context)
            //{
            //    var products=_context.Products.ToList();
            //    return products;
            //}

            List<Product> GetSimilarProducts(ApplicationDbContext context, int mostSellingProductId)
            {
                // Replace Product with your actual model and OrderDetails with your actual table
                var mostSellingProductOrders = context.OrderDetails
                    .Where(od => od.ProductId == mostSellingProductId)
                    .Select(od => od.OrderHeaderId)
                    .ToList();

                var similarProducts = context.OrderDetails
                    .Where(od => mostSellingProductOrders.Contains(od.OrderHeaderId) && od.ProductId != mostSellingProductId)
                    .GroupBy(od => od.ProductId)
                    .OrderByDescending(group => group.Count())
                    .Select(group => new
                    {
                        ProductId = group.Key,
                        CoOccurrenceCount = group.Count()
                    })
                    .Take(5)
                    .ToList();

                // Retrieve the actual products based on the similar product ids

                var similarProductIds = similarProducts.Select(sp => sp.ProductId).ToList();
                var products = context.Products
                    .Where(p => similarProductIds.Contains(p.Id))
                    .ToList();

                return products;
            }

            //***
            var mostSellingProduct = _context.OrderDetails
            .GroupBy(od => od.ProductId)
            .OrderByDescending(group => group.Sum(od => od.Count))
            .Select(group => group.Key)
            .FirstOrDefault();

            if (mostSellingProduct != null)
            {
               // var product = _context.Products.FirstOrDefault(p => p.Id == id);
                var similarProducts = GetSimilarProducts(_context, mostSellingProduct);
                return View(similarProducts);
            }

            return View(product);
        }

        public IActionResult Search(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
           
            return View(product);
        }
        
    }
}
