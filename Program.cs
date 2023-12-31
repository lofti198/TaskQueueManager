using Microsoft.EntityFrameworkCore;

using RabbitMQ.Client;

using ValidLoaderShared.Consts;
using ValidLoaderShared.Context;
using ValidLoaderShared.MiddleWare;
using ValidLoaderShared.Utilities;
using ValidLoaderShared.Utilities.Logging;


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Set up configuration sources.
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();

    var logger = VL_LoggerFactory.CreateLogger(LogType.Console | LogType.Serilog, "TaskQueueManager",false);

    logger.Log("TaskQueueManager Initializing...");

    string sqlserverConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    await CriticalServicesWaiters.WaitForSqlServer(sqlserverConnectionString);

    await CriticalServicesWaiters.WaitForRabbitMQAndQueue("localhost", VLConstants.TaskNotificationQueue);

    builder.Services.AddSingleton<ISimplifiedLogger>(logger);

    // Configure the application to listen on specified ports
    builder.WebHost.UseUrls($"http://localhost:{VLConstants.TaskQueueManagerLocalPort}",
        $"https://localhost:{VLConstants.TaskQueueManagerLocalPort + 1}");

    // Add services to the container.
    Console.WriteLine($"Connecting DB with connection string: " + sqlserverConnectionString);
    // $"{builder.Configuration.GetConnectionString("DefaultConnection")}");
    builder.Services.AddDbContext<PageLoaderServiceContext>(options =>
        options.UseSqlServer(sqlserverConnectionString));//);
           
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Register ConnectionFactory as a singleton
    builder.Services.AddSingleton(new ConnectionFactory() { HostName = "localhost" });


    var app = builder.Build();

    // Register the exception handling middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();

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

