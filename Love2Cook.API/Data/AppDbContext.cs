using Microsoft.EntityFrameworkCore;
using Love2Cook.API.Models;

namespace Love2Cook.API.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet для кожної таблиці
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<UserAllergen> UserAllergens { get; set; }
        public DbSet<IngredientAllergen> IngredientAllergens { get; set; }
        public DbSet<UserFridge> UserFridges { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Налаштування складених первинних ключів (Composite Primary Keys) =====

            // RecipeIngredient: складений ключ (recipe_id, ingredient_id)
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            // UserAllergen: складений ключ (user_id, allergen_id)
            modelBuilder.Entity<UserAllergen>()
                .HasKey(ua => new { ua.UserId, ua.AllergenId });

            // IngredientAllergen: складений ключ (ingredient_id, allergen_id)
            modelBuilder.Entity<IngredientAllergen>()
                .HasKey(ia => new { ia.IngredientId, ia.AllergenId });

            // Favorite: складений ключ (user_id, recipe_id)
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.RecipeId });

            // ===== Налаштування зв'язків (Relationships) =====

            // User -> Role (Many-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Recipe -> User (Many-to-One)
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Recipe -> Cuisine (Many-to-One)
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Cuisine)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CuisineId)
                .OnDelete(DeleteBehavior.SetNull);

            // Ingredient -> IngredientCategory (Many-to-One)
            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Ingredients)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // RecipeIngredient -> Recipe (Many-to-One)
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // RecipeIngredient -> Ingredient (Many-to-One)
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserAllergen -> User
            modelBuilder.Entity<UserAllergen>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAllergens)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserAllergen -> Allergen
            modelBuilder.Entity<UserAllergen>()
                .HasOne(ua => ua.Allergen)
                .WithMany(a => a.UserAllergens)
                .HasForeignKey(ua => ua.AllergenId)
                .OnDelete(DeleteBehavior.Cascade);

            // IngredientAllergen -> Ingredient
            modelBuilder.Entity<IngredientAllergen>()
                .HasOne(ia => ia.Ingredient)
                .WithMany(i => i.IngredientAllergens)
                .HasForeignKey(ia => ia.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // IngredientAllergen -> Allergen
            modelBuilder.Entity<IngredientAllergen>()
                .HasOne(ia => ia.Allergen)
                .WithMany(a => a.IngredientAllergens)
                .HasForeignKey(ia => ia.AllergenId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserFridge -> User
            modelBuilder.Entity<UserFridge>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.FridgeItems)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserFridge -> Ingredient
            modelBuilder.Entity<UserFridge>()
                .HasOne(uf => uf.Ingredient)
                .WithMany(i => i.UserFridgeItems)
                .HasForeignKey(uf => uf.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShoppingList -> User
            modelBuilder.Entity<ShoppingList>()
                .HasOne(sl => sl.User)
                .WithMany(u => u.ShoppingLists)
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShoppingListItem -> ShoppingList
            modelBuilder.Entity<ShoppingListItem>()
                .HasOne(sli => sli.ShoppingList)
                .WithMany(sl => sl.Items)
                .HasForeignKey(sli => sli.ListId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShoppingListItem -> Ingredient (optional)
            modelBuilder.Entity<ShoppingListItem>()
                .HasOne(sli => sli.Ingredient)
                .WithMany(i => i.ShoppingListItems)
                .HasForeignKey(sli => sli.IngredientId)
                .OnDelete(DeleteBehavior.SetNull);

            // Favorite -> User
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite -> Recipe
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Recipe)
                .WithMany(r => r.Favorites)
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review -> User
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review -> Recipe
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Recipe)
                .WithMany(rec => rec.Reviews)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // MealPlan -> User
            modelBuilder.Entity<MealPlan>()
                .HasOne(mp => mp.User)
                .WithMany(u => u.MealPlans)
                .HasForeignKey(mp => mp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // MealPlan -> Recipe
            modelBuilder.Entity<MealPlan>()
                .HasOne(mp => mp.Recipe)
                .WithMany(r => r.MealPlans)
                .HasForeignKey(mp => mp.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Унікальні індекси =====

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.RecipeId })
                .IsUnique();

            modelBuilder.Entity<UserFridge>()
                .HasIndex(uf => new { uf.UserId, uf.IngredientId })
                .IsUnique();
        }
    }
}