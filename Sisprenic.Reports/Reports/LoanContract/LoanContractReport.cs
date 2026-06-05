using Sisprenic.Reports.Abstractions;

namespace Sisprenic.Reports.Reports.LoanContract;

public sealed class LoanContractReport : PdfReport
{
    private readonly LoanContractViewModel _model;

    public LoanContractReport(LoanContractViewModel model)
    {
        _model = model;
    }

    internal override string ViewPath => "~/Reports/LoanContract/LoanContract.cshtml";

    internal override object Model => _model;
}
