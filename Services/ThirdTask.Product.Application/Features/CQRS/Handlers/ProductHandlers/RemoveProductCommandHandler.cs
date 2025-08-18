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
    public class RemoveProductCommandHandler
    {
        private readonly IRepository<Product> _repository;
        private readonly IDistributedCache _cache;

        public RemoveProductCommandHandler(IRepository<Product> repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }
        public async Task Handle(RemoveProductCommand command)
        {
            var value = await _repository.GetByIdAsync(command.Id);
            await _repository.DeleteAsync(value);
            await _cache.RemoveAsync("getProductList");
            await _cache.RemoveAsync($"getProductById:{command.Id}");
        }
    }
}
