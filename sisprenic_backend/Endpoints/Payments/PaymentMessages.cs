using sisprenic_backend.Common;

namespace sisprenic_backend.Endpoints.Payments;

public static class PaymentMessages
{
    public static IReadOnlyList<ApiMessage>? Build(
        decimal requestedInterest,
        decimal requestedPrincipal,
        decimal totalInterestOutstanding,
        decimal interestApplied,
        decimal principalApplied,
        decimal requestedTotal,
        decimal totalApplied,
        decimal unapplied)
    {
        List<ApiMessage> messages = [];

        decimal interestDelta =
            interestApplied - requestedInterest;

        decimal principalDelta =
            principalApplied - requestedPrincipal;

        if (interestDelta > 0m)
        {
            messages.Add(new(
                Code: "interest_applied_from_principal",
                Message:
                    $"Existía interés pendiente; {interestDelta:0.00} enviados como capital fueron aplicados a interés."));
        }

        if (principalDelta > 0m)
        {
            string message =
                totalInterestOutstanding == 0m
                    ? $"No había interés pendiente; {principalDelta:0.00} enviados como interés fueron aplicados a capital."
                    : $"{principalDelta:0.00} enviados como interés excedían el interés pendiente y fueron aplicados a capital.";

            messages.Add(new(
                Code: "interest_applied_to_principal",
                Message: message));
        }

        if (unapplied > 0m)
        {
            messages.Add(new(
                Code: "amount_unapplied",
                Message:
                    $"Se enviaron {requestedTotal:0.00} pero el sistema solo aplicó {totalApplied:0.00} " +
                    $"para no exceder la deuda total. {unapplied:0.00} no fueron registrados."));
        }

        return messages.Count == 0
            ? null
            : messages;
    }
}