using System;

namespace TickedPriorityQueue
{
	/// <summary>
	/// Abstract implementation of ITicked, implements the properties, leaves OnTicked abstract
	/// </summary>
	public abstract class ATicked : ITicked
	{
		/// <summary>
		/// Constant default priority, used to set Priortity in the constructor.
		/// </summary>
		public readonly int DefaultPriority = 1;
		
		/// <summary>
		/// Constant default tick length, used to set TickLength in the constructor.
		/// </summary>
		public readonly double DefaultTickLength = 0.25;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TickedPriorityQueue.ATicked"/> class.
		/// </summary>
		public ATicked()
		{
			Priority = DefaultPriority;
			TickLength = DefaultTickLength;
		}
		
		/// <summary>
		/// Gets or sets the priority. Lower values used for higher priority
		/// </summary>
		/// <value>
		/// The priority.
		/// </value>
		public virtual int Priority { get; set; }
		
		/// <summary>
		/// Gets or sets the length of the tick in seconds.
		/// </summary>
		/// <value>
		/// The length of the tick (Seconds).
		/// </value>
		public virtual double TickLength { get; set; }
		
		/// <summary>
		/// Raised when the tick length has elapsed.
		/// </summary>
		public abstract void OnTicked();
	}
}