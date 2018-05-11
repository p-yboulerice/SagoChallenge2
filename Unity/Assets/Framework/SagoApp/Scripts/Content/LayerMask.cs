namespace SagoApp.Content {
	
	using UnityEngine;
	
	/// <summary>
	/// The LayerMask class provides an alternative to <see cref="UnityEngine.LayerMask" /> 
	/// that lets you specify use a custom mapping between layer names and indices.
	/// </summary>
	public static class LayerMask {
		
		/// <summary>
		/// Gets a mask for the specified layers.
		/// </summary>
		/// <param name="map">The layer map.</param>
		/// <param name="names">The names of the layers.</param>
		public static int GetMask(string[] map, params string[] names) {
			string[] temp = new string[names.Length];
			for (int index = 0; index < names.Length; index++) {
				int layer = NameToLayer(map, names[index]);
				temp[index] = UnityEngine.LayerMask.LayerToName(layer);
			}
			return UnityEngine.LayerMask.GetMask(temp);
		}
		
		/// <summary>
		/// Gets the name of the specified layer.
		/// </summary>
		/// <param name="map">The layer map.</param>
		/// <param name="names">The layer.</param>
		public static string LayerToName(string[] map, int layer) {
			if (layer < 0 || layer > 31) {
				Debug.LogErrorFormat("Layer index is out of bounds: {0}", layer);
			}
			return (layer >= 0 && layer < map.Length) ? map[layer] : string.Empty;
		}
		
		/// <summary>
		/// Gets the index of the specified layer.
		/// </summary>
		/// <param name="map">The layer map.</param>
		/// <param name="names">The layer name.</param>
		public static int NameToLayer(string[] map, string name) {
			return System.Array.IndexOf(map, name);
		}
		
	}
	
}