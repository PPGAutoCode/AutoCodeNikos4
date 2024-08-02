
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoLiveDeveloperTypeController : ControllerBase
    {
        private readonly IGoLiveDeveloperTypeService _service;

        public GoLiveDeveloperTypeController(IGoLiveDeveloperTypeService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGoLiveDeveloperType([FromBody] Request<CreateGoLiveDeveloperTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.CreateGoLiveDeveloperType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetGoLiveDeveloperType([FromBody] Request<GoLiveDeveloperTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.GetGoLiveDeveloperType(request.Payload);
                return Ok(new Response<GoLiveDeveloperType> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateGoLiveDeveloperType([FromBody] Request<UpdateGoLiveDeveloperTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.UpdateGoLiveDeveloperType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteGoLiveDeveloperType([FromBody] Request<DeleteGoLiveDeveloperTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.DeleteGoLiveDeveloperType(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListGoLiveDeveloperType([FromBody] Request<ListGoLiveDeveloperTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _service.GetListGoLiveDeveloperType(request.Payload);
                return Ok(new Response<List<GoLiveDeveloperType>> { Payload = result });
            });
        }
    }
}
