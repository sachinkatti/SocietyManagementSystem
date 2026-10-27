
using SMS.Domain;

namespace SMS.Application
{
    public static class TenantSubscriptionValidation
    {
        public static (bool IsValid, string ErrorMessage) Validate(
            TenantSubscription subscription)
        {
            if (subscription.TenantId <= 0)
            {
                return (false, "Please select a valid tenant.");
            }

            if (subscription.PlanId <= 0)
            {
                return (false, "Please select a valid plan.");
            }

            if (subscription.StartDate == default)
            {
                return (false, "Start date is required.");
            }

            if (subscription.EndDate == default)
            {
                return (false, "End date is required.");
            }

            if (subscription.EndDate <= subscription.StartDate)
            {
                return (false, "End date must be later than start date.");
            }

            if (string.IsNullOrWhiteSpace(subscription.Status))
            {
                return (false, "Status is required.");
            }

            if (subscription.Status.Length > 50)
            {
                return (false, "Status cannot exceed 50 characters.");
            }

            if (subscription.AmountPaid < 0)
            {
                return (false, "Amount paid cannot be negative.");
            }

            if (subscription.PaymentReference?.Length > 100)
            {
                return (false, "Payment reference cannot exceed 100 characters.");
            }

            if (subscription.Description?.Length > 500)
            {
                return (false, "Description cannot exceed 500 characters.");
            }

            return (true, string.Empty);
        }
    }
}
