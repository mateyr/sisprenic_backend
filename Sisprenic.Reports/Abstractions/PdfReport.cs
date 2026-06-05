using Microsoft.Playwright;

namespace Sisprenic.Reports.Abstractions;

// Base type for every PDF report.
public abstract class PdfReport
{
    internal abstract string ViewPath { get; }
    internal abstract object Model { get; }

    internal virtual PagePdfOptions PdfOptions => new()
    {
        PrintBackground = true,
        DisplayHeaderFooter = true,
        Margin = new Margin
        {
            Top = "2.54cm",
            Right = "2.54cm",
            Bottom = "2.54cm",
            Left = "2.54cm"
        },
        HeaderTemplate = "<div></div>",
        FooterTemplate = @"
            <div style='width:100%; font-size:10px; color:#647280; text-align:center; padding-top:4px;'>
                Documento generado automáticamente el <span class='date'></span>
            </div>
        "
    };
}
