namespace SagoDrawing {
	
	using SagoTouch;
	using UnityEngine;
	using Touch = SagoTouch.Touch;
	using System.Collections.Generic;

	/// <summary>
	/// The DrawingController class coordinates between the drawing surface and the drawing tool.
	/// </summary>
	public class DrawingController : MonoBehaviour, ISingleTouchObserver {


		#region Events

		public event System.Action<DrawingController> DrawingBegan;

		public event System.Action<DrawingController> DrawingEnded;

		#endregion


		#region Delegates

		public System.Func<Touch, bool> IsAllowedToDrawWithTouch;

		#endregion


		#region Fields

		[SerializeField]
		private int m_Priority;

		[SerializeField]
		private bool m_SwallowTouches;

		[System.NonSerialized]
		private DrawingSurface m_Surface;

		[System.NonSerialized]
		public IDrawingTool m_Tool;

		[System.NonSerialized]
		private List<Touch> m_Touches;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		/// <summary>
		/// The value to use for priority when registering with the <see cref="TouchDispatcher" />.
		/// </summary>
		public int Priority {
			get { return m_Priority; }
		}

		/// <summary>
		/// The value to use for swallow touches when registering with the <see cref="TouchDispatcher" />.
		/// </summary>
		public bool SwallowTouches {
			get { return m_SwallowTouches; }
		}

		/// <summary>
		/// The drawing surface (cached).
		/// </summary>
		public DrawingSurface Surface {
			get { return m_Surface = m_Surface ?? GetComponentInChildren<DrawingSurface>(); }
		}

		/// <summary>
		/// The drawing tool.
		/// </summary>
		public IDrawingTool Tool {
			get { return m_Tool = m_Tool ?? GetComponentInChildren<IDrawingTool>(); }
			set { m_Tool = value; }
		}

		public List<Touch> Touches {
			get { return m_Touches = m_Touches ?? new List<Touch>(); }
		}

		/// <summary>
		/// The transform (cached).
		/// </summary>
		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region MonoBehaviour

		private void OnDisable() {

			if (this.Touches != null && this.Touches.Count > 0) {
				foreach (Touch touch in this.Touches.ToArray()) {
					this.OnTouchCancelled(touch);
				}
			}

			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Remove(this);
			}
		}

		private void OnEnable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Add(this, Priority, SwallowTouches);
			}
		}

		private void OnRenderObject() {
			if (Surface != null && Tool != null) {
				Tool.OnRender(Surface);
			}
		}

		private void Reset() {
			m_Priority = 0;
			m_SwallowTouches = true;
			m_Tool = null;
		}

		#endregion


		#region ISingleTouchObserver

		/// <summary>
		/// <see cref="ISingleTouchObserver.OnTouchBegan" />
		/// </summary>
		public bool OnTouchBegan(Touch touch) {
			OnTouchMoved(touch);
			return true;
		}

		/// <summary>
		/// <see cref="ISingleTouchObserver.OnTouchMoved" />
		/// </summary>
		public void OnTouchMoved(Touch touch) {
			if (Touches.Contains(touch)) {
				if (Surface != null && Tool != null) {
					Tool.OnTouchMoved(Surface, touch);
				}
			}
			else if (IsAllowedToDrawWithTouch == null || IsAllowedToDrawWithTouch(touch)) {
				if (Surface != null && Tool != null) {
					Touches.Add(touch);
					if (Touches.Count == 1 && DrawingBegan != null) {
						DrawingBegan(this);
					}
					Tool.OnTouchBegan(Surface, touch);
				}
			}
		}

		/// <summary>
		/// <see cref="ISingleTouchObserver.OnTouchEnded" />
		/// </summary>
		public void OnTouchEnded(Touch touch) {
			if (Touches.Contains(touch)) {
				if (Surface != null && Tool != null) {
					Tool.OnTouchEnded(Surface, touch);
				}
			}
			Touches.Remove(touch);
			if (Touches.Count == 0 && DrawingEnded != null) {
				DrawingEnded(this);
			}
		}

		/// <summary>
		/// <see cref="ISingleTouchObserver.OnTouchCancelled" />
		/// </summary>
		public void OnTouchCancelled(Touch touch) {
			OnTouchEnded(touch);
		}

		#endregion


	}
	
}