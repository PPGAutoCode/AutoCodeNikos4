using Microsoft.AspNetCore.Mvc;
using ProjectName.Types;
using ProjectName.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectName.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FAQCategoryController : ControllerBase
    {
        private readonly IFAQCategoryService _faqCategoryService;

        public FAQCategoryController(IFAQCategoryService faqCategoryService)
        {
            _faqCategoryService = faqCategoryService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFAQCategory([FromBody] CreateFAQCategoryDto createFAQCategoryDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _faqCategoryService.CreateFAQCategory(createFAQCategoryDto);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetFAQCategory([FromBody] FAQCategoryRequestDto faqCategoryRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _faqCategoryService.GetFAQCategory(faqCategoryRequestDto);
                return Ok(new Response<FAQCategory>(result));
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateFAQCategory([FromBody] UpdateFAQCategoryDto updateFAQCategoryDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _faqCategoryService.UpdateFAQCategory(updateFAQCategoryDto);
                return Ok(new Response<string>(result));
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteFAQCategory([FromBody] DeleteFAQCategoryDto deleteFAQCategoryDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _faqCategoryService.DeleteFAQCategory(deleteFAQCategoryDto);
                return Ok(new Response<bool>(result));
            });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListFAQCategory([FromBody] ListFAQCategoryRequestDto listFAQCategoryRequestDto)
        {
            return await SafeExecutor.ExecuteAsync(async () =>
            {
                var result = await _faqCategoryService.GetListFAQCategory(listFAQCategoryRequestDto);
                return Ok(new Response<List<FAQCategory>>(result));
            });
        }
    }
}
