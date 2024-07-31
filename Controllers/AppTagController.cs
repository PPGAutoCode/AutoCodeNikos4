
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppTagController : ControllerBase
    {
        private readonly IAppTagService _appTagService;

        public AppTagController(IAppTagService appTagService)
        {
            _appTagService = appTagService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAppTag([FromBody] Request<CreateAppTagDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appTagService.CreateAppTag(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAppTag([FromBody] Request<AppTagRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appTagService.GetAppTag(request.Payload);
                return Ok(new Response<AppTag> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAppTag([FromBody] Request<UpdateAppTagDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appTagService.UpdateAppTag(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAppTag([FromBody] Request<DeleteAppTagDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appTagService.DeleteAppTag(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAppTag([FromBody] Request<ListAppTagRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appTagService.GetListAppTag(request.Payload);
                return Ok(new Response<List<AppTag>> { Payload = result });
            });
        }
    }
}
