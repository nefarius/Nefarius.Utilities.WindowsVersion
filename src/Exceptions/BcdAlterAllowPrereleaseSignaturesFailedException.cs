using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Nefarius.Utilities.WindowsVersion.Exceptions;

/// <summary>
///     Thrown on failure to change testsigning state.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class BcdAlterAllowPrereleaseSignaturesFailedException : Exception
{
    /// <inheritdoc />
    internal BcdAlterAllowPrereleaseSignaturesFailedException()
    {
    }

    internal BcdAlterAllowPrereleaseSignaturesFailedException(string message) : base(message)
    {
    }

    internal BcdAlterAllowPrereleaseSignaturesFailedException(string message, Exception innerException) : base(message,
        innerException)
    {
    }

    internal BcdAlterAllowPrereleaseSignaturesFailedException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}