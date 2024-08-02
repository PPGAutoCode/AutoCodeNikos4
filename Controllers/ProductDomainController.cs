
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductDomainController : ControllerBase
    {
        private readonly IProductDomainService _productDomainService;

        public ProductDomainController(IProductDomainService productDomainService)
        {
            _productDomainService = productDomainService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProductDomain([FromBody] Request<CreateProductDomainDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productDomainService.CreateProductDomain(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetProductDomain([FromBody] Request<ProductDomainRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productDomainService.GetProductDomain(request.Payload);
                return Ok(new Response<ProductDomain> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateProductDomain([FromBody] Request<UpdateProductDomainDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productDomainService.UpdateProductDomain(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProductDomain([FromBody] Request<DeleteProductDomainDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productDomainService.DeleteProductDomain(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListProductDomain([FromBody] Request<ListProductDomainRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _productDomainService.GetListProductDomain(request.Payload);
                return Ok(new Response<List<ProductDomain>> { Payload = result });
            });
        }
    }
}
