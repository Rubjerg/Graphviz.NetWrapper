using System;

namespace Rubjerg.Graphviz
{
    /// <summary>
    /// An exception that is raised when a contract is violated,
    /// e.g. in the SMContract class.
    /// </summary>
    public class GraphvizException : Exception
    {
        public GraphvizException() { }
        public GraphvizException(string message) : base(message) { }
        public GraphvizException(string message, Exception inner) : base(message, inner) { }
    }

}
