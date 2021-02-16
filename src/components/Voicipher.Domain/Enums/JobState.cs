namespace Voicipher.Domain.Enums
{
    public enum JobState
    {
        Idle,
        Initialized,
        Validating,
        Validated,
        Converting,
        Converted,
        Processing,
        Processed,
        Completed
    }
}
