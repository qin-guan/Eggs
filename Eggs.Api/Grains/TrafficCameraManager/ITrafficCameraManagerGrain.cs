namespace Eggs.Api.Grains.TrafficCameraManager;

public interface ITrafficCameraManagerGrain : IGrainWithIntegerKey
{
    public Task<HashSet<string>> GetTrafficCamerasAsync();
    public Task AddTrafficCameraAsync(string id);
}