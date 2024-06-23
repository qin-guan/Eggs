using Eggs.Api.Constants;
using Eggs.Api.Core;
using Eggs.Api.Grains.Charge;
using Eggs.Api.Grains.Charge.Events;
using Eggs.Api.Grains.Sighting;
using Eggs.Api.Options;
using Microsoft.Extensions.Options;
using Orleans.Runtime;
using Orleans.Streams;

namespace Eggs.Api.Grains.SightingToChargeProcessor;

[ImplicitStreamSubscription(StreamConstants.SightingsNamespace)]
public class SightingToChargeProcessorGrain(
    IOptions<ChargeOptions> chargeOptions,
    ILogger<SightingToChargeProcessorGrain> logger
) : Grain, ISightingToChargeProcessorGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Activated for sighting {Id}", this.GetPrimaryKey());

        var streamProvider = this.GetStreamProvider(StreamConstants.DefaultProvider);
        var streamId = StreamId.Create(StreamConstants.SightingsNamespace, this.GetPrimaryKey());
        var stream = streamProvider.GetStream<SightingState>(streamId);

        await stream.SubscribeAsync(OnNextAsync);
    }

    public async Task OnNextAsync(SightingState sighting, StreamSequenceToken? token = null)
    {
        var trafficCameraInRules = chargeOptions.Value.Rules.ContainsKey(sighting.TrafficCamera);
        var rule = trafficCameraInRules
            ? chargeOptions.Value.Rules[sighting.TrafficCamera]
            : chargeOptions.Value.Default;

        if (!rule.TryGetValue(DateTime.Now.DayOfWeek, out var todayRule))
        {
            logger.LogInformation(
                "No charge created for sighting {Id} as there are no rules for {Day}",
                this.GetPrimaryKey(), DateTime.Now.DayOfWeek
            );

            return;
        }

        // TODO : Determine vehicle type
        var carType = VehicleType.Motorbike;

        if (!todayRule.TryGetValue(carType, out var todayCarRule))
        {
            logger.LogInformation(
                "No charge created for sighting {Id} as there are no rules for {Type}",
                this.GetPrimaryKey(), carType
            );

            return;
        }

        var time = TimeOnly.FromDateTime(DateTime.Now);
        var activeRule = todayCarRule.SingleOrDefault(r => time > r.StartTime && time < r.EndTime);
        if (activeRule is null)
        {
            logger.LogInformation(
                "No charge created for sighting {Id} as there are no rules for {Time}",
                this.GetPrimaryKey(), time
            );

            return;
        }

        var chargeId = Guid.NewGuid();
        var charge = GrainFactory.GetGrain<IChargeGrain>(chargeId);

        logger.LogInformation(
            "Charge {ChargeId} created for {SightingId}",
            chargeId, this.GetPrimaryKey()
        );

        await charge.CreateAsync(
            new ChargeCreatedEvent(
                chargeId,
                this.GetPrimaryKey(),
                activeRule.Amount
            )
        );
    }
}