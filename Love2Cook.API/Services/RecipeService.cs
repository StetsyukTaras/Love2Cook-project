using Love2Cook.API.Data;
using Love2Cook.API.Models;
using Love2Cook.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Love2Cook.API.Services {
    public interface IRecipeService {
        Task<List<RecipeListDto>> GetAllRecipesAsync(string? searchTerm = null);
        Task<RecipeDetailsDto?> GetRecipeByIdAsync(int recipeId);
        Task<RecipeDetailsDto?> CreateRecipeAsync(CreateUpdateRecipeDto dto, int authorId);
        Task<RecipeDetailsDto?> UpdateRecipeAsync(int recipeId, CreateUpdateRecipeDto dto, int userId);
        Task<bool> DeleteRecipeAsync(int recipeId, int userId);
    }

    public class RecipeService : IRecipeService {
        private readonly AppDbContext _context;

        public RecipeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecipeListDto>> GetAllRecipesAsync(string? searchTerm = null)
        {
            var query = _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.Cuisine)
                .Include(r => r.Reviews)
                .Where(r => r.IsPublished);

            // Пошук за назвою
            if (!string.IsNullOrWhiteSpace(searchTerm)) {
                query = query.Where(r => r.Title.Contains(searchTerm));
            }

            var recipes = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return recipes.Select(r => new RecipeListDto
            {
                RecipeId = r.RecipeId,
                Title = r.Title,
                Description = r.Description,
                Complexity = r.Complexity,
                CookingTimeMin = r.CookingTimeMin,
                PrepTimeMin = r.PrepTimeMin,
                ImageUrl = r.ImageUrl,
                AuthorName = r.Author?.Username ?? "Unknown",
                CuisineName = r.Cuisine?.Name,
                AverageRating = r.Reviews.Any() ? (decimal)r.Reviews.Average(rev => rev.Rating) : 0,
                ReviewsCount = r.Reviews.Count,
                ViewsCount = r.ViewsCount
            }).ToList();
        }

        public async Task<RecipeDetailsDto?> GetRecipeByIdAsync(int recipeId)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Author)
                .Include(r => r.Cuisine)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Reviews)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.IsPublished);

            if (recipe == null) {
                return null;
            }

            // Збільшення лічильника переглядів
            recipe.ViewsCount++;
            await _context.SaveChangesAsync();

            return new RecipeDetailsDto
            {
                RecipeId = recipe.RecipeId,
                Title = recipe.Title,
                Description = recipe.Description,
                Instructions = recipe.Instructions,
                Complexity = recipe.Complexity,
                CookingTimeMin = recipe.CookingTimeMin,
                PrepTimeMin = recipe.PrepTimeMin,
                Servings = recipe.Servings,
                ImageUrl = recipe.ImageUrl,
                VideoUrl = recipe.VideoUrl,
                AuthorId = recipe.AuthorId,
                AuthorName = recipe.Author?.Username ?? "Unknown",
                CuisineId = recipe.CuisineId,
                CuisineName = recipe.Cuisine?.Name,
                Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientDto
                {
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.Ingredient?.Name ?? "Unknown",
                    Quantity = ri.Quantity,
                    Unit = ri.Unit,
                    Notes = ri.Notes,
                    IsOptional = ri.IsOptional,
                    CaloriesPer100g = ri.Ingredient?.CaloriesPer100g ?? 0,
                    ProteinPer100g = ri.Ingredient?.ProteinPer100g ?? 0,
                    FatPer100g = ri.Ingredient?.FatPer100g ?? 0,
                    CarbsPer100g = ri.Ingredient?.CarbsPer100g ?? 0
                }).ToList(),
                AverageRating = recipe.Reviews.Any() ? (decimal)recipe.Reviews.Average(r => r.Rating) : 0,
                ReviewsCount = recipe.Reviews.Count,
                ViewsCount = recipe.ViewsCount,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt
            };
        }

        public async Task<RecipeDetailsDto?> CreateRecipeAsync(CreateUpdateRecipeDto dto, int authorId)
        {
            var recipe = new Recipe
            {
                AuthorId = authorId,
                CuisineId = dto.CuisineId,
                Title = dto.Title,
                Description = dto.Description,
                Instructions = dto.Instructions,
                Complexity = dto.Complexity,
                CookingTimeMin = dto.CookingTimeMin,
                PrepTimeMin = dto.PrepTimeMin,
                Servings = dto.Servings,
                ImageUrl = dto.ImageUrl,
                VideoUrl = dto.VideoUrl,
                IsPublished = true,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // Додавання інгредієнтів
            foreach (var ingredient in dto.Ingredients) {
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = recipe.RecipeId,
                    IngredientId = ingredient.IngredientId,
                    Quantity = ingredient.Quantity,
                    Unit = ingredient.Unit,
                    Notes = ingredient.Notes,
                    IsOptional = ingredient.IsOptional
                };
                _context.RecipeIngredients.Add(recipeIngredient);
            }

            await _context.SaveChangesAsync();

            return await GetRecipeByIdAsync(recipe.RecipeId);
        }

        public async Task<RecipeDetailsDto?> UpdateRecipeAsync(int recipeId, CreateUpdateRecipeDto dto, int userId)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null || recipe.AuthorId != userId) {
                return null; // Not found or not authorized
            }

            // Оновлення основної інформації
            recipe.Title = dto.Title;
            recipe.Description = dto.Description;
            recipe.Instructions = dto.Instructions;
            recipe.Complexity = dto.Complexity;
            recipe.CookingTimeMin = dto.CookingTimeMin;
            recipe.PrepTimeMin = dto.PrepTimeMin;
            recipe.Servings = dto.Servings;
            recipe.CuisineId = dto.CuisineId;
            recipe.ImageUrl = dto.ImageUrl;
            recipe.VideoUrl = dto.VideoUrl;
            recipe.UpdatedAt = DateTime.UtcNow;

            // Видалення старих інгредієнтів
            _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);

            // Додавання нових інгредієнтів
            foreach (var ingredient in dto.Ingredients) {
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = recipe.RecipeId,
                    IngredientId = ingredient.IngredientId,
                    Quantity = ingredient.Quantity,
                    Unit = ingredient.Unit,
                    Notes = ingredient.Notes,
                    IsOptional = ingredient.IsOptional
                };
                _context.RecipeIngredients.Add(recipeIngredient);
            }

            await _context.SaveChangesAsync();

            return await GetRecipeByIdAsync(recipe.RecipeId);
        }

        public async Task<bool> DeleteRecipeAsync(int recipeId, int userId)
        {
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null || recipe.AuthorId != userId) {
                return false; // Not found or not authorized
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}