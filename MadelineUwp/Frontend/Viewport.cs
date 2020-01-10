using Madeline.Backend;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend
{
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

    internal class HoverInfo
    {
        public int node;
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
            node = -1;
            slot = SlotProximity.Empty;
            wire = Slot.Empty;
        }
    }

    internal struct Aabb
    {
        public Vector2 start;
        public Vector2 end;

        public Rect ToRect()
        {
            return new Rect(start.ToPoint(), end.ToPoint());
        }
    }

    internal class Components
    {
        public List<int> nodes = new List<int>();
        public List<Slot> slots = new List<Slot>();
        public List<Slot> wires = new List<Slot>();

        public void Clear()
        {
            nodes.Clear();
            slots.Clear();
            wires.Clear();
        }
    }

    internal class SelectionInfo
    {
        public Components candidates = new Components();
        public Components active = new Components();
        public Aabb box;

        public int ActiveNode
        {
            get
            {
                List<int> nodes = active.nodes;
                return nodes.Count > 0 ? nodes[0] : -1;
            }
            set
            {
                List<int> nodes = active.nodes;
                nodes.Clear();
                nodes.Add(value);
            }
        }

        public Slot ActiveSlot
        {
            get
            {
                List<Slot> slots = active.slots;
                return slots.Count > 0 ? slots[0] : Slot.Empty;
            }
            set
            {
                List<Slot> slots = active.slots;
                slots.Clear();
                slots.Add(value);
            }
        }

        public Slot ActiveWire
        {
            get
            {
                List<Slot> wires = active.wires;
                return wires.Count > 0 ? wires[0] : Slot.Empty;
            }
            set
            {
                List<Slot> wires = active.wires;
                wires.Clear();
                wires.Add(value);
            }
        }
    }

    internal class RewiringInfo
    {
        public Slot src = Slot.Empty;
        public Slot dst = Slot.Empty;
    }

    internal class Viewport
    {
        public Graph graph = SampleData.DefaultGraph();
        public Vector2 translate = new Vector2(400, 300);
        public float zoom = 1f;
        public int viewing = -1;

        public HoverInfo hover = new HoverInfo();
        public SelectionInfo selection = new SelectionInfo();
        public RewiringInfo rewiring = new RewiringInfo();

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
}
