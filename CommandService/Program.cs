using CommandService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
{
     System.Console.WriteLine("--> Using InMemory Database");
        options.UseInMemoryDatabase("InMem");
});

builder.Services.AddScoped<ICommandRepo, CommandRepo>();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.Run();
