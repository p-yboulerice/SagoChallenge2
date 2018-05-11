namespace SagoPhoto {
	
	using SagoEasing;
	using UnityEngine;
	using System.Collections;
	
	public enum PhotoFlashState {
		Unknown,
		Black,
		White,
		Fade,
		Complete
	}

	/// <summary>
	/// A black then white fullscreen 'flash'.  Call PhotoFlash.Create().
	/// </summary>
	public class PhotoFlash : MonoBehaviour {
		
		
		#region Public Methods
		
		/// <summary>
		/// Creates a flash in front of the given camera the specified distance.
		/// It will last for a short time and destroy itself.
		/// </summary>
		/// <param name="camera">Camera.</param>
		/// <param name="camZ">PhotoFlash Z relative to camera.</param>
		public static PhotoFlash Create(Camera camera, float camZ, System.Action completeCallback = null) {
			
			GameObject go = new GameObject("PhotoFlash");
			Transform goT = go.GetComponent<Transform>();
			goT.parent = camera.GetComponent<Transform>();
			goT.localPosition = new Vector3(0.0f, 0.0f, camZ);
			
			PhotoFlash flash = go.AddComponent<PhotoFlash>();
			flash.Camera = camera;
			flash.OnComplete = completeCallback;
			return flash;
		}
		
		#endregion
		
		
		#region MonoBehaviour

		virtual protected IEnumerator Start() {
			
			GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			Transform quadTransform = quad.GetComponent<Transform>();
			quadTransform.parent = this.Transform;
			quadTransform.localPosition = Vector3.zero;
			
			quad.GetComponent<MeshCollider>().enabled = false;
			
			MeshRenderer quadRenderer = quadTransform.GetComponent<MeshRenderer>();
			quadRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			quadRenderer.receiveShadows = false;
			quadRenderer.material = CreateMaterial();

			// need to calculate scale to that it fills entire screen
			Bounds bounds = quadRenderer.bounds;
			Vector3 pCorner = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, this.Transform.localPosition.z));
			Vector3 pMid = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, this.Transform.localPosition.z));
			float xScale = (pCorner.x - pMid.x) / bounds.extents.x;
			float yScale = (pCorner.y - pMid.y) / bounds.extents.y;
			quadTransform.localScale = new Vector3(xScale, yScale, 1.0f);

			// flash black, then white
			// this.State = PhotoFlashState.Black;
			// quadRenderer.material.color = Color.white;
			// yield return new WaitForSeconds(1f / 20.0f);
			
			this.State = PhotoFlashState.White;
			quadRenderer.material.color = Color.white;
			yield return new WaitForSeconds(1f / 20.0f);
			
			this.State = PhotoFlashState.Fade;
			Color color = Color.white;
			float elapsed = 0f;
			float duration = 0.5f;
			while (elapsed < duration) {
				elapsed += Time.deltaTime;
				color.a = Cubic.EaseIn(1f, 0f, Mathf.Clamp(elapsed / duration, 0f, 1f));
				quadRenderer.material.color = color;
				yield return null;
			}
			
			this.State = PhotoFlashState.Complete;
			if (this.OnComplete != null) {
				this.OnComplete();
			}
			Destroy(this.gameObject);
			
		}

		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		protected Camera m_Camera;
		
		[System.NonSerialized]
		protected Transform m_Transform;
		
		#endregion
		
		
		#region Internal Properties
		
		virtual protected Camera Camera {
			get {
				if (!m_Camera) {
					m_Camera = Camera.main;
				}
				return m_Camera;
			}
			set {
				m_Camera = value;
			}
		}
		
		virtual protected System.Action OnComplete {
			get;
			set;
		}
		
		virtual protected PhotoFlashState State {
			get; 
			set;
		}
		
		virtual protected Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region Internal Methods

		virtual protected Material CreateMaterial() {

			// dangerous if this material isnt used in scene
			return new Material(Shader.Find("Sago/Transparent Color"));
		}

		#endregion
		
		
	}
}