using App.Shared;

namespace App.Core;

public sealed record BotCommand(string Name, IReadOnlyDictionary<string, string> Arguments, string RawText, long? UserId, long? ChatId);

public sealed record BotResponse(string Message, bool IsSuccess);

public interface ICommandRouter
{
    Task<Result<BotResponse>> RouteAsync(BotCommand command, CancellationToken cancellationToken);
}

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Guid? userId, string permissionKey, CancellationToken cancellationToken);
}

public sealed class BotCommandParser
{
    private readonly char _prefix;

    public BotCommandParser(char prefix = '!')
    {
        _prefix = prefix;
    }

    public Result<BotCommand> Parse(string text, long? userId = null, long? chatId = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Result<BotCommand>.Fail("command.empty", "Komut metni boş olamaz.");
        }

        var normalized = text.Trim();
        if (!normalized.StartsWith(_prefix))
        {
            return Result<BotCommand>.Fail("command.prefix", "Komut için doğru prefix kullanılmadı.");
        }

        var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return Result<BotCommand>.Fail("command.invalid", "Komut çözümlenemedi.");
        }

        var name = tokens[0][1..].ToLowerInvariant();
        var arguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var buffer = new List<string>();
        string? currentKey = null;

        foreach (var token in tokens.Skip(1))
        {
            if (token.StartsWith('#'))
            {
                if (currentKey is not null)
                {
                    arguments[currentKey] = string.Join(' ', buffer);
                    buffer.Clear();
                }

                currentKey = token[1..];
                continue;
            }

            buffer.Add(token);
        }

        if (currentKey is not null)
        {
            arguments[currentKey] = string.Join(' ', buffer);
        }
        else if (tokens.Length > 1)
        {
            arguments["value"] = string.Join(' ', tokens.Skip(1));
        }

        return Result<BotCommand>.Success(new BotCommand(name, arguments, text, userId, chatId));
    }
}
