using Orleans.Runtime;

namespace Eggs.Api.Grains.TrafficCameraManagement;

public sealed class TrafficCameraManagementGrain(
    [PersistentState("TrafficCameraManagement", "traffic-camera-management")]
    IPersistentState<HashSet<string>> state
) : Grain, ITrafficCameraManagementGrain
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