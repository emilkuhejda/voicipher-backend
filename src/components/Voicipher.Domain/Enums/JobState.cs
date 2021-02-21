﻿namespace Voicipher.Domain.Enums
{
    public enum JobState
    {
        Idle = 0,
        Initialized = 1,
        Validating = 2,
        Validated = 3,
        Converting = 4,
        Converted = 5,
        Uploading = 6,
        Uploaded = 7,
        Processing = 6,
        Processed = 7,
        Completed = 8
    }
}
