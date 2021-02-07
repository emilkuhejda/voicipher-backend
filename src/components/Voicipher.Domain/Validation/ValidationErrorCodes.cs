namespace Voicipher.Domain.Validation
{
    public static class ValidationErrorCodes
    {
        public static string EmptyField => nameof(EmptyField);

        public static string TextTooLong => nameof(TextTooLong);

        public static string InvalidId => nameof(InvalidId);

        public static string InvalidDateTime => nameof(InvalidDateTime);

        public static string NotFound => nameof(NotFound);

        public static string InvalidStoredValues => nameof(InvalidStoredValues);

        public static string InvalidPassword => nameof(InvalidPassword);
    }
}
