namespace SagoUtils {

	using System;
	using System.Reflection;
	using UnityEngine;

	public class EnumMaskAttribute : PropertyAttribute {

		public Type EnumType;
		public int Mask;

		public EnumMaskAttribute(Type enumType) {
            #if UNITY_EDITOR
            if (!enumType.IsEnum) {
                throw new InvalidOperationException(string.Format("{0} must be an Enum", enumType));
            }
            #endif

            this.EnumType = enumType;
		}

	}

}
