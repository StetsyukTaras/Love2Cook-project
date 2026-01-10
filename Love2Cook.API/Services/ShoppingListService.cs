using Love2Cook.API.Data;
using Love2Cook.API.Models;
using Love2Cook.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Love2Cook.API.Services {
    public interface IShoppingListService {
        Task<List<ShoppingListDto>> GetUserShoppingListsAsync(int userId);
        Task<ShoppingListDto?> GetShoppingListByIdAsync(int listId, int userId);
        Task<ShoppingListDto?> CreateShoppingListAsync(CreateShoppingListDto dto, int userId);
        Task<bool> DeleteShoppingListAsync(int listId, int userId);
        Task<ShoppingListItemDto?> AddItemToListAsync(int listId, CreateShoppingListItemDto dto, int userId);
        Task<bool> ToggleItemPurchasedAsync(int itemId, int userId);
        Task<bool> DeleteItemAsync(int itemId, int userId);
    }

    public class ShoppingListService : IShoppingListService {
        private readonly AppDbContext _context;

        public ShoppingListService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShoppingListDto>> GetUserShoppingListsAsync(int userId)
        {
            var lists = await _context.ShoppingLists
                .Include(sl => sl.Items)
                    .ThenInclude(item => item.Ingredient)
                .Where(sl => sl.UserId == userId)
                .OrderByDescending(sl => sl.CreatedAt)
                .ToListAsync();

            return lists.Select(sl => MapToDto(sl)).ToList();
        }

        public async Task<ShoppingListDto?> GetShoppingListByIdAsync(int listId, int userId)
        {
            var list = await _context.ShoppingLists
                .Include(sl => sl.Items)
                    .ThenInclude(item => item.Ingredient)
                .FirstOrDefaultAsync(sl => sl.ListId == listId && sl.UserId == userId);

            return list != null ? MapToDto(list) : null;
        }

        public async Task<ShoppingListDto?> CreateShoppingListAsync(CreateShoppingListDto dto, int userId)
        {
            var shoppingList = new ShoppingList
            {
                UserId = userId,
                Name = dto.Name,
                PlannedDate = dto.PlannedDate,
                IsCompleted = false,
                TotalEstimatedCost = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ShoppingLists.Add(shoppingList);
            await _context.SaveChangesAsync();

            return await GetShoppingListByIdAsync(shoppingList.ListId, userId);
        }

        public async Task<bool> DeleteShoppingListAsync(int listId, int userId)
        {
            var list = await _context.ShoppingLists
                .FirstOrDefaultAsync(sl => sl.ListId == listId && sl.UserId == userId);

            if (list == null) {
                return false;
            }

            _context.ShoppingLists.Remove(list);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ShoppingListItemDto?> AddItemToListAsync(int listId, CreateShoppingListItemDto dto, int userId)
        {
            // Перевірка чи список належить користувачу
            var list = await _context.ShoppingLists
                .FirstOrDefaultAsync(sl => sl.ListId == listId && sl.UserId == userId);

            if (list == null) {
                return null;
            }

            var item = new ShoppingListItem
            {
                ListId = listId,
                IngredientId = dto.IngredientId,
                CustomItemName = dto.CustomItemName,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                EstimatedPrice = dto.EstimatedPrice,
                IsPurchased = false,
                Notes = dto.Notes
            };

            _context.ShoppingListItems.Add(item);
            await _context.SaveChangesAsync();

            // Оновлення загальної вартості
            await UpdateTotalCostAsync(listId);

            var addedItem = await _context.ShoppingListItems
                .Include(i => i.Ingredient)
                .FirstOrDefaultAsync(i => i.ItemId == item.ItemId);

            return MapItemToDto(addedItem!);
        }

        public async Task<bool> ToggleItemPurchasedAsync(int itemId, int userId)
        {
            var item = await _context.ShoppingListItems
                .Include(i => i.ShoppingList)
                .FirstOrDefaultAsync(i => i.ItemId == itemId && i.ShoppingList.UserId == userId);

            if (item == null) {
                return false;
            }

            item.IsPurchased = !item.IsPurchased;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteItemAsync(int itemId, int userId)
        {
            var item = await _context.ShoppingListItems
                .Include(i => i.ShoppingList)
                .FirstOrDefaultAsync(i => i.ItemId == itemId && i.ShoppingList.UserId == userId);

            if (item == null) {
                return false;
            }

            var listId = item.ListId;
            _context.ShoppingListItems.Remove(item);
            await _context.SaveChangesAsync();

            // Оновлення загальної вартості
            await UpdateTotalCostAsync(listId);

            return true;
        }

        // Допоміжні методи
        private async Task UpdateTotalCostAsync(int listId)
        {
            var list = await _context.ShoppingLists
                .Include(sl => sl.Items)
                .FirstOrDefaultAsync(sl => sl.ListId == listId);

            if (list != null) {
                list.TotalEstimatedCost = list.Items
                    .Where(i => i.EstimatedPrice.HasValue)
                    .Sum(i => i.EstimatedPrice!.Value);

                await _context.SaveChangesAsync();
            }
        }

        private ShoppingListDto MapToDto(ShoppingList list)
        {
            return new ShoppingListDto
            {
                ListId = list.ListId,
                Name = list.Name,
                PlannedDate = list.PlannedDate,
                IsCompleted = list.IsCompleted,
                TotalEstimatedCost = list.TotalEstimatedCost,
                Items = list.Items.Select(i => MapItemToDto(i)).ToList(),
                CreatedAt = list.CreatedAt
            };
        }

        private ShoppingListItemDto MapItemToDto(ShoppingListItem item)
        {
            return new ShoppingListItemDto
            {
                ItemId = item.ItemId,
                IngredientId = item.IngredientId,
                ItemName = item.Ingredient?.Name ?? item.CustomItemName ?? "Unknown",
                Quantity = item.Quantity,
                Unit = item.Unit,
                EstimatedPrice = item.EstimatedPrice,
                IsPurchased = item.IsPurchased,
                Notes = item.Notes
            };
        }
    }
}