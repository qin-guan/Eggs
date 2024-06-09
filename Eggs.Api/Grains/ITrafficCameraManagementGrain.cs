namespace Eggs.Api.Grains;

public interface ITrafficCameraManagementGrain : IGrainWithIntegerKey
{
    public Task<HashSet<string>> GetTrafficCamerasAsync();
    public Task AddTrafficCameraAsync(string id);
}