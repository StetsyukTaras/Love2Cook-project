using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("meal_plans")]
    public class MealPlan
    {
        [Key]
        [Column("plan_id")]
        public int PlanId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [Required]
        [Column("scheduled_date")]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("meal_type")]
        public string MealType { get; set; } = "Dinner"; // Breakfast, Lunch, Dinner, Snack

        [Column("servings")]
        public int Servings { get; set; } = 2;

        [Column("is_cooked")]
        public bool IsCooked { get; set; } = false;

        [MaxLength(255)]
        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("RecipeId")]
        public virtual Recipe? Recipe { get; set; }
    }
}
