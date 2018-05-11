namespace SagoUtils {
	
	using System.Reflection;
	using UnityEngine;
	
	/// <summary>
	/// Allows you to serialize an integer, but choose bits from
	/// within it.
	/// </summary>
	public class BitMaskAttribute : PropertyAttribute {

		public enum Depth {
			StencilBuffer = 8,
			Eight = 8,
			Sixteen = 16,
			ThirtyTwo = 32
		}

		public int BitDepth {
			get;
			protected set;
		}

		public BitMaskAttribute(Depth bitDepth) : this((int)bitDepth) {
			
		}

		public BitMaskAttribute(int bitDepth) {
			this.BitDepth = Mathf.Clamp(bitDepth, 1, 32);
		}

	}
	
}
