
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProjectName.Types;
using ProjectName.Interfaces;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasicPageController : ControllerBase
    {
        private readonly IBasicPageService _basicPageService;

        public BasicPageController(IBasicPageService basicPageService)
        {
            _basicPageService = basicPageService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBasicPage([FromBody] Request<CreateBasicPageDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _basicPageService.CreateBasicPage(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetBasicPage([FromBody] Request<BasicPageRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _basicPageService.GetBasicPage(request.Payload);
                return Ok(new Response<BasicPageDto> { Payload = result });
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateBasicPage([FromBody] Request<UpdateBasicPageDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _basicPageService.UpdateBasicPage(request.Payload);
                return Ok(new Response<string> { Payload = result });
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteBasicPage([FromBody] Request<DeleteBasicPageDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _basicPageService.DeleteBasicPage(request.Payload);
                return Ok(new Response<bool> { Payload = result });
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListBasicPage([FromBody] Request<ListBasicPageRequestDto> request)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _basicPageService.GetListBasicPage(request.Payload);
                return Ok(new Response<List<BasicPageDto>> { Payload = result });
            });
        }
    }
}
