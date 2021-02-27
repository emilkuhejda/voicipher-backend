using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads.ControlPanel
{
    public record InternalValuePayload<T>
    {
        public InternalValuePayload(InternalValueKey key, T defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public InternalValueKey Key { get; }

        public T DefaultValue { get; }
    }
}
