using Project_2_Ecomm_116.DataAccess.Data;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2_Ecomm_116.DataAccess.Repository
{
    public class SuggestedProductsRepository : Repository<SuggestedProduct>, ISuggestedProductsRepository
    {
        private readonly ApplicationDbContext context;
        public SuggestedProductsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
