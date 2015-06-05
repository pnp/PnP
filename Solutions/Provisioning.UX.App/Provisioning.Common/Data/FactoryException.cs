using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data
{
    /// <summary>
    /// Initializes a new instance of the FactoryException class
    /// </summary>
    [Serializable]
    public sealed class FactoryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the FactoryException class with a system supplied message.
        /// </summary>
        public FactoryException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FactoryException class with the specified message string.
        /// </summary>
        /// <param name="message"> A string that describes the exception.</param>
        public FactoryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FactoryException class with a specified error message and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="message">A string that describes the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public FactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FactoryException class from serialized data.
        /// </summary>
        /// <param name="info">The object that contains the serialized data.</param>
        /// <param name="context">The stream that contains the serialized data.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.-or-The context parameter is null.</exception>
        private FactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
