using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}


public static class AreaEndpoints
{
	public static void MapAreaEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Area").WithTags(nameof(Area));

        group.MapGet("/", async (CoffeeShop01Context db) =>
        {
            return await db.Areas.ToListAsync();
        })
        .WithName("GetAllAreas")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Area>, NotFound>> (int areaid, CoffeeShop01Context db) =>
        {
            return await db.Areas.AsNoTracking()
                .FirstOrDefaultAsync(model => model.AreaId == areaid)
                is Area model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetAreaById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int areaid, Area area, CoffeeShop01Context db) =>
        {
            var affected = await db.Areas
                .Where(model => model.AreaId == areaid)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.AreaId, area.AreaId)
                  .SetProperty(m => m.AreaName, area.AreaName)
                  .SetProperty(m => m.Status, area.Status)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateArea")
        .WithOpenApi();

        group.MapPost("/", async (Area area, CoffeeShop01Context db) =>
        {
            db.Areas.Add(area);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Area/{area.AreaId}",area);
        })
        .WithName("CreateArea")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int areaid, CoffeeShop01Context db) =>
        {
            var affected = await db.Areas
                .Where(model => model.AreaId == areaid)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteArea")
        .WithOpenApi();
    }
}