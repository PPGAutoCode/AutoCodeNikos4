
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GettingStartedCompletedConditionController : ControllerBase
    {
        private readonly IGettingStartedCompletedConditionService _service;

        public GettingStartedCompletedConditionController(IGettingStartedCompletedConditionService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGettingStartedCompletedCondition([FromBody] Request<CreateGettingStartedCompletedConditionDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.CreateGettingStartedCompletedCondition(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetGettingStartedCompletedCondition([FromBody] Request<GettingStartedCompletedConditionRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.GetGettingStartedCompletedCondition(request.Payload);
                return Ok(new Response<GettingStartedCompletedCondition> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateGettingStartedCompletedCondition([FromBody] Request<UpdateGettingStartedCompletedConditionDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.UpdateGettingStartedCompletedCondition(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteGettingStartedCompletedCondition([FromBody] Request<DeleteGettingStartedCompletedConditionDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.DeleteGettingStartedCompletedCondition(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListGettingStartedCompletedCondition([FromBody] Request<ListGettingStartedCompletedConditionRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.GetListGettingStartedCompletedCondition(request.Payload);
                return Ok(new Response<List<GettingStartedCompletedCondition>> { Payload = result });
            });
        }
    }
}
