using App.Core;
using App.Shared;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace App.Connectors.Telegram;

public sealed class TelegramConnector
{
    private readonly ITelegramBotClient _client;
    private readonly BotCommandParser _parser;
    private readonly ICommandRouter _router;

    public TelegramConnector(ITelegramBotClient client, BotCommandParser parser, ICommandRouter router)
    {
        _client = client;
        _parser = parser;
        _router = router;
    }

    public async Task<Result> HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message?.Text is null)
        {
            return Result.Success();
        }

        var parseResult = _parser.Parse(update.Message.Text, update.Message.From?.Id, update.Message.Chat.Id);
        if (!parseResult.IsSuccess || parseResult.Value is null)
        {
            return Result.Fail(parseResult.Error?.Code ?? "parse.error", parseResult.Error?.Message ?? "Komut çözümlenemedi.");
        }

        var response = await _router.RouteAsync(parseResult.Value, cancellationToken);
        if (!response.IsSuccess || response.Value is null)
        {
            var message = response.Error?.Message ?? "İşlem sırasında hata oluştu.";
            await _client.SendTextMessageAsync(update.Message.Chat.Id, message, cancellationToken: cancellationToken);
            return Result.Fail(response.Error?.Code ?? "command.error", message);
        }

        await _client.SendTextMessageAsync(update.Message.Chat.Id, response.Value.Message, cancellationToken: cancellationToken);
        return Result.Success();
    }
}
