using service_message.Service;
using Microsoft.AspNetCore.WebSockets;
using service_message.HubMessage;
using service_message.Repository;
using MessageService.Repository;
using Serilog;
using service_message.Logger;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(120);
});

builder.Host.UseSerilog();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<WebSocketServer>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, service_message.Service.MessageService>();
builder.Services.AddLogging();


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.WithOrigins("http://localhost:5001")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

builder.Services.AddSingleton<WebSocketServer>();
builder.Services.AddSignalR();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseWebSockets();


app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocketServer = context.RequestServices.GetRequiredService<WebSocketServer>();
            await webSocketServer.HandleConnections(context);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


try
{
    Log.Information("Запуск приложения");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение завершилось с критической ошибкой");
}
finally
{
    Log.CloseAndFlush();
}
