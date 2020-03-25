using System;
using System.Runtime.Serialization;

namespace EsiNet.Expressions
{
    public class InvalidExpressionException : Exception
    {
        public InvalidExpressionException()
        {
        }

        protected InvalidExpressionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InvalidExpressionException(string message) : base(message)
        {
        }

        public InvalidExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}