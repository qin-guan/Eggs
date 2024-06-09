using Eggs.Api.Extensions;
using Eggs.Api.Grains.TrafficCamera;
using Orleans.Runtime;

namespace Eggs.Api.StartupTasks;

public sealed class SeedTrafficCameraData(IGrainFactory grainFactory) : IStartupTask
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        var faker = new TrafficCameraState().GetBogusFaker();

        var tasks = faker.GenerateLazy(5).Select(async camera =>
        {
            var grain = grainFactory.GetGrain<ITrafficCameraGrain>(camera.Id);
            await grain.CreateTrafficCameraAsync(camera);
        });

        await Task.WhenAll(tasks);
    }
}