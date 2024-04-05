using Microsoft.AspNetCore.Mvc;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models.ViewModels;
using System.Globalization;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DateWiseOrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DateWiseOrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var viewModel = new DateWiseOrderVM
            {
                OrderHeaders = _unitOfWork.OrderHeader.GetAll(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                SelectedInterval = "empty"
            };
            return View(viewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public IActionResult Index(DateTime startDate, DateTime endDate, DateWiseOrderVM date)
        {
            DateWiseOrderVM viewModel = new DateWiseOrderVM();
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.OrderHeaders = _unitOfWork.OrderHeader.GetAll();
            viewModel.OrderHeaders = viewModel.OrderHeaders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToList();
            if (date.SelectedInterval == "Weekly")
            {
                viewModel.WeeklySummary = viewModel.OrderHeaders
             .GroupBy(o => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(o.OrderDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday))
             .ToDictionary(g => "Week " + g.Key, g => g.Count());
            }
            if (date.SelectedInterval == "Monthly")
            {
                viewModel.MonthSummary = viewModel.OrderHeaders
               .GroupBy(o => o.OrderDate.Month)
               .ToDictionary(g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key), g => g.Count());
            }

            return View("Index", viewModel);
        }




    }
}
