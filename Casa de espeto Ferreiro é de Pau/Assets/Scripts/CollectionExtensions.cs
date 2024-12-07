using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public static class CollectionExtensions 
{
	public static void Shuffle<T>(this IList<T> list)
	{
		Random rng = new Random();

		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void Shuffle<T>(this IList<T> list, Random rng)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void Shuffle<T>(this Queue<T> queue)
	{
		List<T> list = queue.ToList();
		list.Shuffle(); // Reutiliza o método de extensão para IList<T>
		queue.Clear();
		foreach (T item in list)
		{
			queue.Enqueue(item);
		}
	}
}
