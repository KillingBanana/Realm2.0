using System;

public class Heap<T> where T : IHeapItem<T> {
	private readonly T[] items;
	public int Count { get; private set; }

	public Heap(int maxSize) {
		items = new T[maxSize];
	}

	public void Add(T item) {
		item.HeapIndex = Count;
		items[Count] = item;
		SortUp(item);
		Count++;
	}

	public void Update(T item) {
		SortUp(item);
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		Count--;

		items[0] = items[Count];
		items[0].HeapIndex = 0;
		SortDown(items[0]);

		return firstItem;
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	private void SortDown(T item) {
		while (true) {
			int child1Index = item.HeapIndex * 2 + 1;
			int child2Index = item.HeapIndex * 2 + 2;

			if (child1Index < Count) {
				T childToSwap = items[child1Index];
				if (child2Index < Count) {
					if (childToSwap.CompareTo(items[child2Index]) < 0) {
						childToSwap = items[child2Index];
					}
				}

				if (item.CompareTo(childToSwap) < 0) {
					Swap(item, childToSwap);
				} else {
					return;
				}
			} else {
				return;
			}
		}
	}

	private void SortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) {
			T parent = items[parentIndex];

			if (item.CompareTo(parent) > 0) {
				Swap(item, parent);
			} else {
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	private void Swap(T a, T b) {
		items[a.HeapIndex] = b;
		items[b.HeapIndex] = a;

		int aIndex = a.HeapIndex;
		a.HeapIndex = b.HeapIndex;
		b.HeapIndex = aIndex;
	}
}

public interface IHeapItem<in T> : IComparable<T> {
	int HeapIndex { get; set; }
}