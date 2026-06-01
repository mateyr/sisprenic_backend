using sisprenic_backend.Modules.Loans.CreateLoan;
using sisprenic_backend.Modules.Loans.DeleteLoan;
using sisprenic_backend.Modules.Loans.GetAllLoans;
using sisprenic_backend.Modules.Loans.GetLoanById;
using sisprenic_backend.Modules.Loans.GetLoanPayments;
using sisprenic_backend.Modules.Loans.UpdateLoan;

namespace sisprenic_backend.Modules.Loans;

public static class LoansModule
{
    public static void MapLoansModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/loans").WithTags("Loans");

        group.MapGetAllLoans();
        group.MapGetLoanById();
        group.MapGetLoanPayments();
        group.MapCreateLoan();
        group.MapUpdateLoan();
        group.MapDeleteLoan();
    }
}
