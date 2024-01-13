using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Order.API.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OpenTelemetryConstants>(builder.Configuration.GetSection("OpenTelemetry"));

var OpenTelemetryConstants = (builder.Configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstants>())!;

builder.Services.AddOpenTelemetry().WithTracing(options =>
{
    options
        .AddSource(OpenTelemetryConstants.ActivitySourceName)
        .ConfigureResource(resource =>
        {
            resource.AddService(OpenTelemetryConstants.ServiceName, serviceVersion: OpenTelemetryConstants.ServiceVersion);
        });

    options.AddAspNetCoreInstrumentation();
    options.AddConsoleExporter();
    options.AddOtlpExporter(); //Jaeger
});

ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(OpenTelemetryConstants.ActivitySourceName);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();