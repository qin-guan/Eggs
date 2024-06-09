namespace Eggs.Api.Grains.Charge;

public class ChargeState
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public string User { get; set; }
    [Id(2)] public string TrafficCamera { get; set; }
    [Id(3)] public DateTimeOffset CreatedAt { get; set; }
    [Id(4)] public DateTimeOffset? PaidAt { get; set; }
}