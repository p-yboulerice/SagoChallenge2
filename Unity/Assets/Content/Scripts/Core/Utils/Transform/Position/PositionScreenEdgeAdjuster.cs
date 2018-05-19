namespace Juice.Utils {
	
	using UnityEngine;
	using System.Collections;

	public enum ScreenEdge {
		Left,
		Right,
		Up,
		Down
	}

	public class PositionScreenEdgeAdjuster : MonoBehaviour {

		#region Fields

		[SerializeField]
		private ScreenEdge m_Edge;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private Transform m_RootParent;

		[System.NonSerialized]
		private Camera m_Camera;

		#endregion


		#region Properties

		public Transform Transform {
			get { 
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		private Transform RootParent {
			get { 
				if (m_RootParent == null) {
					m_RootParent = Transform;
					while (m_RootParent.parent != null) {
						m_RootParent = m_RootParent.parent;
					}
				}
				return m_RootParent;
			}
		}

		private ScreenEdge Edge {
			get { return m_Edge; }
		}

		private Camera Camera {
			get { return m_Camera = m_Camera ?? FindObjectOfType<GameCamera>().Camera; }
		}

		private Vector3 OriginalPosition {
			get;
			set;
		}

		#endregion


		#region Methods

		void Start() {
			AdjustPosition();
		}

		public void AdjustPosition() {
			OriginalPosition = Transform.position;
			Vector3 offset = OriginalPosition - RootParent.position;
			Vector3 rootScreenPosition = Camera.WorldToScreenPoint(RootParent.position);
			Vector3 worldPos = new Vector3();

			switch (Edge) {
			case ScreenEdge.Up:
				worldPos = Camera.ScreenToWorldPoint(new Vector3(rootScreenPosition.x, Screen.height, 0));
				offset.y = worldPos.y - RootParent.position.y;
				break;
			case ScreenEdge.Down:
				worldPos = Camera.ScreenToWorldPoint(new Vector3(rootScreenPosition.x, 0, 0));
				offset.y = worldPos.y - RootParent.position.y;
				break;
			case ScreenEdge.Left:
				worldPos = Camera.ScreenToWorldPoint(new Vector3(0, rootScreenPosition.y, 0));
				offset.x = worldPos.x - RootParent.position.x;
				break;
			case ScreenEdge.Right:
				worldPos = Camera.ScreenToWorldPoint(new Vector3(Screen.width, rootScreenPosition.y, 0));
				offset.x = worldPos.x - RootParent.position.x;
				break;
			}

			Transform.position = RootParent.position + offset;
		}

		#endregion


	}
	
}