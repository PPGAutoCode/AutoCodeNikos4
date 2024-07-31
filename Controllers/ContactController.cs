
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateContact([FromBody] Request<CreateContactDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _contactService.CreateContact(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetContact([FromBody] Request<ContactRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _contactService.GetContact(request.Payload);
                return Ok(new Response<Contact> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateContact([FromBody] Request<UpdateContactDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _contactService.UpdateContact(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteContact([FromBody] Request<DeleteContactDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _contactService.DeleteContact(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListContact([FromBody] Request<ListContactRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _contactService.GetListContact(request.Payload);
                return Ok(new Response<List<Contact>> { Payload = result });
            });
        }
    }
}
