namespace Sisprenic.Reports.Extensions;

// Ensures the browser chromium is installed.
public static class ReportingBootstrapper
{
    // Returns the Playwright installer exit code (0 = success).
    public static int EnsureBrowserInstalled()
    {
        return Microsoft.Playwright.Program.Main(
            ["install", "chromium"]
        );
    }
}
