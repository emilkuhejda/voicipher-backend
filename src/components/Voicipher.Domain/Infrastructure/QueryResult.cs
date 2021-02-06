using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Infrastructure
{
    public record QueryResult<T> : QueryResultBase
    {
        protected QueryResult()
            : this(OperationResult.Success, null, default, null)
        {
        }

        public QueryResult(T value)
            : this(OperationResult.Success, null, value, null)
        {
        }

        public QueryResult(OperationError error)
            : this(OperationResult.Error, error, default, null)
        {
        }

        public QueryResult(IEnumerable<ValidationError> validationErrors)
            : this(OperationResult.Error, null, default, validationErrors)
        {
        }

        public QueryResult(OperationResult result, OperationError error, IEnumerable<ValidationError> validationErrors)
            : this(result, error, default, validationErrors)
        {
        }

        private QueryResult(OperationResult result, OperationError error, T value, IEnumerable<ValidationError> validationErrors)
            : base(result, error, validationErrors)
        {
            Value = value;
        }

        public static QueryResult<T> Success { get; } = new QueryResult<T>();

        public T Value { get; }
    }
}
