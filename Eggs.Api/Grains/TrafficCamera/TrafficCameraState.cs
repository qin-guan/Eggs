namespace Eggs.Api.Grains.TrafficCamera;

[GenerateSerializer]
[Alias("Eggs.Api.Grains.TrafficCamera.TrafficCameraState")]
public class TrafficCameraState
{
    [Id(0)] public string Id { get; set; }
    [Id(1)] public string FriendlyName { get; set; }
    [Id(2)] public string Location { get; set; }
    [Id(3)] public double Latitude { get; set; }
    [Id(4)] public double Longitude { get; set; }
    [Id(5)] public DateTimeOffset FirstSeenAt { get; set; }
    [Id(6)] public DateTimeOffset LastSeenAt { get; set; }
}