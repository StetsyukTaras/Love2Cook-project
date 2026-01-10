using Microsoft.EntityFrameworkCore;
using Love2Cook.API.Data;
using Love2Cook.API.Services;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("=== Love2Cook API Starting ===");

// ===== Підключення до MySQL =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Реєстрація сервісів =====
Console.WriteLine("Registering services...");
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<ISmartLeftoversService, SmartLeftoversService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();

// ===== Контролери =====
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ===== Тест підключення до БД =====
Console.WriteLine("Testing database connection...");
try {
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var canConnect = await dbContext.Database.CanConnectAsync();
    if (canConnect) {
        Console.WriteLine("✅ Database connected successfully!");

        // Перевірка таблиць
        var usersCount = await dbContext.Users.CountAsync();
        var recipesCount = await dbContext.Recipes.CountAsync();
        var ingredientsCount = await dbContext.Ingredients.CountAsync();

        Console.WriteLine($"✅ Users: {usersCount}");
        Console.WriteLine($"✅ Recipes: {recipesCount}");
        Console.WriteLine($"✅ Ingredients: {ingredientsCount}");
    } else {
        Console.WriteLine("❌ Cannot connect to database");
    }
} catch (Exception ex) {
    Console.WriteLine($"❌ Database error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

// ===== Middleware =====
Console.WriteLine("Configuring middleware...");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("=== Love2Cook API Started ===");
Console.WriteLine($"Swagger UI: https://localhost:{app.Urls.FirstOrDefault()?.Split(':').Last()}/swagger");

app.Run();