using Core.Tests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Tests.Grpc.Interceptors;

/// <summary>
/// Startup test class with a pre-configured Grpc service (via <see cref="TGrpcService" />)
/// to be used together with <see cref="GrpcTestFixture{TStartup}" />.
/// To inject an existing instance, make sure to register a singleton <see cref="TGrpcService" />
/// in the constructor for <see cref="GrpcTestFixture{TStartup}" />
/// </summary>
/// <typeparam name="TGrpcService"></typeparam>
public class TestStartup<TGrpcService> where TGrpcService : class
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            // Registers the grpc service type
            endpoints.MapGrpcService<TGrpcService>();
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
    }
}