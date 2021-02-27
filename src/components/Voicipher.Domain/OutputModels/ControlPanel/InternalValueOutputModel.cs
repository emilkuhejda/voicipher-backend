namespace Voicipher.Domain.OutputModels.ControlPanel
{
    public record InternalValueOutputModel<T>
    {
        public InternalValueOutputModel(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
