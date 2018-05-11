namespace SagoMeshEditor.Internal {
    
    using System.Text.RegularExpressions;
    using UnityEngine;
    
    static class StringExtensions {
        
        // ================================================================= //
        // Extension Methods
        // ================================================================= //
        
        public static Vector2 ToVector2(this string value) {
            
            Regex pattern;
            pattern = new Regex("^(.+?),(.+?)$");
            
            Match match;
            match = pattern.Match(value);
            
            if (match != null) {
                return new Vector2(
                    System.Convert.ToSingle(match.Groups[1].ToString()), 
                    System.Convert.ToSingle(match.Groups[2].ToString())
                );
            }
            
            return Vector2.zero;
            
        }
        
        
    }
    
}