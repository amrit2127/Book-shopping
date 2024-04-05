using Microsoft.AspNetCore.Mvc;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApprovedOrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApprovedOrderController(IUnitOfWork unitOfWork)
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
            var approved = _unitOfWork.OrderHeader.GetAll(a => a.OrderStatus == "Approved").ToList();
            if (approved == null)
                return Json(new { success = false, message = "Something went wrong!!!" });
            return Json(new { data = approved });
        }

        public IActionResult ViewDetail(int id)
        {
            var orders = _unitOfWork.OrderHeader.FirstOrDefault(e => e.Id == id);
            return View(orders);
        }
        #endregion
    }
}
