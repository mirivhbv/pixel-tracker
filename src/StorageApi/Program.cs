using Core.Grpc;
using Core.Grpc.Interceptors;
using ProtoBuf.Grpc.Server;
using StorageApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCodeFirstGrpc(opt => { opt.Interceptors.Add<RpcExceptionInterceptor>(new RpcExceptionWrapper()); });
builder.Services.AddTransient<IStorageHandler, FilesystemStorageHandler>();

var app = builder.Build();

app.MapGrpcService<StorageService>();

// TODO: drop them
app.MapGet("/", () => "Hello World!");
app.MapGet("/store", () => "Store it");

app.Run();
