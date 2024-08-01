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
            return Ok(await SafeExecutor.ExecuteAsync(() => _appStatusService.CreateAppStatus(createAppStatusDto)));
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAppStatus([FromBody] AppStatusRequestDto appStatusRequestDto)
        {
            return Ok(await SafeExecutor.ExecuteAsync(() => _appStatusService.GetAppStatus(appStatusRequestDto)));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAppStatus([FromBody] UpdateAppStatusDto updateAppStatusDto)
        {
            return Ok(await SafeExecutor.ExecuteAsync(() => _appStatusService.UpdateAppStatus(updateAppStatusDto)));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAppStatus([FromBody] DeleteAppStatusDto deleteAppStatusDto)
        {
            return Ok(await SafeExecutor.ExecuteAsync(() => _appStatusService.DeleteAppStatus(deleteAppStatusDto)));
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAppStatus([FromBody] ListAppStatusRequestDto listAppStatusRequestDto)
        {
            return Ok(await SafeExecutor.ExecuteAsync(() => _appStatusService.GetListAppStatus(listAppStatusRequestDto)));
        }
    }
}
