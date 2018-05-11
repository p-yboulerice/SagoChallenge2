namespace SagoUtils {
	
	using UnityEngine;
	
	/// <summary>
	/// Apply to a Vector2 property to make a nice slider that
	/// keeps the [0] value less than or equal to the [1] value.
	/// Using integer values for the min/max will clamp all
	/// values to integers.
	/// </summary>
	public class MinMaxAttribute : PropertyAttribute {


		#region Properties

		public bool IsInteger {
			get;
			protected set;
		}

		public Vector2 Range {
			get;
			protected set;
		}

		#endregion


		#region Constructor

		public MinMaxAttribute(float min, float max) {
			if (max < min) {
				float tmp = min;
				min = max;
				max = tmp;
			}
			this.Range = new Vector2(min, max);
		}

		public MinMaxAttribute(int min, int max) : this((float)min, (float)max) {
			this.IsInteger = true;
		}

		#endregion

		
	}
	
}
