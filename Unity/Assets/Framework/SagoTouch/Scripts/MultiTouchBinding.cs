namespace SagoTouch {
	
	using UnityEngine;
	
	/// <summary>
	/// Represents the connection between one or more touches and an observer in a <see cref="MultiTouchHandler" /> object.
	/// </summary>
	public class MultiTouchBinding {
		
		
		#region Static Properties
		
		private static int UniqueIdentifier {
			get; set;
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Compares two bindings and returns a value indicating whether 
		/// one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="binding">The first binding to compare.</param>
		/// <param name="other">The second binding to compare.</param>
		/// <returns>A signed integer that indicates the relative values of binding and other.</returns>
		public static int Compare(MultiTouchBinding binding, MultiTouchBinding other) {
			int result = 0;
			if (result == 0) {
				result = binding.Priority.CompareTo(other.Priority) * -1;
			}
			if (result == 0) {
				result = binding.Identifier.CompareTo(other.Identifier);
			}
			return result;
		}
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets or sets the unique identifier for the binding.
		/// </summary>
		/// <value>The unique identifier for the binding.</value>
		public int Identifier {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the obverver associated with the binding.
		/// </summary>
		/// <value>The obverver associated with the binding.</value>
		public IMultiTouchObserver Observer {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the priority of the binding. Higher values 
		/// mean higher priority, lower values mean lower priority. 
		/// The default value is zero.
		/// </summary>
		/// <value>The priority of the binding.</value>
		public int Priority {
			get; set;
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Creates a new MultiTouchBinding object.
		/// </summary>
		public MultiTouchBinding() {
			this.Identifier = MultiTouchBinding.UniqueIdentifier++;
			this.Priority = 0;
		}
		
		#endregion
		
		
	}
	
}