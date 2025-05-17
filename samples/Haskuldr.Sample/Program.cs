using System.Reflection;
using Haskuldr.DependencyInjection;
using Haskuldr.MinimalApi;

namespace Haskuldr.Sample;

internal sealed class Program
{
    private static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddEventBus(ExecutingAssembly);
        builder.Services.AddEndpoints(ExecutingAssembly);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapEndpoints();

        await app.RunAsync();
    }
}