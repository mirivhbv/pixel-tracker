using StorageApi.Messaging;
using StorageApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStorageHandler, FilesystemStorageHandler>();
builder.Services.AddHostedService<TrackEventConsumer>();

var app = builder.Build();

app.Run();
