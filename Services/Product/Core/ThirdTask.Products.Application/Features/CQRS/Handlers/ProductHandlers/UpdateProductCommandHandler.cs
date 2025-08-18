using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Products.Application.Features.CQRS.Commands.ProductCommands;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Domain.Entities;

namespace ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers
{
    public class UpdateProductCommandHandler
    {
        private readonly IRepository<Product> _repository;
        private readonly IDistributedCache _cache;

        public UpdateProductCommandHandler(IRepository<Product> repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }
        public async Task Handle(UpdateProductCommand command)
        {
            var values = await _repository.GetByIdAsync(command.Id);
            values.Description = command.Description;
            values.Price = command.Price;
            values.ProductName = command.ProductName;
            values.Status = command.Status;
            values.Brand = command.Brand;
            await _repository.UpdateAsync(values);
            await _cache.RemoveAsync("getProductList");
            await _cache.RemoveAsync($"getProductById:{command.Id}");
        }
    }
}
