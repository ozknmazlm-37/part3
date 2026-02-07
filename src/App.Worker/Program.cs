using App.Connectors.Telegram;
using App.Connectors.WhatsApp;
using App.Core;
using App.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;

var builder = Host.CreateDefaultBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/worker-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.UseSerilog();

builder.ConfigureServices((context, services) =>
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(context.Configuration.GetConnectionString("Default")));

    services.AddSingleton(new BotCommandParser('!'));
    services.AddScoped<IPermissionService, PermissionService>();
    services.AddScoped<ICommandRouter, BotCommandRouter>();

    var telegramToken = context.Configuration["Telegram:Token"] ?? string.Empty;
    services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(telegramToken));
    services.AddScoped<TelegramConnector>();

    services.AddSingleton(new WhatsAppConnector(isEnabled: false));

    services.AddHostedService<WorkerService>();
});

var host = builder.Build();
await host.RunAsync();
