using Eggs.Api.Grains.TrafficCameraManagement;
using Orleans.Runtime;

namespace Eggs.Api.Grains.TrafficCamera;

public sealed class TrafficCameraGrain(
    [PersistentState("TrafficCamera", "traffic-camera")]
    IPersistentState<TrafficCameraState> state
) : Grain, ITrafficCameraGrain
{
    public Task<TrafficCameraState> GetStateAsync()
    {
        return Task.FromResult(state.State);
    }

    public async Task CreateTrafficCameraAsync(TrafficCameraState initialState)
    {
        state.State = initialState;
        var management = GrainFactory.GetGrain<ITrafficCameraManagementGrain>(0);
        await management.AddTrafficCameraAsync(this.GetPrimaryKeyString());
        await state.WriteStateAsync();
    }

    public Task UpdateLastSeenAtAsync()
    {
        state.State.LastSeenAt = DateTimeOffset.Now;
        return Task.CompletedTask;
    }

    public async Task DetectedVehicleAsync(string id)
    {
        throw new NotImplementedException();
    }
}