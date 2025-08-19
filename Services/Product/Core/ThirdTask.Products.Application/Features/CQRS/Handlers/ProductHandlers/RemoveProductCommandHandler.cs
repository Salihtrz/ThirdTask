using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using ThirdTask.Products.Application.Features.CQRS.Commands.ProductCommands;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Domain.Entities;

namespace ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers
{
    public class RemoveProductCommandHandler
    {
        private readonly IRepository<Product> _repository;
        private readonly IDistributedCache _cache;
        private readonly ILogService _logService;

        public RemoveProductCommandHandler(IRepository<Product> repository,IDistributedCache cache,ILogService logService)
        {
            _repository = repository;
            _cache = cache;
            _logService = logService;
        }

        public async Task Handle(RemoveProductCommand command)
        {
            var product = await _repository.GetByIdAsync(command.Id);
            if (product == null)
            {
                await _logService.LogAsync("ProductService", "ERROR", $"Product with ID {command.Id} not found.");
                return;
            }

            await _repository.DeleteAsync(product);
            await _logService.LogAsync("ProductService", "INFO", $"Product '{product.ProductName}' (ID {product.Id}) deleted from database.");

            await _cache.RemoveAsync("getProductList");
            await _cache.RemoveAsync($"getProductById:{command.Id}");
            await _logService.LogAsync("ProductService", "WARNING", $"Cache invalidated for product list and product ID {command.Id}.");
        }
    }
}
