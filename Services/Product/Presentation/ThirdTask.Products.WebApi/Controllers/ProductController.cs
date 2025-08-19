using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Diagnostics;
using ThirdTask.Products.Application.Features.CQRS.Commands.ProductCommands;
using ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers;
using ThirdTask.Products.Application.Features.CQRS.Queries.ProductQueries;
using ThirdTask.Products.Application.Interfaces;

namespace ThirdTask.Product.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CreateProductCommandHandler _createCommandHandler;
        private readonly UpdateProductCommandHandler _updateCommandHandler;
        private readonly RemoveProductCommandHandler _removeCommandHandler;
        private readonly GetProductByIdQueryHandler _getProductByIdQueryHandler;
        private readonly GetProductQueryHandler _getProductQueryHandler;
        private readonly ILogService _logService;

        public ProductController(
            GetProductQueryHandler getProductQueryHandler,
            GetProductByIdQueryHandler getProductByIdQueryHandler,
            RemoveProductCommandHandler removeCommandHandler,
            UpdateProductCommandHandler updateCommandHandler,
            CreateProductCommandHandler createCommandHandler,
            ILogService logService)
        {
            _getProductQueryHandler = getProductQueryHandler;
            _getProductByIdQueryHandler = getProductByIdQueryHandler;
            _removeCommandHandler = removeCommandHandler;
            _updateCommandHandler = updateCommandHandler;
            _createCommandHandler = createCommandHandler;
            _logService = logService;
        }

        [Authorize(Policy = "ReaderOrWriter")]
        [EnableRateLimiting("ProductRateLimit")]
        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            var sw = Stopwatch.StartNew();
            await _logService.LogAsync("ProductAPI", "INFO", $"GET /api/product called by {User?.Identity?.Name ?? "anonymous"}");

            var values = await _getProductQueryHandler.Handle();

            sw.Stop();
            await _logService.LogAsync("ProductAPI", "INFO", $"GET /api/product completed in {sw.ElapsedMilliseconds} ms");
            return Ok(values);
        }

        [Authorize(Policy = "ReaderOrWriter")]
        [EnableRateLimiting("ProductRateLimit")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var sw = Stopwatch.StartNew();
            await _logService.LogAsync("ProductAPI", "INFO", $"GET /api/product/{id} called by {User?.Identity?.Name ?? "anonymous"}");

            var values = await _getProductByIdQueryHandler.Handle(new GetProductByIdQuery(id));

            sw.Stop();
            await _logService.LogAsync("ProductAPI", "INFO", $"GET /api/product/{id} completed in {sw.ElapsedMilliseconds} ms");
            return Ok(values);
        }

        [Authorize(Policy = "Writer")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            await _logService.LogAsync("ProductAPI", "INFO", "POST /api/product called to create new product");
            await _createCommandHandler.Handle(command);
            await _logService.LogAsync("ProductAPI", "INFO", "Product created successfully");
            return Ok("product added successfully");
        }

        [Authorize(Policy = "Writer")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductCommand command)
        {
            await _logService.LogAsync("ProductAPI", "INFO", $"PUT /api/product called to update product {command.Id}");
            await _updateCommandHandler.Handle(command);
            await _logService.LogAsync("ProductAPI", "INFO", $"Product {command.Id} updated successfully");
            return Ok("product updated successfully");
        }

        [Authorize(Policy = "Writer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _logService.LogAsync("ProductAPI", "INFO", $"DELETE /api/product/{id} called");
            await _removeCommandHandler.Handle(new RemoveProductCommand(id));
            await _logService.LogAsync("ProductAPI", "INFO", $"Product {id} deleted successfully");
            return Ok("product deleted successfully");
        }
    }
}
