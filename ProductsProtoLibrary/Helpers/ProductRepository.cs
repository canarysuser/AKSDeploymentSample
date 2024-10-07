using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductsProtoLibrary.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsProtoLibrary.Helpers
{
    public interface IProductRepositoryAsync
    {
        Task<List<Product>> GetAllProductsAsync(string criteria);
        Task<Product> GetProductByIdAsync(int productId);

    }
    public class ProductRepository : IProductRepositoryAsync
    {
        private readonly ProductDbContext _db;
        private ILogger<ProductRepository> _logger;
        public ProductRepository(ProductDbContext db,
            ILogger<ProductRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        private void WriteLogMessages(string message, bool onError = false)
        {
            if (onError)
                _logger.LogError("{0} ERROR: {1}", nameof(ProductRepository), message);
            else
                _logger.LogInformation("{0}: {1}", nameof(ProductRepository), message);

        }

        public async Task<List<Product>> GetAllProductsAsync(string criteria)
        {

            WriteLogMessages($"{nameof(GetAllProductsAsync)} stareted..");

            List<Product> products;
            if (criteria.Length > 0)
                products = await _db.Products
                    .AsNoTracking()
                    .Where(c => c.ProductName.Contains(criteria))
                    .ToListAsync();
            else
                products = await _db.Products
                    .AsNoTracking()
                    .ToListAsync();

            WriteLogMessages($"{nameof(GetAllProductsAsync)} completed..");
            return products;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            WriteLogMessages($"{nameof(GetProductByIdAsync)} stareted..");

            Product product = await _db.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.ProductId == productId);

            WriteLogMessages($"{nameof(GetProductByIdAsync)} completed..");
            return product!;
        }
    }
}
