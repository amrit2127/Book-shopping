using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2_Ecomm_116.Models.ViewModels
{
    public class DateWiseOrderVM
    {
        public IEnumerable<OrderHeader> OrderHeaders { get; set; }
        public Dictionary<string, int> WeeklySummary { get; set; }
        public Dictionary<string, int> MonthSummary { get; set; }
        public string SelectedInterval { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
