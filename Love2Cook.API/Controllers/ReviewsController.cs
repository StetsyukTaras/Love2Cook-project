using Microsoft.AspNetCore.Mvc;
using Love2Cook.API.Data;
using Love2Cook.API.Models;
using Love2Cook.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Love2Cook.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Додати або оновити оцінку
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddOrUpdateReview([FromBody] CreateReviewDto dto)
        {
            // Перевірка чи користувач вже оцінював цей рецепт
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.RecipeId == dto.RecipeId && r.UserId == dto.UserId);

            if (existingReview != null) {
                // Оновлення існуючої оцінки
                existingReview.Rating = dto.Rating;
                existingReview.UpdatedAt = DateTime.UtcNow;
            } else {
                // Створення нової оцінки
                var review = new Review
                {
                    RecipeId = dto.RecipeId,
                    UserId = dto.UserId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Reviews.Add(review);
            }

            await _context.SaveChangesAsync();

            // Перерахунок середньої оцінки
            var avgRating = await _context.Reviews
                .Where(r => r.RecipeId == dto.RecipeId)
                .AverageAsync(r => (double)r.Rating);

            return Ok(new {
                success = true,
                message = "Оцінка збережена",
                data = new {
                    averageRating = Math.Round(avgRating, 1),
                    userRating = dto.Rating
                }
            });
        }

        /// <summary>
        /// Отримати оцінку користувача для рецепту
        /// </summary>
        [HttpGet("{recipeId}/user/{userId}")]
        public async Task<ActionResult> GetUserReview(int recipeId, int userId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);

            return Ok(new {
                success = true,
                data = new {
                    rating = review?.Rating ?? 0,
                    hasReview = review != null
                }
            });
        }
    }
}