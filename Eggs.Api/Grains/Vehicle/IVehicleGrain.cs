using Eggs.Api.Core;

namespace Eggs.Api.Grains.Vehicle;

/// <summary>
/// Grain key is car plate number.
/// </summary>
public interface IVehicleGrain : IGrainWithStringKey
{
    public Task CreateOrUpdateAsync(
        string id,
        VehicleType vehicleType,
        string trafficCamera,
        Guid sighting
    );

    public Task<VehicleState> GetVehicleStateAsync();

    public Task UpdateUserAsync(string user);

    public Task AddChargeAsync(Guid charge);
}