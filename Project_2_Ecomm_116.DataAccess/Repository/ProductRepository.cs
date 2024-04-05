using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class ProductRepository:Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context):base(context)
        {
           _context = context;
        }

        public List<Product> GetProductsByTitle(string title)
        {
            return _context.Products
            .Where(p => p.Title.Contains(title))
            .ToList();
        }

        public List<Product> GetSimilarProducts(int productId)
        {
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                return _context.Products
                    .Where(p => p.Category == product.Category && p.Id != productId)
                    .Take(5) // Adjust as needed, this is just an example
                    .ToList();
            }

            return new List<Product>();
        }
    }
}
