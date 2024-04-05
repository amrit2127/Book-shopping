using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Utility;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PendingOrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PendingOrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs

        [HttpGet]
        public IActionResult GetAll()
        {
           // var pending = _unitOfWork.OrderHeader.GetAll().Where(a => a.OrderStatus == SD.OrderStatusPending).ToList();
            var pending = _unitOfWork.OrderHeader.GetAll(a => a.OrderStatus == "Pending").ToList();
            if (pending == null)
                return Json(new { success = false, message = "Something went wrong!!!" });
            return Json(new { data = pending });
        }

        public IActionResult ViewDetail(int id)
        {
            var orders = _unitOfWork.OrderHeader.FirstOrDefault(e => e.Id == id);
            return View(orders);
        }
        #endregion


    }
}
