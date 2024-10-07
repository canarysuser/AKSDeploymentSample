using Grpc.Core;
using ProductsProtoLibrary.Grpc;
using ProductsProtoLibrary.Helpers;

namespace ProductGrpcService.Services
{
    public class ProductDbServiceImpl : ProductService.ProductServiceBase
    {
        private readonly ILogger<ProductDbServiceImpl> _logger;
        private readonly IProductRepositoryAsync _repository;
        public ProductDbServiceImpl(
            IProductRepositoryAsync repo,
            ILogger<ProductDbServiceImpl> logger
            )
        {
            _logger = logger;
            _repository = repo;
        }
        private void WriteLogMessages(string message, bool onError = false)
        {
            if (onError)
                _logger.LogError("{0} ERROR: {1}", nameof(ProductRepository), message);
            else
                _logger.LogInformation("{0}: {1}", nameof(ProductRepository), message);

        }
        
        public override async Task<ProductList> UnaryProductListing(FilterCriteriaInput request, ServerCallContext context)
        {
            WriteLogMessages($"{nameof(UnaryProductListing)} started..");
            var productList = await _repository.GetAllProductsAsync(request.Criteria);
            var pl = new ProductList();
            pl.Products.AddRange(productList);
            WriteLogMessages($"{nameof(UnaryProductListing)} ended..");
            return pl;
        }
        public async override Task<Product> UnaryGetProductDetails(DetailsInput request, ServerCallContext context)
        {
            WriteLogMessages($"{nameof(UnaryGetProductDetails)} started..");
            var productList = await _repository.GetProductByIdAsync(request.ProductId);
            
            WriteLogMessages($"{nameof(UnaryGetProductDetails)} ended..");
            return productList;
        }
    }
}
