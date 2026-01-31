using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codes.Common
{
    public class MyPriorityQueue<T>
    {
        private List<(T item, float priority)> heap = new();

        public int Count => heap.Count;

        public void Enqueue(T item, float priority)
        {
            heap.Add((item, priority));
            HeapifyUp(heap.Count - 1);
        }

        public T Dequeue()
        {
            var root = heap[0].item;
            heap[0] = heap[^1];
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);
            return root;
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (heap[i].priority >= heap[parent].priority) break;
                (heap[i], heap[parent]) = (heap[parent], heap[i]);
                i = parent;
            }
        }

        private void HeapifyDown(int i)
        {
            while (true)
            {
                int left = i * 2 + 1;
                int right = left + 1;
                int smallest = i;

                if (left < heap.Count && heap[left].priority < heap[smallest].priority)
                    smallest = left;
                if (right < heap.Count && heap[right].priority < heap[smallest].priority)
                    smallest = right;

                if (smallest == i) break;
                (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
                i = smallest;
            }
        }
    }

}
