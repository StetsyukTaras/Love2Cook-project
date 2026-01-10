using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("ingredients")]
    public class Ingredient
    {
        [Key]
        [Column("ingredient_id")]
        public int IngredientId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("calories_per_100g")]
        public int CaloriesPer100g { get; set; } = 0;

        [Column("protein_per_100g")]
        public decimal ProteinPer100g { get; set; } = 0;

        [Column("fat_per_100g")]
        public decimal FatPer100g { get; set; } = 0;

        [Column("carbs_per_100g")]
        public decimal CarbsPer100g { get; set; } = 0;

        [Column("price_per_kg")]
        public decimal PricePerKg { get; set; } = 0;

        [MaxLength(20)]
        [Column("default_unit")]
        public string DefaultUnit { get; set; } = "g";

        [MaxLength(500)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual IngredientCategory? Category { get; set; }

        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public virtual ICollection<IngredientAllergen> IngredientAllergens { get; set; } = new List<IngredientAllergen>();
        public virtual ICollection<UserFridge> UserFridgeItems { get; set; } = new List<UserFridge>();
        public virtual ICollection<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();
    }
}
