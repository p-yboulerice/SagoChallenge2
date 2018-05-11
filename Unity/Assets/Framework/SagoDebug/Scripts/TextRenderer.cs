namespace SagoDebug {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Draws 3D text labels for one frame (i.e. call from Update).
	/// Used by SagoDebug.Label, but you could make your own.
	/// </summary>
	public class TextRenderer {


		#region Construction

		public TextRenderer(int fontSize = 36) : this(DefaultFontName, fontSize) {
			
		}

		public TextRenderer(string fontName, int fontSize) {
			this.Font = Font.CreateDynamicFontFromOSFont(fontName, fontSize) ?? Font.CreateDynamicFontFromOSFont(DefaultFontName, fontSize);
			Font.textureRebuilt += OnFontTextureRebuilt;
			this.FontMaterialPropertyBlock = new MaterialPropertyBlock();
			this.ColorPropertyId = Shader.PropertyToID("_Color");
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Draw the specified text at worldPosition, in the given color, and 
		/// at the given world size.
		/// </summary>
		public void Draw(string text, Vector3 worldPosition, Color color, float size = 1f) {
			Matrix4x4 mat = Matrix4x4.TRS(worldPosition, Quaternion.identity, new Vector3(size, size, size));
			Draw(text, mat, color);
		}

		/// <summary>
		/// Draw the specified text using the given transformation matrix,
		/// in the given color.
		/// </summary>
		public void Draw(string text, Matrix4x4 mat, Color color) {
			Message message = new Message(text, mat, color);
			WaitingToDraw.Add(message);
			AddCharacters(message.Text);
			DrawMessage(message);
		}

		#endregion


		#region Internal


		#region Types

		class CharMesh {
			public CharacterInfo CharInfo;
			public Mesh Mesh;
		}

		struct Message {
			public readonly string Text;
			public readonly Matrix4x4 Transform;
			public readonly Color Color;

			public Message(string text, Matrix4x4 transform, Color color) {
				this.Text = text;
				this.Transform = transform;
				this.Color = color;
			}
		}

		#endregion


		#region Fields

		const string DefaultFontName = "Courier";

		Dictionary<char, CharMesh> s_CharMeshMap = new Dictionary<char, CharMesh>();
		List<Message> s_WaitingToDraw = new List<Message>();

		#endregion


		#region Properties

		Dictionary<char, CharMesh> CharMeshMap {
			get {
				return s_CharMeshMap;
			}
		}

		int ColorPropertyId {
			get;
			set;
		}

		Font Font {
			get;
			set;
		}

		MaterialPropertyBlock FontMaterialPropertyBlock {
			get;
			set;
		}

		List<Message> WaitingToDraw {
			get {
				return s_WaitingToDraw;
			}
		}

		#endregion


		#region Methods

		static Mesh CreateMesh(CharacterInfo ch, float scale) {
			Mesh mesh = new Mesh();
			var vertices = new Vector3[4];
			var uv = new Vector2[4];

			SetVertices(vertices, ch, scale);
			SetUV(uv, ch);

			mesh.vertices = vertices;
			mesh.triangles = new int[]{ 0, 1, 2, 0, 2, 3 };
			mesh.uv = uv;

			return mesh;
		}

		static void UpdateMesh(Mesh mesh, CharacterInfo ch, float scale) {

			var vertices = mesh.vertices;
			var uv = mesh.uv;

			SetVertices(vertices, ch, scale);
			SetUV(uv, ch);

			mesh.vertices = vertices;
			mesh.uv = uv;

		}

		static void SetVertices(Vector3[] vertices, CharacterInfo ch, float scale) {
			vertices[0] = scale * new Vector3(ch.minX, ch.maxY, 0);
			vertices[1] = scale * new Vector3(ch.maxX, ch.maxY, 0);
			vertices[2] = scale * new Vector3(ch.maxX, ch.minY, 0);
			vertices[3] = scale * new Vector3(ch.minX, ch.minY, 0);
		}

		static void SetUV(Vector2[] uv, CharacterInfo ch) {
			uv[0] = ch.uvTopLeft;
			uv[1] = ch.uvTopRight;
			uv[2] = ch.uvBottomRight;
			uv[3] = ch.uvBottomLeft;
		}

		static float GetScale(Font font) {
			return 1.0f / (float)font.lineHeight;
		}


		void OnFontTextureRebuilt(Font changedFont) {
			if (changedFont != this.Font)
				return;

			RebuildMeshes();

			DrawMessages();
		}

		void DrawMessages() {
			for (int i = 0; i < this.WaitingToDraw.Count; ++i) {
				DrawMessage(this.WaitingToDraw[i]);
			}
			this.WaitingToDraw.Clear();
		}

		void DrawMessage(Message message) {
			Vector3 pos = Vector3.zero;
			float scale = GetScale(this.Font);

			for (int i = 0; i < message.Text.Length; ++i) {
				CharMesh charMesh;
				if (this.CharMeshMap.TryGetValue(message.Text[i], out charMesh)) {
					if (charMesh.Mesh) {
						Matrix4x4 mat = message.Transform * Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
						this.FontMaterialPropertyBlock.SetColor(this.ColorPropertyId, message.Color);
						Graphics.DrawMesh(charMesh.Mesh, mat, this.Font.material, 0, null, 0, this.FontMaterialPropertyBlock,
							false, false, false);

						pos += new Vector3(charMesh.CharInfo.advance * scale, 0, 0);
					}
				}
			}
		}

		void AddCharacters(string text) {
			
			this.Font.RequestCharactersInTexture(text);

			float scale = GetScale(this.Font);

			for (int i = 0; i < text.Length; ++i) {
				CharMesh charMesh;
				if (!this.CharMeshMap.TryGetValue(text[i], out charMesh)) {
					charMesh = new CharMesh();

					CharacterInfo ch;
					if (this.Font.GetCharacterInfo(text[i], out ch)) {
						charMesh.CharInfo = ch;
						charMesh.Mesh = CreateMesh(ch, scale);
					}

					this.CharMeshMap.Add(text[i], charMesh);
				}
			}
		}

		void RebuildMeshes() {

			float scale = GetScale(this.Font);

			List<char> removed = new List<char>();

			foreach (var kvp in CharMeshMap) {

				char character = kvp.Key;

				CharacterInfo charInfo;
				if (this.Font.GetCharacterInfo(character, out charInfo)) {
					kvp.Value.CharInfo = charInfo;

					Mesh mesh = kvp.Value.Mesh;
					if (mesh) {
						UpdateMesh(mesh, charInfo, scale);
					} else {
						kvp.Value.Mesh = CreateMesh(charInfo, scale);
					}

				} else {
					removed.Add(character);
				}

			}

			//if (removed.Count > 0) {
			//	Debug.LogFormat("Font removed characters: {0}", new string(removed.ToArray()));
			//}

			foreach (var character in removed) {
				CharMeshMap.Remove(character);
			}

		}

		#endregion


		#endregion


	}
}