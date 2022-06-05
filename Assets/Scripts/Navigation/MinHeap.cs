using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T : IHeapItem<T>
{
	private T[] items;
	private int itemCount;

	public MinHeap(int maxSize)
	{
		items = new T[maxSize];
	}

	public void Add(T item)
	{
		item.HeapIndex = itemCount;
		items[itemCount] = item;
		SortUp(item);
		itemCount++;
	}

	public T RemoveFirst()
	{
		T firstItem = items[0];

		itemCount--;
		items[0] = items[itemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);

		return firstItem;
	}

	public int Count { get { return itemCount; } }

	public bool Contains(T item)
	{
		return Equals(items[item.HeapIndex], item);
	}

	public void UpdateItem(T item)
	{
		SortUp(item);
		// SortDown(item);
	}

	private void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true)
		{
			T parentItem = items[parentIndex];

			if (item.CompareTo(parentItem) > 0) // Child is higher priority than parents
				Swap(item, parentItem);
			else
				break;

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	private void SortDown(T item)
	{
		while (true)
		{
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;

			if (childIndexLeft < itemCount)
			{
				// Decide to swap with left or right
				int swapIndex = childIndexLeft;

				if (childIndexRight < itemCount)
				{
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
						swapIndex = childIndexRight;
				}

				if (item.CompareTo(items[swapIndex]) < 0) // Do swap
				{
					Swap(item, items[swapIndex]);
				}
				else
					return;
			}
			else
				return;
		}
	}

	private void Swap(T a, T b)
	{
		items[a.HeapIndex] = b;
		items[b.HeapIndex] = a;

		int aIndex = a.HeapIndex;
		a.HeapIndex = b.HeapIndex;
		b.HeapIndex = aIndex;
	}
}

public interface IHeapItem<T> : IComparable<T>
{
	public int HeapIndex { get; set; }
}
