namespace Juice {
	using UnityEngine;
	using System.Collections;

	public class TweakerIntRangeAttribute : TweakerAttribute {
		public int Min;
		public int Max;

		public TweakerIntRangeAttribute(int min, int max) {
			this.Min = min;
			this.Max = max;
		}

	}
}