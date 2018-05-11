namespace SagoMesh {
    
    using System.Text.RegularExpressions;
    using UnityEngine;
    
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshAnimatorLayer : MonoBehaviour {
        
        [System.NonSerialized]
        protected MeshFilter m_MeshFilter;
        
        [System.NonSerialized]
        protected MeshRenderer m_Renderer;
        
        /// <summary>
        /// Gets the mesh filter (lazy loaded, cached)
        /// </summary>
        /// <value>The mesh filter.</value>
        public MeshFilter MeshFilter {
        	get {
        		m_MeshFilter = m_MeshFilter ?? GetComponent<MeshFilter>();
        		return m_MeshFilter;
        	}
        }
        
        /// <summary>
        /// Gets the renderer (lazy loaded, cached)
        /// </summary>
        /// <value>The renderer.</value>
        public MeshRenderer Renderer {
        	get {
        		m_Renderer = m_Renderer ?? GetComponent<MeshRenderer>();
        		return m_Renderer;
        	}
        }
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        public static string IndexToName(int index) {
            return index >= 0 ? string.Format("L{0:0000}", index) : null;
        }
        
        public static int NameToIndex(string name) {
            
            Regex pattern;
            pattern = new Regex("^L(\\d{4})$");
            
            Match match;
            match = pattern.Match(name);
            
            return match != null ? int.Parse(name.Substring(name.Length - 4)) : -1;
            
        }
        
        
    }
    
}

