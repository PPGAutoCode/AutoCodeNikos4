
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProductCategory([FromBody] Request<CreateProductCategoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productCategoryService.CreateProductCategory(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetProductCategory([FromBody] Request<ProductCategoryRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productCategoryService.GetProductCategory(request.Payload);
                return Ok(new Response<ProductCategory> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateProductCategory([FromBody] Request<UpdateProductCategoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productCategoryService.UpdateProductCategory(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProductCategory([FromBody] Request<DeleteProductCategoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productCategoryService.DeleteProductCategory(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListProductCategory([FromBody] Request<ListProductCategoryRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productCategoryService.GetListProductCategory(request.Payload);
                return Ok(new Response<List<ProductCategory>> { Payload = result });
            });
        }
    }
}
