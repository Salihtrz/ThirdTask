using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using ThirdTask.Events.Dtos;
using ThirdTask.Products.Application.Features.CQRS.Commands.ProductCommands;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Domain.Entities;

public class CreateProductCommandHandler
{
    private readonly IRepository<Product> _repository;
    private readonly IDistributedCache _cache;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogService _logService;

    public CreateProductCommandHandler(IRepository<Product> repository,IDistributedCache cache,IEventPublisher eventPublisher,ILogService logService)
    {
        _repository = repository;
        _cache = cache;
        _eventPublisher = eventPublisher;
        _logService = logService;
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
        await _logService.LogAsync("ProductService", "INFO", $"Product '{product.ProductName}' created in the database.");

        await _cache.RemoveAsync("getProductList");
        await _logService.LogAsync("ProductService", "WARNING", "Cache invalidated for product list.");

        await _eventPublisher.PublishAsync(new ProductCreatedEventDto
        {
            Id = product.Id,
            Brand = product.Brand,
            ProductName = product.ProductName,
            Price = product.Price,
            Status = product.Status
        });
        await _logService.LogAsync("ProductService", "INFO", $"ProductCreatedEvent published for '{product.ProductName}'.");
    }
}
