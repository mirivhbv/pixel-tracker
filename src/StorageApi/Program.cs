using ProtoBuf.Grpc.Server;
using StorageApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCodeFirstGrpc();
builder.Services.AddTransient<IStorageHandler, FilesystemStorageHandler>();

var app = builder.Build();

app.MapGrpcService<StorageService>();

app.MapGet("/", () => "Hello World!");
app.MapGet("/store", () => "Store it");

app.Run();
