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
        private readonly SafeExecutor _safeExecutor;

        public AppStatusController(IAppStatusService appStatusService, SafeExecutor safeExecutor)
        {
            _appStatusService = appStatusService;
            _safeExecutor = safeExecutor;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAppStatus([FromBody] Request<CreateAppStatusDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _appStatusService.CreateAppStatus(request.Payload)));
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAppStatus([FromBody] Request<AppStatusRequestDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _appStatusService.GetAppStatus(request.Payload)));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAppStatus([FromBody] Request<UpdateAppStatusDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _appStatusService.UpdateAppStatus(request.Payload)));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAppStatus([FromBody] Request<DeleteAppStatusDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _appStatusService.DeleteAppStatus(request.Payload)));
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAppStatus([FromBody] Request<ListAppStatusRequestDto> request)
        {
            return await _safeExecutor.ExecuteAsync(async () => 
                Ok(await _appStatusService.GetListAppStatus(request.Payload)));
        }
    }
}