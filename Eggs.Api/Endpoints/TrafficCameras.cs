using Eggs.Api.Grains.TrafficCamera;
using Eggs.Api.Grains.TrafficCameraManager;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Eggs.Api.Endpoints;

public static class TrafficCameras
{
    public static IEndpointRouteBuilder MapCameraEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("traffic-cameras");
        api.MapGet("/", GetTrafficCameras);
        api.MapPost("/{cameraId}/sightings/{vehicleId}", CreateSighting);

        return app;
    }

    private static async Task<Ok<List<TrafficCameraState>>> GetTrafficCameras(
        IGrainFactory grainFactory
    )
    {
        var managerGrain = grainFactory.GetGrain<ITrafficCameraManagerGrain>(0);
        var cameraIds = await managerGrain.GetTrafficCamerasAsync();
        var cameras = await Task.WhenAll(
            cameraIds.Select(async (id) =>
                await grainFactory.GetGrain<ITrafficCameraGrain>(id)
                    .GetAsync()
            )
        );

        return TypedResults.Ok(cameras.ToList());
    }

    private static async Task<Created<Guid>> CreateSighting(
        IGrainFactory grainFactory,
        string cameraId,
        string vehicleId
    )
    {
        var trafficCamera = grainFactory.GetGrain<ITrafficCameraGrain>(cameraId);
        var sightingId = await trafficCamera.AddSightingAsync(vehicleId);

        return TypedResults.Created($"/sightings/{sightingId}", sightingId);
    }
}