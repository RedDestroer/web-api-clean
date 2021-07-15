using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WebApiClean.Domain.Exceptions
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class DomainException : Exception
    {
        public DomainException()
            : this("Unknown domain exception.")
        {
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DomainException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
