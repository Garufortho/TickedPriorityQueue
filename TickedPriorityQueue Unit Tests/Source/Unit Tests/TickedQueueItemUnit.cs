using System;
using NUnit.Framework;
using TickedPriorityQueue;

namespace TickedPriorityQueue {
	[TestFixture()]
	public class TickedQueueItemUnit
	{
		[Test()]
		public void TestCase ()
		{
			TickedObject obj = new TickedObject(null);
			obj.Priority = 6;
			obj.TickLength = 7;
			TickedQueueItem item = new TickedQueueItem(obj);
			Assert.AreEqual(item.Priority, obj.Priority, "TickedQueueItem should report the same priority as the wrapped object");
			
			DateTime testTime = DateTime.UtcNow;
			Assert.IsFalse(item.CheckTickReady(testTime), "CheckTickReady should return false when time is before next scheduled tick");
			
			DateTime testTimePlus4 = testTime.AddSeconds(4);
			Assert.IsFalse(item.CheckTickReady(testTimePlus4), "CheckTickReady should return false when time is before next scheduled tick");
			
			DateTime testTimePlus8 = testTime.AddSeconds(8);
			Assert.IsTrue(item.CheckTickReady(testTimePlus8), "CheckTickReady should return true when time is after next scheduled tick");
		}
		
		[Test()]
		public void TestTickTimeReset()
		{
			TickedObject obj = new TickedObject(null);
			obj.Priority = 6;
			obj.TickLength = 7;
			var now = DateTime.UtcNow;
			TickedQueueItem item = new TickedQueueItem(obj, now);
			Assert.AreEqual(item.NextTickTime, now.AddSeconds(obj.TickLength), "Initial next tick time did not match");
			
			var future = now.AddSeconds(3);
			item.ResetTickFromTime(future);
			Assert.AreEqual(item.NextTickTime, future.AddSeconds(obj.TickLength), "Next tick time did not match after reset");
		}
	}
}