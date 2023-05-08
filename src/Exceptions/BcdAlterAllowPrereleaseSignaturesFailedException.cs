using System;
using System.Runtime.Serialization;

namespace Nefarius.Utilities.WindowsVersion.Exceptions;

public class BcdAlterAllowPrereleaseSignaturesFailedException : Exception
{
    public BcdAlterAllowPrereleaseSignaturesFailedException()
    {
    }

    public BcdAlterAllowPrereleaseSignaturesFailedException(string message) : base(message)
    {
    }

    public BcdAlterAllowPrereleaseSignaturesFailedException(string message, Exception innerException) : base(message,
        innerException)
    {
    }

    protected BcdAlterAllowPrereleaseSignaturesFailedException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}