namespace Voicipher.Domain.Enums
{
    public enum JobState
    {
        Idle = 0,
        Initialized = 1,
        Validating = 2,
        Validated = 3,
        Converting = 4,
        Converted = 5,
        Splitting = 6,
        Split = 7,
        Processing = 8,
        Processed = 9,
        Completing = 10,
        Completed = 11
    }
}
