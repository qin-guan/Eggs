using Eggs.Api.Core;
using Orleans.Runtime;

namespace Eggs.Api.Grains.Vehicle;

public class VehicleGrain(
    [PersistentState("Vehicle", "vehicle")]
    IPersistentState<VehicleState> state
) : Grain, IVehicleGrain
{
    public async Task CreateOrUpdateAsync(string id, VehicleType vehicleType, string trafficCamera, Guid sighting)
    {
        if (string.IsNullOrEmpty(state.State.Id))
        {
            state.State.Id = id;
            state.State.VehicleType = vehicleType;
            state.State.FirstSeenAt = DateTimeOffset.Now;
            state.State.FirstSeenAtTrafficCamera = trafficCamera;
            state.State.Charges = [];
            state.State.Sightings = [];
        }

        state.State.LastSeenAt = DateTimeOffset.Now;
        state.State.LastSeenAtTrafficCamera = trafficCamera;
        state.State.Sightings.Add(sighting);

        await state.WriteStateAsync();
    }

    public Task<VehicleState> GetVehicleStateAsync()
    {
        return Task.FromResult(state.State);
    }

    public async Task UpdateUserAsync(string user)
    {
        state.State.User = user;
        await state.WriteStateAsync();
    }

    public async Task AddChargeAsync(Guid charge)
    {
        state.State.Charges.Add(charge);
        await state.WriteStateAsync();
    }
}