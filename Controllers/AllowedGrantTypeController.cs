using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AllowedGrantTypeController : ControllerBase
    {
        private readonly IAllowedGrantTypeService _allowedGrantTypeService;

        public AllowedGrantTypeController(IAllowedGrantTypeService allowedGrantTypeService)
        {
            _allowedGrantTypeService = allowedGrantTypeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAllowedGrantType([FromBody] CreateAllowedGrantTypeDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.CreateAllowedGrantType(request);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetAllowedGrantType([FromBody] AllowedGrantTypeRequestDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.GetAllowedGrantType(request);
                return Ok(new Response<AllowedGrantType>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAllowedGrantType([FromBody] UpdateAllowedGrantTypeDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.UpdateAllowedGrantType(request);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAllowedGrantType([FromBody] DeleteAllowedGrantTypeDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.DeleteAllowedGrantType(request);
                return Ok(new Response<bool>(result));
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAllowedGrantType([FromBody] ListAllowedGrantTypeRequestDto request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _allowedGrantTypeService.GetListAllowedGrantType(request);
                return Ok(new Response<List<AllowedGrantType>>(result));
            });
        }
    }
}
