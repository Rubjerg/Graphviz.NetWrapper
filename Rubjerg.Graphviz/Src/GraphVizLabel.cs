using System;
using System.Drawing;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz
{
    /// <summary>
    /// In GraphViz the way coordinates of bounding boxes are represented may differ.
    /// We want to provide a uniform API with bottom left coords only, so we use this enum to
    /// keep track of the current internal representation and convert if needed.
    /// </summary>
    internal enum BoundingBoxCoords
    {
        Centered,
        BottomLeft
    }

    /// <summary>
    /// Wraps a graphviz label for any kind of graphviz object.
    /// </summary>
    public class GraphVizLabel : GraphVizThing
    {
        private readonly BoundingBoxCoords representation;
        private readonly PointF offset;

        /// <summary>
        /// Unfortunately the way the bounding box is stored differs per object that the label belongs to.
        /// Therefore some extra information is needed to uniformly define a Label object.
        /// </summary>
        internal GraphVizLabel(IntPtr ptr, BoundingBoxCoords representation, PointF offset = default(PointF))
            : base(ptr)
        {
            this.representation = representation;
            this.offset = offset;
        }

        public string FontName()
        {
            return LabelFontname(_ptr);
        }

        /// <summary>
        /// Label size in points.
        /// </summary>
        public float FontSize()
        {
            return Convert.ToSingle(LabelFontsize(_ptr));
        }

        public string Text()
        {
            return LabelText(_ptr);
        }

        public RectangleF BoundingBox()
        {
            float x = Convert.ToSingle(LabelX(_ptr)) + offset.X;
            float y = Convert.ToSingle(LabelY(_ptr)) + offset.Y;
            float w = Convert.ToSingle(LabelWidth(_ptr));
            float h = Convert.ToSingle(LabelHeight(_ptr));
            if (representation == BoundingBoxCoords.Centered)
                return new RectangleF(x - w / 2, y - h / 2, w, h);
            else
                return new RectangleF(x, y, w, h);
        }

    }
}
