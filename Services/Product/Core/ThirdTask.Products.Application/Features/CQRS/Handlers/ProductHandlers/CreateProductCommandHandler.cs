using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Events.Dtos;
using ThirdTask.Products.Application.Features.CQRS.Commands.ProductCommands;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Domain.Entities;

namespace ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers
{
    public class CreateProductCommandHandler
    {
        private readonly IRepository<Product> _repository;
        private readonly IDistributedCache _cache;
        private readonly IEventPublisher _eventPublisher;

        public CreateProductCommandHandler(IRepository<Product> repository, IDistributedCache cache, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _cache = cache;
            _eventPublisher = eventPublisher;
        }
        public async Task Handle(CreateProductCommand command)
        {
            var product = new Product
            {
                Brand = command.Brand,
                Description = command.Description,
                Price = command.Price,
                ProductName = command.ProductName,
                Status = command.Status
            };

            await _repository.CreateAsync(product);
            await _cache.RemoveAsync("getProductList");

            await _eventPublisher.PublishAsync(new ProductCreatedEventDto
            {
                Id = product.Id,
                Brand = product.Brand,
                ProductName = product.ProductName,
                Price = product.Price,
                Status = product.Status
            });
        }
    }
}
