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
        public async Task<IActionResult> CreateAppStatus([FromBody] CreateAppStatusDto createAppStatusDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.CreateAppStatus(createAppStatusDto);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAppStatus([FromBody] AppStatusRequestDto appStatusRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.GetAppStatus(appStatusRequestDto);
                return Ok(new Response<AppStatus>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAppStatus([FromBody] UpdateAppStatusDto updateAppStatusDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.UpdateAppStatus(updateAppStatusDto);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAppStatus([FromBody] DeleteAppStatusDto deleteAppStatusDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.DeleteAppStatus(deleteAppStatusDto);
                return Ok(new Response<bool>(result));
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAppStatus([FromBody] ListAppStatusRequestDto listAppStatusRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _appStatusService.GetListAppStatus(listAppStatusRequestDto);
                return Ok(new Response<List<AppStatus>>(result));
            });
        }
    }
}
