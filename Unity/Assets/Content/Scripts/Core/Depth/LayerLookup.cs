namespace Juice.Depth {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Juice.Depth;
	using Juice.Utils;


	public abstract class LayerKey {}
	public class DragLayer : LayerKey {}


	/// <summary>
	/// For finding 'global' layers, such as drag.
	/// </summary>
	public class LayerLookup : MonoBehaviour {
		
		
		#region Types
		
		[System.Serializable]
		public struct LayerLookupConfig {


			#region Serialized Fields

			[SerializeField]
			private Layer m_Layer;

			[Subclass(typeof(LayerKey))]
			[SerializeField]
			private string m_LayerKey;

			#endregion


			#region Properties

			public Layer Layer {
				get { return m_Layer; }
				set { m_Layer = value; }
			}

			public System.Type LayerKey {
				get { return System.Type.GetType(m_LayerKey); }
				set { m_LayerKey = SubclassAttribute.GetSerializedName(value); }
			}

			#endregion


		}
		
		#endregion
		
		
		#region Serialized Fields
		
		[SerializeField]
		protected LayerLookupConfig[] m_Layers;
		
		#endregion
		
		
		#region Public Properties
		
		public Layer DragLayer {
			get {
				return GetLayer<DragLayer>();
			}
		}
		
		#endregion
		
		
		#region Public Methods

		public Layer GetLayer<T>() where T : LayerKey {
			return GetLayer(typeof(T));
		}

		public Layer GetLayer(System.Type layerKey) {
			LayerLookupConfig result;
			if (this.LayerKeyMap.TryGetValue(layerKey, out result)) {
				return result.Layer;
			} else {
				return null;
			}
		}

		#endregion
		

		#region Internal Fields

		private Dictionary<System.Type, LayerLookupConfig> m_LayerKeyMap;

		#endregion
		
		
		#region Internal Properties

		private Dictionary<System.Type, LayerLookupConfig> LayerKeyMap {
			get {
				if (m_LayerKeyMap == null) {
					var map = new Dictionary<System.Type, LayerLookupConfig>();
					if (m_Layers != null) {
						for (int i = 0; i < m_Layers.Length; ++i) {
							var config = m_Layers[i];
							if (!map.ContainsKey(config.LayerKey)) {
								map.Add(config.LayerKey, config);
							}
						}
					}
					m_LayerKeyMap = map;
				}
				return m_LayerKeyMap;
			}

		}

		#endregion

		
	}
	
}
