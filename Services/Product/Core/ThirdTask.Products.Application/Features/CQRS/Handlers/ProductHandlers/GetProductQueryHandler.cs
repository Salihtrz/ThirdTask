using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThirdTask.Products.Application.Features.CQRS.Results.ProductResults;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Domain.Entities;

namespace ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers
{
    public class GetProductQueryHandler
    {
        private readonly IRepository<Product> _repository;
        private readonly IDistributedCache _cache;
        private readonly ILogService _logService;

        public GetProductQueryHandler(IRepository<Product> repository,IDistributedCache cache,ILogService logService)
        {
            _repository = repository;
            _cache = cache;
            _logService = logService;
        }

        public async Task<List<GetProductQueryResult>> Handle()
        {
            string cacheKey = "getProductList";

            // CACHE kontrolü
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                await _logService.LogAsync("ProductService", "INFO", "CACHE HIT: Product list fetched from Redis.");
                return JsonConvert.DeserializeObject<List<GetProductQueryResult>>(cachedData);
            }

            await _logService.LogAsync("ProductService", "WARNING", "CACHE MISS: Product list will be fetched from database.");

            // Veritabanından ürünlerin alınması
            var products = await _repository.GetAllAsync();
            if (products == null || !products.Any())
            {
                await _logService.LogAsync("ProductService", "ERROR", "No products found in the database.");
                return new List<GetProductQueryResult>();
            }

            var result = products
                .OrderBy(x => x.Id)
                .Select(x => new GetProductQueryResult
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Brand = x.Brand,
                    Description = x.Description,
                    Price = x.Price,
                    Status = x.Status
                })
                .ToList();

            // Cache’e ekleme
            var serializedResult = JsonConvert.SerializeObject(result);
            await _cache.SetStringAsync(cacheKey, serializedResult, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
            });

            await _logService.LogAsync("ProductService", "INFO", "Product list cached successfully.");
            return result;
        }
    }
}
