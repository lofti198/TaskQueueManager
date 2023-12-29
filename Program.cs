using Microsoft.EntityFrameworkCore;
using ValidLoaderShared.Consts;
using ValidLoaderShared.Context;
using ValidLoaderShared.Utilities;


try
{
    Console.WriteLine("TaskQueueManager Initializing...");

    await CriticalServicesWaiters.WaitForSqlServer(VLDatabaseConstants.LocalDbConnectionString);

    await CriticalServicesWaiters.WaitForRabbitMQAndQueue("localhost", VLConstants.TaskNotificationQueue);

    var builder = WebApplication.CreateBuilder(args);

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

