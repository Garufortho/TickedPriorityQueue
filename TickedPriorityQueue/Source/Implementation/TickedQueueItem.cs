using System;

namespace TickedPriorityQueue
{
	/// <summary>
	/// Internal class used for storing a reference to a ticked object, and the times involved for ticking it
	/// </summary>
	internal sealed class TickedQueueItem
	{
		private ITicked _ticked;
		private DateTime _nextTickTime;
		
		internal void ResetTickFromTime(DateTime time)
		{
			_nextTickTime = time.AddSeconds(_ticked.TickLength);
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TickedPriorityQueue.TickedQueueItem"/> class.
		/// </summary>
		/// <param name='ticked'>
		/// Object to be ticked.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public TickedQueueItem(ITicked ticked) : this(ticked, DateTime.UtcNow)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TickedPriorityQueue.TickedQueueItem"/> class.
		/// </summary>
		/// <param name='ticked'>
		/// Object to be ticked.
		/// </param>
		/// <param name='currentTime'>
		/// Current time.
		/// </param>
		/// <param name="isLooped">Indicates if this item should be looped</param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to the constructor is invalid because it is <see langword="null" /> .
		/// </exception>
		public TickedQueueItem(ITicked ticked, DateTime currentTime, bool isLooped = true)
		{
            if (ticked == null) throw new ArgumentNullException("Missing a valid ITicked reference");
            _ticked = ticked;
			ResetTickFromTime(currentTime);
			Priority = _ticked.Priority;
			Loop = isLooped;
			IsActive = true;
		}
		
		
		/// <summary>
		/// Checks whether the class is ready to be ticked.
		/// </summary>
		/// <returns>
		/// The tick ready status.
		/// </returns>
		/// <param name='current'>
		/// The current time in <see cref="System.DateTime"/> format.
		/// </param>
		public bool CheckTickReady(DateTime current)
		{
			return (current > _nextTickTime);
		}
		
		/// <summary>
		/// Called when the class is being processed for tick elapsed.
		/// </summary>
		/// <param name='current'>
		/// The current time in <see cref="System.DateTime"/> format.
		/// </param>
		public void Tick(DateTime current)
		{
			ResetTickFromTime(current);
			_ticked.OnTicked();
		}
		
		/// <summary>
		/// Gets the priority associated with the wrapped ticked object.
		/// </summary>
		/// <value>
		/// The priority.
		/// </value>
		public int Priority { get; private set; }
		
		/// <summary>
		/// Gets the tick length associated with the wrapped ticked object.
		/// </summary>
		/// <value>
		/// The minimum length of the tick.
		/// </value>
		public double TickLength
		{
			get { return _ticked.TickLength; }
		}
		
		/// <summary>
		/// Checks if this instance wraps the specified <see cref="TickedPriorityQueue.ITicked"/> object.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the wrapped object is ticked, else <c>false</c>.
		/// </returns>
		/// <param name='ticked'>
		/// The <see cref="TickedPriorityQueue.ITicked"/> object to check.
		/// </param>
		public bool ContainsTicked(ITicked ticked)
		{
			return ticked == _ticked;
		}
		
		/// <summary>
		/// Returns the earliest time the instance can be ticked.
		/// </summary>
		public DateTime NextTickTime
		{
			get { return _nextTickTime; }
		}
		
		/// <summary>
		/// Returns the wrapped <see cref="TickedPriorityQueue.ITicked"/> object.
		/// </summary>
		public ITicked Ticked
		{
			get { return _ticked; }
		}
		
		/// <summary>
		/// Sets whether the instance will be repeatedly ticked, or ticked once
		/// </summary>
		public bool Loop
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		public bool IsActive { get; set; }

	}
}

