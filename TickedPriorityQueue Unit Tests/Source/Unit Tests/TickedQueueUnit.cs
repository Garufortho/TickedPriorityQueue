using System;
using System.Linq;
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


		void InitializeTestValues()
		{
			test1 = -1;

			aCalled = 0;
			bCalled = 0;
			cCalled = 0;

			test3 = -1;
		}

		
		[Test()]
		public void TestPriority ()
		{
			InitializeTestValues();
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(CallbackSetTest1, 0);
			a.Priority = 0;
			TickedObject b = new TickedObject(CallbackSetTest1, 2);
			b.Priority = 2;
			TickedObject c = new TickedObject(CallbackSetTest1, 1);
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
		public void TestPauseBasic ()
		{
			InitializeTestValues();

			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(CallbackSetTest1, 0);

			queue.Add(a);

			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
			queue.IsPaused = true;
			queue.Update();
			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
		}


		[Test()]
		public void TestUnPauseBasic ()
		{
			InitializeTestValues();

			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(CallbackSetTest1, 0);

			queue.Add(a);

			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
			queue.IsPaused = true;
			queue.Update();
			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");

			queue.IsPaused = false;
			queue.Update();
			Assert.AreEqual(-1, test1, "test1 should be set to 0 after the queue is updated");
		}

		public void TestUnPausePriority ()
		{
			InitializeTestValues();
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(CallbackSetTest1, 0);
			a.Priority = 0;
			TickedObject b = new TickedObject(CallbackSetTest1, 2);
			b.Priority = 2;
			TickedObject c = new TickedObject(CallbackSetTest1, 1);
			c.Priority = 1;

			queue.Add(a);
			queue.Add(b);
			queue.Add(c);

			Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
			queue.IsPaused = true;
			queue.Update(DateTime.UtcNow.AddSeconds(2));
			Assert.AreEqual(-1, test1, "test1 should be still be -1, since we are paused");

			queue.IsPaused = false;
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
			
			bool result = queue.Remove(a);
			test3 = -1;

			Assert.IsTrue(result, "Call to remove the item should have returned true");
			
			queue.Update(DateTime.UtcNow.AddSeconds(4));
			Assert.AreEqual(-1, test3, "Callback should not have been called for removed item");
		}

		[Test()]
		public void TestInvalidRemove()
		{
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(Callback3, 0);
			a.TickLength = 1;
			queue.Add(a);

			var b = new TickedObject(Callback3, 0);

			bool result = queue.Remove(b);

			Assert.IsFalse(result, "Call to remove the B should have returned false");
		}

		[Test()]
		public void TestEnumerator()
		{
			TickedQueue queue = new TickedQueue();
			TickedObject a = new TickedObject(Callback3, 0);
			a.TickLength = 1;

			queue.Add(a);

			Assert.IsTrue(queue.Items.Any(), "There should be items on the queue");
			Assert.IsTrue(queue.Items.Contains(a), "Queue should contain the new item");
			Assert.AreEqual(1, queue.Items.Count(), "Queue should contain only one item");

			TickedObject b = new TickedObject(Callback3, 0);
			queue.Add(b);

			Assert.IsTrue(queue.Items.Contains(b), "Queue should contain the second item");
			Assert.AreEqual(2, queue.Items.Count(), "Queue should contain two items");

			queue.Remove(a);

			Assert.AreEqual(1, queue.Items.Count(), "Queue should contain only one item again");
			Assert.IsFalse(queue.Items.Contains(a), "Queue should not contain the original item");
		}

		void CallbackSetTest1(object obj)
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