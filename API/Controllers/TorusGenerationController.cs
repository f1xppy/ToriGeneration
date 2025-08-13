using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ToriGeneration.Core.Abstract.Services;
using ToriGeneration.Core.Models.Dto.Parameters;

namespace ToriGeneration.API.Controllers
{
    [ApiController]
    public class TorusGenerationController : ControllerBase
    {
        private readonly ITorusGenerationService _generationService;

        public TorusGenerationController(ITorusGenerationService generationService)
        {
            _generationService = generationService;
        }

        [HttpPost]
        [Route("api/TorusGeneration/GetList")]
        public async Task<IActionResult> GetTorusList(TorusGenerationParameters parameters)
        {
            try
            {
                var result = await _generationService.GenerateAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
