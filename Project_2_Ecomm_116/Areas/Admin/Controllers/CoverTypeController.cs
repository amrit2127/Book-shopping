using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2_Ecomm_116.DataAccess.Data;
using Project_2_Ecomm_116.DataAccess.Repository;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using Project_2_Ecomm_116.Utility;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
       private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
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
            var covertypeList = _unitOfWork.CoverType.GetAll();
            return Json(new {data=covertypeList});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var covertypeInDb = _unitOfWork.CoverType.Get(id);
            if (covertypeInDb == null)
                return Json(new { success = false, message = "Something Went Wrong while Delete Data!!!" });
            _unitOfWork.CoverType.Remove(covertypeInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Deleted Successfully!!!" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType coverType=new CoverType();
            if (id == null) return View(coverType);
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if(coverType==null) return NotFound();
            return View(coverType); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            if (coverType.Id == 0)
                _unitOfWork.CoverType.Add(coverType);
            else
                _unitOfWork.CoverType.Update(coverType);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
