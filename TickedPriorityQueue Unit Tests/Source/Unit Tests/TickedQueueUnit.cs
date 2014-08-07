using System;
using System.Linq;
using NUnit.Framework;
using TickedPriorityQueue;

namespace TickedPriorityQueueUnitTests
{
    [TestFixture]
    public class TickedQueueUnit
    {
        private int test1 = -1;

        private int aCalled;
        private int bCalled;
        private int cCalled;

        private int test3 = -1;


        private void InitializeTestValues()
        {
            test1 = -1;

            aCalled = 0;
            bCalled = 0;
            cCalled = 0;

            test3 = -1;
        }


        [Test]
        public void TestPriority()
        {
            InitializeTestValues();
            var queue = new TickedQueue();
            var a = new TickedObject(CallbackSetTest1, 0, 0) {Priority = 0};
            var b = new TickedObject(CallbackSetTest1, 0, 2) {Priority = 2};
            var c = new TickedObject(CallbackSetTest1, 0, 1) {Priority = 1};

            queue.Add(a);
            queue.Add(b);
            queue.Add(c);

            Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
            queue.Update();
            Assert.AreEqual(-1, test1,
                "test1 should still be same after first update, as none of the objects should have ticked");
            queue.Update(DateTime.UtcNow.AddSeconds(2));
            Assert.AreNotEqual(-1, test1, "test1 should have changed after all three items ticked");
            Assert.AreEqual(2, test1, "test1 should have been updated to the last object");
        }


        [Test]
        public void TestPauseBasic()
        {
            InitializeTestValues();

            var queue = new TickedQueue();
            var a = new TickedObject(CallbackSetTest1, 0);

            queue.Add(a);

            Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
            queue.IsPaused = true;
            queue.Update();
            Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
            queue.Update(DateTime.UtcNow.AddSeconds(2));
            Assert.AreEqual(-1, test1, "test1 should be initialized with -1");
        }


        [Test]
        public void TestUnPauseBasic()
        {
            InitializeTestValues();

            var queue = new TickedQueue();
            var a = new TickedObject(CallbackSetTest1, 0);

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

        [Test]
        public void TestUnPausePriority()
        {
            InitializeTestValues();
            var queue = new TickedQueue();
            var a = new TickedObject(CallbackSetTest1, 0, 0) {Priority = 0};
            var b = new TickedObject(CallbackSetTest1, 0, 2) {Priority = 2};
            var c = new TickedObject(CallbackSetTest1, 0, 1) {Priority = 1};

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


        [Test]
        public void TestTiming()
        {
            var queue = new TickedQueue();
            var a = new TickedObject(Callback2, 1, 0) {Priority = 1};
            var b = new TickedObject(Callback2, 5, 2) {Priority = 3};
            var c = new TickedObject(Callback2, 2, 1) {Priority = 2};

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

            queue.Update(DateTime.UtcNow.AddSeconds(5.01f));
            Assert.AreEqual(2, aCalled, "a should have been called twice");
            Assert.AreEqual(2, cCalled, "c should have been called twice");
            Assert.AreEqual(1, bCalled, "b should have been called once");
        }

        [Test]
        public void TestRemove()
        {
            var queue = new TickedQueue();
            var a = new TickedObject(Callback3, 0);
            a.TickLength = 1;

            queue.Add(a);

            queue.Update(DateTime.UtcNow.AddSeconds(2));
            Assert.AreEqual(1, test3, "Callback should have been called for added item");

            var result = queue.Remove(a);
            test3 = -1;

            Assert.IsTrue(result, "Call to remove the item should have returned true");

            queue.Update(DateTime.UtcNow.AddSeconds(4));
            Assert.AreEqual(-1, test3, "Callback should not have been called for removed item");
        }

        [Test]
        public void TestRemoveWhileEnqueued()
        {
            var queue = new TickedQueue();
            queue.MaxProcessedPerUpdate = 1;

            var aVal = 0;
            var a = new TickedObject((x => aVal++), 0);

            var bVal = 0;
            var b = new TickedObject((x => bVal++), 0);

            var cVal = 0;
            var c = new TickedObject((x => cVal++), 0);

            queue.Add(a, true);
            queue.Add(b, true);
            queue.Add(c, true);

            // Verify the queue works as expected
            queue.Update(DateTime.UtcNow.AddSeconds(0.5f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the first update");
            Assert.AreEqual(0, bVal, "Invalid bVal after the first update");
            Assert.AreEqual(0, cVal, "Invalid cVal after the first update");

            Assert.IsTrue(queue.Remove(b), "Error removing B");

            queue.Update(DateTime.UtcNow.AddSeconds(1f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the second update");
            Assert.AreEqual(0, bVal, "B should not have been ticked after being removed");
            Assert.AreEqual(1, cVal, "Invalid cVal after the second update");

            queue.Update(DateTime.UtcNow.AddSeconds(1.5f));
            Assert.AreEqual(2, aVal, "Invalid aVal after the third update");
            Assert.AreEqual(0, bVal, "B should not have been ticked after being removed");
            Assert.AreEqual(1, cVal, "Invalid cVal after the third update");
        }

        /// <summary>
        /// Verifies that we can remove an item even while it is already on the work queue 
        /// </summary>
        [Test]
        public void TestRemoveFromHandlerWhileInWorkQueue()
        {
            var queue = new TickedQueue();
            queue.MaxProcessedPerUpdate = 10;

            var aVal = 0;
            var a = new TickedObject((x => aVal++), 0);

            var bVal = 0;
            var b = new TickedObject((x => bVal++), 0);

            var cVal = 0;
            var c = new TickedObject((x =>
            {
                cVal += 2;
                queue.Remove(b);
            }), 0);

            queue.Add(a, true);
            queue.Add(c, true); // c will execute before B and remove it
            queue.Add(b, true);

            // Verify the queue works as expected
            queue.Update(DateTime.UtcNow.AddSeconds(0.5f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the first update");
            Assert.AreEqual(2, cVal, "Invalid cVal after the first update");
            Assert.AreEqual(0, bVal, "b should not have executed");
            Assert.IsFalse(queue.Items.Contains(b), "b should not be on the queue");

            queue.Update(DateTime.UtcNow.AddSeconds(1f));
            Assert.AreEqual(2, aVal, "Invalid aVal after the second update");
            Assert.AreEqual(4, cVal, "Invalid cVal after the second update");
            Assert.AreEqual(0, bVal, "b should not have executed");
            Assert.IsFalse(queue.Items.Contains(b), "b should still not be on the queue");
        }

        /// <summary>
        /// Verifies that we can remove an item even while it is already on the work queue 
        /// </summary>
        [Test]
        public void TestRemoveFromHandlerAfterPassingInWorkQueue()
        {
            var queue = new TickedQueue {MaxProcessedPerUpdate = 10};

            var aVal = 0;
            var a = new TickedObject((x => aVal++));

            var bVal = 0;
            var b = new TickedObject((x => bVal++));

            var cVal = 0;
            var c = new TickedObject((x =>
            {
                cVal += 2;
                queue.Remove(b);
            }));

            var time = DateTime.UtcNow;
            queue.Add(a, time, true);
            queue.Add(b, time, true);
            queue.Add(c, time, true); // c will remove b, so it should execute only once

            // Verify the queue works as expected
            queue.Update(DateTime.UtcNow.AddSeconds(0.5f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the first update");
            Assert.AreEqual(2, cVal, "Invalid cVal after the first update");
            Assert.AreEqual(1, bVal, "b should have executed the first time");
            Assert.IsFalse(queue.Items.Contains(b), "b should no longer be on the queue");

            queue.Update(DateTime.UtcNow.AddSeconds(1f));
            Assert.AreEqual(2, aVal, "Invalid aVal after the second update");
            Assert.AreEqual(4, cVal, "Invalid cVal after the second update");
            Assert.AreEqual(1, bVal, "b should not have executed again");
            Assert.IsFalse(queue.Items.Contains(b), "b should still not be on the queue");
        }


        [Test]
        public void TestInvalidRemove()
        {
            var queue = new TickedQueue();
            var a = new TickedObject(Callback3, 1);
            queue.Add(a);

            var b = new TickedObject(Callback3);

            var result = queue.Remove(b);

            Assert.IsFalse(result, "Call to remove the B should have returned false");
        }

        [Test]
        public void TestNoExceptionHandler()
        {
            var queue = new TickedQueue {MaxProcessedPerUpdate = 1};

            var aVal = 0;
            var a = new TickedObject((x => aVal++), 0);
            var b = new TickedObject((x => { throw new NotImplementedException("Not implemented method"); }), 0);

            queue.Add(a, true);
            queue.Add(b, true);

            // Verify the queue works as expected
            queue.Update(DateTime.UtcNow.AddSeconds(0.5f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the first update");

            TestDelegate testDelegate = (() => queue.Update(DateTime.UtcNow.AddSeconds(1f)));

            Assert.Throws<NotImplementedException>(testDelegate, "Expected a not-implemented exception");
            Assert.AreEqual(1, aVal, "Invalid aVal after the third update");

            queue.Update(DateTime.UtcNow.AddSeconds(1.5f));
            Assert.AreEqual(2, aVal, "Invalid aVal after the third update");
        }

        [Test]
        public void TestExceptionHandler()
        {
            Exception raised = null;
            ITicked itemException = null;

            var queue = new TickedQueue {MaxProcessedPerUpdate = 1};
            queue.TickExceptionHandler += delegate(Exception e, ITicked t)
            {
                raised = e;
                itemException = t;
            };

            var aVal = 0;
            var a = new TickedObject((x => aVal++), 0);
            var b = new TickedObject((x => { throw new NotImplementedException("HELLO WORLD!"); }), 0);

            queue.Add(a, true);
            queue.Add(b, DateTime.UtcNow.AddMilliseconds(1), true);

            // Verify the queue works as expected
            queue.Update(DateTime.UtcNow.AddSeconds(0.5f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the first update");

            TestDelegate testDelegate = (() => queue.Update(DateTime.UtcNow.AddSeconds(1f)));

            Assert.DoesNotThrow(testDelegate, "Did not expect any exceptions to be thrown");
            Assert.AreEqual(1, aVal, "Invalid aVal after the third update");
            Assert.AreEqual("HELLO WORLD!", raised.Message);
            Assert.IsInstanceOf<NotImplementedException>(raised);
            Assert.AreSame(itemException, b);
        }


        [Test]
        public void TestMaxProcessedPerUpdate()
        {
            var queue = new TickedQueue {MaxProcessedPerUpdate = 1};

            var aVal = 0;
            var a = new TickedObject((x => aVal++));

            var bVal = 0;
            var b = new TickedObject((x => bVal++));

            var cVal = 0;
            var c = new TickedObject((x => cVal++));

            queue.Add(a, true);
            queue.Add(b, true);
            queue.Add(c, true);

            // Verify the queue works as expected
            queue.Update(DateTime.UtcNow.AddSeconds(0.5f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the first update");
            Assert.AreEqual(0, bVal, "Invalid bVal after the first update");
            Assert.AreEqual(0, cVal, "Invalid cVal after the first update");

            queue.Update(DateTime.UtcNow.AddSeconds(1f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the second update");
            Assert.AreEqual(1, bVal, "Invalid bVal after the second update");
            Assert.AreEqual(0, cVal, "Invalid cVal after the second update");

            queue.Update(DateTime.UtcNow.AddSeconds(1.5f));
            Assert.AreEqual(1, aVal, "Invalid aVal after the third update");
            Assert.AreEqual(1, bVal, "Invalid bVal after the third update");
            Assert.AreEqual(1, cVal, "Invalid cVal after the third update");

            queue.MaxProcessedPerUpdate = 2;

            queue.Update(DateTime.UtcNow.AddSeconds(2f));
            Assert.AreEqual(2, aVal, "Invalid aVal after the first update");
            Assert.AreEqual(2, bVal, "Invalid bVal after the first update");
            Assert.AreEqual(1, cVal, "Invalid cVal after the first update");
        }


        [Test]
        public void TestEnumerator()
        {
            var queue = new TickedQueue();
            var a = new TickedObject(Callback3, 0);
            a.TickLength = 1;

            queue.Add(a);

            Assert.IsTrue(queue.Items.Any(), "There should be items on the queue");
            Assert.IsTrue(queue.Items.Contains(a), "Queue should contain the new item");
            Assert.AreEqual(1, queue.Items.Count(), "Queue should contain only one item");

            var b = new TickedObject(Callback3, 0);
            queue.Add(b);

            Assert.IsTrue(queue.Items.Contains(b), "Queue should contain the second item");
            Assert.AreEqual(2, queue.Items.Count(), "Queue should contain two items");

            queue.Remove(a);

            Assert.AreEqual(1, queue.Items.Count(), "Queue should contain only one item again");
            Assert.IsFalse(queue.Items.Contains(a), "Queue should not contain the original item");
        }

        private void CallbackSetTest1(object obj)
        {
            if (obj is int)
            {
                var i = (int) obj;
                if (i != test1 + 1) Assert.Fail("Callbacks called in the wrong order", obj);
                test1 = i;
            }
            else Assert.Fail("Junk sent back in callback", obj);
        }

        private void Callback2(object obj)
        {
            if (obj is int)
            {
                var i = (int) obj;
                switch (i)
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

        private void Callback3(object obj)
        {
            test3 = 1;
        }
    }
}