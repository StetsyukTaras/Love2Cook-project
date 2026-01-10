using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Love2Cook.API.Data;
using Love2Cook.API.DTOs;

namespace Love2Cook.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase {
        private readonly AppDbContext _context;

        public IngredientsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати всі інгредієнти
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<IngredientDto>>>> GetAllIngredients([FromQuery] string? search = null)
        {
            var query = _context.Ingredients
                .Include(i => i.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search)) {
                query = query.Where(i => i.Name.Contains(search));
            }

            var ingredients = await query
                .OrderBy(i => i.Name)
                .Select(i => new IngredientDto
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category != null ? i.Category.Name : null,
                    CaloriesPer100g = i.CaloriesPer100g,
                    ProteinPer100g = i.ProteinPer100g,
                    FatPer100g = i.FatPer100g,
                    CarbsPer100g = i.CarbsPer100g,
                    PricePerKg = i.PricePerKg,
                    DefaultUnit = i.DefaultUnit,
                    ImageUrl = i.ImageUrl
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<IngredientDto>>
            {
                Success = true,
                Message = $"Found {ingredients.Count} ingredients",
                Data = ingredients
            });
        }

        /// <summary>
        /// Отримати інгредієнт за ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<IngredientDto>>> GetIngredientById(int id)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.Category)
                .Where(i => i.IngredientId == id)
                .Select(i => new IngredientDto
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category != null ? i.Category.Name : null,
                    CaloriesPer100g = i.CaloriesPer100g,
                    ProteinPer100g = i.ProteinPer100g,
                    FatPer100g = i.FatPer100g,
                    CarbsPer100g = i.CarbsPer100g,
                    PricePerKg = i.PricePerKg,
                    DefaultUnit = i.DefaultUnit,
                    ImageUrl = i.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (ingredient == null) {
                return NotFound(new ApiResponse<IngredientDto>
                {
                    Success = false,
                    Message = "Ingredient not found"
                });
            }

            return Ok(new ApiResponse<IngredientDto>
            {
                Success = true,
                Message = "Ingredient retrieved successfully",
                Data = ingredient
            });
        }

        /// <summary>
        /// Отримати всі кухні
        /// </summary>
        [HttpGet("cuisines")]
        public async Task<ActionResult<ApiResponse<List<CuisineDto>>>> GetAllCuisines()
        {
            var cuisines = await _context.Cuisines
                .Select(c => new CuisineDto
                {
                    CuisineId = c.CuisineId,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<CuisineDto>>
            {
                Success = true,
                Message = $"Found {cuisines.Count} cuisines",
                Data = cuisines
            });
        }
    }
}