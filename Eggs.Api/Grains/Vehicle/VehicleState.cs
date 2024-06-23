using Eggs.Api.Core;

namespace Eggs.Api.Grains.Vehicle;

[GenerateSerializer]
[Alias("Eggs.Api.Grains.Vehicle.VehicleState")]
public class VehicleState
{
    [Id(0)] public string Id { get; set; }

    /// <summary>
    /// User defined friendly name. Will be null if <see cref="User"/> is null.
    /// </summary>
    [Id(1)]
    public string FriendlyName { get; set; }

    [Id(2)] public VehicleType VehicleType { get; set; }

    [Id(3)] public DateTimeOffset FirstSeenAt { get; set; }
    [Id(4)] public string FirstSeenAtTrafficCamera { get; set; }
    [Id(5)] public DateTimeOffset LastSeenAt { get; set; }
    [Id(6)] public string LastSeenAtTrafficCamera { get; set; }

    [Id(7)] public List<Guid> Charges { get; set; }
    
    [Id(8)] public List<Guid> Sightings { get; set; }

    /// <summary>
    /// <see cref="Eggs.Api.Grains.User.IUserGrain"/> to associate to once the vehicle has been claimed.
    /// </summary>
    [Id(9)]
    public string? User { get; set; }
}