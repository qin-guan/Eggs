namespace Eggs.Api.Grains.Vehicle;

/// <summary>
/// Grain key is car plate number.
/// </summary>
public interface IVehicleGrain : IGrainWithStringKey
{
    public Task<VehicleState> GetVehicleStateAsync();
    public Task CreateOrUpdateVehicleAsync(VehicleState initialState);
}