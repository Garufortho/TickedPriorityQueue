using System;
using NUnit.Framework;
using TickedPriorityQueue;

namespace TickedPriorityQueueUnitTests
{
	[TestFixture()]
	public class TickedQueueItemComparerUnit
	{
		[Test()]
		public void TestPriorityCompare ()
		{
			TickedObject a = new TickedObject(Callback, 0, 0);
			a.Priority = 5;
			TickedObject b = new TickedObject(Callback, 0, 1);
			b.Priority = 2;
			
			DateTime time = DateTime.UtcNow;
			//forcing the time used so that there isn't any difference in the two calls
			TickedQueueItem itemA = new TickedQueueItem(a, time);
			TickedQueueItem itemB = new TickedQueueItem(b, time);
			
			TickedQueueItemComparer comparer = new TickedQueueItemComparer();
			Assert.AreEqual(1, comparer.Compare(itemA, itemB), "Comparison should yield lower for a");
			Assert.AreEqual(-1, comparer.Compare(itemB, itemA), "Reverse comparison should yield lower for b");
			a.Priority = 2;
			itemA = new TickedQueueItem(a, time);
			Assert.AreEqual(0, comparer.Compare(itemA, itemB), "Identical priorities should give equal comparison - {0}  {1}", itemA);
		}
		
		[Test()]
		public void TestTickTimeCompare()
		{
			TickedObject a = new TickedObject(Callback, 0, 0);
			a.Priority = 2;
			a.TickLength = 2;
			TickedObject b = new TickedObject(Callback, 0, 1);
			b.Priority = 2;
			b.TickLength = 1;
			
			TickedQueueItem itemA = new TickedQueueItem(a);
			TickedQueueItem itemB = new TickedQueueItem(b);
			
			TickedQueueItemComparer comparer = new TickedQueueItemComparer();
			Assert.AreEqual(1, comparer.Compare(itemA, itemB), "B should be lower due to lower tick time");
			a.Priority = 1;
			itemA = new TickedQueueItem(a);
			Assert.AreEqual(-1, comparer.Compare(itemA, itemB), "A should be sorted lower due to the priority");
		}
		
		[Test()]
		public void TestTimeCompare()
		{
			TickedObject a = new TickedObject(Callback, 0, 0);
			a.Priority = 2;
			a.TickLength = 2;
			TickedObject b = new TickedObject(Callback, 0, 1);
			b.Priority = 2;
			b.TickLength = 1;
			
			TickedQueueItem itemA = new TickedQueueItem(a, DateTime.UtcNow);
			TickedQueueItem itemB = new TickedQueueItem(b, DateTime.UtcNow.AddSeconds(2));
			
			TickedQueueItemComparer comparer = new TickedQueueItemComparer();
			Assert.AreEqual(-1, comparer.Compare(itemA, itemB), "A should be lower due to earlier tick time");
			b.Priority = 1;
			itemB = new TickedQueueItem(b);
			Assert.AreEqual(1, comparer.Compare(itemA, itemB), "B should be sorted lower due to the priority");
		}
				
		void Callback(object obj)
		{
		}
	}
}