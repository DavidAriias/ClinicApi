namespace ClinicApi.Domain.Enums
{
    public static class AppointmentStatus
    {
        public const string Scheduled = "Scheduled";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static bool IsValid(string status)
            => status is Scheduled or Completed or Cancelled;
    }
}