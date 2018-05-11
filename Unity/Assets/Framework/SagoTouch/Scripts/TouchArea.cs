namespace SagoTouch {
	
	using UnityEngine;
	
	/// <summary>
	/// A component that simplifies the task of observing touches in a defined area.
	/// </summary>
	public class TouchArea : MonoBehaviour, IUnboundSingleTouchObserver {


		#region Serialized Fields
		
		/// <summary>
		/// Gets or sets the camera used to convert touches from screen space to world space.
		/// </summary>
		[SerializeField]
		public Camera Camera;

		[SerializeField]
		private int m_Priority;

		/// <summary>
		/// Gets or sets a value indicating if the touch area should observe 
		/// unbound touch events, which is required in order to detect OnTouchEnter events.
		/// </summary>
		/// <value>
		/// <c>true</c> if the touch area should observe unbound touch events, otherwise <c>false</c>.
		/// </value>
		[SerializeField]
		public bool UseUnboundTouches;

		[SerializeField]
		private bool m_PassTouchesThrough;

		/// <summary>
		/// Gets or sets a value indicating whether the touch area 
		/// should detect when a touch leaves it's bounds.
		/// </summary>
		/// <value>
		/// <c>true</c> if the touch area should detect when a touch leaves it's bounds, otherwise <c>false</c>.
		/// </value>
		[SerializeField]
		public bool DetectTouchExit;

		/// <summary>
		/// Gets or sets a value indicating whether the touch area should ignore 
		/// it's bounds and bind any touch not swallowed by a higher-priority observer.
		/// </summary>
		/// <value>
		/// <c>true</c> if the touch area should bind any touch, otherwise <c>false</c>.
		/// </value>
		[SerializeField]
		public bool TouchAnywhere;
		
		/// <summary>
		/// Gets or sets a value indicating if the touch area should automatically 
		/// calculate it's bounds to match the bounds of any <see cref="UnityEngine.Renderer" /> 
		/// components in the touch area or it's children.
		/// </summary>
		/// <value>
		/// <c>true</c> if the touch area should use the renderer bounds, otherwise <c>false</c>.
		/// </value>
		[SerializeField]
		public bool UseRendererBounds;

		/// <summary>
		/// Gets or sets center of the touch area.
		/// </summary>
		/// <value>
		/// The center of the touch area, in local space.
		/// </value>
		[ContextMenuItem("Match Renderer Bounds", "MatchRendererBounds")]
		[SerializeField]
		public Vector2 Center;

		/// <summary>
		/// Gets or sets extents of the touch area.
		/// </summary>
		/// <value>
		/// The extents of the touch area, in local space.
		/// </value>
		[ContextMenuItem("Match Renderer Bounds", "MatchRendererBounds")]
		[SerializeField]
		public Vector2 Extents;

		#endregion


		#region Inspector Functions

		private void MatchRendererBounds() {

			Bounds bounds;
			bounds = CalculateRendererBounds();

			Vector3 localCenter;
			localCenter = this.Transform.InverseTransformPoint(bounds.center);

			Vector3 localMax;
			localMax = this.Transform.InverseTransformPoint(bounds.max);

			this.Center = localCenter;
			this.Extents = localMax - localCenter;

		}

		#endregion

		
		#region Properties
		
		/// <summary>
		/// Gets or sets the bounds of the touch area.
		/// </summary>
		/// <value>
		/// The bounds of the touch area, in world space.
		/// </value>
		public Bounds Bounds {
			get {
				
				Bounds result;
				result = new Bounds();
				
				if (this.UseRendererBounds) {
					result = CalculateRendererBounds();
				} else {
					result = CalculateBounds(this.Center, this.Extents);
				}
				
				return result;
				
			}

			set {
				this.Center = this.Transform.InverseTransformPoint(value.center);
				this.Extents = (Vector2)this.Transform.InverseTransformPoint(value.max) - this.Center;
			}
		}

		/// <summary>
		/// Gets or sets the bounds of the touch area.
		/// </summary>
		/// <value>
		/// The bounds of the touch area, in local space.
		/// </value>
		public Bounds LocalBounds {
			get {
				Bounds bounds;
				bounds = this.Bounds;
				
				Vector3 localCenter;
				localCenter = this.Transform.InverseTransformPoint(bounds.center);
				
				Vector3 localMax;
				localMax = this.Transform.InverseTransformPoint(bounds.max);
				
				Vector3 localSize;
				localSize = 2 * (localMax - localCenter);
				
				return new Bounds(localCenter, localSize);
			}
			set {
				this.Center = value.center;
				this.Extents = value.extents;
			}
		}
		
		/// <summary>
		/// Gets or sets value indicating whether the touch area should swallow touches.
		/// </summary>
		/// <value>
		/// <c>false</c> if the touch area should swallow touches, <c>true></c> if 
		/// the touch area should NOT swallow touches. The default value is <c>false</c>.
		/// </value>
		public bool PassTouchesThrough {
			get { return m_PassTouchesThrough; }
			set {
				if (m_PassTouchesThrough != value) {
					m_PassTouchesThrough = value;
					OnEnable();
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the priority of the touch area. Higher values 
		/// mean higher priority, lower values mean lower priority. 
		/// The default value is zero.
		/// </summary>
		/// <value>
		/// The priority of the binding.
		/// </value>
		public int Priority {
			get { return m_Priority; }
			set {
				if (m_Priority != value) {
					m_Priority = value;
					OnEnable();
				}
			}
		}
		
		/// <summary>
		/// The Transform component (cached, lazy loaded).
		/// </summary>
		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// Calculates the bounds of all <see cref="UnityEngine.Renderer" /> 
		/// components in the touch area or any of it's children.
		/// </summary>
		public Bounds CalculateRendererBounds() {

			Vector2 min;
			min = new Vector2(Mathf.Infinity, Mathf.Infinity);
			
			Vector2 max;
			max = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
			
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
				
				Bounds bounds;
				bounds = renderer.bounds;

				if (bounds.size == Vector3.zero) {
					bounds.center = renderer.GetComponent<Transform>().position;
				}

				min.x = Mathf.Min(min.x, bounds.min.x);
				min.y = Mathf.Min(min.y, bounds.min.y);
				max.x = Mathf.Max(max.x, bounds.max.x);
				max.y = Mathf.Max(max.y, bounds.max.y);
				
			}
			
			min.x = (min.x != Mathf.Infinity) ? min.x : this.Transform.position.x;
			min.y = (min.y != Mathf.Infinity) ? min.y : this.Transform.position.y;
			max.x = (max.x != Mathf.NegativeInfinity) ? max.x : this.Transform.position.x;
			max.y = (max.y != Mathf.NegativeInfinity) ? max.y : this.Transform.position.y;
			
			Vector3 center;
			center = (Vector3)(min + 0.5f * (max - min)) + this.Transform.position.z * Vector3.forward;

			Vector3 size;
			size = (Vector3)(max - min);

			return new Bounds(center, size);

		}


		/// <summary>
		/// Calculates the AABB in world space from a local center and local extents
		/// </summary>
		public Bounds CalculateBounds(Vector3 localCenter, Vector3 localExtents) {
			
			Vector2[] localPoints;
			localPoints = new Vector2[] {
				this.Center - this.Extents,
				this.Center - new Vector2(this.Extents.x, -this.Extents.y),
				this.Center + this.Extents,
				this.Center + new Vector2(this.Extents.x, -this.Extents.y),
			};

			Vector3 min;
			min = new Vector3(Mathf.Infinity, Mathf.Infinity, this.Transform.position.z);

			Vector3 max;
			max = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, this.Transform.position.z);

			foreach (Vector2 localPoint in localPoints) {

				Vector2 worldPoint;
				worldPoint = this.Transform.TransformPoint(localPoint);

				min.x = Mathf.Min(min.x, worldPoint.x);
				min.y = Mathf.Min(min.y, worldPoint.y);
				max.x = Mathf.Max(max.x, worldPoint.x);
				max.y = Mathf.Max(max.y, worldPoint.y);

			}

			Bounds bounds;
			bounds = new Bounds();
			bounds.SetMinMax(min, max);

			return bounds;

		}

		/// <summary>
		/// Tests whether the point is hitting the <see cref="TouchArea" />.
		/// </summary>
		/// <param name="screenPoint">
		/// The point to test, in screen space.
		/// </param>
		/// <returns>
		/// <c>true</c> if the point is hitting the <see cref="TouchArea" />, 
		/// otherwise <c>false</c>.
		/// </returns>
		public bool HitTest(Touch touch) {
			return HitTest(touch.Position);
		}
		
		virtual public bool HitTest(Vector3 screenPoint) {

			if (!this.isActiveAndEnabled) {
				return false;
			}

			if (this.TouchAnywhere) {
				return true;
			}

			if (!this.Camera) {
				#if UNITY_EDITOR
				Debug.LogWarning(string.Format("{0} is not assigned a Camera component.", this), this);
				#endif
				return false;
			}

			Bounds bounds;
			bounds = this.Bounds;

			screenPoint.z = this.Bounds.center.z - this.Camera.GetComponent<Transform>().position.z;
			
			Vector2 worldPoint;
			worldPoint = this.Camera.ScreenToWorldPoint(screenPoint);

			bounds.center = (Vector2)bounds.center;

			return bounds.Contains(worldPoint);

		}
		
		#endregion
		
		
		#region MonoBehaviour Methods

		private void OnEnable() {
			if (TouchDispatcher.Instance && this.isActiveAndEnabled) {
				TouchDispatcher.Instance.Add(this, this.Priority, !this.PassTouchesThrough);
			}
		}
		
		private void OnDisable() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Remove(this);
			}
		}
		
		private void OnDrawGizmos() {
			if (this.Transform) {
				Bounds bounds;
				bounds = this.Bounds;
				Gizmos.color = (this.enabled ? Color.red : new Color(1f, 0f, 0f, 0.125f));
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}
		
		/// <summary>
		/// Resets the touch area to it's default values.
		/// </summary>
		public void Reset() {
			this.Priority = 0;
			this.PassTouchesThrough = false;
			this.TouchAnywhere = false;
			this.UseRendererBounds = true;
		}
		
		#endregion
		
		
		#region IUnboundSingleTouchObserver
		
		/// <summary cref="ISingleTouchObserver.OnTouchBegan" />
		public bool OnTouchBegan(Touch touch) {
			if (HitTest(touch)) {
				foreach (ITouchAreaObserver observer in GetComponents(typeof(ITouchAreaObserver))) {
					observer.OnTouchDown(this, touch);
				}
				return true;
			}
			return false;
		}
		
		/// <summary cref="ISingleTouchObserver.OnTouchMoved" />
		public void OnTouchMoved(Touch touch) {
			if (this.DetectTouchExit && !HitTest(touch) && HitTest(touch.PreviousPosition)) {
				foreach (ITouchAreaObserver observer in GetComponents(typeof(ITouchAreaObserver))) {
					observer.OnTouchExit(this, touch);
				}
			}
		}
		
		/// <summary cref="ISingleTouchObserver.OnTouchEnded" />
		public void OnTouchEnded(Touch touch) {
			foreach (ITouchAreaObserver observer in GetComponents(typeof(ITouchAreaObserver))) {
				observer.OnTouchUp(this, touch);
			}
		}
		
		/// <summary cref="ISingleTouchObserver.OnTouchCancelled" />
		public void OnTouchCancelled(Touch touch) {
			foreach (ITouchAreaObserver observer in GetComponents(typeof(ITouchAreaObserver))) {
				observer.OnTouchCancelled(this, touch);
			}
		}
		
		/// <summary cref="IUnboundSingleTouchObserver.OnUnboundTouchBegan" />
		public bool OnUnboundTouchBegan(Touch touch) {
			return false;
		}
		
		/// <summary cref="IUnboundSingleTouchObserver.OnUnboundTouchMoved" />
		public bool OnUnboundTouchMoved(Touch touch) {
			if (this.UseUnboundTouches && HitTest(touch)) {
				foreach (ITouchAreaObserver observer in GetComponents(typeof(ITouchAreaObserver))) {
					observer.OnTouchEnter(this, touch);
				}
				return true;
			}
			return false;
		}
		
		/// <summary cref="IUnboundSingleTouchObserver.OnUnboundTouchEnded" />
		public bool OnUnboundTouchEnded(Touch touch) {
			return false;
		}
		
		/// <summary cref="IUnboundSingleTouchObserver.OnUnboundTouchCancelled" />
		public bool OnUnboundTouchCancelled(Touch touch) {
			return false;
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		private Transform m_Transform;
		
		#endregion
		
		
	}
	
}
