using System;
using System.Collections.Generic;
using System.Text;

namespace CompletelyFairScheduler
{
    public class BinomialHeap<T> where T : IComparable
    {
        private BinomialHeapNode<T> Root { get; set; }

        public BinomialHeap() => Root = null;

        public BinomialHeap(BinomialHeapNode<T> root)
        {
            this.Root = root;
        }

        public void Clear()
        {
            Root = null;
        }

        public bool IsEmpty()
        {
            return (Root != null);
        }

        public void Insert(T value)
        {
            BinomialHeapNode<T> p = (Root != null) ? Root.Parent : null;
            BinomialHeapNode<T> singletonNode = new BinomialHeapNode<T>(value, p);
            Root = UnionHeap(singletonNode, Root);
        }

        public T GetMin()
        {
            T min = default(T);
            if (Root != null)
            {
                BinomialHeapNode<T> temp = Root;
                min = Root.Value;

                while (temp != null)
                {
                    if (temp.Value.CompareTo(min) < 0)
                    {
                        min = temp.Value;
                    }
                    temp = temp.Sibling;
                }
            }
            return min;
        }

        public T ExtractMin()
        {
            BinomialHeapNode<T> previous = null, min = null, temp = null, next = null;

            T minValue;
            if (Root != null)
            {
                BinomialHeapNode<T> minPrevious = null; // previous node of min node
                min = Root;
                temp = Root.Sibling;
                previous = Root;

                // find min and minPrev of heaps
                while (temp != null)
                {
                    if (temp.Value.CompareTo(min.Value) < 0)
                    {
                        min = temp;
                        minPrevious = previous;
                    }

                    previous = previous.Sibling;
                    temp = temp.Sibling;
                }

                // If prev, assign prev sibling to min sibling
                // else if no prev min sibling is new root
                if (minPrevious != null)
                {
                    minPrevious.Sibling = min.Sibling;
                }
                else
                {
                    Root = min.Sibling;
                }

                // Update all children nodes parent pointers to null
                next = min.Child;
                temp = next;

                while (temp != null)
                {
                    temp.Parent = min.Parent;
                    temp = temp.Sibling;
                }

                // detach and delete min
                min.Sibling = null;
                min.Child = null;
                min.Parent = null;
                minValue = min.Value;

                // Union of the two detached heaps
                Root = UnionHeap(Root, next);
            }
            else
                throw Exception("Empty Heap!");

            return minValue;
        }

        public int Size()
        {
            int i = 0;

            if (Root != null)
            {
                Root.Size(i);
            }

            return i;
        }

        public void DeleteKey (T v)
        {
            DecreaseKey(v, GetMin());
            ExtractMin();
        }

        public void DecreaseKey (T v, T newValue)
        {
            if (Root != null)
            {
                if (v.CompareTo(newValue) > 0)
                {
                    // maintains heap property by bubbling current until parent > current
                    BinomialHeapNode<T> current = Root.Find(v), parent = null;

                    T temp;
                    if (current != null)
                    {
                        current.Value = newValue;
                        parent = current.Parent;

                        while ((current != null) && (parent != null) && 
                            (current.Value.CompareTo(parent.Value) > 0))
                        {
                            temp = current.Value;
                            current.Value = parent.Value;
                            parent.Value = temp;
                            current = parent;
                            parent = current.Parent;
                        }
                    }
                    else
                    {
                        throw Exception("Key not found!");
                    }
                }
                else
                {
                    throw Exception("New key must be less than current key");
                }
            }
            else
            {
                throw Exception("Heap is empty!");
            }
        }

        public BinomialHeapNode<T> FindKey (T value)
        {
            return Root?.Find(value);
        }

        public int Order(BinomialHeapNode<T> heap)
        {
            if (heap != null)
            {
                int i = 0;
                BinomialHeapNode<T> child = heap.Child;
                while (child != null)
                {
                    ++i;
                    child = child.Sibling;
                }

                return i;
            }
            return -1;
        }

        public BinomialHeapNode<T> UnionHeap(BinomialHeapNode<T> heapA, BinomialHeapNode<T> heapB)
        {
            // Merges heapA and heapB assuming both are sorted in heap order
            // concatenates heaps of same order so that at most one tree of each order

            // get merged heap of A and B
            BinomialHeapNode<T> heapUnion = MergeHeap(heapA, heapB);

            if (heapUnion != null)
            {
                BinomialHeapNode<T> current = heapUnion, previous = null, next = null;

                int orderA, orderB;

                while ((current != null) && (current.Sibling != null))
                {
                    next = current.Sibling;
                    orderA = current.order;
                    orderB = next.order;
                    if (orderA == orderB && orderA != Order(next.Sibling))
                    {
                        if (current.Value.CompareTo(next.Value) < 0)
                        {
                            current.Sibling = next.Sibling;
                            current.AddChild(next);
                            previous = current;
                            current = current.Sibling;
                        }
                        else
                        {
                            if (previous != null)
                            {
                                previous.Sibling = next;
                            }
                            else
                            {
                                heapUnion = next;
                            }
                            next.AddChild(current);
                            previous = next;
                            current = next.Sibling;
                        }
                    }
                    else
                    {
                        if (previous == null)
                        {
                            heapUnion = current;
                        }
                        previous = current;
                        current = next;
                    }
                }
            }
            return heapUnion;
        }

        public BinomialHeapNode<T> MergeHeap(BinomialHeapNode<T> heapA, BinomialHeapNode<T> heapB)
        {
            BinomialHeapNode<T> mergeHeap = null;

            if (heapA != null || heapB != null)
            {
                if (heapA != null && heapB == null)
                {
                    mergeHeap = heapA;
                }
                else if (heapA == null && heapB != null)
                {
                    mergeHeap = heapB;
                }
                else
                {
                    BinomialHeapNode<T> temp = null, next = null, previous = null, current = null;

                    if (heapA.order > heapB.order)
                    {
                        mergeHeap = heapB;
                        next = heapA;
                    }
                    else
                    {
                        mergeHeap = heapA;
                        next = heapB;
                    }

                    current = mergeHeap;
                    // merge heaps by reassigning sibling pointers
                    while (current != null && next != null && current != next)
                    {
                        if (current.order <= next.order)
                        {
                            if (current.Sibling != null)
                            {
                                temp = current.Sibling;
                                current.Sibling = next;
                                previous = current;
                                current = next;
                                next = temp;
                            }
                            else
                            {
                                current.Sibling = next;
                                current = next; // breaks loop
                            }
                        }
                        else
                        {
                            if (previous != null)
                            {
                                previous.Sibling = next;
                            }
                            else
                            {
                                mergeHeap = next;
                            }
                            temp = next.Sibling;
                            next.Sibling = current;
                            previous = next;
                            next = temp;
                        }
                    }
                }
            }

            return mergeHeap;
        }

        private Exception Exception(string v)
        {
            throw new NotImplementedException();
        }
    }
}
