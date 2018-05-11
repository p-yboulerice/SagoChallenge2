namespace SagoBiz {

	using SagoLayout;
	using System.Collections;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// PromoState defines the possible states for the <see cref="Promo" /> class.
	/// </summary>
	public enum PromoState {
		Unknown,
		Loading,
		Complete,
		Error
	}
	
	/// <summary>
	/// The Promo class provides functionality for loading the promo content and turning it on and off.
	/// </summary>
	public class Promo : MonoBehaviour {
		
		
		#region Types
		
		/// <summary>
		/// The method signature for event handlers for the <see cref="Promo" /> component.
		/// </summary>
		public delegate void PromoCallback(Promo promo);
		
		#endregion
		
		
		#region Events
		
		/// <summary>
		/// Adds and removes event handlers to call when the component finishes loading.
		/// </summary>
		public event PromoCallback OnComplete;
		
		/// <summary>
		/// Adds and removes event handlers to call when the component has an error while loading.
		/// </summary>
		public event PromoCallback OnError;
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// Cached reference to the <see cref="Content" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Content m_Content;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets a reference to the <see cref="Content" /> component.
		/// </summary>
		public Content Content {
			get {
				m_Content = m_Content ?? this.GetComponentInChildren<Content>();
				return m_Content;
			}
		}
		
		/// <summary>
		/// Gets and sets the error that occured while loading.
		/// </summary>
		public string Error {
			get; protected set;
		}
		
		/// <summary>
		/// Gets and sets the texture loaded from the <see cref="ImageUrl" />.
		/// </summary>
		public Texture2D Image {
			get; protected set;
		}
		
		/// <summary>
		/// Gets the url of the button image (not available until the component has finished loading).
		/// </summary>
		public string ImageUrl {
			get {
				if (Controller.Instance && Controller.Instance.Service && Controller.Instance.Service.PromoData != null) {
					return Controller.Instance.Service.PromoData.ImageUrl;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets and sets the sprite created using the <see cref="Image" />.
		/// </summary>
		public Sprite Sprite {
			get; protected set;
		}
		
		/// <summary>
		/// Gets and sets the current state.
		/// </summary>
		public PromoState State {
			get; protected set;
		}
		
		#endregion
		
		
		#region Public Methods

		public void Awake() {
			Artboard artboard;
			artboard = this.GetComponentInChildren<Artboard>();
			artboard.Size = new Vector2(Screen.width, Screen.height);
		}

		/// <summary>
		/// Clears the component and resets it to the default state.
		/// </summary>
		public void Clear() {
			
			this.StopAllCoroutines();
			
			this.Content.Clear();
			Destroy(this.Image);
			Destroy(this.Sprite);
			
			this.Error = null;
			this.Image = null;
			this.Sprite = null;
			this.State = PromoState.Unknown;
			
		}
		
		/// <summary>
		/// Loads the component.
		/// </summary>
		public void Load() {
			if (this.State == PromoState.Unknown) {
				this.StartCoroutine(this.LoadAsync());
			}
		}
		
		/// <summary>
		/// Turns the component's <see cref="Content" /> on.
		/// </summary>
		public void TurnContentOn(bool animated) {
			Debug.Log ("Promo -> Turn Content on " + ((animated) ? "animated" : ""));
			this.Content.TurnOn(animated);
		}
		
		/// <summary>
		/// Turns the component's <see cref="Content" /> off.
		/// </summary>
		public void TurnContentOff(bool animated) {
			Debug.Log ("Promo -> Turn Content off " + ((animated) ? "animated" : ""));
			this.Content.TurnOff(animated);
		}
		
		#endregion
		
		
		#region Coroutine Methods
		
		/// <summary>
		/// Loads the component asynchronously.
		/// </summary>
		protected IEnumerator LoadAsync() {
			
			this.State = PromoState.Loading;
			
			if (this.State != PromoState.Error) {
				yield return this.StartCoroutine(this.LoadImageAsync());
			}
			
			if (this.State != PromoState.Error) {
				yield return this.StartCoroutine(this.LoadWebViewAsync());
			}
			
			if (this.State != PromoState.Error) {
				this.State = PromoState.Complete;
				if (this.OnComplete != null) {
					this.OnComplete(this);
				}
				yield break;
			}
			
			if (this.OnError != null) {
				this.OnError(this);
			}
			yield break;
			
		}
		
		/// <summary>
		/// Loads the component's image asynchronously.
		/// </summary>
		protected IEnumerator LoadImageAsync() {
			
			WWW www;
			www = new WWW(this.ImageUrl);
			
			yield return www;
			
			if (!string.IsNullOrEmpty(www.error)) {
				this.Error = string.Format("Could not download image: {0}", www.error);
				this.State = PromoState.Error;
				yield break;
			}
			
			try {
				this.Image = new Texture2D(www.texture.width, www.texture.height, www.texture.format, false);
				www.LoadImageIntoTexture(this.Image);
			} catch (System.Exception e) {
				Destroy(this.Image);
				this.Image = null;
				this.Error = string.Format("Could not load image into texture: {0}", e);
				this.State = PromoState.Error;
				yield break;
			}
			
			try {
				this.Sprite = Sprite.Create(
					this.Image,
					new Rect(0,0,this.Image.width, this.Image.height), 
					Vector2.right, 
					1
				);
			} catch (System.Exception e) {
				Destroy(this.Sprite);
				this.Sprite = null;
				this.Error = string.Format("Could not create sprite: {0}", e);
				this.State = PromoState.Error;
				yield break;
			}
			
			SpriteRenderer spriteRenderer;
			spriteRenderer = this.Content.GetComponentInChildren<SpriteRenderer>();
			spriteRenderer.sprite = this.Sprite;
		
			switch (this.Content.State) {
				case ContentState.Unknown:
				case ContentState.Off:
				case ContentState.TurnOff:
					this.TurnContentOff(false);
				break;
			}
			
		}
		
		/// <summary>
		/// Loads the component's web view.
		/// </summary>
		protected IEnumerator LoadWebViewAsync() {
			
			Native native;
			native = Controller.Instance ? Controller.Instance.Native : null;
			
			while (native != null) {
				
				native.PreLoadPromoWebView();

				// error
				if (!string.IsNullOrEmpty(native.PromoWebViewError)) {
					this.Error = native.PromoWebViewError;
					this.State = PromoState.Error;
					yield break;
				}
				
				// success
				if (native.PromoWebViewReady) {
					yield break;
				}
				
				// not supported
				if (!native.PromoWebViewSupported) {
					yield break;
				}
				
				// wait
				yield return null;
				
			}
			
		}
		
		#endregion
		
		
	}

}