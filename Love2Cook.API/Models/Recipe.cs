using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace Love2Cook.API.Models
{
    [Table("recipes")]
    public class Recipe
    {
        [Key]
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [Column("author_id")]
        public int AuthorId { get; set; }

        [Column("cuisine_id")]
        public int? CuisineId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("instructions")]
        public string? Instructions { get; set; }

        [Column("complexity")]
        [Range(1, 10)]
        public byte? Complexity { get; set; }

        [Column("cooking_time_min")]
        public int? CookingTimeMin { get; set; }

        [Column("prep_time_min")]
        public int PrepTimeMin { get; set; } = 0;

        [Column("servings")]
        public int Servings { get; set; } = 2;

        [MaxLength(500)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        [Column("video_url")]
        public string? VideoUrl { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; } = true;

        [Column("is_approved")]
        public bool IsApproved { get; set; } = false;

        [Column("views_count")]
        public int ViewsCount { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AuthorId")]
        public virtual User? Author { get; set; }

        [ForeignKey("CuisineId")]
        public virtual Cuisine? Cuisine { get; set; }

        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
    }
}
