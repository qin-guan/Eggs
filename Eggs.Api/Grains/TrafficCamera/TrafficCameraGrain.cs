using Eggs.Api.Grains.Sighting;
using Eggs.Api.Grains.TrafficCameraManager;
using Orleans.Runtime;

namespace Eggs.Api.Grains.TrafficCamera;

public sealed class TrafficCameraGrain(
    [PersistentState("TrafficCamera", "traffic-camera")]
    IPersistentState<TrafficCameraState> state
) : Grain, ITrafficCameraGrain
{
    public async Task CreateAsync(TrafficCameraState initialState)
    {
        state.State = initialState;
        await state.WriteStateAsync();

        var manager = GrainFactory.GetGrain<ITrafficCameraManagerGrain>(0);
        await manager.AddTrafficCameraAsync(this.GetPrimaryKeyString());
    }

    public Task<TrafficCameraState> GetAsync()
    {
        if (string.IsNullOrEmpty(state.State.Id))
        {
            throw new Exception("TrafficCameraGrain was called before created.");
        }

        return Task.FromResult(state.State);
    }

    public async Task UpdateLastSeenAtAsync()
    {
        if (string.IsNullOrEmpty(state.State.Id))
        {
            throw new Exception("TrafficCameraGrain was called before created.");
        }

        state.State.LastSeenAt = DateTimeOffset.Now;
        await state.WriteStateAsync();
    }

    public async Task<Guid> AddSightingAsync(string vehicleId)
    {
        if (string.IsNullOrEmpty(state.State.Id))
        {
            throw new Exception("TrafficCameraGrain was called before created.");
        }

        var sightingId = Guid.NewGuid();
        var sighting = GrainFactory.GetGrain<ISightingGrain>(sightingId);

        await sighting.CreateAsync(
            new SightingState
            {
                Id = sightingId,
                Vehicle = vehicleId,
                TrafficCamera = this.GetPrimaryKeyString(),
                Lane = 0,
                CreatedAt = DateTimeOffset.Now
            }
        );

        return sightingId;
    }
}