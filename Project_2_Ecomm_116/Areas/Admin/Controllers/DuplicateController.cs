using Microsoft.AspNetCore.Mvc;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using Project_2_Ecomm_116.Utility;
using System.Security.Claims;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DuplicateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DuplicateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var applicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(x => x.Id == claim.Value);
            var userAllOrders = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeader.ApplicationUser.Id == applicationUser.Id, 
                includeProperties: "OrderHeader,Product");
            return View(userAllOrders);
        }

        public IActionResult Duplicate(int id)
        {
            IEnumerable<OrderDetail> order = _unitOfWork.OrderDetail.GetAll(o => o.OrderHeader.Id == id,includeProperties:"OrderHeader,Product");
                                                                          //includeProperties "OrderHeader,Product") ;

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.Id = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return NotFound();
            shoppingCart.ApplicationUserId = claim.Value;
            foreach (var item in order)
            {
                shoppingCart.ProductId = item.ProductId;
                var shoppingCartFromDb = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.ApplicationUserId == claim.Value && sc.ProductId == item.ProductId);
                if (shoppingCartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                    _unitOfWork.Save();
                    shoppingCart = new ShoppingCart();
                    shoppingCart.ApplicationUserId = claim.Value;
                }
                else
                {
                    shoppingCartFromDb.Count += shoppingCart.Count;
                    _unitOfWork.Save();
                }
                if (claim != null)
                {
                    var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                    HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
                }

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
