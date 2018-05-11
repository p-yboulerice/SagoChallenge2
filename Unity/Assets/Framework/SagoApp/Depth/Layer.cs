namespace SagoApp.Depth {

	using System.Collections.Generic;
	using UnityEngine;

	public class Layer : MonoBehaviour, IComparer<Layer> {


		#region Static Methods

		public static void ClearCachedFields(Layer layer) {
			layer.m_ChildList = null;
			layer.m_ChildTable = null;
			layer.m_Parent = null;
			layer.m_Thickness = 0;
		}

		public static void RegisterWithParent(Layer parent, Layer child) {
			if (parent) {
				parent.RegisterChild(child);
			}
		}

		public static void DeregisterFromParent(Layer parent, Layer child) {
			if (parent) {
				parent.DeregisterChild(child);
			}
		}

		#endregion


		#region Static Properties

		private static float Spacing {
			get { return 0.125f; }
		}

		private static Vector3 Direction {
			get { return Vector3.back; }
		}

		#endregion


		#region Events

		public event System.Action<Layer> OnDepthDidChange;

		#endregion


		#region Methods

		public void Add(Layer layer) {
			Insert(this.ChildCount, layer);
		}

		public bool Contains(Layer layer) {
			return this.ChildTable.ContainsKey(layer);
		}

		public Layer[] GetChildren() {
			return this.ChildList.ToArray();
		}

		public int IndexOf(Layer layer) {
			return Contains(layer) ? this.ChildTable[layer] : GetInsertionChildIndex(this.ChildList.BinarySearch(layer, this));
		}

		public void Insert(int index, Layer layer) {

			if (layer.Parent) {
				layer.Parent.Remove(layer);
			}

			int childIndex;
			childIndex = GetInsertionChildIndex(index);

			int siblingIndex;
			siblingIndex = GetInsertionSiblingIndex(childIndex);

			layer.Transform.SetParent(this.Transform);
			layer.Transform.SetSiblingIndex(siblingIndex);

			RemoveChild(layer);
			InsertChild(childIndex, layer);

			layer.Parent = this;

		}

		public void Remove(Layer layer) {
			if (layer.Parent == this) {
				layer.Parent = null;
				layer.Transform.SetParent(null);
				RemoveChild(layer);
			}
		}

		#endregion


		#region Properties

		public int ChildCount {
			get { return this.ChildList.Count; }
		}

		public Layer Parent {
			get {
				m_Parent = m_Parent ?? FindParent();
				return m_Parent;
			}
			private set { m_Parent = value; }
		}

		public int Rank {
			get { return CalculateRank(); }
		}

		public Layer Root {
			get { return this.Parent ? this.Parent.Root : this; }
		}

		public float Thickness {
			get {
				m_Thickness = Mathf.Max(Layer.Spacing, m_Thickness);
				return m_Thickness;
			}
			private set { m_Thickness = value; }
		}

		public Transform Transform {
			get { return transform; }
		}

		#endregion


		#region Fields

		private List<Layer> m_ChildList;
		private Dictionary<Layer, int> m_ChildTable;
		private Layer m_Parent;
		private float m_Thickness;

		#endregion


		#region Internal Properties

		private List<Layer> ChildList {
			get {
				m_ChildList = m_ChildList ?? new List<Layer>();
				return m_ChildList;
			}
		}

		private Dictionary<Layer, int> ChildTable {
			get {
				m_ChildTable = m_ChildTable ?? new Dictionary<Layer, int>();
				return m_ChildTable;
			}
		}

		#endregion


		#region MonoBehaviour

		private void OnEnable() {
			ResetZScale();
			RegisterWithParent(this.Parent, this);
		}

		private void OnDisable() {
			DeregisterFromParent(this.Parent, this);
		}

		#endregion


		#region Register / Deregister

		private bool RegisterChild(Layer child) {
			if (!Contains(child)) {
				InsertChild(IndexOf(child), child);
				return true;
			}
			return false;
		}

		private void DeregisterChild(Layer child) {
			RemoveChild(child);
		}

		private Layer FindParent() {
			return this.Transform.parent ? this.Transform.parent.GetComponent<Layer>() : null;
		}

		#endregion


		#region Insert / Remove

		private void InsertChild(int index, Layer layer) {
			SetLocalDepth(layer, GetLocalDepthAt(index));
			this.ChildList.Insert(index, layer);
			this.ChildTable.Add(layer, index);
			ChangeThicknessAt(index, layer.Thickness, 1);
		}

		private void RemoveChild(Layer layer) {
			if (Contains(layer)) {
				ChangeThicknessAt(IndexOf(layer), -layer.Thickness, -1);
				this.ChildList.RemoveAt(IndexOf(layer));
				this.ChildTable.Remove(layer);
			}
		}

		private bool ChildExistsAt(int index) {
			if (index < 0) return false;
			if (index >= this.ChildCount) return false;
			return true;
		}

		private Layer GetChildAt(int index) {
			return ChildExistsAt(index) ? this.ChildList[index] : null;
		}

		private int GetInsertionChildIndex(int index) {
			index = (index < 0) ? ~index : index;
			index = Mathf.Min(index , this.ChildCount);
			return index;
		}

		private int GetInsertionSiblingIndex(int childIndex) {

			int result;
			result = this.Transform.childCount;

			Layer leftChild;
			leftChild = GetChildAt(childIndex - 1);

			Layer rightChild;
			rightChild = GetChildAt(childIndex);

			if (leftChild) result = leftChild.Transform.GetSiblingIndex() + 1;
			else if (rightChild) result = rightChild.Transform.GetSiblingIndex();

			return result;

		}

		#endregion


		#region Depth

		private void OnChildThicknessChange(Layer child, float delta) {
			if (Contains(child)) {
				ChangeThicknessAt(IndexOf(child), delta, 0);
			}
		}

		private void ChangeThicknessAt(int index, float deltaThickness, int deltaIndex) {
			
			while (++index < this.ChildCount) {
				ChangeChildIndex(GetChildAt(index), deltaIndex);
				ChangeLocalDepth(GetChildAt(index), deltaThickness);
			}

			this.Thickness += deltaThickness;

			if (this.Parent) {
				this.Parent.OnChildThicknessChange(this, deltaThickness);
			}

		}

		private void ChangeChildIndex(Layer layer, int delta) {
			if (Contains(layer)) {
				this.ChildTable[layer] += delta;
			}
		}

		private void ChangeLocalDepth(Layer layer, float delta) {
			layer.Transform.localPosition += delta * Layer.Direction;
			layer.NotifyDepthDidChange();
		}

		private void SetLocalDepth(Layer layer, float value) {
			ChangeLocalDepth(layer, value - GetLocalDepth(layer));
		}

		private float GetLocalDepth(Layer layer) {
			return Vector3.Dot(layer.Transform.localPosition, Layer.Direction);
		}

		private float GetLocalDepthAt(int index) {
			return ChildExistsAt(index) ? GetLocalDepth(GetChildAt(index)) : this.Thickness;
		}

		private int CalculateRank() {

			Vector3 delta;
			delta = this.Transform.position - this.Root.Transform.position;

			float rank;
			rank = Vector3.Dot(delta, Layer.Direction) / Layer.Spacing;

			return Mathf.RoundToInt(rank);

		}

		private void ResetZScale() {
			Vector3 scale = this.Transform.localScale;
			scale.z = 1f;
			this.Transform.localScale = scale;
		}

		#endregion


		#region Notification

		private void NotifyDepthDidChange() {

			if (this.OnDepthDidChange != null) {
				this.OnDepthDidChange(this);
			}

			foreach (Layer child in this.ChildList) {
				child.NotifyDepthDidChange();
			}

		}

		#endregion


		#region IComparer

		public int Compare(Layer a, Layer b) {

			int indexA;
			indexA = a.Transform.GetSiblingIndex();

			int indexB;
			indexB = b.Transform.GetSiblingIndex();

			int delta;
			delta = indexA - indexB;

			float result;
			result = Mathf.Sign(delta) * Mathf.Ceil(Mathf.Clamp01(Mathf.Abs(delta)));

			return (int)result;

		}

		#endregion


	}

}