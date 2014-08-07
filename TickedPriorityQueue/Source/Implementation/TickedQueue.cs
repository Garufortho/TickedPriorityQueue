using System;
using System.Collections.Generic;
using System.Linq;

namespace TickedPriorityQueue
{
	
	/// <summary>
	/// A class which manages ITicked objects, ticking them in order of priority.
	/// </summary>
	/// <remarks>
	/// Will never tick an item more than once in a frame, and sets the updated tick
	/// time to the sum of processed time and the object's Tick Length.
	/// And and Update can use a user provided DateTime for the current time, allowing for custom timing, e.g. for pausing the game.
	/// </remarks>
	public sealed class TickedQueue
	{
		public static readonly int PreAllocateSize = 100;
	
		/// <summary>
		/// Default max ITicked objects to be processed per update.
		/// </summary>
		public readonly int DefaultMaxProcessedPerUpdate = 10;
		
		public readonly float DefaultMaxProcessingTimePerUpdate = 0.1f;
		
		/// <summary>
		/// Sets whether new items added are looped or not.
		/// Overriden by setting the loop mode in Add.
		/// </summary>
		public bool LoopByDefault { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this queue instance is paused.
		/// Paused queues will ignore Update calls.
		/// </summary>
		/// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
		public bool IsPaused { get; set; }
		
		/// <summary>
		/// The queue.
		/// </summary>
		private List<TickedQueueItem> _queue;
		
		/// <summary>
		/// Pre-allocated working queue from which items will be evaluated.
		/// </summary>
		private List<TickedQueueItem> _workingQueue;

		/// <summary>
		/// Gets or sets the exception handler.
		/// </summary>
		/// <value>The exception handler.</value>
		/// <remarks>If the queue has an exception handler, any exceptions caught
		/// will be sent to the handler - otherwise they are thrown</remarks>
		public Action<Exception, ITicked> TickExceptionHandler { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TickedPriorityQueue.TickedQueue"/> class.
		/// </summary>
		public TickedQueue()
		{
			LoopByDefault = true;
			_queue = new List<TickedQueueItem>(PreAllocateSize);
			_workingQueue = new List<TickedQueueItem>(PreAllocateSize);
			MaxProcessedPerUpdate = DefaultMaxProcessedPerUpdate;
			_maxProcessingTimePerUpdate = TimeSpan.FromSeconds(DefaultMaxProcessingTimePerUpdate);
		}
		
		/// <summary>
		/// Gets or sets the max ITicked objects to be processed in a single Update call.
		/// </summary>
		public int MaxProcessedPerUpdate { get; set; }
		
		private TimeSpan _maxProcessingTimePerUpdate;
		
		/// <summary>
		/// Gets or sets the max time allowed for processing ITicked objects in a single Update call.
		/// Note - this is in real time, setting custom update times will not affect it.
		/// </summary>
		public float MaxProcessingTimePerUpdate
		{ 
			get { return (float)_maxProcessingTimePerUpdate.TotalSeconds; }
			set { _maxProcessingTimePerUpdate = TimeSpan.FromSeconds(value); }
		}
		
		/// <summary>
		/// Gets the internal queue count.
		/// </summary>
		public int QueueCount { get { return _queue.Count; } }

		/// <summary>
		/// Returns an IEnumerable for the ticked items on the queue
		/// </summary>
		/// <value>The ticked items.</value>
		public IEnumerable<ITicked> Items 
		{ 
			get 
			{ 
				return _queue.Select(x => x.Ticked); 
			} 
		}
		
		/// <summary>
		/// Add the specified ticked object to the queue.
		/// </summary>
		/// <param name='ticked'>
		/// The ITicked object.
		/// </param>
		public void Add(ITicked ticked)
		{
			Add(ticked, DateTime.UtcNow, LoopByDefault);
		}
		
		/// <summary>
		/// Add the specified ticked object to the queue.
		/// </summary>
		/// <param name='ticked'>
		/// The ITicked object.
		/// </param>
		/// <param name='looped'>
		/// Sets whether the ticked item will be called once, or looped.
		/// </param>
		public void Add(ITicked ticked, bool looped)
		{
			Add(ticked, DateTime.UtcNow, looped);
		}
		
		/// <summary>
		/// Add the specified ticked object to the queue, using currentTime as the time to use for the tick check.
		/// </summary>
		/// <param name='ticked'>
		/// The ITicked object.
		/// </param>
		/// <param name='currentTime'>
		/// Current time. Doesn't have to be the real time.
		/// </param>
		public void Add(ITicked ticked, DateTime currentTime)
		{
			Add(ticked, currentTime, LoopByDefault);
		}
		
		/// <summary>
		/// Add the specified ticked object to the queue, using currentTime as the time to use for the tick check.
		/// </summary>
		/// <param name='ticked'>
		/// The ITicked object.
		/// </param>
		/// <param name='currentTime'>
		/// Current time. Doesn't have to be the real time.
		/// </param>
		/// <param name='looped'>
		/// Sets whether the ticked item will be called once, or looped.
		/// </param>
		public void Add(ITicked ticked, DateTime currentTime, bool looped)
		{
			var item = new TickedQueueItem(ticked, currentTime, looped);
			Add(item, currentTime);
		}
		
		/// <summary>
		/// Add the specified item and currentTime.
		/// </summary>
		/// <param name='item'>
		/// The TickedQueueItem element to add to the list.
		/// </param>
		/// <param name='currentTime'>
		/// Current time. Doesn't have to be the real time.
		/// </param>
		/// <remarks>
		/// Notice that unlike the two public methods that receive an ITicked, 
		/// this one expects a TickedQueueItem.  It was done to avoid having to
		/// discard a TickedQueueItem instance every time that a looped item is
		/// ticked and re-added to the queue.  As such, it expects to already 
		/// have been configured for if to loop or not.
		/// </remarks>
		private void Add(TickedQueueItem item, DateTime currentTime)
		{
			item.ResetTickFromTime(currentTime);
			int index = _queue.BinarySearch(item, new TickedQueueItemComparer());
			
			//if the binary search doesn't find something identical, it'll return a
			//negative value signifying where the new item should reside, so bitflipping
			//that gives the new index
			if (index < 0) index = ~index;
			_queue.Insert(index, item);
		}
		
		/// <summary>
		/// Remove the specified ticked object from the queue.
		/// Will only remove the same object once, even if multiple instances exist.
		/// </summary>
		/// <param name='ticked'>
		/// The ITicked object to remove.
		/// </param>
		/// <returns>True if the item was successfully removed, false otherwise</returns>
		public bool Remove(ITicked ticked)
		{
			bool found = false;
			foreach(var item in _queue)
			{
				if (item.ContainsTicked(ticked))
				{
					// In case the item is added to a work queue
					item.IsActive = false;	
					found = _queue.Remove(item);
					break;
				}
			}
			return found;
		}
		
		/// <summary>
		/// Updates the queue, calling OnTicked for the first MaxProcessedPerUpdate items which have timed out.
		/// </summary>
		public void Update()
		{
			Update(DateTime.UtcNow);
		}
		
		/// <summary>
		/// Updates the queue, calling OnTicked for the first MaxProcessedPerUpdate items which have timed out.
		/// Uses a user provided DateTime for the current time, allowing for custom timing, e.g. for pausing the game.
		/// </summary>
		/// <param name='currentTime'>
		/// Current time to use.
		/// </param>
		public void Update(DateTime currentTime)
		{
			if (IsPaused) return;

			int found = 1;
			DateTime startTime = DateTime.UtcNow;
						
			_workingQueue.Clear();
			_workingQueue.AddRange(_queue);
			
			for (var i = 0; i < _workingQueue.Count; i++)
			{
				if (found > MaxProcessedPerUpdate) break;
				
				var item  = _workingQueue[i];
				if (item.IsActive && item.CheckTickReady(currentTime))
				{
					++found;
					_queue.Remove(item);
					if (item.Loop)
					{
						Add(item, currentTime);
					}
					try
					{
						item.Tick(currentTime);
					}
					catch (Exception e)
					{
						if (TickExceptionHandler != null)
						{
							TickExceptionHandler(e, item.Ticked);
						}
						else
						{
							throw;
						}
					}
				}
				
				if (DateTime.UtcNow - startTime > _maxProcessingTimePerUpdate)
				{
					break;
				}
			}
		}
		
	}
}

