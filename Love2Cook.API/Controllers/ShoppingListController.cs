using Microsoft.AspNetCore.Mvc;
using Love2Cook.API.DTOs;
using Love2Cook.API.Services;

namespace Love2Cook.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingListController : ControllerBase {
        private readonly IShoppingListService _shoppingListService;

        public ShoppingListController(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        /// <summary>
        /// Отримати всі списки покупок користувача
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<ShoppingListDto>>>> GetUserLists(int userId)
        {
            var lists = await _shoppingListService.GetUserShoppingListsAsync(userId);

            return Ok(new ApiResponse<List<ShoppingListDto>>
            {
                Success = true,
                Message = $"Found {lists.Count} shopping lists",
                Data = lists
            });
        }

        /// <summary>
        /// Отримати конкретний список покупок
        /// </summary>
        [HttpGet("{listId}")]
        public async Task<ActionResult<ApiResponse<ShoppingListDto>>> GetList(int listId, [FromQuery] int userId)
        {
            var list = await _shoppingListService.GetShoppingListByIdAsync(listId, userId);

            if (list == null) {
                return NotFound(new ApiResponse<ShoppingListDto>
                {
                    Success = false,
                    Message = "Shopping list not found"
                });
            }

            return Ok(new ApiResponse<ShoppingListDto>
            {
                Success = true,
                Message = "Shopping list retrieved successfully",
                Data = list
            });
        }

        /// <summary>
        /// Створити новий список покупок
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ShoppingListDto>>> CreateList(
            [FromBody] CreateShoppingListDto dto,
            [FromQuery] int userId)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new ApiResponse<ShoppingListDto>
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var list = await _shoppingListService.CreateShoppingListAsync(dto, userId);

            if (list == null) {
                return BadRequest(new ApiResponse<ShoppingListDto>
                {
                    Success = false,
                    Message = "Failed to create shopping list"
                });
            }

            return CreatedAtAction(
                nameof(GetList),
                new { listId = list.ListId, userId = userId },
                new ApiResponse<ShoppingListDto>
                {
                    Success = true,
                    Message = "Shopping list created successfully",
                    Data = list
                });
        }

        /// <summary>
        /// Видалити список покупок
        /// </summary>
        [HttpDelete("{listId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteList(int listId, [FromQuery] int userId)
        {
            var success = await _shoppingListService.DeleteShoppingListAsync(listId, userId);

            if (!success) {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Shopping list not found"
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Shopping list deleted successfully",
                Data = true
            });
        }

        /// <summary>
        /// Додати товар до списку
        /// </summary>
        [HttpPost("{listId}/items")]
        public async Task<ActionResult<ApiResponse<ShoppingListItemDto>>> AddItem(
            int listId,
            [FromBody] CreateShoppingListItemDto dto,
            [FromQuery] int userId)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new ApiResponse<ShoppingListItemDto>
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var item = await _shoppingListService.AddItemToListAsync(listId, dto, userId);

            if (item == null) {
                return NotFound(new ApiResponse<ShoppingListItemDto>
                {
                    Success = false,
                    Message = "Shopping list not found"
                });
            }

            return Ok(new ApiResponse<ShoppingListItemDto>
            {
                Success = true,
                Message = "Item added successfully",
                Data = item
            });
        }

        /// <summary>
        /// Позначити/зняти позначку "куплено"
        /// </summary>
        [HttpPatch("items/{itemId}/toggle")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleItemPurchased(int itemId, [FromQuery] int userId)
        {
            var success = await _shoppingListService.ToggleItemPurchasedAsync(itemId, userId);

            if (!success) {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Item not found"
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Item status toggled successfully",
                Data = true
            });
        }

        /// <summary>
        /// Видалити товар зі списку
        /// </summary>
        [HttpDelete("items/{itemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteItem(int itemId, [FromQuery] int userId)
        {
            var success = await _shoppingListService.DeleteItemAsync(itemId, userId);

            if (!success) {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Item not found"
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Item deleted successfully",
                Data = true
            });
        }
    }
}