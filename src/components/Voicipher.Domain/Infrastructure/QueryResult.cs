using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Infrastructure
{
    public record QueryResult<T> : QueryResultBase
    {
        protected QueryResult()
            : this(OperationResult.Success, null, new List<T>(), null)
        {
        }

        public QueryResult(IEnumerable<T> items)
            : this(OperationResult.Success, null, items, null)
        {
        }

        public QueryResult(OperationError error)
            : this(OperationResult.Error, error, new List<T>(), null)
        {
        }

        public QueryResult(IEnumerable<ValidationError> validationErrors)
            : this(OperationResult.Error, null, new List<T>(), validationErrors)
        {
        }

        public QueryResult(OperationResult result, OperationError error, IEnumerable<ValidationError> validationErrors)
            : this(result, error, new List<T>(), validationErrors)
        {
        }

        private QueryResult(OperationResult result, OperationError error, IEnumerable<T> items, IEnumerable<ValidationError> validationErrors)
            : base(result, error, validationErrors)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public static QueryResult<T> Success { get; } = new QueryResult<T>();

        public IEnumerable<T> Items { get; }
    }
}
