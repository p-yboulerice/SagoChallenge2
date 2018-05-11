namespace SagoApp.Depth {

	using SagoTouch;
	using UnityEngine;

	public class LayerRankedTouchPriority : MonoBehaviour {


		#region Serialized Fields

		[SerializeField]
		private Layer m_Layer;

		[SerializeField]
		private TouchArea m_TouchArea;

		#endregion


		#region Internal Properties

		private Layer Layer {
			get {
				m_Layer = m_Layer ?? GetComponentInParent<Layer>();
				return m_Layer;
			}
		}

		private TouchArea TouchArea {
			get {
				m_TouchArea = m_TouchArea ?? GetComponentInParent<TouchArea>();
				return m_TouchArea;
			}
		}

		#endregion


		#region MonoBehaviour

		private void OnEnable() {
			SubscribeToLayer(this.Layer);
		}

		private void OnDisable() {
			UnsubscribeFromLayer(this.Layer);
		}

		#endregion


		#region Internal Methods

		private void SubscribeToLayer(Layer layer) {
			UnsubscribeFromLayer(layer);
			if (layer) {
				layer.OnDepthDidChange += OnDepthChange;
			}
		}

		private void UnsubscribeFromLayer(Layer layer) {
			if (layer) {
				layer.OnDepthDidChange -= OnDepthChange;
			}
		}

		private void OnDepthChange(Layer layer) {
			SetTouchPriority(layer.Rank);
		}

		private void SetTouchPriority(int value) {
			if (this.TouchArea && this.TouchArea.Priority != value) {
				this.TouchArea.Priority = value;
			}
		}

		#endregion


	}

}
