using Eggs.Api.Grains.Charge.Events;
using Eggs.Api.Grains.Sighting;
using Eggs.Api.Grains.Vehicle;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Runtime;

namespace Eggs.Api.Grains.Charge;

[StorageProvider(ProviderName = "charge-log-storage")]
[LogConsistencyProvider(ProviderName = "charge-log-storage")]
public sealed class ChargeGrain(
    [PersistentState("Charge", "charge")] IPersistentState<ChargeState> state
) : JournaledGrain<ChargeState>, IChargeGrain
{
    public async Task CreateAsync(ChargeCreatedEvent @event)
    {
        var sightingGrain = GrainFactory.GetGrain<ISightingGrain>(@event.Sighting);
        var sighting = await sightingGrain.GetAsync();
        var vehicleGrain = GrainFactory.GetGrain<IVehicleGrain>(sighting.Vehicle);
        await vehicleGrain.AddChargeAsync(this.GetPrimaryKey());
        
        RaiseEvent(@event);
        await ConfirmEvents();
    }

    public async Task CreatePaymentIntentAsync(PaymentIntentCreatedEvent @event)
    {
        RaiseEvent(@event);
        await ConfirmEvents();
    }

    public async Task CompletePaymentAsync(PaymentCompletedEvent @event)
    {
        RaiseEvent(@event);
        await ConfirmEvents();
    }
}