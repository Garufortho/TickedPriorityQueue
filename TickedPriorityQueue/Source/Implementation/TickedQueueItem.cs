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
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to the constructor is invalid because it is <see langword="null" /> .
		/// </exception>
		public TickedQueueItem(ITicked ticked, DateTime currentTime)
		{
			_ticked = ticked;
			if (_ticked == null) throw new ArgumentNullException("Missing a valid ITicked reference");
			_nextTickTime = currentTime.AddSeconds(_ticked.TickLength);
			Priority = _ticked.Priority;
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
			_nextTickTime = current.AddSeconds(_ticked.TickLength);
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
		/// Checks if this instance wraps the specified ITicked object.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the wrapped object is ticked, else <c>false</c>.
		/// </returns>
		/// <param name='ticked'>
		/// The ITicked object to check.
		/// </param>
		public bool ContainsTicked(ITicked ticked)
		{
			return ticked == _ticked;
		}
	}
}

