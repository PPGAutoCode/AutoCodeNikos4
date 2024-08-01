
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserTypeController : ControllerBase
    {
        private readonly IUserTypeService _userTypeService;

        public UserTypeController(IUserTypeService userTypeService)
        {
            _userTypeService = userTypeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserType([FromBody] Request<CreateUserTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userTypeService.CreateUserType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetUserType([FromBody] Request<UserTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userTypeService.GetUserType(request.Payload);
                return Ok(new Response<UserType> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserType([FromBody] Request<UpdateUserTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userTypeService.UpdateUserType(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUserType([FromBody] Request<DeleteUserTypeDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userTypeService.DeleteUserType(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListUserType([FromBody] Request<ListUserTypeRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _userTypeService.GetListUserType(request.Payload);
                return Ok(new Response<List<UserType>> { Payload = result });
            });
        }
    }
}
