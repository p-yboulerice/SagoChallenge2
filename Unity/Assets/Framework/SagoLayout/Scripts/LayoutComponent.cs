namespace SagoLayout {

	using UnityEngine;
	
	abstract public class LayoutComponent : MonoBehaviour {
		
		
		//
		// Inspector Properties
		//
		public bool ApplyOnAwake;
		public bool ApplyOnStart;
		public bool ApplyOnUpdate;
		public bool ShowOptionsInEditor;


		//
		// Properties
		//
		public Transform Transform {
			get {
				m_Transform = m_Transform ? m_Transform : transform;
				return m_Transform;
			}
		}


		//
		// Public Methods
		//
		virtual public void Apply() {
		}

		
		//
		// Member Variables
		//
		private Transform m_Transform;

		
		//
		// MonoBehaviour
		//
		virtual protected void Reset() {
			this.ApplyOnAwake = true;
			this.ApplyOnStart = false;
			this.ApplyOnUpdate = false;
			this.ShowOptionsInEditor = false;
		}

		virtual protected void OnDrawGizmos() {
		}

		virtual protected void Awake() {
			if (this.enabled && this.ApplyOnAwake) Apply();
		}

		virtual protected void Start() {
			if (this.ApplyOnStart) Apply();
		}
		
		virtual protected void Update() {
			if (this.ApplyOnUpdate) Apply();
		}

		
	}
	
}
