using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("allergens")]
    public class Allergen
    {
        [Key]
        [Column("allergen_id")]
        public int AllergenId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        [MaxLength(50)]
        [Column("icon")]
        public string? Icon { get; set; }

        // Navigation properties
        public virtual ICollection<UserAllergen> UserAllergens { get; set; } = new List<UserAllergen>();
        public virtual ICollection<IngredientAllergen> IngredientAllergens { get; set; } = new List<IngredientAllergen>();
    }
}
