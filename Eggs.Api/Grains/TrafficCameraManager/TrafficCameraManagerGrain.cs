using Orleans.Runtime;

namespace Eggs.Api.Grains.TrafficCameraManager;

public sealed class TrafficCameraManagerGrain(
    [PersistentState("TrafficCameraManager", "traffic-camera-manager")]
    IPersistentState<HashSet<string>> state
) : Grain, ITrafficCameraManagerGrain
{
    public Task<HashSet<string>> GetTrafficCamerasAsync()
    {
        return Task.FromResult(state.State);
    }

    public async Task AddTrafficCameraAsync(string id)
    {
        state.State.Add(id);
        await state.WriteStateAsync();
    }
}