using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCodeFirstGrpc();

var app = builder.Build();

app.MapGrpcService<StorageApi.Services.StorageService>();

app.MapGet("/", () => "Hello World!");
app.MapGet("/store", () => "Store it");

app.Run();
