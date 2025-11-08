using System;
using System.Collections.Generic;

public class PriorityQueue<T, TKey>
{
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    private readonly List<T> heap;
    private readonly Func<T, TKey> keySelector;
    private readonly SortOrder order;
    private readonly Comparer<TKey> comparer;

    public int Count => heap.Count;

    public PriorityQueue(Func<T, TKey> keySelector, SortOrder order = SortOrder.Ascending)
    {
        this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        this.order = order;
        this.comparer = Comparer<TKey>.Default;
        heap = new List<T>();
    }

    public void Enqueue(T item)
    {
        heap.Add(item);
        HeapifyUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T root = heap[0];
        T last = heap[^1];
        heap.RemoveAt(heap.Count - 1);

        if (heap.Count > 0)
        {
            heap[0] = last;
            HeapifyDown(0);
        }

        return root;
    }

    public T Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");
        return heap[0];
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (Compare(heap[index], heap[parent]) >= 0)
                break;

            (heap[index], heap[parent]) = (heap[parent], heap[index]);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;

            if (left <= lastIndex && Compare(heap[left], heap[smallest]) < 0)
                smallest = left;
            if (right <= lastIndex && Compare(heap[right], heap[smallest]) < 0)
                smallest = right;
            if (smallest == index)
                break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }

    private int Compare(T a, T b)
    {
        int cmp = comparer.Compare(keySelector(a), keySelector(b));
        return order == SortOrder.Ascending ? cmp : -cmp;
    }

    public bool IsEmpty() => heap.Count == 0;
    public void Clear() => heap.Clear();
}
