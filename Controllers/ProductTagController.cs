
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTagController : ControllerBase
    {
        private readonly IProductTagService _productTagService;

        public ProductTagController(IProductTagService productTagService)
        {
            _productTagService = productTagService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProductTag([FromBody] Request<CreateProductTagDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productTagService.CreateProductTag(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetProductTag([FromBody] Request<ProductTagRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productTagService.GetProductTag(request.Payload);
                return Ok(new Response<ProductTag> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateProductTag([FromBody] Request<UpdateProductTagDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productTagService.UpdateProductTag(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProductTag([FromBody] Request<DeleteProductTagDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productTagService.DeleteProductTag(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListProductTag([FromBody] Request<ListProductTagRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productTagService.GetListProductTag(request.Payload);
                return Ok(new Response<List<ProductTag>> { Payload = result });
            });
        }
    }
}
