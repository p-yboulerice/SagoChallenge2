namespace SagoMeshEditor {
    
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;
    
    public class EditorSettings : ScriptableObject {
        
        // ================================================================= //
        // Enums
        // ================================================================= //
        
        public enum DefineMode {
            Exists, // only use the external if it exists
            Always, // always use the external, throw errors if it doesn't exist
            Never, // never use the external module
        }
        
        public enum DefineSymbols {
            SAGO_MESH_USE_SAGO_AUDIO
        }
        
        // ================================================================= //
        // Singleton
        // ================================================================= //
        
        public static EditorSettings Instance {
            get {
                
                string path;
                path = "Assets/SagoMesh.asset";
                
                EditorSettings instance;
                instance = AssetDatabase.LoadAssetAtPath(path, typeof(EditorSettings)) as EditorSettings;
                
                if (instance == null) {
                    instance = ScriptableObject.CreateInstance<EditorSettings>();
                    instance.AutoImport = true;
                    instance.DeleteIntermediateFiles = true;
                    instance.PixelsPerMeter = 100;
                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.SaveAssets();
                }
                
                return instance;
                
            }
        }
        
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        [MenuItem("CONTEXT/EditorSettings/Update Define Symbols")]
        static void UpdateDefineSymbolsMenuItem(MenuCommand command) {
            (command.context as EditorSettings).UpdateDefineSymbols();
        }
        
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        [SerializeField]
        protected bool m_AutoImport;
        
        [SerializeField]
        protected bool m_DeleteIntermediateFiles;
        
        [SerializeField]
        protected int m_PixelsPerMeter;
        
        [SerializeField]
        protected DefineMode m_SagoAudioMode;
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public bool AutoImport {
            get { return m_AutoImport; }
            set {
                if (m_AutoImport != value) {
                    m_AutoImport = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }
        
        public bool DeleteIntermediateFiles {
            get { return m_DeleteIntermediateFiles; }
            set { 
                if (m_DeleteIntermediateFiles != value) {
                    m_DeleteIntermediateFiles = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }
        
        public float MetersPerPixel {
            get { return 1f / m_PixelsPerMeter; }
        }
        
        public int PixelsPerMeter {
            get { return m_PixelsPerMeter; }
            set {
                if (m_PixelsPerMeter != value) {
                    m_PixelsPerMeter = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }
        
        public bool SagoAudioEnabled {
            get {
                if (m_SagoAudioMode == DefineMode.Always) {
                    return true;
                } else if (m_SagoAudioMode == DefineMode.Never) {
                    return false;
                }
                return AssetDatabase.GetAllAssetPaths().Where(p => p.Contains("SagoAudio") && Directory.Exists(p)).ToArray().Length > 0;
            }
        }
        
        public DefineMode SagoAudioMode {
            get { return m_SagoAudioMode; }
            set {
                if (m_SagoAudioMode != value) {
                    m_SagoAudioMode = value;
                    this.UpdateDefineSymbols();
                    EditorUtility.SetDirty(this);
                }
            }
        }
        
        
        // ================================================================= //
        // Compiler Flags Methods
        // ================================================================= //
        
        public void UpdateDefineSymbols() {
            this.UpdateDefineSymbol(DefineSymbols.SAGO_MESH_USE_SAGO_AUDIO, this.SagoAudioEnabled);
        }
        
        public void UpdateDefineSymbol(DefineSymbols defineSymbol, bool enabled) {
            foreach (BuildTargetGroup buildTargetGroup in System.Enum.GetValues(typeof(BuildTargetGroup))) {
                
                string delimiter;
                delimiter = ";";
                
                string value;
                value = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                
                HashSet<string> hash;
                hash = new HashSet<string>(value.Split(delimiter.ToCharArray()));
                
                if (enabled) {
                    hash.Add(defineSymbol.ToString());
                } else {
                    hash.Remove(defineSymbol.ToString());
                }
                
                value = string.Join(delimiter, hash.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, value);
                
            }
        }
        
        
    }
    
}