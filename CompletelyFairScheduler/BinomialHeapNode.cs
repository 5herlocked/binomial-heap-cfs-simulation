using System;

namespace CompletelyFairScheduler
{
    public class BinomialHeapNode<T> where T : IComparable
    {
        internal T Value { get; set; }
        internal BinomialHeapNode<T> Sibling { get; set; } = null;
        internal BinomialHeapNode<T> Child { get; set; } = null;
        internal BinomialHeapNode<T> Parent { get; set; } = null;
        internal int order;

        public void AddChild(BinomialHeapNode<T> child)
        {
            if (child != null)
            {
                child.Sibling = null;
                if (Child != null)
                {
                    BinomialHeapNode<T> temp = Child;
                    while (temp.Sibling != null)
                    {
                        temp = temp.Sibling;
                    }
                    temp.Sibling = child;
                }
                else
                {
                    Child = child;
                }

                ++order;
                child.Parent = this;
            }
        }

        public BinomialHeapNode<T> Find (T toFind)
        {
            BinomialHeapNode<T> found = null;
            if (Value.Equals(toFind))
                return this;
            else if (Sibling != null)
            {
                found = Sibling.Find(toFind);
            }
            if (Value.CompareTo(toFind) < 0 && found == null)
            {
                if (Child != null)
                {
                    found = Child.Find(toFind);
                }
            }
            return found;
        }

        public int Size (int i)
        {
            ++i;
            if (Sibling != null)
                Sibling.Size(i);
            if (Child != null)
                Child.Size(i);

            return i;
        }

        public void AddTo (BinomialHeap<T> otherHeap)
        {
            otherHeap.Insert(Value);
            if (Sibling != null)
            {
                Sibling.AddTo(otherHeap);
            }
            
            if (Child != null)
            {
                Child.AddTo(otherHeap);
            }
        }

        public BinomialHeapNode(T value, BinomialHeapNode<T> parent)
        {
            Value = value;
            Parent = parent;
            Sibling = Child = null;
            order = 0;
        }
    }
}