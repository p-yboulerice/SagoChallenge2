namespace SagoApp {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using SagoLayout;
	using SagoTouch;
	using SagoNavigation;
	using Touch = SagoTouch.Touch;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// Quit button for any Android platforms that requires a mechanism that quits and clears running app from memory stack.
	/// </summary>
	public class QuitButton : MonoBehaviour, ISingleTouchObserver {


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
				m_Camera = m_Camera ?? this.GetComponentInParent<Artboard>().GetComponentInChildren<Camera>();
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
		/// Gets a reference to the <see cref="Transform" /> component>
		/// </summary>
		public Transform Transform {
			get {
				m_Transform = m_Transform ?? this.GetComponent<Transform>();
				return m_Transform;
			}

		}

		/// <summary>
		/// Gets a reference to the <see cref="BounceScaleBehaviour" /> component>
		/// </summary>
		/// <value>The bounce scale behaviour.</value>
		public BounceScaleBehaviour BounceScaleBehaviour {
			get {
				m_BounceScaleBehaviour = m_BounceScaleBehaviour ?? this.GetComponentInChildren<BounceScaleBehaviour>();
				return m_BounceScaleBehaviour;
			}
		}

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
		/// Cached reference to the <see cref="Transform" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Transform m_Transform;

		/// <summary>
		/// Cached reference to the <see cref="BounceScaleBehaviour" /> component.
		/// </summary>
		[System.NonSerialized]
		protected BounceScaleBehaviour m_BounceScaleBehaviour;

		/// <summary>
		/// Time to wait before actually quitting.
		/// This time can be synchronized to an animation behaviour on this button.
		/// </summary>
		[SerializeField]
		protected float m_TimeToWaitBeforeQuit;

		#endregion


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
			if (touch.IsTap && this.HitTest(touch)) {
				// Quitting out of game so that it is not left in memory stack.
				// This is for any Android derived OS that does not have back button or home button that users can interact with.
				// Right now doing this only for PanasonicEx platform but we can add more platforms to the macro.
				#if SAGO_PANASONIC_EX
					BounceScaleBehaviour.Trigger();
					StartCoroutine(WaitAndQuit(m_TimeToWaitBeforeQuit));
				#endif
			}
		}

		/// <see cref="ISingleTouchObserver.OnTouchCancelled" />
		public void OnTouchCancelled(Touch touch) {

		}

		#endregion


		#region Coroutines

		IEnumerator WaitAndQuit(float secondsToWait) {
			yield return new WaitForSeconds(secondsToWait);
			Application.Quit();
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


		#region Static Methods

		public static void Init(SceneController sceneController = null) {
			// Automatically instantiating a quit button for any Android derived platform that requires this mechanism.
			// Right now doing this only for PanasonicEx platform but we can add more platforms to the macro.
			#if SAGO_PANASONIC_EX
				GameObject quitButtonResource = Resources.Load("SagoApp/QuitButton") as GameObject;
				if (quitButtonResource) {
					GameObject quitButtonObject = Instantiate(quitButtonResource) as GameObject;
					QuitButton quitButton = quitButtonObject.GetComponentInChildren<QuitButton>();
					if (quitButton) {
						Debug.Log("QuitButton-> Successfully loaded and instantiated QuitButton.");
						if (sceneController != null) {
							quitButtonObject.transform.parent = sceneController.transform;
						}
					}
				}
			#endif
		}

		#endregion

	}

}
