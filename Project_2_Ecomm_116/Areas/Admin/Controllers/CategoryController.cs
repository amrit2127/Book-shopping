using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using Project_2_Ecomm_116.Utility;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
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
            var categoryList = _unitOfWork.Category.GetAll();
            return Json(new {data=categoryList});   
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryIndb = _unitOfWork.Category.Get(id);
            if (categoryIndb == null)
                return Json(new { success = false, message = "Something went wrong while deleting Data!!!" });
            _unitOfWork.Category.Remove(categoryIndb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Deleted Successfully!!!" });  
        }
        #endregion
       
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null) return View(category);
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        
        public IActionResult Upsert(Category category)
        {
            if(category == null) return NotFound();

            // Check if the category already exists in the database
            var existingCategory = _unitOfWork.Category.FirstOrDefault(c => c.Name == category.Name);

            if (existingCategory != null && existingCategory.Id != category.Id)
            {
                ModelState.AddModelError("Name", "Category already exists.");
                // Return the view with the model to display the error message
                return View(category);
            }
            if (!ModelState.IsValid) return View(category);
            if(category.Id==0)
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
