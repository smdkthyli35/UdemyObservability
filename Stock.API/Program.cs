using Common.Shared;
using Logging.Shared;
using OpenTelemetry.Shared;
using Serilog;
using Stock.API;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Logging.Shared.Logging.ConfigureLogging);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<StockService>();

builder.Services.AddOpenTelemetryExt(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOpenTelemetryTraceIdExtension();

app.UseMiddleware<RequestAndResponseActivityMiddleware>();

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
