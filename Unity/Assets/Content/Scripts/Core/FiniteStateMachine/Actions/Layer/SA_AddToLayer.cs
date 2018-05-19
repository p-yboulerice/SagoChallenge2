namespace Juice.FSM {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Juice.Depth;
	using Juice.Utils;

	public class SA_AddToLayer : StateAction {


		#region StateAction

		public override void Run() {
			if (this.LayerLookup) {
				var layer = this.LayerLookup.GetLayer(this.LayerKey);
				if (layer) {
					layer.Add(this.Layer);
				}
			}
		}

		#endregion


		#region Serialized Fields

		[Subclass(typeof(LayerKey))]
		[SerializeField]
		private string m_ParentLayerKey;

		[SerializeField]
		private Layer m_StateMachineLayer;

		#endregion


		#region MonoBehaviour

		public void Reset() {
			this.LayerKey = typeof(DragLayer);
			m_StateMachineLayer = GetComponentInParent<Layer>();
		}

		#endregion


		#region Internal Fields

		private LayerLookup m_LayerLookup;

		#endregion


		#region Internal Properties

		private Layer Layer {
			get {
				if (!m_StateMachineLayer) m_StateMachineLayer = this.GetComponentInParent<Layer>();
				return m_StateMachineLayer;
			}
		}

		private System.Type LayerKey {
			get { return System.Type.GetType(m_ParentLayerKey); }
			set { m_ParentLayerKey = SubclassAttribute.GetSerializedName(value); }
		}

		private LayerLookup LayerLookup {
			get {
				if (!m_LayerLookup)	m_LayerLookup = GetComponentInParent<LayerLookup>();
				return m_LayerLookup;
			}
		}

		#endregion


	}
	
}
