namespace Juice.Utils {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using SagoUtils;

	/// <summary>
	/// Helper window to create simple mesh assets (e.g. circle, grid)
	/// </summary>
	public class SimpleMeshMaker : EditorWindow {


		#region Types

		protected enum Shape {
			Circle,
			Grid,
			Ring,
			Quad,
			RoundedRectangle,
			RoundedLine
		}

		public enum UVType {
			Null,
			Default,
			FishEye
		}

		protected interface ShapeData {
			void OnGUI();

			void MakeMesh();

			bool IsValid { get; }

			string AssetName { get; }
		}

		protected class CircleData : ShapeData {
			protected string customName = "";
			protected float radius = 0.5f;
			protected int numSides = 30;
			protected int numRings = 0;
			protected bool useAngles = false;
			protected Vector2 angleRange = new Vector2(0f, 360f);
			protected bool useColors = false;
			protected Color color0 = Color.gray;
			protected Color color1 = Color.gray;
			protected int colorRingOffset = 0;
			protected UVType uvType;
			protected AnimationCurve distortion;

			virtual public void OnGUI() {
				this.customName = CustomNameField(this.customName, this.AssetName);
				this.radius = EditorGUILayout.Slider("Radius", this.radius, 0.1f, 10.0f);
				this.numSides = EditorGUILayout.IntSlider("Number of Sides", numSides, 4, 50);
				this.numRings = EditorGUILayout.IntSlider("Number of Rings", numRings, 0, 20);

				this.useAngles = EditorGUILayout.Foldout(this.useAngles, "Use Angles");
				if (this.useAngles) {
					EditorGUI.indentLevel++;
					this.angleRange = EditorGUILayout.Vector2Field("Angle Range", angleRange);
					float min = angleRange[0];
					float max = angleRange[1];
					EditorGUI.indentLevel++;
					EditorGUILayout.MinMaxSlider(
						ref min, ref max, -360f, 360f);
					this.angleRange = new Vector2(min, max);
					EditorGUI.indentLevel--;
					EditorGUI.indentLevel--;
				}

				this.useColors = EditorGUILayout.Foldout(this.useColors, "Use Colors");
				if (this.useColors) {
					EditorGUI.indentLevel++;
					this.color0 = EditorGUILayout.ColorField("Inner Color", this.color0);
					this.color1 = EditorGUILayout.ColorField("Outer Color", this.color1);
					this.colorRingOffset = EditorGUILayout.IntSlider("Ring Offset", this.colorRingOffset, -this.numRings, this.numRings);
					EditorGUI.indentLevel--;
				}

				this.uvType = (UVType)EditorGUILayout.EnumFlagsField(this.uvType, "UV Type");
				if (this.uvType != UVType.Null) {
					if (this.distortion == null) {
						this.distortion = new AnimationCurve();
					}
					EditorGUILayout.CurveField(distortion);
				}

			}

			virtual public void MakeMesh() {
				Vector2 angles = this.useAngles ? angleRange : Vector2.zero;
				MakeMeshCircle(this.AssetName, this.radius, this.numSides, this.uvType, this.distortion, this.numRings, angles, this.useColors, this.color0, this.color1, this.colorRingOffset);
			}

			virtual public bool IsValid {
				get {
					return true;  // sliders handle it
				}
			}

			virtual public string AssetName {
				get {
					if (string.IsNullOrEmpty(this.customName)) {
						return string.Format("mesh_circle_{0}_sides{1}{2}", 
							numSides,
							(numRings == 0 ? "" : string.Format("_{0}rings", numRings)),
							(useColors ? "_colored" : ""));
					} else {
						return this.customName;
					}
				}
			}
		}

		protected class GridData : ShapeData {
			protected string customName = "";
			protected Vector2 dimensions;
			protected int xGridCount;
			protected int yGridCount;
			protected bool faceUp;

			protected ColorDirection colorDirection;
			protected Color color0 = Color.gray;
			protected Color color1 = Color.gray;

			protected enum ColorDirection {
				None,
				X,
				Y
			}

			public GridData() {
				dimensions = new Vector2(1.0f, 1.0f);
				xGridCount = 5;
				yGridCount = 5;
				faceUp = false;

				colorDirection = ColorDirection.None;
				color0 = Color.gray;
				color1 = Color.gray;
			}

			virtual public void OnGUI() {
				this.customName = CustomNameField(this.customName, this.AssetName);
				this.dimensions = EditorGUILayout.Vector2Field("Dimensions", this.dimensions);
				this.xGridCount = EditorGUILayout.IntSlider("X Grid Count", this.xGridCount, 1, 50);
				this.yGridCount = EditorGUILayout.IntSlider("Y Grid Count", this.yGridCount, 1, 50);
				this.faceUp = EditorGUILayout.Toggle("Upward", this.faceUp);
				this.colorDirection = (ColorDirection)EditorGUILayout.EnumPopup("Colors", this.colorDirection);
				if (this.colorDirection != ColorDirection.None) {
					EditorGUI.indentLevel++;
					this.color0 = EditorGUILayout.ColorField("Color 1", this.color0);
					this.color1 = EditorGUILayout.ColorField("Color 2", this.color1);
					EditorGUI.indentLevel--;
				}
			}

			virtual public void MakeMesh() {
				MakeMeshGrid(this.AssetName, 
					this.dimensions.x, this.dimensions.y, 
					xGridCount, yGridCount, 
					faceUp,
					this.colorDirection != ColorDirection.None, 
					this.color0, this.color1, 
					this.colorDirection == ColorDirection.Y);
			}

			virtual public bool IsValid {
				get {
					return (dimensions.x > 0.0f && dimensions.y > 0.0f);
				}
			}

			virtual public string AssetName {
				get {
					if (string.IsNullOrEmpty(this.customName)) {
						return string.Format("mesh_grid_{0}x{1}{2}{3}", 
							xGridCount, yGridCount, 
							(colorDirection == ColorDirection.None ? "" : "-colored"),
							(faceUp ? "-up" : ""));
					} else {
						return this.customName;
					}
				}
			}
		}

		protected class QuadData : ShapeData {
			protected string customName = "";
			protected Vector2 vertex_0;
			protected Vector2 vertex_1;
			protected Vector2 vertex_2;
			protected Vector2 vertex_3;

			protected Color color_0 = Color.gray;
			protected Color color_1 = Color.gray;
			protected Color color_2 = Color.gray;
			protected Color color_3 = Color.gray;


			public QuadData() {
				vertex_0 = new Vector2(-1, -1);
				vertex_1 = new Vector2(1, -1);
				vertex_2 = new Vector2(1, 1);
				vertex_3 = new Vector2(-1, 1);

				color_0 = Color.gray;
				color_1 = Color.gray;
				color_2 = Color.gray;
				color_3 = Color.gray;
			}

			virtual public void OnGUI() {
				this.customName = CustomNameField(this.customName, this.AssetName);

				this.vertex_0 = EditorGUILayout.Vector2Field("Vertex 0 : ", this.vertex_0);
				this.vertex_1 = EditorGUILayout.Vector2Field("Vertex 1 : ", this.vertex_1);
				this.vertex_2 = EditorGUILayout.Vector2Field("Vertex 2 : ", this.vertex_2);
				this.vertex_3 = EditorGUILayout.Vector2Field("Vertex 3 : ", this.vertex_3);

				this.color_0 = EditorGUILayout.ColorField("Color 0 : ", color_0);
				this.color_1 = EditorGUILayout.ColorField("Color 1 : ", color_1);
				this.color_2 = EditorGUILayout.ColorField("Color 2 : ", color_2);
				this.color_3 = EditorGUILayout.ColorField("Color 3 : ", color_3);

			}

			virtual public void MakeMesh() {
				MakeQuad(this.AssetName, vertex_0, vertex_1, vertex_2, vertex_3, color_0, color_1, color_2, color_3);
			}

			virtual public bool IsValid {
				get {
					return true;
				}
			}

			virtual public string AssetName {
				get {
					if (string.IsNullOrEmpty(this.customName)) {
						return "quad";
					} else {
						return this.customName;
					}
				}
			}
		}

		protected class RingData : ShapeData {
			protected string customName = "";
			protected float innerRadius = 0.25f;
			protected float outerRadius = 0.5f;
			protected bool isElliptical = false;
			protected float innerRadiusY = 0.25f;
			protected float outerRadiusY = 0.5f;
			protected int numSides = 30;
			protected float minAngle = 0;
			protected float maxAngle = 360f;
			protected int numRings = 0;
			protected bool useColors = false;
			protected Color color0 = Color.gray;
			protected Color color1 = Color.gray;
			protected int colorRingOffset = 0;

			static readonly GUIContent GcRadius = new GUIContent("Inner/Outer Radius");
			static readonly GUIContent GcRadiusY = new GUIContent("Y Inner/Outer Radius");
			static readonly GUIContent GcAngleRange = new GUIContent("Angle Range");

			virtual public void OnGUI() {
				this.customName = CustomNameField(this.customName, this.AssetName);

				this.innerRadius = EditorGUILayout.Slider("Inner Radius", this.innerRadius, 0.1f, this.outerRadius);
				this.outerRadius = EditorGUILayout.Slider("Outer Radius", this.outerRadius, this.innerRadius, 10.0f);
				EditorGUILayout.MinMaxSlider(GcRadius, ref this.innerRadius, ref this.outerRadius, 0f, 10f);

				this.isElliptical = EditorGUILayout.Foldout(this.isElliptical, "Elliptical?");
				if (this.isElliptical) {
					this.innerRadiusY = EditorGUILayout.Slider("Y Inner Radius", this.innerRadiusY, 0.1f, this.outerRadiusY);
					this.outerRadiusY = EditorGUILayout.Slider("Y Outer Radius", this.outerRadiusY, this.innerRadiusY, 10.0f);
					EditorGUILayout.MinMaxSlider(GcRadiusY, ref this.innerRadiusY, ref this.outerRadiusY, 0f, 10f);
				}

				this.numSides = EditorGUILayout.IntSlider("Number of Sides", numSides, 4, 50);

				this.minAngle = EditorGUILayout.Slider("Min Angle", this.minAngle, -360f, this.maxAngle);
				this.maxAngle = EditorGUILayout.Slider("Max Angle", this.maxAngle, this.minAngle, 360f);
				EditorGUILayout.MinMaxSlider(GcAngleRange, ref minAngle, ref maxAngle, 0f, 360f);

				this.numRings = EditorGUILayout.IntSlider("Number of Inner Rings", numRings, 0, 20);
				this.useColors = EditorGUILayout.Foldout(this.useColors, "Use Colors");
				if (this.useColors) {
					EditorGUI.indentLevel++;
					this.color0 = EditorGUILayout.ColorField("Inner Color", this.color0);
					this.color1 = EditorGUILayout.ColorField("Outer Color", this.color1);
					this.colorRingOffset = EditorGUILayout.IntSlider("Ring Offset", this.colorRingOffset, -this.numRings, this.numRings);
					EditorGUI.indentLevel--;
				}
			}

			virtual public void MakeMesh() {

				Vector2 yRadius;
				if (this.isElliptical) {
					yRadius = new Vector2(this.innerRadiusY, this.outerRadiusY);
				} else {
					yRadius = new Vector2(this.innerRadius, this.outerRadius);
				}

				MakeMeshRing(this.AssetName,
					this.innerRadius, this.outerRadius, 
					yRadius[0], yRadius[1],
					this.numSides, this.minAngle, this.maxAngle,
					this.numRings, 
					this.useColors, this.color0, this.color1, this.colorRingOffset);
			}

			virtual public bool IsValid {
				get {
					return true;  // sliders handle it
				}
			}

			virtual public string AssetName {
				get {
					if (string.IsNullOrEmpty(this.customName)) {
						return string.Format("mesh_{3}ring_{0}_sides{1}{2}", 
							numSides,
							(numRings == 0 ? "" : string.Format("_{0}rings", numRings)),
							(useColors ? "_colored" : ""),
							(isElliptical ? "elliptical_" : ""));
					} else {
						return this.customName;
					}
				}
			}
		}

		protected class RoundedRectangleData : ShapeData {
			protected string customName = "";
			protected Vector2 outerDimensions = Vector2.one;
			protected float cornerRadius = 0.125f;
			protected int cornerNumSides = 8;
			protected bool onlyBorder = false;
			protected bool useColors = false;
			protected Color color0 = Color.gray;
			protected Color color1 = Color.gray;

			protected Vector2 innerDimensions {
				get {
					return outerDimensions - 2f * new Vector2(cornerRadius, cornerRadius);
				}
				set {
					Vector2 clampedValue = Vector2.Max(value, Vector2.zero);
					outerDimensions = clampedValue + 2f * new Vector2(cornerRadius, cornerRadius);
				}
			}

			virtual public void OnGUI() {
				this.customName = CustomNameField(this.customName, this.AssetName);

				this.outerDimensions = EditorGUILayout.Vector2Field("Outer Dimensions", this.outerDimensions);

				EditorGUI.BeginDisabledGroup(true);
				this.innerDimensions = EditorGUILayout.Vector2Field("Inner Dimensions", this.innerDimensions);
				EditorGUI.EndDisabledGroup();

				this.cornerRadius = EditorGUILayout.Slider("Corner Radius", this.cornerRadius, 0.001f, 100f);
				this.cornerNumSides = EditorGUILayout.IntSlider("Corner Side Count", this.cornerNumSides, 4, 50);

				this.onlyBorder = EditorGUILayout.Toggle("Only Border", this.onlyBorder);

				this.useColors = EditorGUILayout.Foldout(this.useColors, "Use Colors");
				if (this.useColors) {
					EditorGUI.indentLevel++;
					this.color0 = EditorGUILayout.ColorField("Inner Color", this.color0);
					this.color1 = EditorGUILayout.ColorField("Outer Color", this.color1);
				
					EditorGUI.indentLevel--;
				}
			}

			virtual public void MakeMesh() {
				MakeMeshRoundedRectangle(
					this.AssetName, 
					this.innerDimensions,
					this.cornerRadius, this.cornerNumSides, 
					this.onlyBorder,
					this.useColors, this.color0, this.color1);
			}

			virtual public bool IsValid {
				get {
					return true;  // sliders handle it
				}
			}

			virtual public string AssetName {
				get {
					if (string.IsNullOrEmpty(this.customName)) {
						return string.Format("mesh_rounded_rectangle{0:F1}x{1:F1}_radius{2}_sides{3}{4}", 
							outerDimensions.x, outerDimensions.y, cornerRadius, cornerNumSides,
							(useColors ? "_colored" : ""));
					} else {
						return this.customName;
					}
				}
			}
		}

		protected class RoundedLineData : ShapeData {
			protected string customName = "";
			protected float length = 2f;
			protected float radius0 = 0.5f;
			protected float radius1 = 1.0f;
			protected int roundedNumSides = 15;
			protected bool useColors = false;
			protected Color colorCenter0 = Color.white;
			protected Color colorCenter1 = Color.white;
			protected Color colorEdges0 = Color.grey;
			protected Color colorEdges1 = Color.grey;

			virtual public void OnGUI() {
				this.customName = CustomNameField(this.customName, this.AssetName);

				this.length = EditorGUILayout.Slider("Length", this.length, 0.01f, 100f);
				this.radius0 = EditorGUILayout.Slider("Radius 0", this.radius0, 0.001f, 100f);
				this.radius1 = EditorGUILayout.Slider("Radius 1", this.radius1, 0.001f, 100f);

				this.roundedNumSides = EditorGUILayout.IntSlider("Rounded Vert Count", this.roundedNumSides, 4, 50);

				this.useColors = EditorGUILayout.Foldout(this.useColors, "Use Colors");
				if (this.useColors) {
					EditorGUI.indentLevel++;
					this.colorCenter0 = EditorGUILayout.ColorField("Center Color 0", this.colorCenter0);
					this.colorEdges0 = EditorGUILayout.ColorField("Edges Color 0", this.colorEdges0);
					this.colorCenter1 = EditorGUILayout.ColorField("Center Color 1", this.colorCenter1);
					this.colorEdges1 = EditorGUILayout.ColorField("Edges Color 1", this.colorEdges1);
					EditorGUI.indentLevel--;
				}
			}

			virtual public void MakeMesh() {
				MakeMeshRoundedLine(
					this.AssetName, 
					this.length,
					this.radius0, this.radius1,
					this.roundedNumSides,
					this.useColors, this.colorCenter0, this.colorEdges0, this.colorCenter1, this.colorEdges1);
			}

			virtual public bool IsValid {
				get {
					return (Mathf.Max(radius0, radius1) - Mathf.Min(radius0, radius1)) < this.length; // sliders handle rest
				}
			}

			virtual public string AssetName {
				get {
					if (string.IsNullOrEmpty(this.customName)) {
						return string.Format("mesh_rounded_line_radii_{0:F1}x{1:F1}x{2:F1}_sides{3}{4}", 
							length, radius0, radius1, roundedNumSides, (useColors ? "_colored" : ""));
					} else {
						return this.customName;
					}
				}
			}
		}

		#endregion


		#region Properties

		protected Shape SelectedShape { get; set; }

		protected Dictionary<Shape,ShapeData> AllShapeData { get; set; }

		#endregion


		#region Public Methods


		#region General

		/// <summary>
		/// Creates all necessary folders in the Assets/ tree to ensure
		/// the given path exists.
		/// </summary>
		/// <param name="newPath">New path.</param>
		public static void CreateFolders(string newPath) {
			string[] split = newPath.Split('/');
			if (split[0] == "Assets") {
				string parent = split[0];
				for (int i = 1; i < split.Length; ++i) {
					string subfolder = parent + "/" + split[i];
					if (!AssetDatabase.IsValidFolder(subfolder)) {
						AssetDatabase.CreateFolder(parent, split[i]);
					}
					parent = subfolder;
				}
			}
			Debug.Assert(AssetDatabase.IsValidFolder(newPath));
		}

		private static string GetPath(string assetName) {
			string path;
			if (AssetDatabase.IsValidFolder("Assets/Project")) {
				path = "Assets/Project/Meshes";
			} else {
				path = "Assets/Meshes";
			}
			CreateFolders(path);

			path = string.Format("{0}/{1}.asset", path, assetName);

			path = AssetDatabase.GenerateUniqueAssetPath(path);

			return path;
		}

		[UnityEditor.MenuItem("Sago/Content/Juice/Mesh/Make Simple Mesh")]
		public static void CreateWindow() {
			EditorWindow.GetWindow<SimpleMeshMaker>(true, "Simple Mesh Maker", true);
		}

		#endregion


		#region Circle

		public static void MakeMeshCircle(string assetName, float radius, int numSides, UVType uvType, AnimationCurve uvAnimationCurve, int numRings = 0, Vector2 angleRange = default(Vector2),
		                                  bool useColors = false, Color32 color0 = default(Color32), Color32 color1 = default(Color32), int colorRingOffset = 0) {
	
			string path = GetPath(assetName);

			Mesh mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;

			if (!mesh) {
				mesh = new Mesh();
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}

			bool isFullCircle;
			float sideAngle;
			if (Mathf.Approximately(MathUtil.WrappedAngle(angleRange[0]), MathUtil.WrappedAngle(angleRange[1]))) {
				isFullCircle = true;
				sideAngle = Mathf.PI * 2f / (float)numSides;
			} else {
				isFullCircle = false;
				sideAngle = ((angleRange[1] - angleRange[0]) * Mathf.Deg2Rad) / (float)numSides;
			}

			numSides = isFullCircle ? numSides : numSides + 1;
			Vector3[] vertices = new Vector3[numSides * (numRings + 1) + 1];
			Vector3[] normals = new Vector3[vertices.Length];
			int[] triangles = new int[3 * (numSides + numRings * numSides * 2)];
			Color32[] colors = (useColors) ? new Color32[vertices.Length] : null;
			Vector2[] uvs = (uvType != UVType.Null) ? new Vector2[vertices.Length] : null;

			vertices[0] = Vector3.zero;
			normals[0] = Vector3.back;
			if (useColors)
				colors[0] = color0;
			if (uvType != UVType.Null) {
				uvs[0] = Vector2.one * 0.5f;
			}

			for (int ring = 1; ring <= (numRings + 1); ++ring) {
				float ringT = (float)ring / (numRings + 1.0f);
				float ringRadius = radius * ringT;
				for (int i = 0; i < numSides; ++i) {
					float theta = i * sideAngle + Mathf.Deg2Rad * angleRange[0];
					int idx = i + (ring - 1) * numSides + 1;
					vertices[idx] = new Vector3(ringRadius * Mathf.Cos(theta), ringRadius * Mathf.Sin(theta), 0.0f);
					normals[idx] = normals[0];
					if (useColors) {
						float colorT = (float)Mathf.Clamp(ring + colorRingOffset, 0, numRings + 1) / Mathf.Max(numRings + 1.0f + colorRingOffset, 1);
						colors[idx] = Color32.Lerp(color0, color1, colorT);
					}

					switch (uvType) {
					case UVType.Default:
						break;
					case UVType.FishEye:
						float p = vertices[idx].magnitude / radius;
						Vector3 vect = vertices[idx].normalized * 0.5f;

						uvs[idx] = new Vector2(0.5f, 0.5f) + (Vector2)vect * uvAnimationCurve.Evaluate(p);
						break;
					}
				}
			}

			// inner triangles
			for (int i = 0; i < numSides; ++i) {
				int tIdx = 3 * i;

				if (!isFullCircle && i >= numSides - 1)
					break;

				triangles[tIdx] = i + 1;

				triangles[tIdx + 1] = 0;

				if (i >= numSides - 1) {
					triangles[tIdx + 2] = 1;
				} else {
					triangles[tIdx + 2] = i + 2;
				}
			}

			// outer 'quads' (triangle pairs)
			if (numRings > 0) {
				for (int ring = 1; ring <= (numRings); ++ring) {
					for (int i = 0; i < numSides; ++i) {

						if (!isFullCircle && i >= numSides - 1)
							break;
						
						int tIdx = 3 * (numSides + 2 * ((ring - 1) * numSides + i));
						int vIdx = i + ring * numSides + 1;

						triangles[tIdx] = vIdx;

						triangles[tIdx + 1] = vIdx - numSides;

						if (i >= numSides - 1) {
							triangles[tIdx + 2] = vIdx - numSides * 2 + 1;
						} else {
							triangles[tIdx + 2] = vIdx - numSides + 1;
						}

						triangles[tIdx + 3] = triangles[tIdx];
						triangles[tIdx + 4] = triangles[tIdx + 2];

						if (i >= numSides - 1) {
							triangles[tIdx + 5] = vIdx - numSides + 1;
						} else {
							triangles[tIdx + 5] = vIdx + 1;
						}
					}
				}
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.triangles = triangles;
			if (useColors) {
				mesh.colors32 = colors;
			}
			if (uvType != UVType.Null) {
				mesh.uv = uvs;
			}
			mesh.RecalculateBounds();

			EditorUtility.SetDirty(mesh);
			AssetDatabase.SaveAssets();

			EditorGUIUtility.PingObject(mesh);
			Debug.Log("Created " + path);
		}

		#endregion


		#region Grid

		public static void MakeMeshGrid(string assetName, 
		                                float width, float height, int xGridCount, int yGridCount, 
		                                bool faceUp = false,
		                                bool useColors = false,
		                                Color32 color0 = default(Color32), Color32 color1 = default(Color32),
		                                bool colorsVertical = false) {

			string path = GetPath(assetName);

			Mesh mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;

			if (!mesh) {
				mesh = new Mesh();
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}

			int numVertices = (xGridCount + 1) * (yGridCount + 1);

			Vector3[] vertices = new Vector3[numVertices];
			Vector3[] normals = new Vector3[vertices.Length];
			int[] triangles = new int[3 * 2 * (xGridCount * yGridCount)];
			Color32[] colors = (useColors) ? new Color32[vertices.Length] : null;

			float xDelta = width / (float)xGridCount;
			float yDelta = height / (float)yGridCount;
			float y = -0.5f * height;
			float colorT = 0;
			for (int j = 0; j <= yGridCount; ++j) {

				float x = -0.5f * width;
				if (useColors && colorsVertical) {
					colorT = Mathf.InverseLerp(0f, (float)yGridCount, (float)j);
				}

				for (int i = 0; i <= xGridCount; ++i) {

					int idx = j * (xGridCount + 1) + i;

					if (useColors) {
						if (!colorsVertical) {
							colorT = Mathf.InverseLerp(0f, (float)xGridCount, (float)i);
						}
						colors[idx] = Color32.Lerp(color0, color1, colorT);
					}

					if (faceUp) {
						vertices[idx] = new Vector3(x, 0.0f, y);
						normals[idx] = Vector3.up;
					} else {
						vertices[idx] = new Vector3(x, y, 0.0f);
						normals[idx] = Vector3.back;
					}


					x += xDelta;

				}

				y += yDelta;
			}

			for (int j = 0; j < yGridCount; ++j) {
				for (int i = 0; i < xGridCount; ++i) {

					int tIdx = 3 * 2 * (j * xGridCount + i);

					int idx = j * (xGridCount + 1) + i;

					triangles[tIdx] = idx;
					triangles[tIdx + 1] = idx + xGridCount + 1;
					triangles[tIdx + 2] = idx + 1;

					triangles[tIdx + 3] = triangles[tIdx + 2];
					triangles[tIdx + 4] = triangles[tIdx + 1];
					triangles[tIdx + 5] = idx + xGridCount + 2;
				}
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.triangles = triangles;
			mesh.colors32 = colors;
			mesh.RecalculateBounds();

			EditorUtility.SetDirty(mesh);
			AssetDatabase.SaveAssets();

			EditorGUIUtility.PingObject(mesh);
			Debug.Log("Created " + path);
		}

		#endregion


		#region Ring

		public static void MakeMeshRing(string assetName, 
		                                float innerRadius, float outerRadius, 
		                                float innerRadiusY, float outerRadiusY, 
		                                int numSides, float minAngle, float maxAngle,
		                                int numRings = 0,
		                                bool useColors = false, 
		                                Color32 color0 = default(Color32), 
		                                Color32 color1 = default(Color32), 
		                                int colorRingOffset = 0) {

			string path = GetPath(assetName);

			Mesh mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;

			if (!mesh) {
				mesh = new Mesh();
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}

			//minAngle = MathUtil.WrappedAngle(minAngle);
			//maxAngle = MathUtil.WrappedAngle(maxAngle);
			bool broken = !Mathf.Approximately(minAngle, maxAngle);

			float sideAngle, sideAngleOffset;
			if (broken) {
				sideAngleOffset = Mathf.Deg2Rad * minAngle;
				sideAngle = Mathf.Deg2Rad * (maxAngle - minAngle) / (float)(numSides - 1f);
			} else {
				sideAngleOffset = 0f;
				sideAngle = Mathf.PI * 2.0f / (float)numSides;
			}

			Vector3[] vertices = new Vector3[numSides * (numRings + 2)];
			Vector3[] normals = new Vector3[vertices.Length];
			int[] triangles = new int[3 * 2 * (numSides + (broken ? -1 : 0)) * (1 + numRings)];
			Color32[] colors = (useColors) ? new Color32[vertices.Length] : null;

			vertices[0] = Vector3.zero;
			normals[0] = Vector3.back;
			if (useColors)
				colors[0] = color0;

			int vIdx = 0;

			for (int ring = 0; ring <= (numRings + 1); ++ring) {
				float ringT = Mathf.Clamp01((float)ring / (numRings + 1.0f));
				float ringA = Mathf.Lerp(innerRadius, outerRadius, ringT);
				float ringB = Mathf.Lerp(innerRadiusY, outerRadiusY, ringT);

				for (int i = 0; i < numSides; ++i) {
					float theta = i * sideAngle + sideAngleOffset;

					vertices[vIdx] = new Vector3(ringA * Mathf.Cos(theta), ringB * Mathf.Sin(theta), 0.0f);

					normals[vIdx] = Vector3.back;
					if (useColors) {
						float colorT = (float)Mathf.Clamp(ring + colorRingOffset, 0, numRings + 1) / Mathf.Max(numRings + 1.0f + colorRingOffset, 1);
						colors[vIdx] = Color32.Lerp(color0, color1, colorT);
					}
					vIdx++;
				}
			}

			vIdx = 0;

			// outer 'quads' (triangle pairs)
			int sides = numSides + (broken ? -1 : 0);
			for (int ring = 0; ring <= numRings; ++ring) {
				for (int i = 0; i < sides; ++i) {
					int tIdx = 3 * 2 * (ring * sides + i);
					vIdx = i + (ring + 1) * numSides;

					triangles[tIdx] = vIdx;

					triangles[tIdx + 1] = vIdx - numSides;

					if (i >= numSides - 1) {
						triangles[tIdx + 2] = vIdx - numSides * 2 + 1;
					} else {
						triangles[tIdx + 2] = vIdx - numSides + 1;
					}

					triangles[tIdx + 3] = triangles[tIdx];
					triangles[tIdx + 4] = triangles[tIdx + 2];

					if (i >= numSides - 1) {
						triangles[tIdx + 5] = vIdx - numSides + 1;
					} else {
						triangles[tIdx + 5] = vIdx + 1;
					}
				}
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.triangles = triangles;
			if (useColors) {
				mesh.colors32 = colors;
			}
			mesh.RecalculateBounds();

			EditorUtility.SetDirty(mesh);
			AssetDatabase.SaveAssets();

			EditorGUIUtility.PingObject(mesh);
			Debug.Log("Created " + path);
		}

		#endregion


		#region Quad

		public static void MakeQuad(string assetName, Vector3 vertex_0, Vector3 vertex_1, Vector3 vertex_2, Vector3 vertex_3, Color color_0, Color color_1, Color color_2, Color color_3) {

			string path = GetPath(assetName);

			Mesh mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;

			if (!mesh) {
				mesh = new Mesh();
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}


			mesh.vertices = new Vector3[] {
				vertex_0, vertex_1, vertex_2, vertex_3
			};
			mesh.uv = new Vector2[] {
				new Vector2(0, 0),
				new Vector2(0, 1),
				new Vector2(1, 1),
				new Vector2(1, 0)
			};
			mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

			Color32[] colors = { color_0, color_1, color_2, color_3 };

			mesh.colors32 = colors;
			mesh.RecalculateBounds();

			EditorUtility.SetDirty(mesh);
			AssetDatabase.SaveAssets();

			EditorGUIUtility.PingObject(mesh);
			Debug.Log("Created " + path);
		}

		#endregion


		#region Rounded Rectangle

		public static void MakeMeshRoundedRectangle(string assetName,
		                                            Vector2 innerDimensions,
		                                            float cornerRadius,	int cornerNumSides,
		                                            bool excludeCentralQuad = false,
		                                            bool useColors = false, Color32 color0 = default(Color32), Color32 color1 = default(Color32)) {

			string path = GetPath(assetName);

			Mesh mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;

			if (!mesh) {
				mesh = new Mesh();
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}

			int numVerts = (cornerNumSides + 1) * 4 + 4;
			Vector3[] vertices = new Vector3[numVerts];
			Vector3[] normals = new Vector3[numVerts];

			int numTris = (cornerNumSides + 2) * 4 + (excludeCentralQuad ? 0 : 2);
			int numTriIndices = numTris * 3;
			int[] triangles = new int[numTriIndices];

			Color32[] colors = new Color32[numVerts];


			float sideAngle = Mathf.PI * 0.5f / (float)cornerNumSides;  // portion of 90-degrees
			Vector3 inner = (Vector3)innerDimensions;

			// upper left corner verts
			int vIdx = 0;
			vertices[vIdx] = Vector3.Scale(inner, new Vector3(-1f, 1f, 1f));
			colors[vIdx] = color0;

			for (int i = 0; i <= cornerNumSides; ++i) {
				float theta = i * sideAngle;
				int idx = vIdx + 1 + i;
				vertices[idx] = vertices[vIdx] + new Vector3(-cornerRadius * Mathf.Cos(theta), cornerRadius * Mathf.Sin(theta), 0.0f);
				colors[idx] = color1;
			}

			// upper left corner tris
			int tIdx = 0;
			for (int i = 0; i < cornerNumSides; ++i) {
				triangles[tIdx++] = vIdx;
				triangles[tIdx++] = vIdx + i + 1;
				triangles[tIdx++] = vIdx + i + 2;
			}

			// upper right corner verts
			vIdx += (cornerNumSides + 2);
			vertices[vIdx] = inner;
			colors[vIdx] = color0;

			for (int i = 0; i <= cornerNumSides; ++i) {
				float theta = i * sideAngle;
				int idx = vIdx + 1 + i;
				vertices[idx] = vertices[vIdx] + new Vector3(cornerRadius * Mathf.Sin(theta), cornerRadius * Mathf.Cos(theta), 0.0f);
				colors[idx] = color1;
			}

			// upper quad tris
			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = vIdx - (cornerNumSides + 2);
			triangles[tIdx++] = vIdx - 1;

			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = vIdx - 1;
			triangles[tIdx++] = vIdx + 1;

			// upper right corner tris
			for (int i = 0; i < cornerNumSides; ++i) {
				triangles[tIdx++] = vIdx;
				triangles[tIdx++] = vIdx + i + 1;
				triangles[tIdx++] = vIdx + i + 2;
			}


			// lower right corner verts
			vIdx += (cornerNumSides + 2);
			vertices[vIdx] = Vector3.Scale(inner, new Vector3(1f, -1f, 1f));
			colors[vIdx] = color0;

			for (int i = 0; i <= cornerNumSides; ++i) {
				float theta = i * sideAngle;
				int idx = vIdx + 1 + i;
				vertices[idx] = vertices[vIdx] + new Vector3(cornerRadius * Mathf.Cos(theta), -cornerRadius * Mathf.Sin(theta), 0.0f);
				colors[idx] = color1;
			}

			// right quad tris
			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = vIdx - (cornerNumSides + 2);
			triangles[tIdx++] = vIdx - 1;

			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = vIdx - 1;
			triangles[tIdx++] = vIdx + 1;

			// lower right corner tris
			for (int i = 0; i < cornerNumSides; ++i) {
				triangles[tIdx++] = vIdx;
				triangles[tIdx++] = vIdx + i + 1;
				triangles[tIdx++] = vIdx + i + 2;
			}


			// lower left corner verts
			vIdx += (cornerNumSides + 2);
			vertices[vIdx] = Vector3.Scale(inner, new Vector3(-1f, -1f, 1f));
			colors[vIdx] = color0;

			for (int i = 0; i <= cornerNumSides; ++i) {
				float theta = i * sideAngle;
				int idx = vIdx + 1 + i;
				vertices[idx] = vertices[vIdx] + new Vector3(-cornerRadius * Mathf.Sin(theta), -cornerRadius * Mathf.Cos(theta), 0.0f);
				colors[idx] = color1;
			}

			// lower quad tris
			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = vIdx - (cornerNumSides + 2);
			triangles[tIdx++] = vIdx - 1;

			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = vIdx - 1;
			triangles[tIdx++] = vIdx + 1;

			// lower left corner tris
			for (int i = 0; i < cornerNumSides; ++i) {
				triangles[tIdx++] = vIdx;
				triangles[tIdx++] = vIdx + i + 1;
				triangles[tIdx++] = vIdx + i + 2;
			}

			// left quad tris - wrap back to beginning
			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = 1;
			triangles[tIdx++] = 0;

			triangles[tIdx++] = vIdx;
			triangles[tIdx++] = numVerts - 1;
			triangles[tIdx++] = 1;

			// center quad tris
			if (!excludeCentralQuad) {
				triangles[tIdx++] = 0;
				triangles[tIdx++] = vIdx - (cornerNumSides + 2);
				triangles[tIdx++] = vIdx;

				triangles[tIdx++] = 0;
				triangles[tIdx++] = 0 + (cornerNumSides + 2);
				triangles[tIdx++] = vIdx - (cornerNumSides + 2);
			}


			// normals
			for (int i = 0; i < numVerts; ++i) {
				normals[i] = Vector3.back;
			}

			// assign and finish up
			mesh.vertices = vertices;
			if (useColors) {
				mesh.colors32 = colors;
			}
			mesh.normals = normals;
			mesh.triangles = triangles;
			mesh.RecalculateBounds();

			EditorUtility.SetDirty(mesh);
			AssetDatabase.SaveAssets();

			EditorGUIUtility.PingObject(mesh);
			Debug.Log("Created " + path);
		}

		#endregion


		#region Rounded Line

		public static void MakeMeshRoundedLine(string assetName,
		                                       float length,
		                                       float radius0, float radius1, 
		                                       int roundedNumVerts,
		                                       bool useColors = false, 
		                                       Color32 colorCenter0 = default(Color32), Color32 colorEdges0 = default(Color32),
		                                       Color32 colorCenter1 = default(Color32), Color32 colorEdges1 = default(Color32)) {

			string path = GetPath(assetName);

			Mesh mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;

			if (!mesh) {
				mesh = new Mesh();
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}


			roundedNumVerts = Mathf.Max(roundedNumVerts, 3);
			int numVerts = (roundedNumVerts * 2) + 2;
			Vector3[] vertices = new Vector3[numVerts];
			Vector3[] normals = new Vector3[numVerts];

			int numTris = (roundedNumVerts - 1) * 2 + 4;
			int numTriIndices = numTris * 3;
			int[] triangles = new int[numTriIndices];

			Color32[] colors = useColors ? new Color32[numVerts] : null;

			Vector3[] centers = new Vector3[] {
				Vector3.zero,
				new Vector3(length, 0f, 0f)
			};
			float[] radii = new float[] { radius0, radius1 };
			Color32[] centerColors = useColors ? new Color32[] {
				colorCenter0,
				colorCenter1
			} : null;
			Color32[] edgeColors = useColors ? new Color32[] {
				colorEdges0,
				colorEdges1
			} : null;
			float[] startAngles = new float[2];
			float[] angleIncrements = new float[2];

			length = Mathf.Max(length, 0.02f);

			int larger = (radius0 >= radius1) ? 0 : 1;
			int smaller = 1 - larger;

			float angle = Mathf.Acos((radii[larger] - radii[smaller]) / length);
			startAngles[larger] = angle + Mathf.PI;
			startAngles[smaller] = -angle;
			angleIncrements[larger] = 2f * (Mathf.PI - angle) / (float)(roundedNumVerts - 1);
			angleIncrements[smaller] = 2f * angle / (float)(roundedNumVerts - 1);

			startAngles[0] += Mathf.PI;

			// verts
			for (int end = 0; end < 2; ++end) {

				Vector3 center = centers[end];
				float radius = radii[end];
				Color32 edgeColor = edgeColors[end];
				float startAngle = startAngles[end];
				float angleIncrement = angleIncrements[end];

				int centerIdx = end * (roundedNumVerts + 1);
				vertices[centerIdx] = center;
				if (useColors) {
					colors[centerIdx] = centerColors[end];
				}

				for (int i = 0; i < roundedNumVerts; ++i) {

					float theta = i * angleIncrement + startAngle;
					int idx = centerIdx + 1 + i;
					vertices[idx] = center + new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0.0f);
					if (useColors) {
						colors[idx] = edgeColor;
					}
				}
			}

			// triangles
			int tIdx = 0;

			for (int end = 0; end < 2; ++end) {
				int vCenterIdx = end * (roundedNumVerts + 1);
				for (int i = 0; i < roundedNumVerts - 1; ++i) {
					triangles[tIdx++] = vCenterIdx;
					triangles[tIdx++] = vCenterIdx + 2 + i;
					triangles[tIdx++] = vCenterIdx + 1 + i;
				}
			}

			int vCenter0Idx = 0;
			int vCenter1Idx = roundedNumVerts + 1;
			triangles[tIdx++] = vCenter0Idx;
			triangles[tIdx++] = 1;
			triangles[tIdx++] = vCenter1Idx + roundedNumVerts;

			triangles[tIdx++] = vCenter0Idx;
			triangles[tIdx++] = vCenter1Idx + roundedNumVerts;
			triangles[tIdx++] = vCenter1Idx;

			triangles[tIdx++] = vCenter0Idx;
			triangles[tIdx++] = vCenter1Idx;
			triangles[tIdx++] = vCenter1Idx + 1;

			triangles[tIdx++] = vCenter0Idx;
			triangles[tIdx++] = vCenter1Idx + 1;
			triangles[tIdx++] = vCenter0Idx + roundedNumVerts;


			// normals
			for (int i = 0; i < numVerts; ++i) {
				normals[i] = Vector3.back;
			}

			// assign and finish up
			mesh.vertices = vertices;
			if (useColors) {
				mesh.colors32 = colors;
			}
			mesh.normals = normals;
			mesh.triangles = triangles;
			mesh.RecalculateBounds();

			EditorUtility.SetDirty(mesh);
			AssetDatabase.SaveAssets();

			EditorGUIUtility.PingObject(mesh);
			Debug.Log("Created " + path);

		}

		#endregion


		#endregion


		#region EditorWindow

		protected void OnEnable() {
			if (this.AllShapeData == null) {
				this.AllShapeData = new Dictionary<Shape, ShapeData>();

				// To add shapes, make a class implementing ShapeData, and add it here
				this.AllShapeData.Add(Shape.Circle, new CircleData());
				this.AllShapeData.Add(Shape.Grid, new GridData());
				this.AllShapeData.Add(Shape.Ring, new RingData());
				this.AllShapeData.Add(Shape.Quad, new QuadData());
				this.AllShapeData.Add(Shape.RoundedRectangle, new RoundedRectangleData());
				this.AllShapeData.Add(Shape.RoundedLine, new RoundedLineData());
			}
		}

		/// <summary>
		/// Shapes added to AllShapeData will automatically draw here.
		/// </summary>
		protected void OnGUI() {

			this.SelectedShape = (Shape)EditorGUILayout.EnumPopup("Shape", this.SelectedShape);

			ShapeData currentData = this.AllShapeData[this.SelectedShape];

			EditorGUI.indentLevel++;

			currentData.OnGUI();

			EditorGUI.indentLevel++;

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(!currentData.IsValid);
			if (GUILayout.Button("Make Mesh")) {
				currentData.MakeMesh();
			}
			if (GUILayout.Button("Make Mesh & Close")) {
				currentData.MakeMesh();
				this.Close();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();

		}

		protected static string CustomNameField(string customName, string defaultName) {
			customName = EditorGUILayout.TextField("Name", customName);
			if (string.IsNullOrEmpty(customName)) {
				Rect last = GUILayoutUtility.GetLastRect();
				last.x += EditorGUIUtility.labelWidth;
				last.width -= EditorGUIUtility.labelWidth;
				Color oldColor = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUI.Label(last, defaultName);
				GUI.color = oldColor;
			}
			return customName;
		}

		#endregion


	}

}