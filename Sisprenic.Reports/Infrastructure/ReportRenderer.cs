using Microsoft.Playwright;

using Razor.Templating.Core;

using Sisprenic.Reports.Abstractions;

namespace Sisprenic.Reports.Infrastructure;

// Renders a PDF report by converting a Razor view into HTML and printing it using Playwright.
internal sealed class ReportRenderer : IReportRenderer
{
    private readonly IRazorTemplateEngine _templateEngine;
    private readonly PlaywrightBrowserProvider _browserProvider;

    public ReportRenderer(IRazorTemplateEngine templateEngine, PlaywrightBrowserProvider browserProvider)
    {
        _templateEngine = templateEngine;
        _browserProvider = browserProvider;
    }

    public async Task<byte[]> RenderAsync(PdfReport report, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string html = await _templateEngine.RenderAsync(report.ViewPath, report.Model);

        IBrowser browser = await _browserProvider.GetBrowserAsync();

        // One context per document => full isolation between concurrent PDFs.
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        await page.SetContentAsync(html, new PageSetContentOptions());

        return await page.PdfAsync(report.PdfOptions);
    }
}
