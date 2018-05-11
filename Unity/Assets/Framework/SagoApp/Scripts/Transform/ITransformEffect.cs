namespace SagoApp {

	using UnityEngine;

	public interface ITransformEffect {

		Vector3 LocalPositionOffset { get; }
		Vector3 LocalScaleFactor { get; }

	}

}