using Microsoft.AspNetCore.Mvc;
using Love2Cook.API.DTOs;
using Love2Cook.API.Services;

namespace Love2Cook.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SmartLeftoversController : ControllerBase {
        private readonly ISmartLeftoversService _smartLeftoversService;

        public SmartLeftoversController(ISmartLeftoversService smartLeftoversService)
        {
            _smartLeftoversService = smartLeftoversService;
        }

        /// <summary>
        /// Знайти рецепти на основі наявних інгредієнтів
        /// </summary>
        /// <remarks>
        /// Приклад запиту:
        /// POST /api/smartleftovers/find
        /// {
        ///   "ingredientIds": [1, 2, 3, 5]
        /// }
        /// </remarks>
        [HttpPost("find")]
        public async Task<ActionResult<ApiResponse<List<SmartLeftoversResultDto>>>> FindRecipes(
            [FromBody] SmartLeftoversRequestDto request)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new ApiResponse<List<SmartLeftoversResultDto>>
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var results = await _smartLeftoversService.FindRecipesByIngredientsAsync(request.IngredientIds);

            return Ok(new ApiResponse<List<SmartLeftoversResultDto>>
            {
                Success = true,
                Message = $"Found {results.Count} matching recipes",
                Data = results
            });
        }
    }
}