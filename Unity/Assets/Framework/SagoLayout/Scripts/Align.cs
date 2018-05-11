namespace SagoLayout {

	using UnityEngine;
	
	public enum AlignMode {
		None,
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}
	
	public class Align : ArtboardElement {
		
		
		#region Fields
		
		[SerializeField]
		protected AlignMode m_Mode;
		
		[SerializeField]
		protected Vector2 m_Padding;

		[SerializeField]
		protected bool m_IgnoreSafeAreaInsets;
		
		[System.NonSerialized]
		protected Scale m_Scale;
		
		#endregion
		
		
		#region Properties
		
		override public Bounds Bounds {
			get {
				
				Vector3 padding = Padding;
				padding *= Scale ? Scale.Factor : 1f;

				Bounds bounds = base.Bounds;
				bounds.extents += padding;

				
				return bounds;
				
			}
		}
		
		public AlignMode Mode {
			get { return m_Mode; }
			set { m_Mode = value; }
		}
		
		public Vector2 Padding {
			get { return m_Padding; }
			set { m_Padding = value; }
		}

		public bool IgnoreSafeAreaInsets {
			get { return m_IgnoreSafeAreaInsets; }
		}
		
		public Scale Scale {
			get {
				m_Scale = m_Scale ?? GetComponent<Scale>();
				return m_Scale;
			}
			set {
				m_Scale = value;
			}
		}

		#endregion

		
		#region MonoBehaviour Methods
		
		override protected void Reset() {
			base.Reset();
			Mode = AlignMode.None;
			Padding = Vector2.zero;
		}
		
		#endregion
		
		
		#region Public Methods
		
		override public void Apply() {
			
			Bounds canvas;
			canvas = Artboard.Bounds;
			
			Bounds bounds;
			bounds = Bounds;
			
			Vector3 offset;
			offset = Transform.position - bounds.center;

			Vector2 ratio;
			ratio = new Vector2(this.Artboard.Size.x / Screen.width, this.Artboard.Size.y / Screen.height);

			RectOffset insets;
			insets = SafeArea.GetSafeAreaInsets();
			float leftInset = insets.left * (this.IgnoreSafeAreaInsets ? 0.0f : ratio.x);
			float bottomInset = insets.bottom * (this.IgnoreSafeAreaInsets ? 0.0f : ratio.y);
			float rightInset = insets.right * (this.IgnoreSafeAreaInsets ? 0.0f : ratio.x);
			float topInset = insets.top * (this.IgnoreSafeAreaInsets ? 0.0f : ratio.y);

			// we don't care about the top inset since we hide the status bar
			topInset = 0;
			
			switch (Mode) {
				case AlignMode.TopLeft:
					Transform.position = new Vector3(
						canvas.min.x + bounds.extents.x + offset.x + leftInset,
						canvas.max.y - bounds.extents.y + offset.y - topInset,
						Transform.position.z
					);
				break;
				case AlignMode.TopCenter:
					Transform.position = new Vector3(
						canvas.center.x + offset.x,
						canvas.max.y - bounds.extents.y + offset.y - topInset,
						Transform.position.z
					);
				break;
				case AlignMode.TopRight:
					Transform.position = new Vector3(
						canvas.max.x - bounds.extents.x + offset.x - rightInset,
						canvas.max.y - bounds.extents.y + offset.y - topInset,
						Transform.position.z
					);
				break;
				case AlignMode.MiddleLeft:
					Transform.position = new Vector3(
						canvas.min.x + bounds.extents.x + offset.x + leftInset,
						canvas.center.y + offset.y,
						Transform.position.z
					);
				break;
				case AlignMode.MiddleCenter:
					Transform.position = new Vector3(
						canvas.center.x + offset.x,
						canvas.center.y + offset.y,
						Transform.position.z
					);
				break;
				case AlignMode.MiddleRight:
					Transform.position = new Vector3(
						canvas.max.x - bounds.extents.x + offset.x - rightInset,
						canvas.center.y + offset.y,
						Transform.position.z
					);
				break;
				case AlignMode.BottomLeft:
					Transform.position = new Vector3(
						canvas.min.x + bounds.extents.x + offset.x + leftInset,
						canvas.min.y + bounds.extents.y + offset.y,
						Transform.position.z
					);
				break;
				case AlignMode.BottomCenter:
					Transform.position = new Vector3(
						canvas.center.x + offset.x,
						canvas.min.y + bounds.extents.y + offset.y + bottomInset,
						Transform.position.z
					);
				break;
				case AlignMode.BottomRight:
					Transform.position = new Vector3(
						canvas.max.x - bounds.extents.x + offset.x - rightInset,
						canvas.min.y + bounds.extents.y + offset.y,
						Transform.position.z
					);
				break;
			}
			
		}
		
		#endregion
		
		
	}
	
}
