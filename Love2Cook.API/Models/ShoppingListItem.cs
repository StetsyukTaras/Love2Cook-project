using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Love2Cook.API.Models
{
    [Table("shopping_list_items")]
    public class ShoppingListItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("list_id")]
        public int ListId { get; set; }

        [Column("ingredient_id")]
        public int? IngredientId { get; set; }

        [MaxLength(100)]
        [Column("custom_item_name")]
        public string? CustomItemName { get; set; }

        [Column("quantity")]
        public decimal? Quantity { get; set; }

        [MaxLength(30)]
        [Column("unit")]
        public string Unit { get; set; } = "g";

        [Column("estimated_price")]
        public decimal? EstimatedPrice { get; set; }

        [Column("is_purchased")]
        public bool IsPurchased { get; set; } = false;

        [Column("recipe_id")]
        public int? RecipeId { get; set; }

        [MaxLength(255)]
        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("ListId")]
        public virtual ShoppingList? ShoppingList { get; set; }

        [ForeignKey("IngredientId")]
        public virtual Ingredient? Ingredient { get; set; }

        [ForeignKey("RecipeId")]
        public virtual Recipe? Recipe { get; set; }
    }
}
