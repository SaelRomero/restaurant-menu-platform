using MenuDigital.Domain.DTOs;
using System.Threading.Tasks;

namespace MenuDigital.Domain.Interfaces
{
    public interface IAdminService
    {
        Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantDto dto);
        Task<CategoryDto> CreateCategoryAsync(int restaurantId, CreateCategoryDto dto);
        Task<MenuItemDto> CreateMenuItemAsync(int restaurantId, CreateMenuItemDto dto);
        Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<byte[]> GenerateQrCodeAsync(int restaurantId, string domain);
    }
}
