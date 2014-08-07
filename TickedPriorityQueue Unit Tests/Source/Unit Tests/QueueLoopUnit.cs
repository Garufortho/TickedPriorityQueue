using System;
using NUnit.Framework;
using TickedPriorityQueue;

namespace TickedPriorityQueueUnitTests
{
	[TestFixture()]
	public class QueueLoopUnit
	{
		int testLoopCount = -1;
		int testNoLoopCount = -1;
		int testDefaultNoLoopCount = -1;
		int testDefaultLoopCount = -1;
		int testAgainstDefaultLoopCount = -1;
		
		[Test()]
		public void TestLoop()
		{
			testLoopCount = -1;
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(TestLoopCallback, 0);
			
			queue.Add(a);
			
			Assert.AreEqual(-1, testLoopCount, "testLoopCount should be initialized with -1");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(0, testLoopCount, "testLoopCount should still be incremented after first update");
			queue.Update(DateTime.UtcNow.AddSeconds(4));
			Assert.AreEqual(1, testLoopCount, "testLoopCount should have incremented again, a is set to loop");
		}
		
		[Test()]
		public void TestNoLoop ()
		{
			testNoLoopCount = -1;
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(TestNoLoopCallback, 0);
			
			queue.Add(a, false);
			
			Assert.AreEqual(-1, testNoLoopCount, "testNoLoopCount should be initialized with -1");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(0, testNoLoopCount, "testNoLoopCount should still be incremented after first update");
			queue.Update(DateTime.UtcNow.AddSeconds(4));
			Assert.AreEqual(0, testNoLoopCount, "testNoLoopCount shouldn't have incremented again, a is set not to loop");
		}
		
		[Test()]
		public void TestDefaultNoLoop ()
		{
			testDefaultNoLoopCount = -1;
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(TestDefaultNoLoopCallback, 0);
			queue.LoopByDefault = false;
			queue.Add(a);
			
			Assert.AreEqual(-1, testDefaultNoLoopCount, "testDefaultNoLoopCount should be initialized with -1");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(0, testDefaultNoLoopCount, "testDefaultNoLoopCount should still be incremented after first update");
			queue.Update(DateTime.UtcNow.AddSeconds(4));
			Assert.AreEqual(0, testDefaultNoLoopCount, "testDefaultNoLoopCount shouldn't have incremented again, a is set not to loop using global defaults");
		}
		
		[Test()]
		public void TestDefaultLoop ()
		{
			testDefaultLoopCount = -1;
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(TestDefaultLoopCallback, 0);
			queue.LoopByDefault = true;
			queue.Add(a);
			
			Assert.AreEqual(-1, testDefaultLoopCount, "testDefaultLoopCount should be initialized with -1");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(0, testDefaultLoopCount, "testDefaultLoopCount should still be incremented after first update");
			queue.Update(DateTime.UtcNow.AddSeconds(4));
			Assert.AreEqual(1, testDefaultLoopCount, "testDefaultLoopCount should have incremented again, a is set to loop using global defaults");
		}
		
		[Test()]
		public void TestAgainstDefaultLoop ()
		{
			testAgainstDefaultLoopCount = -1;
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(TestAgainstDefaultLoopCallback, 0);
			queue.LoopByDefault = true;
			queue.Add(a, false);
			
			Assert.AreEqual(-1, testAgainstDefaultLoopCount, "testAgainstDefaultLoopCount should be initialized with -1");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(0, testAgainstDefaultLoopCount, "testAgainstDefaultLoopCount should still be incremented after first update");
			queue.Update(DateTime.UtcNow.AddSeconds(4));
			Assert.AreEqual(0, testAgainstDefaultLoopCount, "testAgainstDefaultLoopCount shouldn't have incremented again, a is set not to loop");
		}
		
		void TestLoopCallback(object obj)
		{
			++testLoopCount;
		}
		
		void TestNoLoopCallback(object obj)
		{
			++testNoLoopCount;
		}
		
		void TestDefaultNoLoopCallback(object obj)
		{
			++testDefaultNoLoopCount;
		}
		
		void TestDefaultLoopCallback(object obj)
		{
			++testDefaultLoopCount;
		}
		
		void TestAgainstDefaultLoopCallback(object obj)
		{
			++testAgainstDefaultLoopCount;
		}
	}
}