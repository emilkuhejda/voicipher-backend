using System.Collections.Generic;
using System.Linq;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Infrastructure
{
    public class CommandResult : CommandResult<None>
    {
        public CommandResult()
            : base(None.Value)
        {
        }

        public CommandResult(OperationError error)
            : base(error)
        {
        }

        public CommandResult(IEnumerable<ValidationError> validationErrors)
            : base(validationErrors)
        {
        }

        public static CommandResult Success { get; } = new CommandResult();

        public static CommandResult<ICollection<T>> FromList<T>(IEnumerable<T> list)
        {
            return new CommandResult<ICollection<T>>(list.ToList());
        }

        public static CommandResult<T> From<T>(T value)
        {
            return new CommandResult<T>(value);
        }
    }

    public class CommandResult<T> : CommandResultBase
    {
        public CommandResult(T value)
            : this(value, OperationResult.Success, null, EmptyList)
        {
        }

        public CommandResult(OperationError error)
            : this(default(T), OperationResult.Error, error, EmptyList)
        {
        }

        public CommandResult(IEnumerable<ValidationError> validationErrors)
            : this(default(T), OperationResult.Error, null, validationErrors)
        {
        }

        public CommandResult(OperationResult result, T value, OperationError error, IEnumerable<ValidationError> validationErrors)
            : this(value, result, error, validationErrors)
        {
        }

        private CommandResult(T value, OperationResult result, OperationError error, IEnumerable<ValidationError> validationErrors)
        {
            Value = value;
            Result = result;
            Error = error;
            ValidationErrors = validationErrors?.ToList() ?? EmptyList;
        }

        public T Value { get; }
    }
}
