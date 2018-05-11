namespace SagoMesh {
    
    using SagoMesh.Internal;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MeshAnimationCollection : ScriptableObject {
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        [SerializeField]
        protected MeshAnimation[] m_Animations;
        
        [System.NonSerialized]
        protected Dictionary<string,MeshAnimation> m_Dictionary;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public MeshAnimation[] Animations {
            get { return m_Animations; }
            set {
                MeshAnimation[] normalized = ArrayUtil.UniqueAndNotNull(value);
                if (!ArrayUtil.Equal(m_Animations, normalized)) {
                    m_Animations = normalized;
                    m_Dictionary = null;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public Dictionary<string,MeshAnimation> Dictionary {
            get {
                if (m_Dictionary == null) {
                    m_Dictionary = new Dictionary<string,MeshAnimation>();
                    foreach (MeshAnimation animation in m_Animations) {
                        if (animation != null && m_Dictionary.ContainsKey(animation.name) != true) {
                            m_Dictionary.Add(animation.name, animation);
                        }
                    }
                }
                return new Dictionary<string,MeshAnimation>(m_Dictionary);
            }
        }
        
        
        // ================================================================= //
        // MonoBehavior Methods
        // ================================================================= //
        
        public void OnEnable() {
            this.Normalize();
        }
        
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        public bool Contains(MeshAnimation animation) {
            if (animation == null) return false;
            if (this.Animations == null) return false;
            return System.Array.IndexOf(this.Animations, animation) != -1;
        }
        
        public void Normalize() {
            this.Animations = m_Animations;
        }
        
        
    }
    
}