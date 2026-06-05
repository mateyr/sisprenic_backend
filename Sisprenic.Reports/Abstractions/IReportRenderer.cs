namespace Sisprenic.Reports.Abstractions;

public interface IReportRenderer
{
    Task<byte[]> RenderAsync(PdfReport report, CancellationToken cancellationToken = default);
}
