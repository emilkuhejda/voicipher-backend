using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Infrastructure
{
    public abstract class CommandResultBase
    {
        protected static readonly List<ValidationError> EmptyList = new List<ValidationError>();

        public OperationResult Result { get; protected set; }

        public OperationError Error { get; protected set; }

        public IReadOnlyList<ValidationError> ValidationErrors { get; protected set; }
    }
}
