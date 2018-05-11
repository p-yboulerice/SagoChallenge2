namespace SagoBiz {
	
	using SagoLayout;
	using SagoTouch;
	using UnityEngine;
	using Touch = SagoTouch.Touch;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// The Button class provides button functionality for the <see cref="Content" /> 
	/// component using the <see cref="SagoTouch.ISingleTouchObserver" /> interface.
	/// </summary>
	public class Button : MonoBehaviour, ISingleTouchObserver {
		
		
		#region Types
		
		/// <summary>
		/// ButtonEvent provides the method signature for <see cref="Button" /> event handlers.
		/// </summary>
		public delegate void ButtonEvent(Button button);
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// Cached reference to the <see cref="Align" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Align m_Align;
		
		/// <summary>
		/// Cached reference to the <see cref="Camera" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Camera m_Camera;
		
		/// <summary>
		/// The priority used when registering with the <see cref="TouchDispatcher" />.
		/// </summary>
		[SerializeField]
		protected int m_Priority;
		
		/// <summary>
		/// Cached reference to the <see cref="Renderer" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Renderer m_Renderer;
		
		/// <summary>
		/// Cached reference to the <see cref="Scale" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Scale m_Scale;
		
		/// <summary>
		/// Cached reference to the <see cref="Transform" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Transform m_Transform;
		
		#endregion
		
		
		#region Events
		
		/// <summary>
		/// Adds and removes event handlers for the OnTap event.
		/// </summary>
		public ButtonEvent OnTap;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets a reference to the <see cref="Align" /> component>
		/// </summary>
		public Align Align {
			get {
				m_Align = m_Align ?? this.GetComponent<Align>();
				return m_Align;
			}
		}
		
		/// <summary>
		/// Gets a reference to the <see cref="Camera" /> component>
		/// </summary>
		public Camera Camera {
			get {
				m_Camera = m_Camera ?? this.GetComponentInParent<Content>().GetComponentInChildren<Camera>();
				return m_Camera;
			}
		}
		
		/// <summary>
		/// Gets and sets the prioirty used when registering with the <see cref="TouchDispatcher" />.
		/// </summary>
		public int Priority {
			get { return m_Priority; }
			set { m_Priority = value; }
		}
		
		/// <summary>
		/// Gets a reference to the <see cref="Renderer" /> component>
		/// </summary>
		public Renderer Renderer {
			get {
				m_Renderer = m_Renderer ?? this.GetComponentInChildren<Renderer>();
				return m_Renderer;
			}
		}
		
		/// <summary>
		/// Gets a reference to the <see cref="Scale" /> component>
		/// </summary>
		public Scale Scale {
			get {
				m_Scale = m_Scale ?? this.GetComponent<Scale>();
				return m_Scale;
			}
		}
		
		/// <summary>
		/// Gets a reference to the <see cref="Transform" /> component>
		/// </summary>
		public Transform Transform {
			get {
				m_Transform = m_Transform ?? this.GetComponent<Transform>();
				return m_Transform;
			}
			
		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		/// <summary>
		/// Adds the button to the <see cref="TouchDispatcher" />.
		/// </summary>
		void OnEnable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Add(this, this.Priority);
			}
		}
		
		/// <summary>
		/// Removes the button from the <see cref="TouchDispatcher" />.
		/// </summary>
		void OnDisable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Remove(this);
			}
		}
		
		/// <summary>
		/// Resets the component to the default values.
		/// </summary>
		void Reset() {
			this.Priority = 100;
		}
		
		#endregion
		
		// #if SAGO_BIZ_DEBUG
		//
		// private Transform m_Quad;
		//
		// void Start() {
		// 	m_Quad = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<Transform>();
		// 	m_Quad.gameObject.layer = gameObject.layer;
		// 	m_Quad.parent = SagoBiz.Controller.Instance.GetComponent<Transform>();
		// }
		//
		// void LateUpdate() {
		// 	Bounds bounds = Align.Bounds;
		// 	m_Quad.position = bounds.center + Vector3.back;
		// 	m_Quad.localScale = bounds.size;
		// }
		//
		// #endif
		
		
		#region ISingleTouchObserver Methods
		
		/// <see cref="ISingleTouchObserver.OnTouchBegan" />
		public bool OnTouchBegan(Touch touch) {
			return this.HitTest(touch);
		}
		
		/// <see cref="ISingleTouchObserver.OnTouchMoved" />
		public void OnTouchMoved(Touch touch) {
			
		}
		
		/// <see cref="ISingleTouchObserver.OnTouchEnded" />
		public void OnTouchEnded(Touch touch) {
			if (touch.IsTap && this.HitTest(touch) && this.OnTap != null) {
				this.OnTap(this);
			}
		}
		
		/// <see cref="ISingleTouchObserver.OnTouchCancelled" />
		public void OnTouchCancelled(Touch touch) {
			
		}
		
		#endregion
		
		
		#region Helper Methods
		
		/// <summary>
		/// Checks whether the touch is hitting the renderer.
		/// </summary>
		bool HitTest(Touch touch) {
			if (this.Camera && this.Renderer) {
				
				Bounds bounds;
				bounds = Align.Bounds;
				
				Vector3 point;
				point = CameraUtils.TouchToWorldPoint(touch, this.Transform, this.Camera);
				
				return bounds.Contains(point);
				
			}
			return false;
		}
		
		#endregion
		
	}
	
}