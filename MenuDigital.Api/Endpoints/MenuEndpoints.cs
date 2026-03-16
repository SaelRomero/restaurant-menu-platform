using MenuDigital.Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.IO;

namespace MenuDigital.Api.Endpoints
{
    public static class MenuEndpoints
    {
        public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("");

            group.MapGet("/{slug}", (string slug, IWebHostEnvironment env) =>
            {
                var filePath = Path.Combine(env.WebRootPath, "index.html");
                return Results.File(filePath, "text/html");
            });

            group.MapGet("/{slug}/restaurant", async (string slug, IMenuService service) =>
            {
                var r = await service.GetRestaurantBySlugAsync(slug);
                return r is not null ? Results.Ok(r) : Results.NotFound();
            });

            group.MapGet("/{slug}/categories", async (string slug, IMenuService service) =>
            {
                return Results.Ok(await service.GetCategoriesAsync(slug));
            });

            group.MapGet("/{slug}/items/{categoryId}", async (string slug, int categoryId, IMenuService service) =>
            {
                return Results.Ok(await service.GetMenuItemsAsync(slug, categoryId));
            });
        }
    }
}
