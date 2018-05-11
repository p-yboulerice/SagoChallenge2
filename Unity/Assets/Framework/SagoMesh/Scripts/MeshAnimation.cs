namespace SagoMesh {
    
    using SagoMesh.Internal;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MeshAnimation : ScriptableObject {
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        [SerializeField]
        protected Vector2 m_AnchorPoint;
        
        [System.NonSerialized]
        protected string m_AssetBundlePath;
        
        [System.NonSerialized]
        protected bool m_AssetBundlePathLocked;
        
        [SerializeField]
        protected Vector2 m_ContentSize;
        
        [SerializeField]
        protected MeshAnimationFrame[] m_Frames;
        
        [SerializeField]
        protected int m_FramesPerSecond;
        
        [SerializeField]
        protected MeshAnimationLayer[] m_Layers;
        
        [SerializeField]
        protected int m_PixelsPerMeter;
        
        [SerializeField]
        protected string m_Version;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public Vector2 AnchorPoint {
            get { return m_AnchorPoint; }
            set { 
                if (m_AnchorPoint != value) {
                    m_AnchorPoint = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public string AssetBundlePath {
            get { 
                if (!m_AssetBundlePathLocked) {
                    m_AssetBundlePathLocked = true;
                    AssetUtil.SetDirty(this);
                }
                return m_AssetBundlePath;
            }
            set {
                if (m_AssetBundlePathLocked) {
                    throw new System.InvalidOperationException("AssetBundlePath is locked.");
                }
                m_AssetBundlePath = value;
                m_AssetBundlePathLocked = true;
                AssetUtil.SetDirty(this);
            }
        }
        
        public Vector2 ContentSize {
            get { return m_ContentSize; }
            set { 
                if (m_ContentSize != value) {
                    m_ContentSize = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public float Duration {
            get { return (float)this.Frames.Length / this.FramesPerSecond; }
        }
        
        public MeshAnimationFrame[] Frames {
            get { return m_Frames; }
            set { 
                if (m_Frames != value) {
                    m_Frames = value;
                    m_Version = null;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public int FramesPerSecond {
            get { return m_FramesPerSecond; }
            set { 
                if (m_FramesPerSecond != value) {
                    m_FramesPerSecond = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public MeshAnimationLayer[] Layers {
            get { return m_Layers; }
            set { 
                if (m_Layers != value) {
                    m_Layers = value; 
                    m_Version = null;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public float MetersPerPixel {
            get { return 1f / this.PixelsPerMeter; }
        }
        
        public int PixelsPerMeter {
            get { return m_PixelsPerMeter; }
            set { 
                if (m_PixelsPerMeter != value) {
                    m_PixelsPerMeter = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        public string Version {
            get { return m_Version; }
            protected set {
                if (m_Version != value) {
                    m_Version = value;
                    AssetUtil.SetDirty(this);
                }
            }
        }
        
        
        // ================================================================= //
        // Public Methods
        // ================================================================= //
        
        public void ResetVersion() {
            this.Version = string.Format(
                "{0}_{1}_{2}_{3}", 
                this.name, 
                this.Layers.Length, 
                this.Frames.Length, 
                System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond
            );
        }
        
        // ================================================================= //
        // ScriptableObject Methods
        // ================================================================= //
        
        void OnEnable() {
            
        }
        
        void OnDisable() {
            if (!string.IsNullOrEmpty(m_AssetBundlePath)) {
                // TODO: notify the loader that it's safe to unload the bundle...
            }
        }
        
        
    }
    
}