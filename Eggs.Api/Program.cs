using Eggs.Api.Endpoints;
using Eggs.Api.StartupTasks;
using Orleans.Runtime;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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