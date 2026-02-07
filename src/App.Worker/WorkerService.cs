using App.Connectors.Telegram;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace App.Worker;

public sealed class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramConnector _connector;

    public WorkerService(ILogger<WorkerService> logger, ITelegramBotClient botClient, TelegramConnector connector)
    {
        _logger = logger;
        _botClient = botClient;
        _connector = connector;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, stoppingToken);
        _logger.LogInformation("Telegram bot dinlemeye başladı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var result = await _connector.HandleUpdateAsync(update, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Telegram komutu başarısız: {Message}", result.Error?.Message);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram bot hatası");
        return Task.CompletedTask;
    }
}
