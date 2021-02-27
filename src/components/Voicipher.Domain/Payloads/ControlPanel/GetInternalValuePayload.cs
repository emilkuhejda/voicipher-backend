using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads.ControlPanel
{
    public record GetInternalValuePayload<T>
    {
        public GetInternalValuePayload(InternalValueKey key, T defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public InternalValueKey Key { get; }

        public T DefaultValue { get; }
    }
}
