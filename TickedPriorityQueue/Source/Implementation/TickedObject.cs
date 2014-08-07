using System;

namespace TickedPriorityQueue
{
	public delegate void OnTickElapsed(object userData);
	
	/// <summary>
	/// Default implementation of ATicked, provides an event for when the tick is elapsed.
	/// </summary>
	public class TickedObject : ATicked
	{
		/// <summary>
		/// Occurs when a tick has elapsed.
		/// </summary>
		public event OnTickElapsed TickElapsed;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TickedPriorityQueue.TickedObject"/> class.
		/// </summary>
		/// <param name='elapsedCallback'>
		/// Elapsed callback. Delegate declared as void OnTickElapsed(object userData).
		/// </param>
        /// <param name="tickLength">Tick length</param>
        /// <param name='userData'>
		/// Data which is given back through the callback, used for identification purposes.
		/// </param>
		public TickedObject(OnTickElapsed elapsedCallback, float tickLength = 0, object userData = null)
		{
			TickElapsed = elapsedCallback;
            TickLength = tickLength;
            UserData = userData;
		}
		
		/// <summary>
		/// Gets or sets the user data which is given back through the callback, used for identification purposes.
		/// </summary>
		/// <value>
		/// The user data.
		/// </value>
		public object UserData { get; set; }
		
		/// <summary>
		/// Called internally when the ticked event is being raised.
		/// This function should not be called normally.
		/// </summary>
		public override void OnTicked()
		{
			if (TickElapsed != null)
				TickElapsed(UserData);
		}
	}
}

