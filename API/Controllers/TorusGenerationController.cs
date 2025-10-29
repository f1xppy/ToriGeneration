using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Caching.Memory;
using ToriGeneration.Core.Abstract.Services;
using ToriGeneration.Core.Models.Dto.Parameters;

namespace ToriGeneration.API.Controllers
{
    [ApiController]
    public class TorusGenerationController : ControllerBase
    {
        private readonly ITorusGenerationService _generationService;
        private readonly IMemoryCache _cache;

        public TorusGenerationController(
            ITorusGenerationService generationService,
            IMemoryCache cache)
        {
            _generationService = generationService;
            _cache = cache;
        }

        [HttpPost]
        [Route("api/TorusGeneration/GetList")]
        public async Task<IActionResult> GetTorusList(TorusGenerationParameters parameters)
        {
            try
            {
                var result = await _generationService.Generate(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/TorusGeneration/GenerateFiles")]
        public async Task<IActionResult> GenerateFiles(int filesCount, TorusGenerationParameters parameters)
        {
            try
            {
                var result = await _generationService.GenerateMultipleTorusLists(filesCount, parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/TorusGeneration/GetStatus")]
        public async Task<IActionResult> GetStatus(string cacheKey)
        {
            try
            {
                _cache.TryGetValue(cacheKey, out int? result);
                if (result == null || result == -1)
                    return Ok(-1);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
