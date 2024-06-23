using Eggs.Api.Extensions;
using Eggs.Api.Grains.TrafficCamera;
using Orleans.Runtime;

namespace Eggs.Api.StartupTasks;

public sealed class SeedTrafficCameraData(IGrainFactory grainFactory) : IStartupTask
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        var faker = new TrafficCameraState().GetBogusFaker();

        var grain = grainFactory.GetGrain<ITrafficCameraGrain>("01-default");
        var state = faker.Generate();
        state.Id = "01-default";
        await grain.CreateAsync(state);

        var tasks = faker.GenerateLazy(5).Select(async camera =>
        {
            var grain = grainFactory.GetGrain<ITrafficCameraGrain>(camera.Id);
            await grain.CreateAsync(camera);
        });

        await Task.WhenAll(tasks);
    }
}