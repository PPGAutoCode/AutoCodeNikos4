using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllowedGrantTypeController : ControllerBase
    {
        private readonly IAllowedGrantTypeService _allowedGrantTypeService;

        public AllowedGrantTypeController(IAllowedGrantTypeService allowedGrantTypeService)
        {
            _allowedGrantTypeService = allowedGrantTypeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAllowedGrantType([FromBody] Request<CreateAllowedGrantTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.CreateAllowedGrantType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAllowedGrantType([FromBody] Request<AllowedGrantTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.GetAllowedGrantType(request.Payload);
                return Ok(new Response<AllowedGrantType> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAllowedGrantType([FromBody] Request<UpdateAllowedGrantTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.UpdateAllowedGrantType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAllowedGrantType([FromBody] Request<DeleteAllowedGrantTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.DeleteAllowedGrantType(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAllowedGrantType([FromBody] Request<ListAllowedGrantTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.GetListAllowedGrantType(request.Payload);
                return Ok(new Response<List<AllowedGrantType>> { Payload = result });
            });
        }
    }
}
