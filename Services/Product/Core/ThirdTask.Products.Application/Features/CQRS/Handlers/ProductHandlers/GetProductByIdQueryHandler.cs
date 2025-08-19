using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using ThirdTask.Products.Application.Features.CQRS.Queries.ProductQueries;
using ThirdTask.Products.Application.Features.CQRS.Results.ProductResults;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Domain.Entities;

namespace ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers
{
    public class GetProductByIdQueryHandler
    {
        private readonly IRepository<Product> _repository;
        private readonly IDistributedCache _cache;
        private readonly ILogService _logService;

        public GetProductByIdQueryHandler(IRepository<Product> repository,IDistributedCache cache,ILogService logService)
        {
            _repository = repository;
            _cache = cache;
            _logService = logService;
        }

        public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery query)
        {
            string cacheKey = $"getProductById:{query.Id}";

            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                await _logService.LogAsync("ProductService", "INFO", $"CACHE HIT: Product {query.Id} retrieved from Redis.");
                return JsonConvert.DeserializeObject<GetProductByIdQueryResult>(cachedData);
            }

            await _logService.LogAsync("ProductService", "WARNING", $"CACHE MISS: Product {query.Id} will be retrieved from database.");

            var product = await _repository.GetByIdAsync(query.Id);
            if (product == null)
            {
                await _logService.LogAsync("ProductService", "ERROR", $"Product {query.Id} not found in database.");
                return null;
            }

            var result = new GetProductByIdQueryResult
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Brand = product.Brand,
                Description = product.Description,
                Price = product.Price,
                Status = product.Status
            };

            var serializedResult = JsonConvert.SerializeObject(result);
            await _cache.SetStringAsync(cacheKey, serializedResult, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
            });

            await _logService.LogAsync("ProductService", "INFO", $"Product {query.Id} added to cache.");
            return result;
        }
    }
}
