using System;
using System.Net;

namespace H21.Wellness.Exceptions
{
    /// <inheritdoc />
    public class NotFoundException : OperationFailedException
    {
        /// <inheritdoc cref="OperationFailedException" />
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="OperationFailedException" />
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}