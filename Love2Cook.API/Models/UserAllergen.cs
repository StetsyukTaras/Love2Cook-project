using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("user_allergens")]
    public class UserAllergen
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("allergen_id")]
        public int AllergenId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("AllergenId")]
        public virtual Allergen? Allergen { get; set; }
    }
}
