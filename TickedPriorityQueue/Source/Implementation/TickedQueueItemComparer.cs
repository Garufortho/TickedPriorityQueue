using System;
using System.Collections.Generic;
namespace TickedPriorityQueue
{
	/// <summary>
	/// Priority comparer for <see cref="TickedPriorityQueue.TickedQueueItem"/>.
	/// Only used internally.
	/// </summary>
	internal sealed class TickedQueueItemComparer : IComparer<TickedQueueItem>
	{
		/// <summary>
		/// <see cref="System.Collections.Generic.IComparer{TickedPriorityQueue.TickedQueueItem}"/> implementation for sorting <see cref="TickedPriorityQueue.TickedQueueItem"/> instances by Priority, then TickLength for equal priorities.
		/// </summary>
		/// <param name='a'>
		/// First item.
		/// </param>
		/// <param name='b'>
		/// Second Item.
		/// </param>
		public int Compare(TickedQueueItem a, TickedQueueItem b)
		{
			return DefaultCompare(a,b);
		}
		
		/// <summary>
		/// <see cref="System.Collections.Generic.IComparer{TickedPriorityQueue.TickedQueueItem}"/> implementation for sorting <see cref="TickedPriorityQueue.TickedQueueItem"/> instances by Priority, then TickLength for equal priorities.
		/// </summary>
		/// <param name='a'>
		/// First item.
		/// </param>
		/// <param name='b'>
		/// Second Item.
		/// </param>
		public static int DefaultCompare(TickedQueueItem a, TickedQueueItem b)
		{
			int comp = a.Priority.CompareTo(b.Priority);
			if (comp == 0)
			{
				int ret = a.NextTickTime.CompareTo(b.NextTickTime);
				return ret;
			}
			
			else return comp;
		}
	}
}

