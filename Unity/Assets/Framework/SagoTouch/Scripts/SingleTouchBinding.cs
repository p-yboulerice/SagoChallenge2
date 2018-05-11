namespace SagoTouch {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// Represents the connection between one or more touches and an observer in a <see cref="SingleTouchHandler" /> object.
	/// </summary>
	public class SingleTouchBinding {
		
		
		#region Static Properties
		
		public static int UniqueIdentifier {
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
		public static int Compare(SingleTouchBinding binding, SingleTouchBinding other) {
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
		/// <value>The observer associated with the binding.</value>
		public ISingleTouchObserver Observer {
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
		
		/// <summary>
		/// Gets or sets the value indicating whether the binding should swallow touches. 
		/// If the value is <c>true</c> and the observer's <see cref="ISingleTouchObserver.OnTouchBegan" /> 
		/// method returns <c>true</c>, other observers with lower priority will not receive any 
		/// events for the touch.
		/// </summary>
		/// <value><c>true</c> if the binding should swallow touches, otherwise <c>false</c>.</value>
		public bool SwallowsTouches {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the list of touches bound to the observer.
		/// </summary>
		/// <value>The list of touches bound to the observer.</value>
		public List<Touch> Touches {
			get; set;
		}
		
		#endregion
		
		
		#region Constructors
		
		/// <summary>
		/// Creates a new SingleTouchBinding object.
		/// </summary>
		public SingleTouchBinding() {
			this.Identifier = SingleTouchBinding.UniqueIdentifier++;
			this.Priority = 0;
			this.SwallowsTouches = true;
			this.Touches = new List<Touch>();
		}
		
		#endregion
		
		
	}
	
	
}