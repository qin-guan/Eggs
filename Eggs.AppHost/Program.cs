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
var trafficCameraManagementStorage = storage.AddBlobs("traffic-camera-management");
var trafficCameraStorage = storage.AddBlobs("traffic-camera");

var orleans = builder.AddOrleans("default")
    .WithClustering(clusteringTable)
    .WithGrainStorage("traffic-camera-management", trafficCameraManagementStorage)
    .WithGrainStorage("traffic-camera", trafficCameraStorage);

builder.AddProject<Eggs_Api>("api")
    .WithReference(orleans)
    .WithReplicas(3);

builder.Build().Run();