using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;

using ValidLoaderShared.Consts;
using ValidLoaderShared.Context;
using ValidLoaderShared.Utilities;
using ValidLoaderShared.Utilities.Logging;


try
{
    var builder = WebApplication.CreateBuilder(args);

    var logger = VL_LoggerFactory.CreateLogger(LogType.Console | LogType.Serilog, "TaskQueueManager");

    logger.Log("TaskQueueManager Initializing...");

    await CriticalServicesWaiters.WaitForSqlServer(VLDatabaseConstants.LocalDbConnectionString);

    await CriticalServicesWaiters.WaitForRabbitMQAndQueue("localhost", VLConstants.TaskNotificationQueue);

    builder.Services.AddSingleton<ISimplifiedLogger>(logger);

    // builder.Host.UseSerilog(); // Use Serilog for logging


    builder.Services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(sp =>
    {
        var serilogLogger = new SerilogLogger(null); // Assuming SerilogLogger is adapted to work without a decorator
        var consoleLogger = new ConsoleLogger(serilogLogger); // Wrap Serilog with Console
        return consoleLogger;
    });
    // Configure the application to listen on specified ports
    builder.WebHost.UseUrls($"http://localhost:{VLConstants.TaskQueueManagerLocalPort}",
        $"https://localhost:{VLConstants.TaskQueueManagerLocalPort + 1}");

    // Add services to the container.
    Console.WriteLine($"Connecting DB with connection string: {builder.Configuration.GetConnectionString("DefaultConnection")}");
    builder.Services.AddDbContext<PageLoaderServiceContext>(options =>
        options.UseSqlServer(VLDatabaseConstants.LocalDbConnectionString));
            //builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Register ConnectionFactory as a singleton
    builder.Services.AddSingleton(new ConnectionFactory() { HostName = "localhost" });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles(); // Enable serving static files
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllers();
    // Serve the index.html file on the root URL
    app.MapGet("/", async context =>
    {
        await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
    });
    app.Run();

}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception: {ex}");
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
}

