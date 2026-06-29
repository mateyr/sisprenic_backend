using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Sisprenic.Reports.Infrastructure;

// Provides a shared Chromium instance used for PDF rendering.
// Self-healing: if the underlying Chromium process crashes or is OOM-killed,
// the next request transparently relaunches it instead of failing forever.
public sealed class PlaywrightBrowserProvider : IAsyncDisposable
{
    private readonly SemaphoreSlim _launchLock = new(1, 1);
    private readonly ILogger<PlaywrightBrowserProvider> _logger;

    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public PlaywrightBrowserProvider(ILogger<PlaywrightBrowserProvider> logger)
    {
        _logger = logger;
    }

    // Chromium is launched lazily on the first report request (and relaunched on demand
    // if the process dies), so there is no startup cost or dependency on the browser.
    public async Task<IBrowser> GetBrowserAsync() => await EnsureBrowserAsync();

    private async Task<IBrowser> EnsureBrowserAsync()
    {
        if (_browser is { IsConnected: true } connected)
        {
            return connected;
        }

        await _launchLock.WaitAsync();
        try
        {
            if (_browser is { IsConnected: true } reconnected)
            {
                return reconnected;
            }

            if (_browser is not null)
            {
                _logger.LogWarning(
                    "Playwright: Chromium browser is disconnected. Relaunching a new instance.");

                try
                {
                    await _browser.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Playwright: failed to dispose the dead Chromium browser; it was likely already gone.");
                }

                _browser = null;
            }

            _playwright ??= await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync();

            _logger.LogInformation("Playwright: Chromium browser launched and ready for PDF rendering.");

            return _browser;
        }
        finally
        {
            _launchLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
        _launchLock.Dispose();
    }
}
