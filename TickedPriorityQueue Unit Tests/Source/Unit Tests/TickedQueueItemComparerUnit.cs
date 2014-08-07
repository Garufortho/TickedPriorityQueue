using System;
using NUnit.Framework;
using TickedPriorityQueue;

namespace TickedPriorityQueueUnitTests
{
    [TestFixture]
    public class TickedQueueItemComparerUnit
    {
        [Test]
        public void TestPriorityCompare()
        {
            var a = new TickedObject(Callback, 0, 0) {Priority = 5};
            var b = new TickedObject(Callback, 0, 1) {Priority = 2};

            var time = DateTime.UtcNow;
            //forcing the time used so that there isn't any difference in the two calls
            var itemA = new TickedQueueItem(a, time);
            var itemB = new TickedQueueItem(b, time);

            var comparer = new TickedQueueItemComparer();
            Assert.AreEqual(1, comparer.Compare(itemA, itemB), "Comparison should yield lower for a");
            Assert.AreEqual(-1, comparer.Compare(itemB, itemA), "Reverse comparison should yield lower for b");
            a.Priority = b.Priority;

            // For items with equal times and priorities, we will state that the
            // first item is always lower than the second, so that the second is
            // always placed later in the queue
            itemA = new TickedQueueItem(a, time);
            Assert.AreEqual(-1, comparer.Compare(itemA, itemB),
                "Expected ItemA would be deemed lower - {0}  {1}", itemA, itemB);
        }

        [Test]
        public void TestEqualTimeCompare()
        {
            var a = new TickedObject(Callback);
            var b = new TickedObject(Callback);

            var time = DateTime.UtcNow;
            //forcing the time used so that there isn't any difference in the two calls
            var itemA = new TickedQueueItem(a, time);
            var itemB = new TickedQueueItem(b, time);

            // For items with equal times and priorities, we will state that the
            // first item is always lower than the second, so that the second is
            // always placed later in the queue
            var comparer = new TickedQueueItemComparer();
            Assert.AreEqual(-1, comparer.Compare(itemA, itemB), "Comparison should yield lower for a");            
        }

        [Test]
        public void TestTickTimeCompare()
        {
            var a = new TickedObject(Callback, 2, 0) {Priority = 2};
            var b = new TickedObject(Callback, 1, 1) {Priority = 2};

            var itemA = new TickedQueueItem(a);
            var itemB = new TickedQueueItem(b);

            var comparer = new TickedQueueItemComparer();
            Assert.AreEqual(1, comparer.Compare(itemA, itemB), "B should be lower due to lower tick time");
            a.Priority = 1;
            itemA = new TickedQueueItem(a);
            Assert.AreEqual(-1, comparer.Compare(itemA, itemB), "A should be sorted lower due to the priority");
        }

        [Test]
        public void TestTimeCompare()
        {
            var a = new TickedObject(Callback, 2, 0) {Priority = 2};
            var b = new TickedObject(Callback, 1, 1) {Priority = 2};

            var itemA = new TickedQueueItem(a, DateTime.UtcNow);
            var itemB = new TickedQueueItem(b, DateTime.UtcNow.AddSeconds(2));

            var comparer = new TickedQueueItemComparer();
            Assert.AreEqual(-1, comparer.Compare(itemA, itemB), "A should be lower due to earlier tick time");
            b.Priority = 1;
            itemB = new TickedQueueItem(b);
            Assert.AreEqual(1, comparer.Compare(itemA, itemB), "B should be sorted lower due to the priority");
        }

        private void Callback(object obj)
        {
        }
    }
}