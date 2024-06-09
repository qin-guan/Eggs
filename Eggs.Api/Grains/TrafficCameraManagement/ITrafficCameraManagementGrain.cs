namespace Eggs.Api.Grains.TrafficCameraManagement;

public interface ITrafficCameraManagementGrain : IGrainWithIntegerKey
{
    public Task<HashSet<string>> GetTrafficCamerasAsync();
    public Task AddTrafficCameraAsync(string id);
}