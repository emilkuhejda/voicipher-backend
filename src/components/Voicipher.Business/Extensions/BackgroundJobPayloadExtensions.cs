using System;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Extensions
{
    public static class BackgroundJobPayloadExtensions
    {
        public static T GetParameter<T>(this BackgroundJobPayload payload, BackgroundJobParameter parameter, T defaultValue = default)
        {
            if (payload.Parameters.ContainsKey(parameter))
            {
                return (T)Convert.ChangeType(payload.Parameters[parameter], typeof(T));
            }

            return defaultValue;
        }
    }
}
