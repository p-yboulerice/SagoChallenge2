namespace SagoLayout {

	using UnityEngine;

	public class Anchor : ArtboardElement {
		
		
		//
		// Alignments
		//
		public enum Alignments {
			TopLeft,
			TopCenter,
			TopRight,
			BottomLeft,
			BottomCenter,
			BottomRight
		}
		
		
		//
		// Inspector Properties
		//
		public Alignments Alignment;
		public bool IsDeviceSpecific;
		public Vector2 MarginInPointsKindle;
		public Vector2 MarginInPointsPad;
		public Vector2 MarginInPointsPhone;


		//
		// MonoBehaviour
		//
		override protected void Reset() {
			base.Reset();
			this.Alignment = Anchor.Alignments.TopLeft;
			this.IsDeviceSpecific = false;
			this.MarginInPointsKindle = Vector2.zero;
			this.MarginInPointsPad = Vector2.zero;
			this.MarginInPointsPhone = Vector2.zero;
		}
		
		
		//
		// Public Methods
		//
		override public void Apply() {
			
			Bounds bounds;
			bounds = this.Bounds;
			
			Vector3 anchor;
			anchor = bounds.center;
			
			Vector2 delta;
			delta = Vector2.zero;
			
			switch (this.Alignment) {
			case Anchor.Alignments.BottomCenter:
				anchor += bounds.extents.y * Vector3.down;
				delta = this.BottomCenter - anchor;
				break;
			case Anchor.Alignments.BottomLeft:
				anchor += bounds.extents.y * Vector3.down + bounds.extents.x * Vector3.left;
				delta = this.BottomLeft - anchor;
				break;
			case Anchor.Alignments.BottomRight:
				anchor += bounds.extents.y * Vector3.down + bounds.extents.x * Vector3.right;
				delta = this.BottomRight - anchor;
				break;
			case Anchor.Alignments.TopCenter:
				anchor += bounds.extents.y * Vector3.up;
				delta = this.TopCenter - anchor;
				break;
			case Anchor.Alignments.TopLeft:
				anchor += bounds.extents.y * Vector3.up + bounds.extents.x * Vector3.left;
				delta = this.TopLeft - anchor;
				break;
			case Anchor.Alignments.TopRight:
				anchor += bounds.extents.y * Vector3.up + bounds.extents.x * Vector3.right;
				delta = this.TopRight - anchor;
				break;
			default:
				break;
			}
			
			this.Transform.position += (Vector3)delta;
			
		}
		

		//
		// Helper
		//
		private Vector2 Margin {
			get { return this.MarginInPoints * (this.ArtboardHeight / ScreenUtility.HeightInPoints); }
		}

		private Vector2 MarginInPoints {
			get { return (this.IsDeviceSpecific && !ScreenUtility.IsPhone) ? (ScreenUtility.IsKindle ? this.MarginInPointsKindle : this.MarginInPointsPad) : this.MarginInPointsPhone; }
		}

		private Vector3 BottomCenter {
			get { return this.Artboard.Bounds.center + (this.Artboard.Bounds.extents.y - this.Margin.y) * Vector3.down; }
		}
		
		private Vector3 BottomLeft {
			get { return this.Artboard.Bounds.min + (Vector3)Vector2.Scale(this.Margin, Vector2.one); }
		}
		
		private Vector3 BottomRight {
			get { return this.BottomCenter + (this.Artboard.Bounds.extents.x - this.Margin.x) * Vector3.right; }
		}
		
		private Vector3 TopCenter {
			get { return this.Artboard.Bounds.center + (this.Artboard.Bounds.extents.y - this.Margin.y) * Vector3.up; }
		}
		
		private Vector3 TopLeft {
			get { return this.TopCenter + (this.Artboard.Bounds.extents.x - this.Margin.x) * Vector3.left; }
		}
		
		private Vector3 TopRight {
			get { return this.Artboard.Bounds.max - (Vector3)Vector2.Scale(this.Margin, Vector2.one); }
		}
		
	}
	
}
