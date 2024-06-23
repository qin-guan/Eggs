namespace Eggs.Api.Grains.Charge.Events;

[GenerateSerializer]
[Alias("Eggs.Api.Grains.Charge.Events.PaymentIntentCreatedEvent")]
public record PaymentIntentCreatedEvent(
    [property: Id(0)] string PaymentIntent
);