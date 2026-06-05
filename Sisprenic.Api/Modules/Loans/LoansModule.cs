using Sisprenic.Api.Modules.Loans.CreateLoan;
using Sisprenic.Api.Modules.Loans.DeleteLoan;
using Sisprenic.Api.Modules.Loans.GetAllLoans;
using Sisprenic.Api.Modules.Loans.GetLoanById;
using Sisprenic.Api.Modules.Loans.GetLoanContract;
using Sisprenic.Api.Modules.Loans.GetLoanPayments;
using Sisprenic.Api.Modules.Loans.UpdateLoan;

namespace Sisprenic.Api.Modules.Loans;

public static class LoansModule
{
    public static void MapLoansModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/loans").WithTags("Loans");

        group.MapGetAllLoans();
        group.MapGetLoanById();
        group.MapGetLoanPayments();
        group.MapGetLoanContract();
        group.MapCreateLoan();
        group.MapUpdateLoan();
        group.MapDeleteLoan();
    }
}
