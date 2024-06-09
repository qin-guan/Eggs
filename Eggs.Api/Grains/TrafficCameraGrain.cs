using Orleans.Runtime;

namespace Eggs.Api.Grains;

public sealed class TrafficCameraGrain(
    [PersistentState("TrafficCamera", "traffic-camera")] IPersistentState<TrafficCameraState> state
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

    public Task UpdateLastSeenAsync()
    {
        state.State.LastSeen = DateTimeOffset.Now;
        return Task.CompletedTask;
    }
}