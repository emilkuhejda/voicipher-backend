﻿using System;
using System.Collections.Generic;

namespace Voicipher.Domain.Validation
{
    public static class Validator
    {
        public static IList<ValidationError> ValidateRequired(this IList<ValidationError> errorList, string value, string field)
        {
            if (string.IsNullOrWhiteSpace(value))
                return errorList.Add(ValidationErrorCodes.EmptyField, field);

            return errorList;
        }

        public static IList<ValidationError> ValidateMaxLength(this IList<ValidationError> errorList, string value, string field, int maxLength)
        {
            if (maxLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "MaxLength must be positive value.");
            }

            if (value != null && value.Length > maxLength)
            {
                return errorList.Add(ValidationErrorCodes.TextTooLong, field);
            }

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

        public static IList<ValidationError> Add(this IList<ValidationError> errorList, string code, string field)
        {
            return errorList.AddError(new ValidationError(code, field));
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
