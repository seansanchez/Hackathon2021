using System;
using System.Net;

namespace H21.Wellness.Exceptions
{
    /// <inheritdoc />
    public class ConflictException : OperationFailedException
    {
        /// <inheritdoc cref="OperationFailedException" />
        public ConflictException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="OperationFailedException" />
        public ConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }
}