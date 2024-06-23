using Eggs.Api.Grains.Sighting;
using Eggs.Api.Grains.Vehicle;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Eggs.Api.Endpoints;

public static class Sightings
{
    public static IEndpointRouteBuilder MapSightingsEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("sightings");
        api.MapGet("/", GetSightings);
        api.MapGet("/{sightingId:guid}", GetSighting);

        return app;
    }

    private static async Task<Results<BadRequest<string>, Ok<List<SightingState>>>> GetSightings(
        IGrainFactory grainFactory,
        [FromQuery] string vehicleId
    )
    {
        if (string.IsNullOrWhiteSpace(vehicleId))
        {
            return TypedResults.BadRequest("Vehicle not provided");
        }

        var vehicleGrain = grainFactory.GetGrain<IVehicleGrain>(vehicleId);
        var vehicle = await vehicleGrain.GetVehicleStateAsync();
        var tasks = vehicle.Sightings
            .Select(async id => await grainFactory.GetGrain<ISightingGrain>(id).GetAsync())
            .AsParallel();

        var sightings = await Task.WhenAll(tasks);

        return TypedResults.Ok(sightings.ToList());
    }

    private static async Task<Ok<SightingState>> GetSighting(
        IGrainFactory grainFactory,
        [FromRoute] Guid sightingId
    )
    {
        var sightingGrain = grainFactory.GetGrain<ISightingGrain>(sightingId);
        var sighting = await sightingGrain.GetAsync();

        return TypedResults.Ok(sighting);
    }
}