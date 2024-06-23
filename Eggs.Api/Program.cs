using Azure.Storage.Queues;
using Eggs.Api.Constants;
using Eggs.Api.Endpoints;
using Eggs.Api.Options;
using Eggs.Api.StartupTasks;
using Eggs.ServiceDefaults;
using Orleans.Providers;

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

builder.AddKeyedAzureBlobClient(ProviderConstants.DEFAULT_PUBSUB_PROVIDER_NAME);
builder.AddKeyedAzureBlobClient("charge");
builder.AddKeyedAzureBlobClient("charge-log-storage");
builder.AddKeyedAzureBlobClient("sighting");
builder.AddKeyedAzureBlobClient("vehicle");
builder.AddKeyedAzureBlobClient("traffic-camera");
builder.AddKeyedAzureBlobClient("traffic-camera-manager");

builder.UseOrleans((orleans) =>
{
    orleans.UseTransactions();
    orleans.UseDashboard(options => { options.Port = 18080; });

    orleans.AddLogStorageBasedLogConsistencyProvider("charge-log-storage");
    orleans.AddAzureQueueStreams(StreamConstants.DefaultProvider, optionsBuilder => optionsBuilder.Configure(options =>
        options.QueueServiceClient =
            new QueueServiceClient(builder.Configuration.GetConnectionString("charge-events"))));

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
app.MapSightingsEndpoints();

app.Run();