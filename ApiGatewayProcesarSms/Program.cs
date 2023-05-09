using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using ApiGatewayProcesarSms.Entities;
using ApiGatewayProcesarSms.Handlers;
using ApiGatewayProcesarSms.Middleware;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.GetName().Name,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environments.Development
});

Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
Console.WriteLine($"ContentRoot Name: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Environment Name: {builder.Environment.EnvironmentName}");

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Services.AddSingleton(builder);
builder.Configuration.AddEnvironmentVariables();

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*")
        .WithHeaders()
        .WithMethods("POST");
    });
});

builder.Services.Configure<MicroservicesAuth>(builder.Configuration.GetSection("MicroservicesAuths"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings:BasicAuth"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings:Servicios"));

builder.Services.AddOcelot()
                .AddDelegatingHandler<ProcesarSmsHandler>();



builder.Services.AddOptions();

var app = builder.Build();

app.UseCors();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthorizationMego();

try
{
    app.UseOcelot().Wait();
    app.Run();
}
catch (Exception ex)
{
    throw new ArgumentException(ex.Message);
}
