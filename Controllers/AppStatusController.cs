using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppStatusController : ControllerBase
    {
        private readonly IAppStatusService _appStatusService;

        public AppStatusController(IAppStatusService appStatusService)
        {
            _appStatusService = appStatusService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAppStatus([FromBody] Request<CreateAppStatusDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.CreateAppStatus(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAppStatus([FromBody] Request<AppStatusRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.GetAppStatus(request.Payload);
                return Ok(new Response<AppStatus> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAppStatus([FromBody] Request<UpdateAppStatusDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.UpdateAppStatus(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAppStatus([FromBody] Request<DeleteAppStatusDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.DeleteAppStatus(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAppStatus([FromBody] Request<ListAppStatusRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.GetListAppStatus(request.Payload);
                return Ok(new Response<List<AppStatus>> { Payload = result });
            });
        }
    }
}
