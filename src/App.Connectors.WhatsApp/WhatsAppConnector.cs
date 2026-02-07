using App.Core;
using App.Shared;

namespace App.Connectors.WhatsApp;

public sealed class WhatsAppConnector
{
    public bool IsEnabled { get; }

    public WhatsAppConnector(bool isEnabled = false)
    {
        IsEnabled = isEnabled;
    }

    public Task<Result> HandleCommandAsync(string payload, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Fail("whatsapp.disabled", "WhatsApp bağlantısı devre dışı."));
    }
}
