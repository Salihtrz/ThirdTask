using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public GetProductByIdQueryHandler(IRepository<Product> repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }
        public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery query)
        {
            string cacheKey = $"getProductById:{query.Id}";
            GetProductByIdQueryResult result;

            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                //Console.WriteLine($"CACHE HIT: Ürün {query.Id} Redis'ten geldi.");
                result = JsonConvert.DeserializeObject<GetProductByIdQueryResult>(cachedData);
                return result;
            }
            //Console.WriteLine($"CACHE MISS: Ürün {query.Id} veritabanından çekildi.");
            var values = await _repository.GetByIdAsync(query.Id);

            result = new GetProductByIdQueryResult
            {
                Brand = values.Brand,
                Description = values.Description,
                Id = values.Id,
                Price = values.Price,
                ProductName = values.ProductName,
                Status = values.Status
            };

            var serializeData = JsonConvert.SerializeObject(result);
            await _cache.SetStringAsync(cacheKey, serializeData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
            });
            return result;
        }
    }
}
