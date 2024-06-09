using Eggs.Api.Endpoints;
using Eggs.Api.Options;
using Eggs.Api.StartupTasks;
using Eggs.ServiceDefaults;
using Orleans.Runtime;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOptions<ChargeOptions>()
    .Bind(builder.Configuration.GetSection("ChargeOptions"))
    .Validate((options) =>
    {
        foreach (var (camera, dayConfig)in options.Rules)
        {
            foreach (var (day, vehicleConfig) in dayConfig)
            {
                foreach (var (vehicle, rules) in vehicleConfig)
                {
                    var times = rules.OrderBy(r => r.StartTime).ToList();
                    if (times.Any(t => t.StartTime > t.EndTime))
                    {
                        throw new Exception($"Start time greater than end time for {camera} on {day} for {vehicle}.");
                    }

                    if (times.Any(t => t.Amount <= 0))
                    {
                        throw new Exception($"Amount is negative for {camera} on {day} for {vehicle}.");
                    }

                    for (var i = 0; i < times.Count - 1; i++)
                    {
                        if (times[i].EndTime > times[i + 1].StartTime)
                        {
                            throw new Exception($"Overlapping time rule for {camera} on {day} for {vehicle}.");
                        }
                    }
                }
            }
        }

        return true;
    })
    .ValidateOnStart();

builder.AddKeyedAzureTableClient("clustering");
builder.AddKeyedAzureBlobClient("traffic-camera");
builder.AddKeyedAzureBlobClient("traffic-camera-management");

builder.UseOrleans((orleans) =>
{
    if (builder.Environment.IsDevelopment())
    {
        orleans.AddStartupTask<SeedTrafficCameraData>();
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapDefaultEndpoints();

app.MapCameraEndpoints();

app.Run();

namespace Eggs.Api
{
    public interface ICarPlateGrain : IGrainWithStringKey
    {
    }

    public sealed class CarPlateGrain(
    ) : ICarPlateGrain
    {
    }

    public interface ICarPlateChargeGrain : IGrainWithGuidKey
    {
        public ValueTask IncurCharge(ICarPlateGrain carPlate, string location);
        public ValueTask<ICarPlateGrain> GetCarPlate();
    }

    public sealed class CarPlateChargeGrain(
        [PersistentState(stateName: "charge", storageName: "storage")]
        IPersistentState<CarPlateChargeDetails> state
    ) : ICarPlateChargeGrain
    {
        private ICarPlateGrain _carPlate;

        public async ValueTask IncurCharge(ICarPlateGrain carPlate, string location)
        {
            _carPlate = carPlate;

            state.State = new CarPlateChargeDetails
            {
                Location = location
            };

            await state.WriteStateAsync();
        }

        public ValueTask<ICarPlateGrain> GetCarPlate()
        {
            return ValueTask.FromResult(_carPlate);
        }
    }

    [GenerateSerializer, Alias(nameof(CarPlateChargeDetails))]
    public sealed record class CarPlateChargeDetails
    {
        [Id(0)] public DateTimeOffset ChargeDateTime { get; set; } = DateTimeOffset.Now;

        [Id(1)] public string Location { get; set; }
    }
}