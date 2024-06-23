namespace Eggs.Api.Grains.Charge.Events;

[GenerateSerializer]
[Alias("Eggs.Api.Grains.Charge.Events.ChargeCreatedEvent")]
public record ChargeCreatedEvent(
    [property: Id(0)] Guid Id,
    [property: Id(1)] Guid Sighting,
    [property: Id(2)] double Amount
);