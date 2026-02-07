using App.Core;
using App.Shared;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure;

public sealed class BotCommandRouter : ICommandRouter
{
    private readonly AppDbContext _dbContext;
    private readonly IPermissionService _permissionService;

    public BotCommandRouter(AppDbContext dbContext, IPermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Result<BotResponse>> RouteAsync(BotCommand command, CancellationToken cancellationToken)
    {
        return command.Name switch
        {
            "kofre" => await HandleCofreAsync(command, cancellationToken),
            "sayac" => await HandleMeterAsync(command, cancellationToken),
            "help" => Result<BotResponse>.Success(new BotResponse("Komutlar: !kofre, !sayac, !help, !status", true)),
            "status" => Result<BotResponse>.Success(new BotResponse("Bot aktif. Veritabanı bağlantısı hazır.", true)),
            _ => Result<BotResponse>.Fail("command.unknown", "Bilinmeyen komut. !help yazabilirsiniz.")
        };
    }

    private async Task<Result<BotResponse>> HandleCofreAsync(BotCommand command, CancellationToken cancellationToken)
    {
        if (!command.Arguments.TryGetValue("value", out var value) || !int.TryParse(value, out var cofreNo))
        {
            return Result<BotResponse>.Fail("kofre.invalid", "Kofre numarası geçersiz.");
        }

        var cofre = await _dbContext.Cofres.AsNoTracking().FirstOrDefaultAsync(x => x.CofreNo == cofreNo, cancellationToken);
        if (cofre is null)
        {
            return Result<BotResponse>.Fail("kofre.not_found", "Kofre bulunamadı.");
        }

        var password = await _dbContext.CofrePasswords.AsNoTracking()
            .Where(x => x.CofreNo == cofreNo)
            .OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var message = $"🧰 Kofre: {cofre.CofreNo}\n" +
                      $"🔐 Şifre: {(password?.PasswordValue ?? "-")}\n" +
                      $"📝 Not: {(password?.Note ?? "-")}\n" +
                      $"📍 Konum: X={cofre.XCoord?.ToString("0.#####") ?? "-"}, Y={cofre.YCoord?.ToString("0.#####") ?? "-"}";

        return Result<BotResponse>.Success(new BotResponse(message, true));
    }

    private async Task<Result<BotResponse>> HandleMeterAsync(BotCommand command, CancellationToken cancellationToken)
    {
        if (!command.Arguments.TryGetValue("value", out var value))
        {
            return Result<BotResponse>.Fail("meter.invalid", "Sayaç seri numarası boş.");
        }

        var meter = await _dbContext.Meters.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MeterSerialNo == value, cancellationToken);

        if (meter is null)
        {
            return Result<BotResponse>.Fail("meter.not_found", "Sayaç bulunamadı.");
        }

        var message = $"🔢 Sayaç: {meter.MeterSerialNo}\n" +
                      $"👤 Abone: {meter.SubscriberName ?? "-"}\n" +
                      $"🧰 Kofre: {meter.CofreNo}\n" +
                      $"📍 Konum: X={meter.XCoord?.ToString("0.#####") ?? "-"}, Y={meter.YCoord?.ToString("0.#####") ?? "-"}";

        return Result<BotResponse>.Success(new BotResponse(message, true));
    }
}
