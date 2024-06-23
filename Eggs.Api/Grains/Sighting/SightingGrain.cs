using Eggs.Api.Core;
using Eggs.Api.Grains.Vehicle;
using Orleans.Runtime;

namespace Eggs.Api.Grains.Sighting;

public sealed class SightingGrain(
    [PersistentState("Sighting", "sighting")]
    IPersistentState<SightingState> state,
    ILogger<SightingGrain> logger
) : Grain, ISightingGrain
{
    public async Task CreateAsync(SightingState initialState)
    {
        state.State = initialState;
        await state.WriteStateAsync();

        var vehicle = GrainFactory.GetGrain<IVehicleGrain>(initialState.Vehicle);
        await vehicle.CreateOrUpdateAsync(
            initialState.Vehicle,
            VehicleType.Motorbike,
            initialState.TrafficCamera,
            this.GetPrimaryKey()
        );

        var streamId = StreamId.Create(Constants.StreamConstants.SightingsNamespace, this.GetPrimaryKey());
        var stream = this.GetStreamProvider(Constants.StreamConstants.DefaultProvider)
            .GetStream<SightingState>(streamId);
        await stream.OnNextAsync(state.State);
    }

    public Task<SightingState> GetAsync()
    {
        if (state.State.Id == Guid.Empty)
        {
            throw new Exception("SightingGrain was called before created.");
        }

        return Task.FromResult(state.State);
    }
}