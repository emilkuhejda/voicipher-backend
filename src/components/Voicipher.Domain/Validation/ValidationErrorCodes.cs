namespace Voicipher.Domain.Validation
{
    public static class ValidationErrorCodes
    {
        public static string EmptyField => nameof(EmptyField);

        public static string TextTooLong => nameof(TextTooLong);

        public static string NotFound => nameof(NotFound);

        public static string UnavailableBlobStorage => nameof(UnavailableBlobStorage);

        public static string AdministratorNotFound => nameof(AdministratorNotFound);

        public static string ParameterIsNull => nameof(ParameterIsNull);

        public static string NotSupportedLanguage => nameof(NotSupportedLanguage);

        public static string NotSupportedLanguageModel => nameof(NotSupportedLanguageModel);

        public static string NotSupportedContentType => nameof(NotSupportedContentType);

        public static string NotEnoughSubscriptionTime => nameof(NotEnoughSubscriptionTime);

        public static string OperationFailed => nameof(OperationFailed);

        public static string EndTimeGreaterThanStartTime => nameof(EndTimeGreaterThanStartTime);

        public static string InvalidId => nameof(InvalidId);

        public static string InvalidDateTime => nameof(InvalidDateTime);

        public static string InvalidInputData => nameof(InvalidInputData);

        public static string InvalidStoredValues => nameof(InvalidStoredValues);

        public static string InvalidPassword => nameof(InvalidPassword);

        public static string InvalidEmail => nameof(InvalidEmail);
    }
}
