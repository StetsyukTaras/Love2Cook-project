using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("user_fridge")]
    public class UserFridge
    {
        [Key]
        [Column("fridge_id")]
        public int FridgeId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("ingredient_id")]
        public int IngredientId { get; set; }

        [Required]
        [Column("quantity")]
        public decimal Quantity { get; set; }

        [MaxLength(30)]
        [Column("unit")]
        public string Unit { get; set; } = "g";

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("IngredientId")]
        public virtual Ingredient? Ingredient { get; set; }
    }
}
