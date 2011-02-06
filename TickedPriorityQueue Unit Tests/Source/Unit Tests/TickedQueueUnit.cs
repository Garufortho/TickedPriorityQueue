using System;
using NUnit.Framework;
using TickedPriorityQueue;

namespace TickedPriorityQueueUnitTests{
	[TestFixture()]
	public class TickedQueueUnit
	{
		int test1 = -1;
		
		int aCalled = 0;
		int bCalled = 0;
		int cCalled = 0;
		
		int test3 = -1;
		
		[Test()]
		public void TestPriority ()
		{
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(Callback1, 0);
			a.Priority = 0;
			TickedObject b = new TickedObject(Callback1, 2);
			b.Priority = 2;
			TickedObject c = new TickedObject(Callback1, 1);
			c.Priority = 1;
			
			queue.Add(a);
			queue.Add(b);
			queue.Add(c);
			
			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
			queue.Update();
			Assert.AreEqual(-1, test1, "test1 should still be same after first update, as none of the objects should have ticked");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreNotEqual(-1, test1, "test1 should have changed after all three items ticked");
			Assert.AreEqual(2, test1, "test1 should have been updated to the last object");
		}
		
		[Test()]
		public void TestTiming ()
		{
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(Callback2, 0);
			a.TickLength = 1;
			a.Priority = 1;
			TickedObject b = new TickedObject(Callback2, 2);
			b.TickLength = 5;
			b.Priority = 3;
			TickedObject c = new TickedObject(Callback2, 1);
			c.TickLength = 2;
			c.Priority = 2;
			
			queue.Add(a);
			queue.Add(b);
			queue.Add(c);
			
			Assert.AreEqual(0, aCalled + bCalled + cCalled, "called variables should be initialized with 0");
			queue.Update();
			Assert.AreEqual(0, aCalled + bCalled + cCalled, "called variables should be 0 with 0 time update");
			
			queue.Update(DateTime.UtcNow.AddSeconds(2.9f));
			Assert.AreEqual(1, aCalled, "a should have been called once");
			Assert.AreEqual(1, cCalled, "c should have been called once");
			Assert.AreEqual(0, bCalled, "b should not have been called");
			
			queue.Update(DateTime.UtcNow.AddSeconds(5f));
			Assert.AreEqual(2, aCalled, "a should have been called twice");
			Assert.AreEqual(2, cCalled, "c should have been called twice");
			Assert.AreEqual(1, bCalled, "b should have been called once");
		}
		
		[Test()]
		public void TestRemove()
		{
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(Callback3, 0);
			a.TickLength = 1;
			
			queue.Add(a);
			
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(1, test3, "Callback should have been called for added item");
			
			queue.Remove(a);
			test3 = -1;
			
			queue.Update(DateTime.UtcNow.AddSeconds(4));
			Assert.AreEqual(-1, test3, "Callback should not have been called for removed item");
		}
		
		void Callback1(object obj)
		{
			if (obj is int)
			{
				int i = (int)obj;
				if (i != test1 + 1) Assert.Fail("Callbacks called in the wrong order", obj);
				test1 = i;
			}
			else Assert.Fail("Junk sent back in callback", obj);
		}
		
		void Callback2(object obj)
		{
			if (obj is int)
			{
				int i = (int)obj;
				switch(i)
				{
				case 0:
					++aCalled;
					break;
				case 1:
					++cCalled;
					break;
				case 2:
					++bCalled;
					break;
				}
			}
			else Assert.Fail("Junk sent back in callback", obj);
		}
		
		void Callback3(object obj)
		{
			test3 = 1;
		}
	}
}