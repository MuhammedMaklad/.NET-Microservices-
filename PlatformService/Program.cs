using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Repository;
using PlatformService.Seeds;
using PlatformService.SyncDataServices.Http;


var builder = WebApplication.CreateBuilder(args);

// Add for Docker compatibility
builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ListenAnyIP(5093);  // Listen on all interfaces
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ! Add DbContext
builder.Services.AddDbContext<AppDbContext>(
    // options => options.UseInMemoryDatabase("InMem")
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformURI"))
);
// ! Add Platform repo
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
// ! Add automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// ! Add Service to container
builder.Services.AddControllers();
// ! Add Http Sync Message service
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// ? seed data
PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.MapGet("/", () =>
{
    return "Hello World from Coupon API!, Muhammed on da code ";
});

// Configure the HTTP request pipeline (middleware)
app.UseHttpsRedirection();
app.MapControllers();


app.Run();

