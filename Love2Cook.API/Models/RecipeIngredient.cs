using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("recipe_ingredients")]
    public class RecipeIngredient
    {
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [Column("ingredient_id")]
        public int IngredientId { get; set; }

        [Required]
        [Column("quantity")]
        public decimal Quantity { get; set; }

        [MaxLength(30)]
        [Column("unit")]
        public string Unit { get; set; } = "g";

        [MaxLength(100)]
        [Column("notes")]
        public string? Notes { get; set; }

        [Column("is_optional")]
        public bool IsOptional { get; set; } = false;

        // Navigation properties
        [ForeignKey("RecipeId")]
        public virtual Recipe? Recipe { get; set; }

        [ForeignKey("IngredientId")]
        public virtual Ingredient? Ingredient { get; set; }
    }
}
