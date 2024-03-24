using StorageApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStorageHandler, FilesystemStorageHandler>();

var app = builder.Build();

app.Run();
