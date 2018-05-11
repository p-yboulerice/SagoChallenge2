namespace SagoMesh.Internal {
    
    using System.Collections.Generic;
    
    public class ArrayUtil {
        
        public static bool Equal<T>(T[] a, T[] b) {
            
            if (ReferenceEquals(a,b)) {
                return true;
            }
            
            if (a == null || b == null) {
                return false;
            }
            
            if (a.Length != b.Length) {
                return false;
            }
            
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a.Length; i++) {
                if (!comparer.Equals(a[i], b[i])) return false;
            }
            
            return true;
            
        }
        
        public static T[] Unique<T>(T[] array) {
            if (array != null) {
                
                HashSet<T> hash  = new HashSet<T>();
                for (int index = 0; index < array.Length; index++) {
                    hash.Add(array[index]);
                }
                
                if (hash.Count != array.Length) {
                    array = new T[hash.Count];
                    hash.CopyTo(array);
                }
                
            }
            return array;
        }
        
        public static T[] UniqueAndNotNull<T>(T[] array) {
            if (array != null) {
                
                HashSet<T> hash  = new HashSet<T>();
                for (int index = 0; index < array.Length; index++) {
                    T value = array[index];
                    if (value != null) {
                        hash.Add(value);
                    }
                }
                
                if (hash.Count != array.Length) {
                    array = new T[hash.Count];
                    hash.CopyTo(array);
                }
                
            }
            return array;
        }
        
    }
    
}