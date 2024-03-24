var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// OR go with having actual 1x1 GIF in wwwroot
var gifBytes = new byte[] {
    0x47, 0x49, 0x46, 0x38, 0x39, 0x61, // GIF89a header
    0x01, 0x00, 0x01, 0x00, // Image size (1x1 pixels)
    0x80, 0x00, 0x00, 0x00, // Transparent color
    0x00, 0x00, 0x00, // Image data
    0x21, 0xF9, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, // Graphic Control Extension
    0x2C, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x02, 0x02, 0x44, 0x01, 0x00, // Image Descriptor
    0x3B // GIF Trailer
};

app.MapGet("/track", async (HttpRequest request) =>
{
    var ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
    var userAgent = request.Headers["User-Agent"].ToString();
    var referer = request.Headers["Referer"].ToString();

    request.HttpContext.Response.Headers.Add("Content-Type", "image/gif");
    await request.HttpContext.Response.Body.WriteAsync(gifBytes);
});

app.Run();
