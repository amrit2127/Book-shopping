using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2_Ecomm_116.Models.ViewModels
{
    public class YearlyMostSellingProductViewModel
    {
        public int Year { get; set; }
        public int MostSellingProductId { get; set; }
        public string MostSellingProductName { get; set; }
        public int QuantitySold { get; set; }
    }
}
