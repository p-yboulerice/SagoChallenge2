namespace Juice.Utils {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using SagoMesh;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	
	/// <summary>
	/// Some useful extensions
	/// </summary>
	public static class UtilityExtensions {
		

		#region GameObject

		/// <summary>
		/// Calculates the renderer bounds for all of the renderers in this
		/// GameObject that are of the Renderer-type passed, e.g. you can 
		/// pass MeshRenderer if you want to skip ParticleRenderers.
		/// </summary>
		/// <returns>The bounds of all renderers of type T in the given GameObject.</returns>
		/// <param name="go">The GameObject to search</param>
		/// <typeparam name="T">The type of Renderer; use Renderer for all, or more likely just MeshRenderer.</typeparam>
		public static Bounds CalculateRendererBounds<T>(this GameObject go) where T : Renderer {

			Vector3 min;
			min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
			
			Vector3 max;
			max = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);

			foreach (T renderer in go.GetComponentsInChildren<T>()) {
				
				Bounds bounds;
				bounds = renderer.bounds;

				if (bounds.size == Vector3.zero) {
					bounds.center = renderer.transform.position;
				}

				min.x = Mathf.Min(min.x, bounds.min.x);
				min.y = Mathf.Min(min.y, bounds.min.y);
				min.z = Mathf.Min(min.z, bounds.min.z);
				max.x = Mathf.Max(max.x, bounds.max.x);
				max.y = Mathf.Max(max.y, bounds.max.y);
				max.z = Mathf.Max(max.z, bounds.max.z);
				
			}

			Vector3 pos = go.transform.position;
			min.x = (min.x != Mathf.Infinity) ? min.x : pos.x;
			min.y = (min.y != Mathf.Infinity) ? min.y : pos.y;
			min.z = (min.z != Mathf.Infinity) ? min.z : pos.z;
			max.x = (max.x != Mathf.NegativeInfinity) ? max.x : pos.x;
			max.y = (max.y != Mathf.NegativeInfinity) ? max.y : pos.y;
			max.z = (max.z != Mathf.NegativeInfinity) ? max.z : pos.z;

			Bounds b = new Bounds();
			b.SetMinMax(min, max);
			return b;
		}

		/// <summary>
		/// Calculates the bounds of all MeshFilters on the GameObject,
		/// relative to the given transform, i.e. the Bounds returned
		/// is in local space to the given transform.
		/// </summary>
		/// <returns>The local mesh bounds.</returns>
		/// <param name="go">Root GameObject to search through</param>
		/// <param name="relativeTo">Bounds local to this transform; if null will use GameObject.</param>
		public static Bounds CalculateLocalMeshBounds2D(this GameObject go, Transform relativeTo = null) {

			Vector3 min;
			min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

			Vector3 max;
			max = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);

			relativeTo = relativeTo ?? go.transform;

			MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
			for (int i = 0; i < meshFilters.Length; i++) {
				var mf = meshFilters[i];
				Bounds meshBounds = new Bounds();
				if (!mf.sharedMesh) {
					min = Vector3.Min(min, mf.transform.position);
					max = Vector3.Max(max, mf.transform.position);
				} else {
					meshBounds = mf.sharedMesh.bounds;

					float z = meshBounds.center.z;

					Vector3[] meshCorners = new Vector3[] {
						new Vector3(meshBounds.min.x, meshBounds.min.y, z),
						new Vector3(meshBounds.max.x, meshBounds.max.y, z),
						new Vector3(meshBounds.min.x, meshBounds.max.y, z),
						new Vector3(meshBounds.max.x, meshBounds.min.y, z)
					};

					for (int c = 0; c < meshCorners.Length; ++c) {
						Vector3 meshCorner = meshCorners[c];

						Vector3 worldPos = mf.transform.TransformPoint(meshCorner);

						Vector3 localPos = relativeTo.InverseTransformPoint(worldPos);

						max = Vector3.Max(max, localPos);
						min = Vector3.Min(min, localPos);
					}
				}
			}

			Vector3 pos = go.transform.position;
			min.x = (min.x != Mathf.Infinity) ? min.x : pos.x;
			min.y = (min.y != Mathf.Infinity) ? min.y : pos.y;
			min.z = (min.z != Mathf.Infinity) ? min.z : pos.z;
			max.x = (max.x != Mathf.NegativeInfinity) ? max.x : pos.x;
			max.y = (max.y != Mathf.NegativeInfinity) ? max.y : pos.y;
			max.z = (max.z != Mathf.NegativeInfinity) ? max.z : pos.z;

			Bounds b = new Bounds();
			b.SetMinMax(min, max);
			return b;
		}

		/// <summary>
		/// Determines if any renderers in this GameObject are visible.
		/// Pass the type of Renderer you want to consider, e.g. you can
		/// pass MeshRenderer to skip ParticleRenderers.
		/// </summary>
		/// <returns><c>true</c> if this GameObject is visible; otherwise, <c>false</c>.</returns>
		/// <param name="go">The GameObject to search</param>
		/// <typeparam name="T">The type of Renderer; use Renderer for all, or more likely just MeshRenderer.</typeparam>
		public static bool IsVisible<T>(this GameObject go) where T : Renderer {

			foreach (T renderer in go.GetComponentsInChildren<T>()) {
				if (renderer.isVisible) {
					return true;
				}
			}
			return false;

		}

		/// <summary>
		/// Adds a MeshRenderer, MeshFilter (w/quad mesh) to the GameObject, if missing.
		/// </summary>
		/// <returns>The default renderer.</returns>
		/// <param name="gameObject">Game object.</param>
		public static MeshRenderer AddDefaultRenderer(this GameObject gameObject, PrimitiveType primitive = PrimitiveType.Quad) {
			
			MeshRenderer rend = gameObject.GetComponent<MeshRenderer>();
			if (!rend) {
				rend = gameObject.AddComponent<MeshRenderer>();
				rend.SetDefaults();
			}
			
			MeshFilter filter = gameObject.GetComponent<MeshFilter>();
			if (!filter) {
				filter = gameObject.AddComponent<MeshFilter>();
			}

			if (!filter.sharedMesh) {
				GameObject go = GameObject.CreatePrimitive(primitive);
				filter.sharedMesh = go.GetComponent<MeshFilter>().sharedMesh;
				if (Application.isPlaying) {
					go.SetActive(false);
					GameObject.Destroy(go);
				} else {
					GameObject.DestroyImmediate(go);
				}
			}

			return rend;
		}

		/// <summary>
		/// Sets the layer, recursively.
		/// </summary>
		/// <param name="gameObject">Game object.</param>
		/// <param name="layerId">Layer identifier.</param>
		/// <param name="recursive">If set to <c>true</c> recursive.</param>
		public static void SetLayer(this GameObject gameObject, int layerId, bool recursive = true) {
			gameObject.layer = layerId;
			if (recursive) {
				foreach (Transform child in gameObject.transform) {
					child.gameObject.SetLayer(layerId, true);
				}
			}
		}

		#endregion
		

		#region Transform

		/// <summary>
		/// Sets this Transform's parent and neutralizes the local transform values.
		/// </summary>
		/// <param name="t">T.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="localPos">Local position.</param>
		/// <param name="localScale">Local scale.</param>
		/// <param name="localEuler">Local euler.</param>
		public static Transform AffixTo(this Transform t, Transform parent) {
			
			t.parent = parent;
			if (t.parent) {
				t.localPosition = Vector3.zero;
				t.localScale = Vector3.one;
				t.localRotation = Quaternion.identity;
			}
			return t;
		}

		/// <summary>
		/// Move, scale and rotate this transform so that its MeshRenderer bounds
		/// match the source transform's.
		/// Note:  this implementation is pretty slow; better for editor/one-shot usage.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="source">Source.</param>
		public static void MatchBounds(this Transform target, Transform source) {
			
			Quaternion sourceRotation = source.rotation;
			source.rotation = Quaternion.identity;
			Bounds sourceBounds = source.gameObject.CalculateRendererBounds<MeshRenderer>();
			source.rotation = sourceRotation;

			target.localScale = Vector3.one;
			target.rotation = Quaternion.identity;

			Bounds targetBounds = target.gameObject.CalculateRendererBounds<MeshRenderer>();

			Vector3 targetScale = new Vector3(
				sourceBounds.size.x / targetBounds.size.x,
				sourceBounds.size.y / targetBounds.size.y,
				1f
				);

			target.localScale = targetScale;
			targetBounds = target.gameObject.CalculateRendererBounds<MeshRenderer>();

			sourceBounds = source.gameObject.CalculateRendererBounds<MeshRenderer>();

			target.RotateAround(targetBounds.center, Vector3.forward, sourceRotation.eulerAngles.z);
			target.position += (sourceBounds.center - targetBounds.center);
		}

		#endregion


		#region Bounds

		/// <summary>
		/// Checks for overlap (intersection) of this Bounds with another
		/// Bounds, but sets them to the same Z position (i.e. ignores Z
		/// differences).
		/// </summary>
		public static bool Intersects2D(this Bounds bounds, Bounds otherBounds) {
			Vector3 center = otherBounds.center;
			center.z = bounds.center.z;
			otherBounds.center = center;
			return bounds.Intersects(otherBounds);
		}

		/// <summary>
		/// Check if the bounds contains the given point, ignoring Z.
		/// </summary>
		public static bool Contains2D(this Bounds bounds, Vector2 position) {
			return bounds.Contains2D((Vector3)position);
		}

		/// <summary>
		/// Check if the bounds contains the given point, ignoring Z.
		/// </summary>
		public static bool Contains2D(this Bounds bounds, Vector3 position) {
			position.z = bounds.center.z;
			return bounds.Contains(position);
		}

		/// <summary>
		/// Check if the bounds contains the entire other bounds, ignoring Z.
		/// </summary>
		public static bool Contains2D(this Bounds bounds, Bounds otherBounds) {
			float z = bounds.center.z;
			return 
				bounds.Contains(new Vector3(otherBounds.min.x, otherBounds.min.y, z)) &&
				bounds.Contains(new Vector3(otherBounds.min.x, otherBounds.max.y, z)) &&
				bounds.Contains(new Vector3(otherBounds.max.x, otherBounds.min.y, z)) &&
				bounds.Contains(new Vector3(otherBounds.max.x, otherBounds.max.y, z));
		}

		/// <summary>
		/// The square distance from the position to the bounds, ignoring Z.
		/// </summary>
		public static float SqrDistance2D(this Bounds bounds, Vector2 position) {
			bounds = FlattenBounds(bounds, 0f);
			return bounds.SqrDistance((Vector3)position);
		}

		/// <summary>
		/// The square distance from the position to the bounds, ignoring Z.
		/// </summary>
		public static float SqrDistance2D(this Bounds bounds, Vector3 position) {
			return SqrDistance2D(bounds, (Vector2)position);
		}

		/// <summary>
		/// Flattens the bounds and places it at the given depth.
		/// </summary>
		/// <returns>The bounds.</returns>
		/// <param name="bounds">Bounds.</param>
		/// <param name="z">The z coordinate.</param>
		public static Bounds FlattenBounds(this Bounds bounds, float z) {
			Vector3 pos	= bounds.center;
			pos.z = z;
			bounds.center = pos;

			Vector3 size = bounds.size;
			size.z = 0f;
			bounds.size = size;

			return bounds;
		}

		/// <summary>
		/// Clips the line segment specified by p0 and p1 against 
		/// the bounds, ignoring Z.
		/// </summary>
		/// <returns><c>true</c>, if line touches bounds at all, <c>false</c> otherwise.</returns>
		public static bool ClipLine2D(this Bounds bounds, ref Vector3 p0, ref Vector3 p1) {

			Vector2 v0 = (Vector2)p0;
			Vector2 v1 = (Vector2)p1;

			bool segmentIntersectsBounds = ClipSegment(bounds, ref v0, ref v1);
			if (segmentIntersectsBounds) {
				p0 = new Vector3(v0.x, v0.y, p0.z);
				p1 = new Vector3(v1.x, v1.y, p1.z);
			}
			return segmentIntersectsBounds;
		}


		#region Cohen-Sutherland

		private static bool ClipSegment(Bounds bounds, ref Vector2 p0, ref Vector2 p1) {
			
			var outCode0 = ComputeOutCode(p0, bounds);
			var outCode1 = ComputeOutCode(p1, bounds);
			var intersectsBounds = false;

			while (true) {
				
				// Case 1: both endpoints are inside the clipping region
				if ((outCode0 | outCode1) == OutCode.Inside) {
					intersectsBounds = true;
					break;
				}

				// Case 2: both endpoints share an excluded region, 
				//         so it is impossible for a line between 
				//         them to be within the clipping region
				if ((outCode0 & outCode1) != 0) {
					break;
				}

				// Case 3: the endpoints are in different regions,
				//         so clip against any outcode that is not inside
				if (outCode0 != OutCode.Inside) {
					p0 = CalculateIntersection(bounds, p0, p1, outCode0);
					outCode0 = ComputeOutCode(p0, bounds);
				} else {
					p1 = CalculateIntersection(bounds, p0, p1, outCode1);
					outCode1 = ComputeOutCode(p1, bounds);
				}

			}

			return intersectsBounds;
		}

		/// <summary>
		/// OutCodes for Cohen-Sutherland clipping
		/// </summary>
		[System.Flags]
		private enum OutCode {
			Inside = 0,
			Left = 1,
			Right = 2,
			Bottom = 4,
			Top = 8
		}

		private static bool HasFlag(this OutCode flags, OutCode flag) {
			return (flags & flag) != 0;
		}

		private static OutCode ComputeOutCode(Vector2 point, Bounds bounds) {
			var code = OutCode.Inside;
			if (point.x < bounds.min.x) {
				code |= OutCode.Left;	
			} else if (point.x > bounds.max.x) {
				code |= OutCode.Right;
			}
			if (point.y < bounds.min.y) {
				code |= OutCode.Bottom;
			} else if (point.y > bounds.max.y) {
				code |= OutCode.Top;
			}
			return code;
		}

		/// Clip segment between two points against the clipTo OutCode of one of them
		private static Vector2 CalculateIntersection(Bounds bounds, Vector2 p0, Vector2 p1, OutCode clipTo) {
			var dx = (p1.x - p0.x);
			var dy = (p1.y - p0.y);

			if (clipTo.HasFlag(OutCode.Top)) {
				float top = bounds.max.y;
				var slopeY = dx / dy; // slope to use for possibly-vertical lines
				return new Vector2(
					p0.x + slopeY * (top - p0.y),
					top
				);
			} else if (clipTo.HasFlag(OutCode.Bottom)) {
				float bottom = bounds.min.y;
				var slopeY = dx / dy; // slope to use for possibly-vertical lines
				return new Vector2(
					p0.x + slopeY * (bottom - p0.y),
					bottom
				);
			} else if (clipTo.HasFlag(OutCode.Right)) {
				float right = bounds.max.x;
				var slopeX = dy / dx; // slope to use for possibly-horizontal lines
				return new Vector2(
					right,
					p0.y + slopeX * (right - p0.x)
				);
			} else if (clipTo.HasFlag(OutCode.Left)) {
				float left = bounds.min.x;
				var slopeX = dy / dx; // slope to use for possibly-horizontal lines
				return new Vector2(
					left,
					p0.y + slopeX * (left - p0.x)
				);
			} else {
				throw new System.ArgumentOutOfRangeException("clipTo = " + clipTo);
			}
		}

		#endregion


		#endregion


		#region Vector

		public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion angle) {
			return angle * (point - pivot) + pivot;
		}

		public static Vector3 RotateAroundZ(this Vector3 point, Vector3 pivot, float degrees) {
			return point.RotateAround(pivot, Quaternion.Euler(0f, 0f, degrees));
		}

		public static Vector2 RotateAroundZ(this Vector2 point, Vector2 pivot, float degrees) {
			return (Vector2)(((Vector3)point).RotateAroundZ((Vector3)pivot, degrees));
		}

		#endregion


		#region SagoMesh

		/// <summary>
		/// Calculates the full bounds of every frame of every animator in this mux.
		/// </summary>
		/// <returns>The full bounds.</returns>
		/// <param name="mux">Mux.</param>
		public static Bounds CalcFullBounds(this MeshAnimatorMultiplexer mux) {
			Bounds bounds = new Bounds(mux.transform.position, Vector3.zero);
			foreach (MeshAnimator animator in mux.Animators) {
				bounds.Encapsulate(animator.CalcFullBounds());
			}
			return bounds;
		}

		/// <summary>
		/// Calculates the full bounds of every frame of the animation.
		/// </summary>
		/// <returns>The full bounds.</returns>
		/// <param name="animator">Animator.</param>
		public static Bounds CalcFullBounds(this MeshAnimator animator) {
			
			Transform animT = animator.transform;
			Bounds bounds = new Bounds(animT.position, Vector3.zero);

			foreach (MeshAnimationFrame frame in animator.Animation.Frames) {
				foreach (var mesh in frame.Meshes) {
					Bounds meshBounds = mesh.bounds;

					Vector3[] corners = new Vector3[] {
						meshBounds.min,
						meshBounds.max,
						new Vector3(meshBounds.min.x, meshBounds.max.y),
						new Vector3(meshBounds.max.x, meshBounds.min.y)
					};

					Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, 1f);
					Vector3 max = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, 1f);

					for (int i = 0; i < corners.Length; ++i) {
						corners[i] = animT.TransformPoint(corners[i]);

						min = Vector3.Min(min, corners[i]);
						max = Vector3.Max(max, corners[i]);
					}

					meshBounds.SetMinMax(min, max);
					bounds.Encapsulate(meshBounds);
				}
			}

			return bounds;
		}

		#endregion


		#region Camera

		public static Bounds BoundsAtZ(this Camera camera, float zPos) {
			float z = zPos - camera.transform.position.z;
			Vector3 max = camera.ViewportToWorldPoint(new Vector3(1f, 1f, z));
			Vector3 min = camera.ViewportToWorldPoint(new Vector3(0f, 0f, z));
			Bounds b = new Bounds();
			b.SetMinMax(min, max);
			return b;
		}

		#endregion


		#region MeshRenderer

		/// <summary>
		/// Sets the rendering parameters to the normal ones we use.
		/// In the editor, it will also set the material if it is missing.
		/// </summary>
		/// <param name="rend">Rend.</param>
		public static void SetDefaults(this MeshRenderer rend) {
			rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			rend.receiveShadows = false;
			rend.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			rend.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            rend.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

			#if UNITY_EDITOR
			if (!rend.sharedMaterial) {
				if (AssetDatabase.IsValidFolder("Assets/Framework")) {
					rend.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Framework/SagoMesh/Materials/OpaqueMesh.mat");
				} else {
					rend.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/External/SagoMesh/Materials/OpaqueMesh.mat");
				}
			}
			#endif
		}

		#if UNITY_EDITOR
		[MenuItem("CONTEXT/MeshRenderer/Sago Reset", false, -1000000)]
		private static void MenuSagoifyRenderer(MenuCommand command) {
			(command.context as MeshRenderer).SetDefaults();
		}
		#endif

		#endregion


		#region MeshFilter

		/// <summary>
		/// Calculate the bounds of this mesh relative to another transform,
		/// i.e. in local space to that transform.
		/// </summary>
		/// <returns>The bounds in local space of the relativeTo Transform</returns>
		/// <param name="mf">The MeshFilter of the object showing the mesh</param>
		/// <param name="relativeTo">The Transfrom that the result is relative to.</param>
		public static Bounds CalcLocalMeshBounds2D(this MeshFilter mf, Transform relativeTo) {

			relativeTo = relativeTo ?? mf.transform;

			if (!mf.sharedMesh) {
				return new Bounds(relativeTo.InverseTransformPoint(mf.transform.position), Vector3.zero);
			}

			Bounds meshBounds = mf.sharedMesh.bounds;
			float z = meshBounds.center.z;

			Vector3[] meshCorners = new Vector3[] {
				new Vector3(meshBounds.min.x, meshBounds.min.y, z),
				new Vector3(meshBounds.max.x, meshBounds.max.y, z),
				new Vector3(meshBounds.min.x, meshBounds.max.y, z),
				new Vector3(meshBounds.max.x, meshBounds.min.y, z)
			};

			Vector3 localMin;
			localMin = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

			Vector3 localMax;
			localMax = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);

			for (int i = 0; i < meshCorners.Length; ++i) {
				Vector3 meshCorner = meshCorners[i];

				Vector3 worldPos = mf.transform.TransformPoint(meshCorner);

				Vector3 localPos = relativeTo.InverseTransformPoint(worldPos);

				localMax = Vector3.Max(localMax, localPos);
				localMin = Vector3.Min(localMin, localPos);
			}

			Bounds bounds = new Bounds();
			bounds.SetMinMax(localMin, localMax);
			return bounds;
		}

		/// <summary>
		/// Area of the transformed mesh in this MeshFilter.
		/// </summary>
		public static float Area(this MeshFilter mf) {
			return Area(mf.sharedMesh, mf.transform);
		}

		#endregion


		#region Mesh

		/// <summary>
		/// Area the specified mesh under the given transform.
		/// </summary>
		public static float Area(this UnityEngine.Mesh mesh, Transform transform) {
			float area = 0f;
			if (mesh && transform) {
				Vector3[] vertices = mesh.vertices;
				int[] triangles = mesh.triangles;
				for (int tIdx = 0; tIdx < triangles.Length; tIdx += 3) {
					Vector3 v0 = transform.TransformPoint(vertices[triangles[tIdx]]);
					Vector3 v1 = transform.TransformPoint(vertices[triangles[tIdx + 1]]);
					Vector3 v2 = transform.TransformPoint(vertices[triangles[tIdx + 2]]);
					area += TriangleArea(v0, v1, v2);
				}
			}
			return area;
		}

		/// <summary>
		/// Area the specified mesh under no transformation.
		/// </summary>
		public static float Area(this UnityEngine.Mesh mesh) {
			float area = 0f;
			if (mesh) {
				Vector3[] vertices = mesh.vertices;
				int[] triangles = mesh.triangles;
				for (int tIdx = 0; tIdx < triangles.Length; tIdx += 3) {
					Vector3 v0 = vertices[triangles[tIdx]];
					Vector3 v1 = vertices[triangles[tIdx + 1]];
					Vector3 v2 = vertices[triangles[tIdx + 2]];
					area += TriangleArea(v0, v1, v2);
				}
			}
			return area;
		}

		private static float TriangleArea(Vector3 A, Vector3 B, Vector3 C) {
			Vector3 AB = B - A;
			Vector3 AC = C - A;
			return 0.5f * Vector3.Cross(AB, AC).magnitude;
		}

		#endregion


		#region Color

		public static bool EqualTo(this Color32 a, Color32 b) {
			return a.a == b.a && a.r == b.r && a.g == b.g && a.b == b.b;
		}

		#endregion

		
	}
	
}
