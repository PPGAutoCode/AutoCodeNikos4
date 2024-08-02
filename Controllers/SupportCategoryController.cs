
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportCategoryController : ControllerBase
    {
        private readonly ISupportCategoryService _supportCategoryService;

        public SupportCategoryController(ISupportCategoryService supportCategoryService)
        {
            _supportCategoryService = supportCategoryService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSupportCategory([FromBody] Request<CreateSupportCategoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportCategoryService.CreateSupportCategory(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSupportCategory([FromBody] Request<SupportCategoryRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportCategoryService.GetSupportCategory(request.Payload);
                return Ok(new Response<SupportCategory> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSupportCategory([FromBody] Request<UpdateSupportCategoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportCategoryService.UpdateSupportCategory(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSupportCategory([FromBody] Request<DeleteSupportCategoryDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportCategoryService.DeleteSupportCategory(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSupportCategory([FromBody] Request<ListSupportCategoryRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportCategoryService.GetListSupportCategory(request.Payload);
                return Ok(new Response<List<SupportCategory>> { Payload = result });
            });
        }
    }
}
