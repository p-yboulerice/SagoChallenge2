namespace SagoAudio {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	[System.Serializable]
	public struct AudioAnimationElement {
		
		
		#region Fields
		
		[SerializeField]
		public AudioClip AudioClip;
		
		[SerializeField]
		public int FrameIndex;
		
		[HideInInspector]
		[SerializeField]
		public string GUID;
		
		[SerializeField]
		public bool IsLoop;
		
		[Range(0.1f,2f)]
		[SerializeField]
		public float Pitch;
		
		[Range(0,1)]
		[SerializeField]
		public float Volume;
		
		#endregion
		
		
	}
	
	public class AudioAnimation : ScriptableObject {
		
		
		#region Fields
		
		[SerializeField]
		protected AudioAnimationElement[] m_Elements;
		
		[System.NonSerialized]
		protected Dictionary<int, List<AudioAnimationElement>> m_ElementsAt;
		
		[Range(1,600)]
		[SerializeField]
		protected int m_FrameCount;
		
		[Range(1,60)]
		[SerializeField]
		protected int m_FramesPerSecond;
		
		#endregion
		
		
		#region Properties
		
		public AudioAnimationElement[] Elements {
			get { return m_Elements; }
			set {
				m_Elements = value;
				m_ElementsAt = null;
			}
		}
		
		public int FrameCount {
			get { return m_FrameCount; }
			set { m_FrameCount = Mathf.Max(value, 1); }
		}
		
		public int FramesPerSecond {
			get { return m_FramesPerSecond; }
			set { m_FramesPerSecond = Mathf.Clamp(value, 1, 60); }
		}
		
		#endregion
		
		
		#region Methods
		
		public AudioAnimationElement[] ElementsAt(int frameIndex) {
			if (m_Elements != null) {
				if (m_ElementsAt == null) {
					m_ElementsAt = new Dictionary<int, List<AudioAnimationElement>>();
					for (int otherIndex = 0; otherIndex < FrameCount; otherIndex++) {
						m_ElementsAt.Add(otherIndex, new List<AudioAnimationElement>());
					}
					foreach (AudioAnimationElement element in m_Elements) {
						if (m_ElementsAt.ContainsKey(element.FrameIndex)) {
							m_ElementsAt[element.FrameIndex].Add(element);
						}
					}
				}
				if (m_ElementsAt.ContainsKey(frameIndex)) {
					return m_ElementsAt[frameIndex].ToArray();
				}
			}
			return new AudioAnimationElement[0];
		}
		
		#endregion
		
		
	}
	
}