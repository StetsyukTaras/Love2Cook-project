using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("ingredient_allergens")]
    public class IngredientAllergen
    {
        [Column("ingredient_id")]
        public int IngredientId { get; set; }

        [Column("allergen_id")]
        public int AllergenId { get; set; }

        // Navigation properties
        [ForeignKey("IngredientId")]
        public virtual Ingredient? Ingredient { get; set; }

        [ForeignKey("AllergenId")]
        public virtual Allergen? Allergen { get; set; }
    }
}
