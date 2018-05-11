namespace SagoApp.Depth {

	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(Layer))]
	public class LayerEditor : Editor {


		#region Static Methods

		private static void RefreshTrees(IEnumerable<Layer> layers) {
			foreach (Layer root in FindRoots(layers)) {
				RefreshTree(root);
			}
		}

		private static void RefreshTree(Layer root) {

			Transform[] transforms;
			transforms = root.GetComponentsInChildren<Transform>();

			Undo.RecordObjects(transforms, "Layer Depths");

			foreach (Transform transform in transforms) {
				
				Layer layer;
				layer = transform.GetComponent<Layer>();

				if (layer) {
					Layer.ClearCachedFields(layer);
					Layer.RegisterWithParent(layer.Parent, layer);
				} else {
					transform.localPosition = Vector3.Scale(transform.localPosition, Vector2.one);
				}

			}

		}

		private static HashSet<Layer> FindRoots(IEnumerable<Layer> layers) {

			HashSet<Layer> roots;
			roots = new HashSet<Layer>();

			foreach (Layer layer in layers) {
				roots.Add(layer.Root);
			}

			return roots;

		}

		#endregion


		#region Methods

		override public void OnInspectorGUI() {

			DrawDefaultInspector();

			if (GUILayout.Button("Update Layer Depths")) {
				EditorApplication.delayCall -= UpdateLayerDepths;
				EditorApplication.delayCall += UpdateLayerDepths;
			}

		}

		#endregion


		#region Event Handlers

		private void UpdateLayerDepths() {
			RefreshTrees(new HashSet<Layer>(targets.Cast<Layer>()));	
		}

		#endregion


	}

}
