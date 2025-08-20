using CommandsService;
using CommandsService.Data;
using CommandsService.Repository;
using CommandsService.Repository.IRepository;
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

    builder.Services.AddScoped<ICommandRepository, CommandRepository>();

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