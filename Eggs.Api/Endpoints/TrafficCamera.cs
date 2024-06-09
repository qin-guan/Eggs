using Eggs.Api.Grains.TrafficCamera;
using Eggs.Api.Grains.TrafficCameraManagement;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Eggs.Api.Endpoints;

public static class TrafficCamera
{
    public static IEndpointRouteBuilder MapCameraEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("traffic-cameras");
        api.MapGet("/", GetTrafficCameras);

        return app;
    }

    private static async Task<Ok<List<TrafficCameraState>>> GetTrafficCameras(
        IGrainFactory grainFactory
    )
    {
        var managementGrain = grainFactory.GetGrain<ITrafficCameraManagementGrain>(0);
        var cameraIds = await managementGrain.GetTrafficCamerasAsync();
        var cameras = await Task.WhenAll(
            cameraIds.Select(async (id) =>
                await grainFactory.GetGrain<ITrafficCameraGrain>(id)
                    .GetStateAsync()
            )
        );

        return TypedResults.Ok(cameras.ToList());
    }
}