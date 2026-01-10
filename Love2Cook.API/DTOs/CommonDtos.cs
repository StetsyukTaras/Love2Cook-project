using System.ComponentModel.DataAnnotations;

namespace Love2Cook.API.DTOs {
    // ========== Ingredient ==========
    public class IngredientDto {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int CaloriesPer100g { get; set; }
        public decimal ProteinPer100g { get; set; }
        public decimal FatPer100g { get; set; }
        public decimal CarbsPer100g { get; set; }
        public decimal PricePerKg { get; set; }
        public string DefaultUnit { get; set; } = "g";
        public string? ImageUrl { get; set; }
    }

    // ========== Shopping List ==========
    public class ShoppingListDto {
        public int ListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? PlannedDate { get; set; }
        public bool IsCompleted { get; set; }
        public decimal TotalEstimatedCost { get; set; }
        public List<ShoppingListItemDto> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class ShoppingListItemDto {
        public int ItemId { get; set; }
        public int? IngredientId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public string Unit { get; set; } = "g";
        public decimal? EstimatedPrice { get; set; }
        public bool IsPurchased { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateShoppingListDto {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "My Shopping List";

        public DateTime? PlannedDate { get; set; }
    }

    public class CreateShoppingListItemDto {
        public int? IngredientId { get; set; }

        [StringLength(100)]
        public string? CustomItemName { get; set; }

        public decimal? Quantity { get; set; }

        [StringLength(30)]
        public string Unit { get; set; } = "g";

        public decimal? EstimatedPrice { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }
    }

    // ========== Smart Leftovers ==========
    public class SmartLeftoversRequestDto {
        [Required]
        [MinLength(1, ErrorMessage = "At least one ingredient is required")]
        public List<int> IngredientIds { get; set; } = new();
    }

    public class SmartLeftoversResultDto {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? CookingTimeMin { get; set; }
        public int? Complexity { get; set; }
        public int MatchPercentage { get; set; }
        public int TotalIngredients { get; set; }
        public int MatchedIngredients { get; set; }
        public List<string> MissingIngredients { get; set; } = new();
    }

    // ========== Cuisine ==========
    public class CuisineDto {
        public int CuisineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    // ========== Common Response ==========
    public class ApiResponse<T> {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}