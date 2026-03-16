using MenuDigital.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MenuDigital.Domain.Interfaces
{
    public interface IMenuService
    {
        Task<RestaurantDto?> GetRestaurantBySlugAsync(string slug);
        Task<List<CategoryDto>> GetCategoriesAsync(string slug);
        Task<List<MenuItemDto>> GetMenuItemsAsync(string slug, int categoryId);
    }
}
