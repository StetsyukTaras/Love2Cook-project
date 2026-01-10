using Microsoft.AspNetCore.Mvc;
using Love2Cook.API.DTOs;
using Love2Cook.API.Services;

namespace Love2Cook.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        /// <summary>
        /// Отримати всі рецепти (з можливістю пошуку)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RecipeListDto>>>> GetAllRecipes([FromQuery] string? search = null)
        {
            var recipes = await _recipeService.GetAllRecipesAsync(search);

            return Ok(new ApiResponse<List<RecipeListDto>>
            {
                Success = true,
                Message = $"Found {recipes.Count} recipes",
                Data = recipes
            });
        }

        /// <summary>
        /// Отримати рецепт за ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RecipeDetailsDto>>> GetRecipeById(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);

            if (recipe == null) {
                return NotFound(new ApiResponse<RecipeDetailsDto>
                {
                    Success = false,
                    Message = "Recipe not found"
                });
            }

            return Ok(new ApiResponse<RecipeDetailsDto>
            {
                Success = true,
                Message = "Recipe retrieved successfully",
                Data = recipe
            });
        }

        /// <summary>
        /// Створити новий рецепт
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RecipeDetailsDto>>> CreateRecipe(
            [FromBody] CreateUpdateRecipeDto dto,
            [FromQuery] int authorId)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new ApiResponse<RecipeDetailsDto>
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var recipe = await _recipeService.CreateRecipeAsync(dto, authorId);

            if (recipe == null) {
                return BadRequest(new ApiResponse<RecipeDetailsDto>
                {
                    Success = false,
                    Message = "Failed to create recipe"
                });
            }

            return CreatedAtAction(
                nameof(GetRecipeById),
                new { id = recipe.RecipeId },
                new ApiResponse<RecipeDetailsDto>
                {
                    Success = true,
                    Message = "Recipe created successfully",
                    Data = recipe
                });
        }

        /// <summary>
        /// Оновити рецепт
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RecipeDetailsDto>>> UpdateRecipe(
            int id,
            [FromBody] CreateUpdateRecipeDto dto,
            [FromQuery] int userId)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new ApiResponse<RecipeDetailsDto>
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var recipe = await _recipeService.UpdateRecipeAsync(id, dto, userId);

            if (recipe == null) {
                return NotFound(new ApiResponse<RecipeDetailsDto>
                {
                    Success = false,
                    Message = "Recipe not found or you don't have permission to update it"
                });
            }

            return Ok(new ApiResponse<RecipeDetailsDto>
            {
                Success = true,
                Message = "Recipe updated successfully",
                Data = recipe
            });
        }

        /// <summary>
        /// Видалити рецепт
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRecipe(int id, [FromQuery] int userId)
        {
            var success = await _recipeService.DeleteRecipeAsync(id, userId);

            if (!success) {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Recipe not found or you don't have permission to delete it"
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Recipe deleted successfully",
                Data = true
            });
        }
    }
}