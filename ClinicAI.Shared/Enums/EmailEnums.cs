namespace ClinicAI.Shared.Enums
{
    public enum EmailStatus
    {
        Pending,
        Sent,
        Failed,
        Retrying
    }

    public enum EmailType
    {
        Registration,
        AppointmentConfirmation,
        AppointmentCancellation,
        AppointmentReminder,
        PasswordReset,
        General
    }
}
