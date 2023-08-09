using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("Rubjerg.Graphviz.Test")]
namespace Rubjerg.Graphviz
{
}

/// <summary>
/// This is so we can use records and init keywords in .NET Framework
/// </summary>
namespace System.Runtime.CompilerServices
{
    using System.ComponentModel;
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}
