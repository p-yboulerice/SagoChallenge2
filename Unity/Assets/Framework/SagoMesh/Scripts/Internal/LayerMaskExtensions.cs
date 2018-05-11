namespace SagoMesh.Internal {
    
    using UnityEngine;
    
    public static class LayerMaskExtensions {
        
        public static bool Contains(this LayerMask layerMask, Component component) {
            return component && layerMask.Contains(component.gameObject);
        }
        
        public static bool Contains(this LayerMask layerMask, GameObject gameObject) {
            return gameObject != null && layerMask.Contains(gameObject.layer);
        }
        
        public static bool Contains(this LayerMask layerMask, int layer) {
            return (layerMask.value & (1 << layer)) > 0;
        }
        
    }
    
}