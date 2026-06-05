using Microsoft.Playwright;

namespace Sisprenic.Reports.Infrastructure;

// Provides a shared Chromium instance used for PDF rendering.
public sealed class PlaywrightBrowserProvider : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    public Task<IBrowser> GetBrowserAsync() => Task.FromResult(_browser!);

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
    }
}
