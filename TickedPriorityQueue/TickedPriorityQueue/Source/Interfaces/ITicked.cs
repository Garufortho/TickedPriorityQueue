
namespace TickedPriorityQueue
{
	/// <summary>
	/// Inferface used for all items which need to be ticked
	/// </summary>
	public interface ITicked
	{
		/// <summary>
		/// Gets or sets the priority. Lower values used for higher priority
		/// </summary>
		/// <value>
		/// The priority.
		/// </value>
		int Priority { get; set; }
		
		/// <summary>
		/// Gets or sets the length of the tick in seconds.
		/// </summary>
		/// <value>
		/// The length of the tick (Seconds).
		/// </value>
		double TickLength { get; set; }
		
		/// <summary>
		/// Raised when the tick length has elapsed.
		/// </summary>
		void OnTicked();
	}
}