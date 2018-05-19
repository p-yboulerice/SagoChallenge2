namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using Juice.Utils;


	public class SF_BoundsOutOfView : StateFlag {


		#region Types

		[System.Serializable]
		public enum OutOfViewDirectionMode {
			Horizontal,
			Vertical,
			Any,
			Down,
			HorizontalOrDown
		}

		[System.Serializable]
		public enum MultiMode {
			All,
			Any
		}

		#endregion


		#region Serialized Fields

		[SerializeField]
		private bool m_Invert;

		[SerializeField]
		private MultiMode m_MultipleMode;

		[SerializeField]
		private OutOfViewDirectionMode m_OutOfViewDirectionMode;

		[SerializeField]
		private float m_Tolerance = 0f;

		[SerializeField]
		private MeshRenderer[] m_Targets;

		[System.NonSerialized]
		private GameCamera m_GameCamera;

		#endregion


		#region Properties

		private MeshRenderer[] Targets {
			get { return m_Targets; }
		}

		private Camera Camera {
			get { return this.GameCamera ? this.GameCamera.Camera : null; }
		}

		private GameCamera GameCamera {
			get { return m_GameCamera = m_GameCamera ?? FindObjectOfType<GameCamera>(); }
		}

		private MultiMode MultipleMode {
			get { return m_MultipleMode; }
		}

		private float Tolerance {
			get { return m_Tolerance; }
			set { m_Tolerance = value; }
		}

		public override bool IsActive {
			get {
				bool result = true;
				if (this.Camera && this.Targets != null) {
					if (this.MultipleMode == MultiMode.Any) {
						result = false;	
					}
					for (int i = 0; i < this.Targets.Length; ++i) {
						var target = this.Targets[i];
						if (target) {
							bool outOfView = IsOutOfView(target);
							if (outOfView) {
								if (this.MultipleMode == MultiMode.Any) {
									result = true;
									break;
								}
							} else if (this.MultipleMode == MultiMode.All) {
								result = false;
								break;
							}
						}
					}
				}

				if (m_Invert) {
					result = !result;
				}

				return result;
			}
		}

		private OutOfViewDirectionMode OutOfViewDirection {
			get { return m_OutOfViewDirectionMode; }
		}

		private bool IsOutOfView(MeshRenderer rend) {

			Bounds bounds = rend.bounds;
			Vector3 posMin = this.Camera.WorldToViewportPoint(bounds.min - (Vector3)(this.Tolerance * Vector2.one));
			Vector3 posMax = this.Camera.WorldToViewportPoint(bounds.max + (Vector3)(this.Tolerance * Vector2.one));

			switch (this.OutOfViewDirection) {
			case OutOfViewDirectionMode.Horizontal:
				return posMin.x > 1f || posMax.x < 0f;
			case OutOfViewDirectionMode.Vertical:
				return posMin.y > 1f || posMax.y < 0f;
			case OutOfViewDirectionMode.Any:	
				return posMin.x > 1f || posMax.x < 0f || posMin.y > 1f || posMax.y < 0f;
			case OutOfViewDirectionMode.Down:
				return posMax.y < 0f;
			case OutOfViewDirectionMode.HorizontalOrDown:
				return posMax.y < 0f || posMin.x > 1f || posMax.x < 0f;
			default:
				return true;
			}
		}

		#endregion


	}

}