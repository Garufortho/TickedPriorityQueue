#if DEBUG
using System;
using NUnit.Framework;
using System.Reflection;
namespace TickedPriorityQueue
{
	[TestFixture()]
	public class QueueSaturationUnit
	{
		bool stagnantCalled = false;
		
		[Test()]
		public void TestForStagnation ()
		{
			TickedQueue queue = new TickedQueue();
			queue.MaxProcessedPerUpdate = 100;
			stagnantCalled = false;
			
			DateTime time = DateTime.UtcNow;
			
			for (int i = 0; i < 10000; ++i)
			{
				TickedObject obj = new TickedObject(Callback, i);
				obj.TickLength = 0.5f;
				queue.Add(obj, time);
			}
			
			time = time.AddSeconds(0.5);
			
			for (int i = 0; i < 1000; ++i)
			{
				time = time.AddMilliseconds(1000);
				queue.Update(time);
			}
			Assert.IsTrue(stagnantCalled, "Checking a low priority item doesn't get swamped");
		}
		
		private void Callback(object obj)
		{
			if (obj is int && (int)obj == 9999) stagnantCalled = true;
		}
	}
}
#endif