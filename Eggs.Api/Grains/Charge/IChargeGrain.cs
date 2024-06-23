using Eggs.Api.Grains.Charge.Events;

namespace Eggs.Api.Grains.Charge;

public interface IChargeGrain : IGrainWithGuidKey
{
    public Task CreateAsync(ChargeCreatedEvent @event);
    public Task CreatePaymentIntentAsync(PaymentIntentCreatedEvent @event);
    public Task CompletePaymentAsync(PaymentCompletedEvent @event);
}