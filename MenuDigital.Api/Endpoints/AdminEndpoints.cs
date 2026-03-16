using MenuDigital.Domain.DTOs;
using MenuDigital.Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MenuDigital.Api.Endpoints
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/admin");

            group.MapPost("/restaurants", async (CreateRestaurantDto dto, IAdminService service) =>
            {
                return Results.Created($"/", await service.CreateRestaurantAsync(dto));
            });

            group.MapPost("/{restaurantId}/categories", async (int restaurantId, CreateCategoryDto dto, IAdminService service) =>
            {
                return Results.Ok(await service.CreateCategoryAsync(restaurantId, dto));
            });

            group.MapPost("/{restaurantId}/items", async (int restaurantId, CreateMenuItemDto dto, IAdminService service) =>
            {
                try {
                    return Results.Ok(await service.CreateMenuItemAsync(restaurantId, dto));
                } catch { return Results.BadRequest(); }
            });

            group.MapPut("/items/{id}", async (int id, UpdateMenuItemDto dto, IAdminService service) =>
            {
                var r = await service.UpdateMenuItemAsync(id, dto);
                return r is not null ? Results.Ok(r) : Results.NotFound();
            });

            group.MapDelete("/items/{id}", async (int id, IAdminService service) =>
            {
                var deleted = await service.DeleteMenuItemAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            });

            group.MapGet("/{restaurantId}/qr", async (int restaurantId, HttpContext ctx, IAdminService service) =>
            {
                try {
                    var domain = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
                    var bytes = await service.GenerateQrCodeAsync(restaurantId, domain);
                    return Results.File(bytes, "image/png");
                } catch { return Results.NotFound(); }
            });
        }
    }
}
