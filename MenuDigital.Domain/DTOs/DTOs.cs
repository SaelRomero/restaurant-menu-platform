using System;
using System.Collections.Generic;

namespace MenuDigital.Domain.DTOs
{
    public record RestaurantDto(int Id, string Name, string Slug, string LogoUrl, string? BannerUrl, string Address, string Phone, string? WhatsappNumber, bool IsActive);
    public record CategoryDto(int Id, string Name, int SortOrder, bool IsVisible);
    public record MenuItemDto(int Id, string Name, string Description, decimal Price, string PhotoUrl, int CategoryId, bool IsAvailable, bool IsPromotion, decimal DiscountPercent);
    
    public record CreateRestaurantDto(string Name, string Slug, string LogoUrl, string? BannerUrl, string Address, string Phone, string? WhatsappNumber);
    public record CreateCategoryDto(string Name, int SortOrder);
    public record CreateMenuItemDto(string Name, string Description, decimal Price, string PhotoUrl, int CategoryId, bool IsPromotion, decimal DiscountPercent);
    public record UpdateMenuItemDto(string Name, string Description, decimal Price, string PhotoUrl, int CategoryId, bool IsAvailable, bool IsPromotion, decimal DiscountPercent);
}
