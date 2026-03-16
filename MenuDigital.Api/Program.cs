using MenuDigital.Domain.Entities;
using MenuDigital.Domain.Interfaces;
using MenuDigital.Infrastructure.Data;
using MenuDigital.Infrastructure.Services;
using MenuDigital.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<MenuDigitalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MenuDigitalContext>();
    context.Database.Migrate();

    if (!context.Restaurants.Any())
    {
        var demoRestaurant = new Restaurant
        {
            Name = "Restaurante Demo",
            Slug = "demo",
            LogoUrl = "https://ui-avatars.com/api/?name=Restaurante+Demo&background=random&size=256",
            Address = "Dirección de ejemplo",
            Phone = "5551234567",
            IsActive = true
        };

        context.Restaurants.Add(demoRestaurant);
        context.SaveChanges();

        var catPizzas = new Category { Name = "Pizzas", RestaurantId = demoRestaurant.Id, SortOrder = 1 };
        var catBebidas = new Category { Name = "Bebidas", RestaurantId = demoRestaurant.Id, SortOrder = 2 };
        var catPostres = new Category { Name = "Postres", RestaurantId = demoRestaurant.Id, SortOrder = 3 };

        context.Categories.AddRange(catPizzas, catBebidas, catPostres);
        context.SaveChanges();

        context.MenuItems.AddRange(
            new MenuItem { Name = "Pizza Pepperoni", Description = "Clásica con queso mozzarella", Price = 150m, CategoryId = catPizzas.Id, PhotoUrl = "https://images.unsplash.com/photo-1534308983496-4fabb1a015ce?w=400&q=80" },
            new MenuItem { Name = "Pizza Hawaiana", Description = "Piña y jamón", Price = 160m, CategoryId = catPizzas.Id, PhotoUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&q=80" },
            new MenuItem { Name = "Pizza Mexicana", Description = "Chorizo, jalapeño y cebolla", Price = 180m, CategoryId = catPizzas.Id, IsPromotion = true, DiscountPercent = 10, PhotoUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400&q=80" },
            new MenuItem { Name = "Refresco Cola", Description = "600 ml", Price = 30m, CategoryId = catBebidas.Id, PhotoUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=400&q=80" },
            new MenuItem { Name = "Agua Fresca", Description = "Limón con chía 1L", Price = 40m, CategoryId = catBebidas.Id, PhotoUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=400&q=80" },
            new MenuItem { Name = "Cerveza Artesanal", Description = "IPA 355ml", Price = 80m, CategoryId = catBebidas.Id, PhotoUrl = "https://images.unsplash.com/photo-1535958636474-b021ee887b13?w=400&q=80" },
            new MenuItem { Name = "Tiramisú", Description = "Postre italiano clásico", Price = 90m, CategoryId = catPostres.Id, PhotoUrl = "https://images.unsplash.com/photo-1571115177098-24ec42ed204d?w=400&q=80" },
            new MenuItem { Name = "Helado", Description = "Vainilla o Chocolate", Price = 50m, CategoryId = catPostres.Id, PhotoUrl = "https://images.unsplash.com/photo-1497034825429-c343d7c6a68f?w=400&q=80" }
        );
        context.SaveChanges();
    }
}

app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.MapMenuEndpoints();
app.MapAdminEndpoints();

app.MapFallbackToFile("index.html");

app.Run();
