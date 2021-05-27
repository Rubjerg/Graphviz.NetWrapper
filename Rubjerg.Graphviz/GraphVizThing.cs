using System;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz
{
    /// <summary>
    /// This is the most basic entity for our graphviz wrapper.  It wraps a C pointer to a managed
    /// resource, and wraps C functions with object oriented methods.  Everything that is wrapped,
    /// derives from this.  Since a graphviz thing is a dumb wrapper around a managed pointer, there
    /// can be multiple wrappers for the same pointer.  Ideally we would want to be a simple struct,
    /// but structs can't be subclassed in C#, so we must be a class and live with the
    /// overhead in code and performance.  This implies that we need to override what it means for
    /// two wrappers to be equal (i.e. not reference equality for wrappers, but equality of the
    /// pointers they wrap) to allow usage of common functions (like linq contains) in a way that
    /// makes sense.
    ///
    /// Invariant: ptr member is never 0.
    /// </summary>
    public abstract class GraphvizThing
    {
        internal readonly IntPtr _ptr;

        protected GraphvizThing(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("Can't have a null pointer.");
            _ptr = ptr;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GraphvizThing);
        }

        public virtual bool Equals(GraphvizThing obj)
        {
            return obj != null && _ptr == obj._ptr;
        }

        public override int GetHashCode()
        {
            return _ptr.GetHashCode();
        }

        public static bool operator ==(GraphvizThing a, GraphvizThing b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(GraphvizThing a, GraphvizThing b)
        {
            return !(a == b);
        }

        /// <summary>
        /// A GraphvizContext is used to store various layout
        /// information that is independent of a particular graph and
        /// its attributes.  It holds the data associated with plugins,
        /// parsed - command lines, script engines, and anything else
        /// with a scope potentially larger than one graph, up to the
        /// scope of the application. In addition, it maintains lists of
        /// the available layout algorithms and renderers; it also
        /// records the most recent layout algorithm applied to a graph.
        /// It can be used to specify multiple renderings of a given
        /// graph layout into different associated files.It is also used
        /// to store various global information used during rendering.
        /// There should be just one GVC created for the entire
        /// duration of an application. A single GVC value can be used
        /// with multiple graphs, though with only one graph at a
        /// time. In addition, if gvLayout() was invoked for a graph and
        /// GVC, then gvFreeLayout() should be called before using
        /// gvLayout() again, even on the same graph.
        /// </summary>
        protected static IntPtr GVC
        {
            get {
                lock (_gvc_mutex)
                {
                    if (_gvc == IntPtr.Zero)
                        _gvc = GvContext();
                    return _gvc;
                }
            }
        }
        private static IntPtr _gvc;
        private static readonly object _gvc_mutex = new object();

    }
}
