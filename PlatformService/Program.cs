using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        System.Console.WriteLine("--> Using InMemory Database");
        options.UseInMemoryDatabase("InMem");
    }
    else
    {
        System.Console.WriteLine("--> Using SQL Server Database");
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn"));
    }
});

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.UseRouting();    
app.UseAuthorization();

app.MapControllers();
app.Run();
