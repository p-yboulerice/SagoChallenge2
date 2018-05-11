namespace SagoMeshEditor {
    
    using SagoMesh;
    using SagoMeshEditor;
    using SagoMeshEditor.Internal;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;
    
    public class MeshAnimationPostprocessor : AssetPostprocessor {
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
		[MenuItem("Sago/Mesh/Import", false, 0)]
		static public void Import() {
			OnPostprocessAssets(AssetDatabase.GetAllAssetPaths());
		}
		
		private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] movedTo, string[] movedFrom) {
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				if (EditorSettings.Instance.AutoImport) {
					OnPostprocessAssets(imported);
				}
			#endif
		}
		
		
		private static void OnPostprocessAssets(string[] paths) {
			
			paths = paths.Where(path => !path.StartsWith("Assets/Plugins")).ToArray();
			
			bool dirty;
			dirty = false;
			
			if (OnPostprocessBytes(paths)) {
				dirty = true;
			}
			
			if (OnPostprocessXml(paths)) {
				dirty = true;
			}
			
			if (dirty) {
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			
			SagoEngineEditor.MeshPostprocessor.Import(paths);
			
		}
		
		private static bool OnPostprocessBytes(string[] paths) {
			bool dirty = false;
			try {
				
				AssetDatabase.StartAssetEditing();
				
				BytesAssetHandler bytesHandler;
				bytesHandler = new BytesAssetHandler();
				
				foreach (string path in paths) {
					if (path.EndsWith(".bytes")) {
						dirty = true;
						bytesHandler.HandleAsset(path);
					}
				}
				
			} finally {
				
				AssetDatabase.StopAssetEditing();
				
			}
			return dirty;
		}
		
		private static bool OnPostprocessXml(string[] paths) {
			bool dirty = false;
			try {
				
				AssetDatabase.StartAssetEditing();
				
				XmlAssetHandler xmlHandler;
				xmlHandler = new XmlAssetHandler();
				
				foreach (string path in paths) {
					if (path.EndsWith(".xml")) {
						dirty = true;
						xmlHandler.HandleAsset(path);
					}
				}
				
			} finally {
				
				AssetDatabase.StopAssetEditing();
				
			}
			return dirty;
		}
        
        
    }
    
}

namespace SagoMeshEditor.Internal {
    
    using SagoMesh;
    using SagoMeshEditor;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using EditorSettings = SagoMeshEditor.EditorSettings;
        
    enum BytesRecordType {
        Unknown = 0,
        Mesh = 1
    }
    
    class BytesAssetHandler {
        
        // ================================================================= //
        // Static Properties
        // ================================================================= //
        
        static protected readonly string BytesToken = "sagomesh";
        
        static protected readonly int BytesVersion = 2;
        
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        public void HandleAsset(string bytesPath) {
            
            // read bytes
            byte[] bytes;
            bytes = File.ReadAllBytes(bytesPath);
            
            // check bytes
            if (bytes == null || bytes.Length == 0) {
                throw new System.ArgumentException("Could not read bytes: " + bytesPath);
            }
            
            // create stream
            MemoryStream stream;
            stream = new MemoryStream(bytes);
            
            // create reader
            BinaryReader reader;
            reader = new BinaryReader(stream);
            
            // read type
            string token;
            token = new string(reader.ReadChars(8));
            
            // check type
            if (token != BytesToken) {
                return;
            }
            
            // read version
            int version;
            version = reader.ReadInt32();
            
            // check version
            if (version < BytesVersion) {
                return;
            }
            
            // create record handlers
            MeshRecordHandler meshRecordHandler = new MeshRecordHandler(bytesPath);
            
            // begin handling records
            meshRecordHandler.Begin();
            
            // handle records
            while (reader.BaseStream.Position != reader.BaseStream.Length) {
                
                // read record
                BytesRecordType recordType = (BytesRecordType)reader.ReadInt32();
                int recordLength = reader.ReadInt32();
                byte[] record = reader.ReadBytes(recordLength);
                
                // handle record
                switch (recordType) {
                    case BytesRecordType.Mesh:
                        meshRecordHandler.HandleRecord(record);
                    break;
                }
                
            }
            
            // end handling records
            meshRecordHandler.End();
            
            // delete bytes
            if (EditorSettings.Instance.DeleteIntermediateFiles) {
                AssetDatabase.DeleteAsset(bytesPath);
            }
            
        }
        
        
    }
    
    class MeshRecordHandler {
        
        // ================================================================= //
        // Structs
        // ================================================================= //
        
        protected struct mesh_header_t {
            public ulong identifierOffset;
            public uint identifierSize;
            public ulong indicesOffset;
            public uint indicesSize;
            public ulong verticesOffset;
            public uint verticesSize;
        }
        
        protected struct mesh_vertex_t {
            public float x;
            public float y;
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }
        
        
        // ================================================================= //
        // Member Variables
        // ================================================================= //
        
        protected MeshAnimationAtlas Atlas {
            get; set;
        }
        
        protected bool AtlasCreated {
            get; set;
        }
        
        protected string AtlasPath {
            get; set;
        }
        
        protected string BytesPath {
            get; set;
        }
        
        protected Dictionary<string,Mesh> Meshes {
            get; set;
        }
        
        protected Dictionary<string,bool> MeshesNew {
            get; set;
        }
        
        protected Dictionary<string,bool> MeshesUsed {
            get; set;
        }
        
        
        // ================================================================= //
        // Constructor Methods
        // ================================================================= //
        
        public MeshRecordHandler(string bytesPath) {
            this.BytesPath = bytesPath;
        }
        
        
        // ================================================================= //
        // Public Methods
        // ================================================================= //
        
        public void Begin() {
            
            // create atlas path
            this.AtlasPath = string.Format("{0}/{1}_atlas.asset", Path.GetDirectoryName(this.BytesPath), Path.GetFileNameWithoutExtension(this.BytesPath));
            
            // find or create atlas
            this.Atlas = AssetDatabase.LoadAssetAtPath(this.AtlasPath, typeof(MeshAnimationAtlas)) as MeshAnimationAtlas;
            this.AtlasCreated = false;
            
            if (this.Atlas == null) {
                
                // IMPORTANT: Don't use HideFlags.HideInHierarchy on the atlas 
                // object. Unity forces one of the non-hidden assets in the 
                // file to have the same name as the file. If there is only 
                // one mesh and the atlas is hidden, Unity will overwrite the 
                // name of the mesh to make it match the name of the file.
                
                this.Atlas = ScriptableObject.CreateInstance<MeshAnimationAtlas>();
                this.Atlas.name = Path.GetFileNameWithoutExtension(this.AtlasPath);
                this.AtlasCreated = true;
                AssetDatabase.CreateAsset(this.Atlas, this.AtlasPath);
                
            }
            
            // create meshes dictionary
            this.Meshes = new Dictionary<string,Mesh>();
            
            // create new dictionary
            this.MeshesNew = new Dictionary<string,bool>();
            
            // create flags dictionary
            this.MeshesUsed = new Dictionary<string,bool>();
            
            // populate dictionaries
            foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(this.AtlasPath)) {
                if (asset as Mesh) {
                    this.Meshes.Add(asset.name, asset as Mesh);
                    this.MeshesNew.Add(asset.name, false);
                    this.MeshesUsed.Add(asset.name, false);
                }
            }
            
        }
        
        public void HandleRecord(byte[] bytes) {
            
            // create stream
            MemoryStream stream;
            stream = new MemoryStream(bytes);
            
            // create reader
            BinaryReader reader;
            reader = new BinaryReader(stream);
                
            // create header
            mesh_header_t header;
            header.identifierOffset = reader.ReadUInt64();
            header.identifierSize = reader.ReadUInt32();
            header.indicesOffset = reader.ReadUInt64();
            header.indicesSize = reader.ReadUInt32();
            header.verticesOffset = reader.ReadUInt64();
            header.verticesSize = reader.ReadUInt32();
            
            // get index count and vertex count
            uint indexCount = header.indicesSize / (uint)Marshal.SizeOf(typeof(uint));
            uint vertexCount = header.verticesSize / (uint)Marshal.SizeOf(typeof(mesh_vertex_t));
            
            // read identifier
            string identifier;
            identifier = System.Text.Encoding.UTF8.GetString(reader.ReadBytes((int)header.identifierSize));
            
            // read indices
            int[] indices = new int[indexCount];
            for (int i = 0; i < indexCount; i++) {
                indices[i] = reader.ReadInt32();
            }
            
            // read vertices
            Vector3[] vertices = new Vector3[vertexCount];
            Color32[] colors = new Color32[vertexCount];
            for (uint i = 0; i < vertexCount; i++) {
                vertices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0f) * EditorSettings.Instance.MetersPerPixel;
                colors[i] = new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            }
            
            // find or create mesh
            Mesh mesh;
            if (this.Meshes.ContainsKey(identifier)) {
                mesh = this.Meshes[identifier];
                this.MeshesNew[identifier] = false;
            } else {
                mesh = new Mesh();
                mesh.name = identifier;
                this.MeshesNew[identifier] = true;
            }
            
            // update mesh
            mesh.Clear(false);
            mesh.vertices = vertices;
            mesh.colors32 = colors;
            mesh.triangles = indices;
            mesh.RecalculateBounds();
            EditorUtility.SetDirty(mesh);
            
            // update dictionaries
            this.Meshes[identifier] = mesh;
            this.MeshesUsed[identifier] = true;
            
        }
        
        public void End() {
            
            // remove unused meshes from atlas
            foreach (KeyValuePair<string,bool> flag in this.MeshesUsed) {
                if (flag.Value == false) {
                    Mesh mesh = this.Meshes[flag.Key];
                    Object.DestroyImmediate(mesh, true);
                    this.Meshes.Remove(flag.Key);
                }
            }
            
            // add new meshes to atlas
            foreach (KeyValuePair<string,Mesh> mesh in this.Meshes) {
                if (mesh.Value != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh.Value))) {
                    AssetDatabase.AddObjectToAsset(mesh.Value, this.Atlas);
                }
            }
            
            // set dirty to make sure the atlas gets saved
            EditorUtility.SetDirty(this.Atlas);
            
            // set new meshes as read only
            foreach (Mesh mesh in this.Meshes.Where( kv => this.MeshesNew[kv.Key] == true ).Select( kv => kv.Value )) {
                SagoMeshEditor.Internal.MeshUtil.SetIsReadable(mesh, false);
            }
            
        }
        
        
    }
    
}

namespace SagoMeshEditor.Internal {
    
    using SagoMesh;
    using SagoMeshEditor;
    using SagoMeshEditor.Internal;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
    using UnityEditor;
    using UnityEngine;
    using EditorSettings = SagoMeshEditor.EditorSettings;
    
    class XmlAssetHandler {
        
        // ================================================================= //
        // Public Methods
        // ================================================================= //
        
        public void HandleAsset(string xmlPath) {
            
            // load atlas
            MeshAnimationAtlas atlas = this.FindAtlas(Path.GetDirectoryName(xmlPath), Path.GetFileNameWithoutExtension(xmlPath));
            if (atlas == null) {
                // throw new System.Exception(string.Format("Could not load atlas: {0}", Path.GetFileNameWithoutExtension(xmlPath)));
                return;
            }
            
            // load text
            TextAsset text = AssetDatabase.LoadAssetAtPath(xmlPath, typeof(TextAsset)) as TextAsset;
            if (text == null) {
                // throw new System.Exception(string.Format("Could not load xml: {0}", Path.GetFileNameWithoutExtension(xmlPath)));
                return;
            } 
            
            // load xml
            XmlDocument xml = new XmlDocument();
            try {
                xml.LoadXml(text.text);
            } catch {
                // } catch (System.Exception e) {
                // throw new System.Exception(string.Format("Could not parse xml: {0}", Path.GetFileNameWithoutExtension(xmlPath)), e);
                return;
            }
            
            // validate xml
            if (xml.SelectSingleNode("/timeline") == null) {
                return;
            }
            
            // read anchor point
            Vector2 anchorPoint;
            anchorPoint = xml.SelectSingleNode("timeline").Attributes["anchorPoint"].Value.ToVector2();
            
            // read content size
            Vector2 contentSize;
            contentSize = xml.SelectSingleNode("timeline").Attributes["contentSize"].Value.ToVector2();
            contentSize *= EditorSettings.Instance.MetersPerPixel;
            
            // read frames per second
            int framesPerSecond;
            framesPerSecond = System.Convert.ToInt32(xml.SelectSingleNode("timeline").Attributes["framesPerSecond"].Value);
            
            // read markers
            foreach (XmlNode markerIdentifierNode in xml.SelectNodes("/timeline/marker")) {
                
                string id;
                id = markerIdentifierNode.Attributes["id"].Value;
                
                foreach (XmlNode labelNode in xml.SelectNodes("/timeline/label")) {
                    
                    // create frames
                    List<MarkerAnimationFrame> frames = new List<MarkerAnimationFrame>();
                    foreach (XmlNode markerDataNode in labelNode.SelectNodes(string.Format("frame/marker[@id='{0}']", id))) {
                        MarkerAnimationFrame frame = new MarkerAnimationFrame();
                        frame.Active = System.Convert.ToBoolean(markerDataNode.Attributes["active"].Value);
                        frame.Position = markerDataNode.Attributes["position"].Value.ToVector2() * EditorSettings.Instance.MetersPerPixel;
                        frame.Rotation = System.Convert.ToSingle(markerDataNode.Attributes["rotation"].Value);
                        frame.Scale = markerDataNode.Attributes["scale"].Value.ToVector2();
                        frame.Skew = (
                            markerDataNode.Attributes.GetNamedItem("skew") != null ? 
                            markerDataNode.Attributes["skew"].Value.ToVector2() : 
                            Vector2.zero
                        );
                        frames.Add(frame);
                    }
                    
                    // set path
                    string directoryPath = Path.GetDirectoryName(xmlPath);
                    string animationId = xml.SelectSingleNode("timeline").Attributes["id"].Value;
                    string labelId = Regex.Replace(labelNode.Attributes["id"].Value, string.Format("^{0}_", animationId), "");
                    string markerId = markerIdentifierNode.Attributes["name"].Value;
                    
                    string path;
                    path = MarkerAnimationAssetPathUtil.GetMarkerAnimationAssetPath(directoryPath, animationId, labelId, markerId);
                    
                    // find or create marker
                    MarkerAnimation marker;
                    marker = AssetDatabase.LoadAssetAtPath(path, typeof(MarkerAnimation)) as MarkerAnimation;
                    
                    if (marker == null) {
                        marker = ScriptableObject.CreateInstance<MarkerAnimation>();
                        AssetDatabase.CreateAsset(marker, path);
                    }
                    
                    // update marker
                    marker.hideFlags = HideFlags.None;
                    marker.Frames = frames.ToArray();
                    marker.FramesPerSecond = framesPerSecond;
                    marker.PixelsPerMeter = EditorSettings.Instance.PixelsPerMeter;
                    marker.ResetVersion();
                    
                    // mark marker dirty to make sure it gets saved
                    EditorUtility.SetDirty(marker);
                    
                }
                
            }
            
            //
            Dictionary<string,Mesh> atlasDictionary = new Dictionary<string,Mesh>();
            foreach (Object atlasAsset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(atlas))) {
                if (atlasAsset is Mesh) {
                    atlasDictionary.Add(atlasAsset.name, atlasAsset as Mesh);
                }
            }
            
            // read labels
            foreach (XmlNode labelNode in xml.SelectNodes("/timeline/label")) {
                
                // read frames
                List<MeshAnimationFrame> frames = new List<MeshAnimationFrame>();
                foreach (XmlNode frameNode in labelNode.SelectNodes("frame")) {
                    
                    // read audio clips
                    List<AudioClip> audioClips = new List<AudioClip>();
                    foreach (XmlNode audioClipNode in frameNode.SelectNodes("sound")) {
                        
                        string name;
                        name = audioClipNode.Attributes["id"].Value;
                        
                        string directory;
                        directory = Path.GetDirectoryName(xmlPath);
                        
                        AudioClip audioClip;
                        audioClip = this.FindAudioClip(directory, name);
                        
                        if (audioClip) {
                            audioClips.Add(audioClip);
                        } else {
                            throw new System.Exception(string.Format("Could not find audio clip: {0}", name));
                        }
                        
                    }
                    
                    // read meshes
                    List<Mesh> meshes = new List<Mesh>();
                    foreach (XmlNode meshNode in frameNode.SelectNodes("mesh")) {
                        
                        string name;
                        name = meshNode.Attributes["id"].Value;
                        
                        Mesh mesh;
                        mesh = atlasDictionary.ContainsKey(name) ? atlasDictionary[name] : null;
                        
                        if (mesh) {
                            meshes.Add(mesh);
                        } else {
                            throw new System.Exception(string.Format("Could not find mesh: {0}", name));
                        }
                        
                    }
                    
                    // create frame
                    MeshAnimationFrame frame;
                    frame = new MeshAnimationFrame();
                    frame.AudioClips = audioClips.ToArray();
                    frame.Meshes = meshes.ToArray();
                    frames.Add(frame);
                    
                }
                
                // read layers
                List<MeshAnimationLayer> layers = new List<MeshAnimationLayer>();
                for (int layerIndex = 0; frames.Count != 0 && layerIndex < frames[0].Meshes.Length; layerIndex++) {
                    
                    List<Mesh> meshes = new List<Mesh>();
                    for (int frameIndex = 0; frameIndex < frames.Count; frameIndex++) {
                        meshes.Add(frames[frameIndex].Meshes[layerIndex]);
                    }
                    
                    MeshAnimationLayer layer;
                    layer = new MeshAnimationLayer();
                    layer.Meshes = meshes.ToArray();
                    layers.Add(layer);
                    
                }
                
                // create animation path
                string animationPath;
                animationPath = string.Format("{0}/{1}.asset", Path.GetDirectoryName(xmlPath), labelNode.Attributes["id"].Value);
                
                // find or create animation
                MeshAnimation animation;
                animation = AssetDatabase.LoadAssetAtPath(animationPath, typeof(MeshAnimation)) as MeshAnimation;
                
                if (animation == null) {
                    animation = ScriptableObject.CreateInstance<MeshAnimation>();
                    AssetDatabase.CreateAsset(animation, animationPath);
                }
                
                // update animation
                animation.AnchorPoint = anchorPoint;
                animation.ContentSize = contentSize;
                animation.Frames = frames.ToArray();
                animation.FramesPerSecond = framesPerSecond;
                animation.Layers = layers.ToArray();
                animation.PixelsPerMeter = EditorSettings.Instance.PixelsPerMeter;
                animation.ResetVersion();
                
                // mark animation dirty to make sure it gets saved
                EditorUtility.SetDirty(animation);
                
            }
            
            // delete xml
            if (EditorSettings.Instance.DeleteIntermediateFiles) {
                AssetDatabase.DeleteAsset(xmlPath);
            }
            
        }
        
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        MeshAnimationAtlas FindAtlas(string directory, string name) {
            
            string atlasPath;
            atlasPath = string.Format("{0}/{1}_atlas.asset", directory, name);
            
            return AssetDatabase.LoadAssetAtPath(atlasPath, typeof(MeshAnimationAtlas)) as MeshAnimationAtlas;
            
        }
        
        AudioClip FindAudioClip(string directory, string name) {
            name = Regex.Replace(name, @"_(wav|aif|aiff)$", string.Empty);
            foreach (string assetPath in AssetDatabase.GetAllAssetPaths()) {
                if (!assetPath.EndsWith(".wav") && !assetPath.EndsWith(".aif") && !assetPath.EndsWith(".aiff")) {
                    continue;
                }
                if (Path.GetFileNameWithoutExtension(assetPath) != name) {
                    continue;
                }
                return AssetDatabase.LoadAssetAtPath(assetPath, typeof(AudioClip)) as AudioClip;
            }
            return null;
        }
        
        
    }
    
}