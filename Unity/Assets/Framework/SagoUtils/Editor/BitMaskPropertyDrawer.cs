namespace SagoUtils {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	
	[CustomPropertyDrawer(typeof(BitMaskAttribute))]
	public class BitMaskPropertyDrawer : PropertyDrawer {


		#region GUI

		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			EditorGUI.BeginProperty(position, label, property);

			int value = property.intValue;

			EditorGUI.BeginChangeCheck();

			int newValue = EditorGUI.MaskField(position, label, value, this.Labels);

			if (EditorGUI.EndChangeCheck()) {
				property.intValue = newValue;
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Helper

		protected BitMaskAttribute Attribute {
			get { return this.attribute as BitMaskAttribute; }
		}

		protected string[] m_Labels;
		protected string[] Labels {
			get {
				if (m_Labels == null) {
					m_Labels = new string[this.NumberOfBits];
					for (int i = 0; i < m_Labels.Length; ++i) {
						uint val = (uint)1 << i;
						m_Labels[i] = string.Format("{0} ({1} => {2})", 
							i, 
							val, 
							System.Convert.ToString(val, 2).PadLeft(m_Labels.Length, '0'));
					}
				}
				return m_Labels;
			}
		}

		protected int NumberOfBits {
			get {
				return this.Attribute.BitDepth;
			}
		}

		#endregion

		
	}
	
}
