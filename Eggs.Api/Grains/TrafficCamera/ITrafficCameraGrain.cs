namespace Eggs.Api.Grains.TrafficCamera;

public interface ITrafficCameraGrain : IGrainWithStringKey
{
    public Task CreateAsync(TrafficCameraState initialState);
    public Task<TrafficCameraState> GetAsync();
    public Task UpdateLastSeenAtAsync();
    public Task<Guid> AddSightingAsync(string vehicleId);
}