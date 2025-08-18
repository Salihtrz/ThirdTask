using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ThirdTask.Products.Application.Features.CQRS.Commands.ProductCommands;
using ThirdTask.Products.Application.Features.CQRS.Handlers.ProductHandlers;
using ThirdTask.Products.Application.Features.CQRS.Queries.ProductQueries;

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

        public ProductController(GetProductQueryHandler getProductQueryHandler, GetProductByIdQueryHandler getProductByIdQueryHandler, RemoveProductCommandHandler removeCommandHandler, UpdateProductCommandHandler updateCommandHandler, CreateProductCommandHandler createCommandHandler)
        {
            _getProductQueryHandler = getProductQueryHandler;
            _getProductByIdQueryHandler = getProductByIdQueryHandler;
            _removeCommandHandler = removeCommandHandler;
            _updateCommandHandler = updateCommandHandler;
            _createCommandHandler = createCommandHandler;
        }
        [Authorize(Policy = "ReaderOrWriter")]
        [EnableRateLimiting("ProductRateLimit")]
        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            var values = await _getProductQueryHandler.Handle();
            return Ok(values);
        }
        [Authorize(Policy = "ReaderOrWriter")]
        [EnableRateLimiting("ProductRateLimit")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductList(int id)
        {
            var values = await _getProductByIdQueryHandler.Handle(new GetProductByIdQuery(id));
            return Ok(values);
        }
        [Authorize(Policy = "Writer")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            await _createCommandHandler.Handle(command);
            return Ok("product added successfully");
        }
        [Authorize(Policy = "WriterUpdateProduct")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductCommand command)
        {
            await _updateCommandHandler.Handle(command);
            return Ok("product updated successfully");
        }
        [Authorize(Policy = "Writer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _removeCommandHandler.Handle(new RemoveProductCommand(id));
            return Ok("product deleted successfully");
        }
    }
}
