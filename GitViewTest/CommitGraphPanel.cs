using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GitViewTest
{
    public class CommitGraphPanel : Panel
    {
        public static readonly DependencyProperty NodeSizeProperty = DependencyProperty.Register(
            nameof(NodeSize),
            typeof(Size),
            typeof(CommitGraphPanel),
            new PropertyMetadata(new Size(20, 20)));

        public static readonly DependencyProperty IdProperty = DependencyProperty.RegisterAttached(
            "Id",
            typeof(object),
            typeof(CommitGraphPanel));

        public static readonly DependencyProperty ParentIdsProperty = DependencyProperty.RegisterAttached(
            "ParentIds",
            typeof(IEnumerable),
            typeof(CommitGraphPanel));

        public static readonly DependencyProperty NodeBrushProperty = DependencyProperty.RegisterAttached(
            "NodeBrush",
            typeof(Brush),
            typeof(CommitGraphPanel),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

        public Size NodeSize
        {
            get { return (Size)GetValue(NodeSizeProperty); }
            set { SetValue(NodeSizeProperty, value); }
        }

        public static object GetId(DependencyObject obj)
        {
            return obj.GetValue(IdProperty);
        }

        public static void SetId(DependencyObject obj, object value)
        {
            obj.SetValue(IdProperty, value);
        }

        public static IEnumerable GetParentIds(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(ParentIdsProperty);
        }

        public static void SetParentIds(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(ParentIdsProperty, value);
        }

        public static Brush GetNodeBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(NodeBrushProperty);
        }

        public static void SetNodeBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(NodeBrushProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var graphBuilder = new GraphBuilder();
            int maxX = 0;

            foreach (UIElement child in Children)
            {
                var node = graphBuilder.GetNode(child);
                if (node.X > maxX) maxX = node.X;

                child.Measure(availableSize);
            }

            return new Size((maxX + 1) * NodeSize.Height, Children.Count * NodeSize.Height);
        }

        // TODO: Use dependency property for this.
        private readonly Brush[] _brushes = new Brush[]
        {
            new SolidColorBrush(Color.FromRgb(21,160,191)),
            new SolidColorBrush(Color.FromRgb(6,105,247)),
            new SolidColorBrush(Color.FromRgb(142,0,194)),
            new SolidColorBrush(Color.FromRgb(197,23,182)),
            new SolidColorBrush(Color.FromRgb(217,1,113)),
            new SolidColorBrush(Color.FromRgb(205,1,1))
        };

        protected override Size ArrangeOverride(Size finalSize)
        {
            var graphBuilder = new GraphBuilder();

            foreach (UIElement child in Children)
            {
                var node = graphBuilder.GetNode(child);

                Point location = new Point(node.X * NodeSize.Width, node.Y * NodeSize.Height);

                SetNodeBrush(child, _brushes[node.X]);

                child.Arrange(new Rect(location, NodeSize));
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext dc)
        {
            Vector nodeSize = (Vector)NodeSize;
            Vector halfSize = nodeSize * 0.5;

            var graphBuilder = new GraphBuilder();

            foreach (UIElement child in Children)
            {
                var node = graphBuilder.GetNode(child);

                Point a = new Point(node.X * nodeSize.X, node.Y * nodeSize.Y) + halfSize;

                for (int i = 0; i < node.Children.Length; i++)
                {
                    var reference = node.Children[i];
                    var childNode = reference.Node;

                    Point b = new Point(childNode.X * nodeSize.X, childNode.Y * nodeSize.Y) + halfSize;

                    Pen pen = new Pen(_brushes[childNode.X], 2);

                    if (a.X == b.X)
                    {
                        dc.DrawLine(pen, a, b);
                    }
                    else if (childNode.IsMerge && !reference.IsFirst)
                    {
                        pen = new Pen(_brushes[node.X], 2);
                        DrawReference(dc, pen, b, a);
                    }
                    else
                    {
                        DrawReference(dc, pen, a, b);
                    }
                }
            }
        }

        private void DrawReference(DrawingContext dc, Pen pen, Point a, Point b)
        {
            Size arcSize = new Size(10, 10);

            Point a1 = a.X < b.X ? new Point(b.X - arcSize.Width, a.Y) : new Point(b.X + arcSize.Width, a.Y);
            Point b1 = a.Y < b.Y ? new Point(b.X, a.Y + arcSize.Height) : new Point(b.X, a.Y - arcSize.Height);
            SweepDirection sd = (a.X - b.X) * (a.Y - b.Y) < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

            dc.DrawGeometry(null, pen,
                new PathGeometry(new[]
                {
                    new PathFigure(a, new PathSegment[]
                    {
                        new LineSegment(a1, true),
                        new ArcSegment(b1, arcSize, 0, false, sd, true),
                        new LineSegment(b, true)
                    }, false)
                }));
        }

        private class GraphBuilder
        {
            private readonly List<CommitReference> _refs = new List<CommitReference>();
            private readonly CommitReference[] _refArray = new CommitReference[64];
            private int _y;

            public CommitNode GetNode(UIElement element)
            {
                var id = GetId(element);
                var children = DequeueChildren(id).ToArray();

                var node = new CommitNode
                {
                    Id = GetId(element),
                    Element = element,
                    Children = children,
                    X = children.Length > 0 ? children[0].Index : GetAvailableIndex(),
                    Y = _y++
                };

                var parentIds = GetParentIds(element);
                if (parentIds != null)
                {
                    int i = 0;
                    foreach (object parentId in parentIds)
                    {
                        var r = new CommitReference
                        {
                            Index = i == 0 ? node.X : GetAvailableIndex(),
                            IsFirst = i == 0,
                            Node = node,
                            ParentId = parentId
                        };

                        _refs.Add(r);
                        _refArray[r.Index] = r;

                        i++;
                    }

                    node.IsMerge = i > 1;
                }

                return node;
            }

            private int GetAvailableIndex()
            {
                for (int i = 0; i < _refArray.Length; i++)
                    if (_refArray[i] == null)
                        return i;

                throw new Exception("No available index found!");
            }

            private IEnumerable<CommitReference> DequeueChildren(object id)
            {
                for (int i = 0; i < _refs.Count; i++)
                {
                    var r = _refs[i];
                    if (Equals(r.ParentId, id))
                    {
                        _refArray[r.Index] = null;
                        _refs.RemoveAt(i);
                        i--;

                        yield return r;
                    }
                }
            }
        }

        private class CommitNode
        {
            public object Id;
            public UIElement Element;
            public CommitReference[] Children;
            public int X, Y;
            public bool IsMerge;

            public override string ToString()
            {
                return Id.ToString();
            }
        }

        private class CommitReference
        {
            public int Index;
            public bool IsFirst;
            public object ParentId;
            public CommitNode Node;
        }
    }
}
