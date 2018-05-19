namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	[RequireComponent(typeof(BounceScale))]
	public class ScaleShake : MonoBehaviour {


		#region Fields

		[SerializeField]
		private bool m_Shake;

		[SerializeField]
		private float m_ShakeTimeStep;

		[SerializeField]
		private Vector3 m_Scale_Min = Vector3.one;

		[SerializeField]
		private Vector3 m_Scale_Max = Vector3.one;

		[System.NonSerialized]
		private BounceScale m_BounceScale;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		public Vector3 Scale_Min {
			get { return m_Scale_Min; }
			set { m_Scale_Min = value; }
		}

		public Vector3 Scale_Max {
			get { return m_Scale_Max; }
			set { m_Scale_Min = value; }
		}
		
		public bool Shake {
			get { return m_Shake; }
			set { m_Shake = value; 
				this.BounceScale.enabled = !value;
			}
		}
		
		public  float ShakeTimeStep {
			get { return m_ShakeTimeStep; }
			set { m_ShakeTimeStep = value; }
		}
		
		private BounceScale BounceScale {
			get { return m_BounceScale = m_BounceScale ?? this.GetComponent<BounceScale>(); }
		}

		private float CurrentTime {
			get;
			set;
		}

		#endregion


		#region Methods

		void Update() {
			if (this.Shake) {
				this.CurrentTime += Time.deltaTime;
				if (this.CurrentTime > this.ShakeTimeStep) {
					this.CurrentTime = 0;
					Vector3 newScale = new Vector3();
					newScale.x = Random.Range(this.Scale_Min.x, this.Scale_Max.x);
					newScale.y = Random.Range(this.Scale_Min.y, this.Scale_Max.y); 
					newScale.z = 1;
					this.BounceScale.ForceScale(newScale);
				}
			}
		}

		#endregion


	}
}