using System;

namespace Rubjerg.Graphviz;

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
}
