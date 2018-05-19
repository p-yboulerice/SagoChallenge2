namespace Juice {
	using UnityEngine;
	using System.Collections;

	public class TweakerFloatRangeAttribute : TweakerAttribute {


		public float Min;
		public float Max;

		public TweakerFloatRangeAttribute(float min, float max) {
			this.Min = min;
			this.Max = max;
		}
	}
}