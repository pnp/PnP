using System;
using System.Runtime.Serialization;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    public class DeployerArgumentsException : Exception
    {
        #region Constructors

        public DeployerArgumentsException() : base() { }

        public DeployerArgumentsException(String message) : base(message) { }

        public DeployerArgumentsException(String message, Exception innerException) : base(message, innerException) { }

        public DeployerArgumentsException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }
}
