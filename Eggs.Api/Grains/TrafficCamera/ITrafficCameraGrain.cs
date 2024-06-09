namespace Eggs.Api.Grains.TrafficCamera;

public interface ITrafficCameraGrain : IGrainWithStringKey
{
    public Task<TrafficCameraState> GetStateAsync();
    public Task CreateTrafficCameraAsync(TrafficCameraState state);
    public Task UpdateLastSeenAtAsync();
    public Task DetectedVehicleAsync(string id);
}