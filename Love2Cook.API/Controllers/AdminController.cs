using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Love2Cook.API.Data;
using Love2Cook.API.DTOs;

namespace Love2Cook.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати статистику
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var stats = new {
                TotalUsers = await _context.Users.CountAsync(),
                TotalRecipes = await _context.Recipes.CountAsync(),
                TotalIngredients = await _context.Ingredients.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalShoppingLists = await _context.ShoppingLists.CountAsync(),
                PublishedRecipes = await _context.Recipes.CountAsync(r => r.IsPublished),
                VipUsers = await _context.Users.CountAsync(u => u.RoleId == 2),
                AdminUsers = await _context.Users.CountAsync(u => u.RoleId == 3)
            };

            return Ok(new {
                success = true,
                message = "Statistics retrieved successfully",
                data = stats,
                errors = null as List<string>
            });
        }

        /// <summary>
        /// Отримати всіх користувачів
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.RoleId,
                    u.IsVip,
                    u.CreatedAt
                })
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return Ok(new {
                success = true,
                message = $"Found {users.Count} users",
                data = users,
                errors = null as List<string>
            });
        }

        /// <summary>
        /// Видалити користувача
        /// </summary>
        [HttpDelete("users/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            if (user.RoleId == 3) {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete admin user"
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "User deleted successfully",
                Data = true
            });
        }

        /// <summary>
        /// Видалити рецепт
        /// </summary>
        [HttpDelete("recipes/{recipeId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRecipe(int recipeId)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);

            if (recipe == null) {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Recipe not found"
                });
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Recipe deleted successfully",
                Data = true
            });
        }
    }
}