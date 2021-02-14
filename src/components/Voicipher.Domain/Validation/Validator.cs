using System;
using System.Collections.Generic;
using Voicipher.Domain.Utils;

namespace Voicipher.Domain.Validation
{
    public static class Validator
    {
        public static IList<ValidationError> ValidateRequired(this IList<ValidationError> errorList, string value, string field, string objectName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return errorList.Add(ValidationErrorCodes.EmptyField, field, objectName);

            return errorList;
        }

        public static IList<ValidationError> ValidateRequired<T>(this IList<ValidationError> errorList, T value, string field, string objectName = null) where T : class
        {
            if (value == null)
                return errorList.Add(ValidationErrorCodes.EmptyField, field, objectName);

            return errorList;
        }

        public static IList<ValidationError> ValidateNullableMaxLength(this IList<ValidationError> errorList, string value, string field, int maxLength, string objectName = null)
        {
            if (value == null)
                return errorList;

            return ValidateMaxLength(errorList, value, field, maxLength, objectName);
        }

        public static IList<ValidationError> ValidateMaxLength(this IList<ValidationError> errorList, string value, string field, int maxLength, string objectName = null)
        {
            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "MaxLength must be positive value.");

            if (value != null && value.Length > maxLength)
                return errorList.Add(ValidationErrorCodes.TextTooLong, field, objectName);

            return errorList;
        }

        public static IList<ValidationError> ValidateGuid(this IList<ValidationError> errorList, Guid value, string field, string objectName = null)
        {
            if (value == Guid.Empty)
                return errorList.Add(ValidationErrorCodes.InvalidId, field, objectName);

            return errorList;
        }

        public static IList<ValidationError> ValidateDateTime(this IList<ValidationError> errorList, DateTime value, string field, string objectName = null)
        {
            if (value == DateTime.MinValue)
                return errorList.Add(ValidationErrorCodes.InvalidDateTime, field, objectName);

            return errorList;
        }

        public static IList<ValidationError> ValidateNotNull(this IList<ValidationError> errorList, object value, string field, string objectName = null)
        {
            if (value == null)
                return errorList.Add(ValidationErrorCodes.ParameterIsNull, field, objectName);

            return errorList;
        }

        public static IList<ValidationError> ValidateLanguage(this IList<ValidationError> errorList, string value, string field, string objectName = null)
        {
            if (!SupportedLanguages.IsSupported(value))
                return errorList.Add(ValidationErrorCodes.NotSupportedLanguage, field, objectName);

            return errorList;
        }

        public static IList<ValidationError> Merge(this IList<ValidationError> errorList, ValidationResult validationResult)
        {
            var errors = errorList ?? new List<ValidationError>();

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(error);
                }
            }

            return errors;
        }

        public static IList<ValidationError> Merge(this IList<ValidationError> errorList, IList<ValidationResult> validationResults)
        {
            var errors = errorList ?? new List<ValidationError>();

            foreach (var validationResult in validationResults)
            {
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        errors.Add(error);
                    }
                }
            }

            return errors;
        }

        public static IList<ValidationError> Add(this IList<ValidationError> errorList, string code, string field, string objectName = null)
        {
            var fieldName = string.IsNullOrWhiteSpace(objectName) ? field : objectName + "." + field;
            return errorList.AddError(new ValidationError(code, fieldName));
        }

        public static IList<ValidationError> AddError(this IList<ValidationError> errorList, ValidationError error)
        {
            var errors = errorList ?? new List<ValidationError>();
            if (error != null)
            {
                errors.Add(error);
            }

            return errors;
        }
    }
}
