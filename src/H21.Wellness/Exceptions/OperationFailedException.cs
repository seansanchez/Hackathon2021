using System;
using System.Net;

namespace H21.Wellness.Exceptions
{
    /// <inheritdoc />
    public class OperationFailedException : Exception
    {
        /// <inheritdoc cref="Exception" />
        public OperationFailedException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="Exception" />
        public OperationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Gets the status code.
        /// </summary>
        public virtual HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    }
}