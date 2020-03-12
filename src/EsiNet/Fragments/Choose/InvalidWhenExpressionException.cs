using System;
using System.Runtime.Serialization;

namespace EsiNet.Fragments.Choose
{
    public class InvalidWhenExpressionException : Exception
    {
        public InvalidWhenExpressionException()
        {
        }

        protected InvalidWhenExpressionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InvalidWhenExpressionException(string message) : base(message)
        {
        }

        public InvalidWhenExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}