using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("shopping_lists")]
    public class ShoppingList
    {
        [Key]
        [Column("list_id")]
        public int ListId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = "My Shopping List";

        [Column("planned_date")]
        public DateTime? PlannedDate { get; set; }

        [Column("is_completed")]
        public bool IsCompleted { get; set; } = false;

        [Column("total_estimated_cost")]
        public decimal TotalEstimatedCost { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
    }
}
