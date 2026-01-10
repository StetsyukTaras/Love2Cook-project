using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("cuisines")]
    public class Cuisine
    {
        [Key]
        [Column("cuisine_id")]
        public int CuisineId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        [MaxLength(500)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        // Navigation property
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
