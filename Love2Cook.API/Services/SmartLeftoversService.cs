using Love2Cook.API.Data;
using Love2Cook.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Love2Cook.API.Services {
    public interface ISmartLeftoversService {
        Task<List<SmartLeftoversResultDto>> FindRecipesByIngredientsAsync(List<int> ingredientIds);
    }

    public class SmartLeftoversService : ISmartLeftoversService {
        private readonly AppDbContext _context;

        public SmartLeftoversService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SmartLeftoversResultDto>> FindRecipesByIngredientsAsync(List<int> ingredientIds)
        {
            // Отримати всі опубліковані рецепти з інгредієнтами
            var recipes = await _context.Recipes
                .Where(r => r.IsPublished)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();

            var results = new List<SmartLeftoversResultDto>();

            foreach (var recipe in recipes) {
                // Підрахунок збігів
                var totalIngredients = recipe.RecipeIngredients.Count;
                var matchedIngredients = recipe.RecipeIngredients
                    .Count(ri => ingredientIds.Contains(ri.IngredientId));

                // Якщо є хоча б один збіг
                if (matchedIngredients > 0) {
                    var matchPercentage = (int)Math.Round((double)matchedIngredients / totalIngredients * 100);

                    // Список відсутніх інгредієнтів
                    var missingIngredients = recipe.RecipeIngredients
                        .Where(ri => !ingredientIds.Contains(ri.IngredientId))
                        .Select(ri => ri.Ingredient?.Name ?? "Unknown")
                        .ToList();

                    results.Add(new SmartLeftoversResultDto
                    {
                        RecipeId = recipe.RecipeId,
                        Title = recipe.Title,
                        Description = recipe.Description,
                        ImageUrl = recipe.ImageUrl,
                        CookingTimeMin = recipe.CookingTimeMin,
                        Complexity = recipe.Complexity,
                        MatchPercentage = matchPercentage,
                        TotalIngredients = totalIngredients,
                        MatchedIngredients = matchedIngredients,
                        MissingIngredients = missingIngredients
                    });
                }
            }

            // Сортування за відсотком збігу (найбільший спочатку)
            return results.OrderByDescending(r => r.MatchPercentage)
                         .ThenByDescending(r => r.MatchedIngredients)
                         .ToList();
        }
    }
}