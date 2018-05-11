namespace SagoApp {

	using SagoTouch;
	using SagoUtils;
	using UnityEngine;

	public class DepthGroupTouchAreaPrioritizer : MonoBehaviour {


		#region Serialized Properties

		[Range(-1000, 1000)]
		[SerializeField]
		public int BasePriority;

		#endregion


		#region Properties

		public DepthGroup DepthGroup {
			get {
				m_DepthGroup = m_DepthGroup ?? GetComponent<DepthGroup>();
				return m_DepthGroup;
			}
		}

		#endregion


		#region Fields

		private DepthGroup m_DepthGroup;

		#endregion


		#region MonoBehaviour

		private void OnEnable() {
			if (this.DepthGroup) {
				this.DepthGroup.OnSetElementDepth += UpdatePriority;
			}
		}

		private void OnDisable() {
			if (this.DepthGroup) {
				this.DepthGroup.OnSetElementDepth -= UpdatePriority;
			}
		}

		#endregion


		#region Functions

		private void UpdatePriority(DepthGroup depthGroup, IDepthGroupElement element) {

			Component component;
			component = element as Component;

			if (component) {

				TouchArea touchArea;
				touchArea = component.GetComponent<TouchArea>();

				if (touchArea) {

					int index;
					index = depthGroup.Elements.IndexOf(element);

					touchArea.Priority = this.BasePriority + index;

				}

			}

		}

		#endregion


	}

}
