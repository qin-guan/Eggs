using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(options =>
    {
        options.WithBlobPort(10000);
        options.WithQueuePort(10001);
        options.WithTablePort(10002);
    });

var clusteringTable = storage.AddTables("clustering");
var pubSubStore = storage.AddBlobs("PubSubStore");
var trafficCameraManagerStorage = storage.AddBlobs("traffic-camera-manager");
var trafficCameraStorage = storage.AddBlobs("traffic-camera");
var sightingStorage = storage.AddBlobs("sighting");
var chargeStorage = storage.AddBlobs("charge");
var vehicleStorage = storage.AddBlobs("vehicle");
var chargeLogStorage = storage.AddBlobs("charge-log-storage");
var chargeEvents = storage.AddQueues("charge-events");

var orleans = builder.AddOrleans("default")
    .WithClustering(clusteringTable)
    .WithGrainStorage("PubSubStore", pubSubStore)
    .WithGrainStorage("traffic-camera", trafficCameraStorage)
    .WithGrainStorage("traffic-camera-manager", trafficCameraManagerStorage)
    .WithGrainStorage("sighting", sightingStorage)
    .WithGrainStorage("charge", chargeStorage)
    .WithGrainStorage("vehicle", vehicleStorage)
    .WithGrainStorage("charge-log-storage", chargeLogStorage);

builder.AddProject<Eggs_Api>("api")
    .WithReference(orleans)
    .WithReference(chargeEvents)
    .WithReplicas(3);

builder.Build().Run();