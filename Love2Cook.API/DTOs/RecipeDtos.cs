using System.ComponentModel.DataAnnotations;

namespace Love2Cook.API.DTOs {
    // ========== Recipe List Item (для каталогу) ==========
    public class RecipeListDto {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public byte? Complexity { get; set; }
        public int? CookingTimeMin { get; set; }
        public int PrepTimeMin { get; set; }
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? CuisineName { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int ViewsCount { get; set; }
    }

    // ========== Recipe Details (повна інформація) ==========
    public class RecipeDetailsDto {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Instructions { get; set; }
        public byte? Complexity { get; set; }
        public int? CookingTimeMin { get; set; }
        public int PrepTimeMin { get; set; }
        public int Servings { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }

        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;

        public int? CuisineId { get; set; }
        public string? CuisineName { get; set; }

        public List<RecipeIngredientDto> Ingredients { get; set; } = new();

        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int ViewsCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ========== Recipe Ingredient ==========
    public class RecipeIngredientDto {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsOptional { get; set; }
        public int CaloriesPer100g { get; set; }    
        public decimal ProteinPer100g { get; set; } 
        public decimal FatPer100g { get; set; }     
        public decimal CarbsPer100g { get; set; }
    }

    // ========== Create/Update Recipe ==========
    public class CreateUpdateRecipeDto {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Instructions are required")]
        public string Instructions { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Complexity must be between 1 and 10")]
        public byte? Complexity { get; set; }

        [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes")]
        public int? CookingTimeMin { get; set; }

        [Range(0, 1440, ErrorMessage = "Prep time must be between 0 and 1440 minutes")]
        public int PrepTimeMin { get; set; } = 0;

        [Range(1, 50, ErrorMessage = "Servings must be between 1 and 50")]
        public int Servings { get; set; } = 2;

        public int? CuisineId { get; set; }

        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }

        public List<CreateRecipeIngredientDto> Ingredients { get; set; } = new();
    }

    public class CreateRecipeIngredientDto {
        [Required]
        public int IngredientId { get; set; }

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(30)]
        public string Unit { get; set; } = "g";

        [StringLength(100)]
        public string? Notes { get; set; }

        public bool IsOptional { get; set; } = false;

    }
}