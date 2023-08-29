using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sinan.AMF3
{
    public class AmfException : Exception
    {
        // constructors
        /// <summary>
        /// Initializes a new instance of the AmfException class.
        /// </summary>
        public AmfException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AmfException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public AmfException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AmfException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AmfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AmfException class.
        /// </summary>
        /// <param name="format">The error message format string.</param>
        /// <param name="args">One or more args for the error message.</param>
        public AmfException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the AmfException class (this overload used by deserialization).
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public AmfException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
