using Eggs.Api.Grains;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Eggs.Api.Endpoints;

public static class CameraEndpoints
{
    public static IEndpointRouteBuilder MapCameraEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("cameras");
        api.MapGet("/", GetCameras);

        return app;
    }

    private static async Task<Ok<List<TrafficCameraState>>> GetCameras(
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