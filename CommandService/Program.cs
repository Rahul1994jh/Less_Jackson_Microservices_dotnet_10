using CommandService;
using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
{
        options.UseInMemoryDatabase("InMem");
});

builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

MappingConfig.RegisterMappings();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.Run();
