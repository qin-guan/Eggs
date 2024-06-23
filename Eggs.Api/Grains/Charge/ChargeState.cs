using Eggs.Api.Grains.Charge.Events;

namespace Eggs.Api.Grains.Charge;

[GenerateSerializer]
[Alias("Eggs.Api.Grains.Charge.ChargeState")]
public class ChargeState
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public Guid Sighting { get; set; }
    [Id(2)] public double Amount { get; set; }
    [Id(3)] public DateTimeOffset CreatedAt { get; set; }
    [Id(4)] public string? PaymentIntent { get; set; }
    [Id(5)] public DateTimeOffset? PaymentCompletedAt { get; set; }

    public void Apply(ChargeCreatedEvent @event)
    {
        Id = @event.Id;
        Sighting = @event.Sighting;
        Amount = @event.Amount;
        CreatedAt = DateTimeOffset.Now;
    }

    public void Apply(PaymentIntentCreatedEvent @event)
    {
        PaymentIntent = @event.PaymentIntent;
    }

    public void Apply(PaymentCompletedEvent @event)
    {
        PaymentCompletedAt = DateTimeOffset.Now;
    }
}