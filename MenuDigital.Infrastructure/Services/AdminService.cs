using MenuDigital.Domain.DTOs;
using MenuDigital.Domain.Entities;
using MenuDigital.Domain.Interfaces;
using MenuDigital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Threading.Tasks;

namespace MenuDigital.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly MenuDigitalContext _context;
        public AdminService(MenuDigitalContext context) => _context = context;

        public async Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantDto dto)
        {
            var r = new Restaurant { Name = dto.Name, Slug = dto.Slug, LogoUrl = dto.LogoUrl, BannerUrl = dto.BannerUrl, Address = dto.Address, Phone = dto.Phone, WhatsappNumber = dto.WhatsappNumber };
            _context.Restaurants.Add(r);
            await _context.SaveChangesAsync();
            return new RestaurantDto(r.Id, r.Name, r.Slug, r.LogoUrl, r.BannerUrl, r.Address, r.Phone, r.WhatsappNumber, r.IsActive);
        }

        public async Task<CategoryDto> CreateCategoryAsync(int restaurantId, CreateCategoryDto dto)
        {
            var c = new Category { Name = dto.Name, RestaurantId = restaurantId, SortOrder = dto.SortOrder };
            _context.Categories.Add(c);
            await _context.SaveChangesAsync();
            return new CategoryDto(c.Id, c.Name, c.SortOrder, c.IsVisible);
        }

        public async Task<MenuItemDto> CreateMenuItemAsync(int restaurantId, CreateMenuItemDto dto)
        {
            // Verify category belongs to restaurant
            var cat = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.RestaurantId == restaurantId);
            if (cat == null) throw new System.Exception("Category not found for this restaurant.");

            var m = new MenuItem { Name = dto.Name, Description = dto.Description, Price = dto.Price, PhotoUrl = dto.PhotoUrl, CategoryId = dto.CategoryId, IsPromotion = dto.IsPromotion, DiscountPercent = dto.DiscountPercent };
            _context.MenuItems.Add(m);
            await _context.SaveChangesAsync();
            return new MenuItemDto(m.Id, m.Name, m.Description, m.Price, m.PhotoUrl, m.CategoryId, m.IsAvailable, m.IsPromotion, m.DiscountPercent);
        }

        public async Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
        {
            var m = await _context.MenuItems.FindAsync(id);
            if (m == null) return null;

            m.Name = dto.Name;
            m.Description = dto.Description;
            m.Price = dto.Price;
            m.PhotoUrl = dto.PhotoUrl;
            m.CategoryId = dto.CategoryId;
            m.IsAvailable = dto.IsAvailable;
            m.IsPromotion = dto.IsPromotion;
            m.DiscountPercent = dto.DiscountPercent;

            await _context.SaveChangesAsync();
            return new MenuItemDto(m.Id, m.Name, m.Description, m.Price, m.PhotoUrl, m.CategoryId, m.IsAvailable, m.IsPromotion, m.DiscountPercent);
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var m = await _context.MenuItems.FindAsync(id);
            if (m == null) return false;
            _context.MenuItems.Remove(m);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> GenerateQrCodeAsync(int restaurantId, string domain)
        {
            var r = await _context.Restaurants.FindAsync(restaurantId);
            if (r == null) throw new System.Exception("Restaurant not found.");

            string url = $"{domain}/{r.Slug}";
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(data);
            return qrCode.GetGraphic(20);
        }
    }
}
