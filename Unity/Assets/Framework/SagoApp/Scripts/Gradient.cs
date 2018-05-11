namespace SagoApp {
    
    using System.Collections;
    using UnityEngine;
    
    public enum GradientOrientation {
        Horizontal,
        Vertical
    }
    
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Gradient : MonoBehaviour {
        
        public GradientOrientation Orientation;
        public Color From;
        public Color To;
        
        private Color32[] m_Colors;
        private Mesh m_Mesh;
        private int[] m_Triangles;
        private Vector3[] m_Vertices;
        
        public void Reset() {
            this.Orientation = GradientOrientation.Vertical;
            this.From = Color.black;
            this.To = Color.white;
        }
        
        public void Update() {
            
            if (m_Colors == null || m_Colors.Length == 0) {
                m_Colors = new Color32[4];
            }
            
            if (m_Triangles == null || m_Triangles.Length == 0) {
                m_Triangles = new int[6];
                m_Triangles[0] = 0;
                m_Triangles[1] = 1;
                m_Triangles[2] = 2;
                m_Triangles[3] = 0;
                m_Triangles[4] = 2;
                m_Triangles[5] = 3;
            }
            
            if (m_Vertices == null || m_Vertices.Length == 0) {
                m_Vertices = new Vector3[4];
                m_Vertices[0] = new Vector3(-0.5f, 0.5f, 0f);
                m_Vertices[1] = new Vector3(0.5f, 0.5f, 0f);
                m_Vertices[2] = new Vector3(0.5f, -0.5f, 0f);
                m_Vertices[3] = new Vector3(-0.5f, -0.5f, 0f);
            }
            
            if (m_Mesh == null) {
                m_Mesh = new Mesh();
                m_Mesh.hideFlags = HideFlags.HideAndDontSave;
                m_Mesh.name = "Gradient";
                m_Mesh.vertices = m_Vertices;
                m_Mesh.triangles = m_Triangles;
                m_Mesh.RecalculateBounds();
                m_Mesh.RecalculateNormals();
                this.GetComponent<MeshFilter>().sharedMesh = m_Mesh;
            }
            
            if (this.Orientation == GradientOrientation.Horizontal) {
                m_Colors[0] = this.From;
                m_Colors[1] = this.To;
                m_Colors[2] = this.To;
                m_Colors[3] = this.From;
                m_Mesh.colors32 = m_Colors;
            } else {
                m_Colors[0] = this.From;
                m_Colors[1] = this.From;
                m_Colors[2] = this.To;
                m_Colors[3] = this.To;
                m_Mesh.colors32 = m_Colors;
            }
            
        }
        
        public void OnDestroy() {
            if (Application.isPlaying) {
                Object.Destroy(m_Mesh);
            } else {
                Object.DestroyImmediate(m_Mesh);
            }
        }
        
    }

}