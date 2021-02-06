namespace Voicipher.Domain.Validation
{
    public record ValidationError : OperationError
    {
        public ValidationError(string errorCode)
            : base(errorCode)
        {
        }

        public ValidationError(string errorCode, string field)
            : base(errorCode)
        {
            Field = field;
        }

        public string Field { get; }
    }
}
