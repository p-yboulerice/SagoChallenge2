namespace SagoUtils {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// Helper window to find components in the project's Assets
	/// </summary>
	public class ComponentFinderWindow : ScriptableWizard {


		public Component m_ComponentType;
		public bool m_InstantiateFoundPrefabs;


		[MenuItem("Sago/Utils/Component Finder")]
		public static void CreateWizard() {
			var wiz = ScriptableWizard.DisplayWizard<ComponentFinderWindow>("Find Components In Assets", "Search");
			wiz.OnWizardUpdate();
		}


		void OnWizardUpdate() {
			var type = this.ComponentType;
			this.isValid = type != null && type != typeof(UnityEngine.Transform);
			if (type == typeof(UnityEngine.Transform)) {
				this.helpString = "All GameObjects have Transforms; pick another type.";
			} else if (this.isValid) {
				this.helpString = string.Format("Look for type: {0} in ALL prefabs in Project assets", type.ToString());
			} else {
				this.helpString = "Select a type to search for (e.g. add one to a GameObject, then drag that into the Component Type box)";
			}
		}

		void OnWizardCreate() {

			try {
				EditorUtility.DisplayProgressBar(titleContent.text, "Finding prefabs...", 0f);

				var type = this.ComponentType;
				var guids = AssetDatabase.FindAssets("t:GameObject");

				List<GameObject> results = new List<GameObject>();
				if (guids != null) {
					for (int i = 0, count = guids.Length; i < count; ++i) {
						
						var path = AssetDatabase.GUIDToAssetPath(guids[i]);

						var msg = string.Format("{0}/{1} {2}", i + 1, count, path);
						EditorUtility.DisplayProgressBar(titleContent.text, msg, (float)(i + 1f) / count);

						var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
						var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

						var foundComponents = go.GetComponentsInChildren(type, true);
						if (foundComponents != null && foundComponents.Length > 0) {
							
							if (m_InstantiateFoundPrefabs) {
								foreach (var foundComponent in foundComponents) {
									var result = foundComponent.gameObject;
									results.Add(result);
									Debug.LogFormat(result, "{0} ({1}) in {2}", result.name, foundComponent.GetType().Name, prefab.name);
								}
							} else {
								results.Add(prefab);
								Debug.LogFormat(prefab, "Prefab: {0} contains {1} {2}", prefab.name, foundComponents.Length, type.Name);
								DestroyImmediate(go);
							}

						} else {
							
							DestroyImmediate(go);

						}
					}
				}
				
				Selection.objects = results.ToArray();
			
			} finally {
				EditorUtility.ClearProgressBar();
			}
		}

		System.Type ComponentType {
			get {
				if (m_ComponentType) {
					return m_ComponentType.GetType();
				} else {
					return null;
				}
			}
		}
		
		
	}
	
}
