using System.Text.Json;
using DiskQueue;
using HookMaker.Data.Context;
using HookMaker.Workers;
using Serilog;
using MediatR;
using Microsoft.EntityFrameworkCore;

try
{
    var builder = WebApplication.CreateBuilder(args);

    //add logging
    builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(hostBuilderContext.Configuration));

    // Add services to the container.
   builder.Services.AddDbContext<WebHooksDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    });
   //register persistent queue implementation to allow same instance everywhere and data persisted to specific location
    builder.Services.AddSingleton<IPersistentQueue>(provider => new PersistentQueue(Directory.GetCurrentDirectory()));
    builder.Services.AddMediatR(typeof(Program).Assembly);
    builder.Services.AddAutoMapper(typeof(Program));
    //register background service for long running tasks which will take care of spawning new threads for database insertion
    //this registration can be commented out to test persistence of the queue,
    //once it it re-enabled it will start processing all the unprocessed items in queue
    builder.Services.AddHostedService<WebHookProcessorService>();
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    app.UseSerilogRequestLogging();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal("Fatal error application failed with the following error:{Error}", ex);
    Log.CloseAndFlush();
}