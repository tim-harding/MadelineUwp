using Madeline.Backend;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend
{
    internal class Viewport
    {
        public Graph graph = new Graph();
        public Vector2 translate;
        public float zoom = 1f;
        public int viewing = -1;
        public int active = -1;

        public HoverInfo hover = new HoverInfo();
        public SelectionInfo selection = new SelectionInfo();
        public RewiringInfo rewiring = new RewiringInfo();
        public History history;

        public Viewport()
        {
            history = new History(graph);
        }

        public void ZoomAround(Vector2 pos, int delta)
        {
            float factor = (float)Math.Pow(1.001, delta);
            zoom *= factor;
            translate = From(Into(translate) - pos * (factor - 1f));
        }

        public void Move(Vector2 delta)
        {
            translate += delta * 1f / zoom;
        }

        public Vector2 Into(Vector2 pos)
        {
            return (pos + translate) * zoom;
        }

        public Aabb Into(Aabb box)
        {
            box.start = Into(box.start);
            box.end = Into(box.end);
            return box;
        }

        public Vector2 From(Vector2 pos)
        {
            return pos / zoom - translate;
        }

        public Aabb From(Aabb box)
        {
            box.start = From(box.start);
            box.end = From(box.end);
            return box;
        }

        public Matrix3x2 Into()
        {
            return Matrix3x2.CreateTranslation(translate) * Matrix3x2.CreateScale(zoom);
        }

        public Matrix3x2 From()
        {
            return Matrix3x2.CreateScale(1f / zoom) * Matrix3x2.CreateTranslation(-translate);
        }
    }

    internal struct SlotProximity
    {
        public float distance;
        public Slot slot;

        public bool IsHover => distance < 256f;

        public SlotProximity(float distance, Slot slot)
        {
            this.distance = distance;
            this.slot = slot;
        }

        public static SlotProximity Empty = new SlotProximity(float.MaxValue, Slot.Empty);
    }

    internal struct NodeHover
    {
        public enum State
        {
            Body,
            Disable,
            Viewing,
        }

        public int id;
        public State state;

        public NodeHover(int id, State state)
        {
            this.id = id;
            this.state = state;
        }

        public static NodeHover Empty = new NodeHover(-1, State.Body);
    }

    internal class HoverInfo
    {
        public NodeHover node;
        public SlotProximity slot;
        public Slot wire;

        public HoverInfo()
        {
            Clear();
        }

        public HoverInfo(HoverInfo src)
        {
            node = src.node;
            slot = src.slot;
            wire = src.wire;
        }

        public void Clear()
        {
            node = NodeHover.Empty;
            slot = SlotProximity.Empty;
            wire = Slot.Empty;
        }
    }

    internal struct Aabb
    {
        public Vector2 start;
        public Vector2 end;

        public Aabb(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public Rect ToRect()
        {
            return new Rect(start.ToPoint(), end.ToPoint());
        }

        public static Aabb Zero = new Aabb(Vector2.Zero, Vector2.Zero);
    }

    internal class Components
    {
        public List<int> nodes = new List<int>();
        public List<Slot> wires = new List<Slot>();

        public void Clear()
        {
            nodes.Clear();
            wires.Clear();
        }
    }

    internal class SelectionInfo
    {
        public Components candidates = new Components();
        public Components active = new Components();
        public Aabb box;
    }

    internal class RewiringInfo
    {
        public Slot src = Slot.Empty;
        public Slot dst = Slot.Empty;
    }
}
