using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2_Ecomm_116.DataAccess.Repository
{
    public class ProductsService:IProductsService
    {
        private readonly IProductRepository _productRepository;

        public ProductsService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetSimilarProducts(string productTitle)
        {
            // Get the product by title
            var product = _productRepository.GetProductsByTitle(productTitle).FirstOrDefault();

            if (product != null)
            {
                // Get similar products based on criteria (e.g., same category)
                return _productRepository.GetSimilarProducts(product.Id);
            }

            return new List<Product>();
        }

        
    }
}
