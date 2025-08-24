using CommandsService;
using CommandsService.AsyncDataServices.MessageBroker;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.Repository;
using CommandsService.Repository.IRepository;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Read configuration early for logging setup
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .AddEnvironmentVariables() // Crucial for Docker/K8s!
    .Build();

// Configure the static Serilog logger first to catch any startup errors
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration) // Read settings from appsettings.json
    .CreateBootstrapLogger(); // Creates a temporary logger

try
{
    Log.Information("Starting web host...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();

    // ! Add Db 
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        // options.UseSqlServer(builder.Configuration.GetConnectionString("CommandDbURI"));
        options.UseInMemoryDatabase("InMen");
    });

    // ! Add Auto Mapper
    builder.Services.AddAutoMapper(typeof(MappingProfiles));

    // ! Add Singleton life time for Event Processor Service
    builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
    // ! Add Scope life time for Command Repository
    builder.Services.AddScoped<ICommandRepository, CommandRepository>();

    // ! map Rabbit mq setting from app-settings
    builder.Services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

    // ! add message bus
    builder.Services.AddHostedService<MessageBusSubscriber>();

    // ! Add Grpc service
    builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    // ! Serilog Middleware
    app.UseSerilogRequestLogging(); // Adds automatic logging for HTTP requests

    // ! Custom Middleware
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseHttpsRedirection();
    app.MapControllers();

    app.MapGet("/", () =>
    {
        return "Muhammed on da code, From Command Service";
    });

    PrepDb.PrepPopulation(app);
    
    app.Run();


}
catch (System.Exception ex)
{
    // This will log any fatal errors during host startup that the regular logger couldn't catch.
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    // Ensure any buffered logs are written to their target before the app closes.
    Log.CloseAndFlush();
}