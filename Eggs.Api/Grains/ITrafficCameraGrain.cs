namespace Eggs.Api.Grains;

public interface ITrafficCameraGrain : IGrainWithStringKey
{
    public Task<TrafficCameraState> GetStateAsync();
    public Task CreateTrafficCameraAsync(TrafficCameraState state);
    public Task UpdateLastSeenAsync();
}

[GenerateSerializer]
[Alias("Eggs.Api.Grains.TrafficCameraState")]
public class TrafficCameraState
{
    [Id(0)] public string Id { get; set; }
    [Id(1)] public string FriendlyName { get; set; }
    [Id(2)] public string Location { get; set; }
    [Id(3)] public double Latitude { get; set; }
    [Id(4)] public double Longitude { get; set; }
    [Id(5)] public DateTimeOffset FirstSeen { get; set; }
    [Id(6)] public DateTimeOffset LastSeen { get; set; }
}