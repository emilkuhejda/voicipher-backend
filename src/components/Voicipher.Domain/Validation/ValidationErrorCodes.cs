namespace Voicipher.Domain.Validation
{
    public static class ValidationErrorCodes
    {
        public static string EmptyField => nameof(EmptyField);

        public static string TextTooLong => nameof(TextTooLong);

        public static string InvalidId => nameof(InvalidId);
    }
}
