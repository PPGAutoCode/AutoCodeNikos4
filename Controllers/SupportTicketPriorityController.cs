
using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportTicketPriorityController : ControllerBase
    {
        private readonly ISupportTicketPriorityService _supportTicketPriorityService;

        public SupportTicketPriorityController(ISupportTicketPriorityService supportTicketPriorityService)
        {
            _supportTicketPriorityService = supportTicketPriorityService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSupportTicketPriority([FromBody] Request<CreateSupportTicketPriorityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketPriorityService.CreateSupportTicketPriority(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetSupportTicketPriority([FromBody] Request<SupportTicketPriorityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketPriorityService.GetSupportTicketPriority(request.Payload);
                return Ok(new Response<SupportTicketPriority> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSupportTicketPriority([FromBody] Request<UpdateSupportTicketPriorityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketPriorityService.UpdateSupportTicketPriority(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSupportTicketPriority([FromBody] Request<DeleteSupportTicketPriorityDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketPriorityService.DeleteSupportTicketPriority(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListSupportTicketPriority([FromBody] Request<ListSupportTicketPriorityRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _supportTicketPriorityService.GetListSupportTicketPriority(request.Payload);
                return Ok(new Response<List<SupportTicketPriority>> { Payload = result });
            });
        }
    }
}
