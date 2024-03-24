using Core.Messaging;
using PixelApi.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IRabbitMqProducer, TrackEventProducer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/track", (HttpRequest request, IRabbitMqProducer rabbitMqProducer) =>
{
    var ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
    var userAgent = request.Headers["User-Agent"].ToString();
    var referer = request.Headers["Referer"].ToString();

    try
    {
        rabbitMqProducer.SendMessage(new TrackEvent
        {
            Date = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Referer = referer
        });
    }
    catch (Exception e)
    {
        app.Logger.LogWarning(e, "");
    }

    return Task.FromResult(Results.File("track.gif", contentType: "image/gif"));
});

app.Run();
