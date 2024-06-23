namespace Eggs.Api.Grains.Sighting;

[GenerateSerializer]
[Alias("Eggs.Api.Grains.Sighting.SightingState")]
public class SightingState
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public string Vehicle { get; set; }
    [Id(2)] public string TrafficCamera { get; set; }
    [Id(3)] public int Lane { get; set; }
    [Id(4)] public DateTimeOffset CreatedAt { get; set; }
}
