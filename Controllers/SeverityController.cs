
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeverityController : ControllerBase
    {
        private readonly ISeverityService _severityService;

        public SeverityController(ISeverityService severityService)
        {
            _severityService = severityService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSeverity([FromBody] Request<CreateSeverityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.CreateSeverity(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSeverity([FromBody] Request<SeverityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.GetSeverity(request.Payload);
                return Ok(new Response<Severity> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSeverity([FromBody] Request<UpdateSeverityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.UpdateSeverity(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSeverity([FromBody] Request<DeleteSeverityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.DeleteSeverity(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSeverity([FromBody] Request<ListSeverityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _severityService.GetListSeverity(request.Payload);
                return Ok(new Response<List<Severity>> { Payload = result });
            });
        }
    }
}
