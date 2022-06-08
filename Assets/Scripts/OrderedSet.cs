using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data structure for a set that preserves insertion order
public class OrderedSet<T> : ICollection<T>
{
	private Dictionary<T, int> elementDict;
	private List<T> elements;

	public int Count => elements.Count;

	public bool IsReadOnly => false; // Currently never readonly

	
	public OrderedSet()
	{
		elementDict = new Dictionary<T, int>();
		elements = new List<T>();
	}

	public void Add(T item)
	{
		if (elementDict.ContainsKey(item))
			throw new System.Exception("Duplicate element added to OrderedSet");

		elements.Add(item);
		elementDict.Add(item, elements.Count - 1);
	}

	public void Clear()
	{
		elementDict.Clear();
		elements.Clear();
	}

	public bool Contains(T item)
	{
		return elementDict.ContainsKey(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		throw new System.NotImplementedException();
	}

	public bool Remove(T item) // Not great but oh well it works for our use case, remove is used relatively rarely anyways
	{
		bool found = elementDict.TryGetValue(item, out int index);

		if (!found)
			return false;

		for (int i = index + 1; i < elements.Count; i++)
		{
			elementDict[elements[i]] -= 1;
		}

		elementDict.Remove(item);
		elements.RemoveAt(index);

		return true;
	}

	public T this[int index] // Can't handle swapping with itself
	{
		get => elements[index];
		set => elements[index] = value;
	}

	public void SwapElements(int indexA, int indexB)
	{
		T elementA = elements[indexA];
		T elementB = elements[indexB];

		elements[indexA] = elementB;
		elements[indexB] = elementA;

		elementDict[elementA] = indexB;
		elementDict[elementB] = indexA;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return elements.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
