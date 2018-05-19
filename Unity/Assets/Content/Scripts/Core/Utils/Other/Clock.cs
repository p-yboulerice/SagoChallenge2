namespace Juice.Utils {
	
	using System;
	using UnityEngine;
	
	/// <summary>
	/// Simple clock
	/// </summary>
	public class Clock : MonoBehaviour {
		
		
		#region Serialized Fields
		
		[SerializeField]
		protected GameObject m_HourHandObject;
		
		[SerializeField]
		protected GameObject m_MinuteHandObject;
		
		[SerializeField]
		protected string m_SpecificTime;
		
		#endregion
		
		
		#region Public Properties
		
		virtual public Transform HourHand {
			get {
				if (!m_HourHand && m_HourHandObject) {
					m_HourHand = m_HourHandObject.GetComponent<Transform>();
				}
				return m_HourHand;
			}
		}
		
		virtual public Transform MinuteHand {
			get {
				if (!m_MinuteHand && m_MinuteHandObject) {
					m_MinuteHand = m_MinuteHandObject.GetComponent<Transform>();
				}
				return m_MinuteHand;
			}
		}
		
		virtual public string SpecificTime {
			get {
				return m_SpecificTime;
			}
			set {
				m_SpecificTime = value;
			}
		}
		
		/// <summary>
		/// The Transform component (lazy loaded, cached).
		/// </summary>
		virtual public Transform Transform {
			get {
				m_Transform = m_Transform ?? this.GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		/// <summary>
		/// Reset to default values.
		/// </summary>
		virtual public void Reset() {
			m_SpecificTime = "";
		}
		
		virtual protected void Update() {
			if (string.IsNullOrEmpty(m_SpecificTime)) {
				UpdateHands(DateTime.Now);
			} else {
				UpdateHands(DateTime.Parse(m_SpecificTime));
			}
		}
		
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		protected Transform m_HourHand;
		
		[System.NonSerialized]
		protected Transform m_MinuteHand;
		
		[System.NonSerialized]
		protected Transform m_Transform;
		
		#endregion
		
		
		#region Internal Methods
		
		protected void UpdateHands(DateTime time) {
			float minute = (float)time.Minute + (float)time.Second / 60.0f;
			float hour = Mathf.Repeat((float)time.Hour + minute / 60.0f, 12.0f);
			this.HourHand.localEulerAngles = new Vector3(0.0f, 0.0f, hour * -30.0f);
			this.MinuteHand.localEulerAngles = new Vector3(0.0f, 0.0f, minute * -6.0f);
		}
		
		#endregion
		
	}
	
}
