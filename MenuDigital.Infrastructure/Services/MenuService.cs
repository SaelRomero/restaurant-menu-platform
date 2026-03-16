using MenuDigital.Domain.DTOs;
using MenuDigital.Domain.Interfaces;
using MenuDigital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MenuDigital.Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly MenuDigitalContext _context;
        public MenuService(MenuDigitalContext context) => _context = context;

        public async Task<RestaurantDto?> GetRestaurantBySlugAsync(string slug)
        {
            var r = await _context.Restaurants.AsNoTracking().FirstOrDefaultAsync(x => x.Slug == slug);
            return r == null ? null : new RestaurantDto(r.Id, r.Name, r.Slug, r.LogoUrl, r.BannerUrl, r.Address, r.Phone, r.WhatsappNumber, r.IsActive);
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync(string slug)
        {
            var restId = await _context.Restaurants.AsNoTracking()
                .Where(x => x.Slug == slug).Select(x => x.Id).FirstOrDefaultAsync();
            if (restId == 0) return new List<CategoryDto>();

            return await _context.Categories.AsNoTracking()
                .Where(c => c.RestaurantId == restId && c.IsVisible)
                .OrderBy(c => c.SortOrder)
                .Select(c => new CategoryDto(c.Id, c.Name, c.SortOrder, c.IsVisible))
                .ToListAsync();
        }

        public async Task<List<MenuItemDto>> GetMenuItemsAsync(string slug, int categoryId)
        {
            var cat = await _context.Categories.AsNoTracking()
                .Include(c => c.Restaurant)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.Restaurant != null && c.Restaurant.Slug == slug);
            if (cat == null) return new List<MenuItemDto>();

            return await _context.MenuItems.AsNoTracking()
                .Where(m => m.CategoryId == categoryId && m.IsAvailable)
                .Select(m => new MenuItemDto(m.Id, m.Name, m.Description, m.Price, m.PhotoUrl, m.CategoryId, m.IsAvailable, m.IsPromotion, m.DiscountPercent))
                .ToListAsync();
        }
    }
}
