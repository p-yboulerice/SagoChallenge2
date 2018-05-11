namespace SagoEngineEditor {
    
	using SagoCore.Submodules;
    using SagoEngine;
    using SagoMeshEditor;
    using SagoMeshEditor.Internal;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;
    
    public class MeshPostprocessor : AssetPostprocessor {
        
        // ================================================================= //
        // Public Methods
        // ================================================================= //
        
        public static void Import(string[] assetPaths) {
            ConvertToAssets(assetPaths);
        }
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        static bool IsMeshAudioPath(string path) {
            return path.EndsWith(".audio.xml");
        }
        
        static bool IsMeshAnimationPath(string path) {
            return (path.EndsWith(".plist") || path.EndsWith(".txt") || path.EndsWith(".xml"));
        }
        
        static bool IsMeshPath(string path) {
            return (path.EndsWith(".bytes") || path.EndsWith(".mesh") || path.EndsWith(".vec"));
        }
        
        
        static void ConvertToAssets(string[] paths) {
            try {
                
                // convert meshes
                List<string> meshPathList;
                meshPathList = new List<string>(paths);
                meshPathList = meshPathList.Distinct().Where( p => IsMeshPath(p) && File.Exists(p) ).ToList();
                meshPathList.Sort((a,b) => { return a.CompareTo(b); });
            
                string[] meshPaths;
                meshPaths = meshPathList.ToArray();
            
                for (int meshIndex = 0; meshIndex < meshPaths.Length; meshIndex++) {
                    string meshPath = meshPaths[meshIndex];
                    EditorUtility.DisplayProgressBar("Converting Meshes", meshPath, (float)meshIndex / (float)meshPaths.Length);
                    ConvertToMeshAsset(meshPath);
                }
                EditorUtility.ClearProgressBar();
            
                // convert animations
                List<string> animationPathList;
                animationPathList = new List<string>(paths);
                animationPathList = animationPathList.Distinct().Where( p => IsMeshAnimationPath(p) && File.Exists(p) ).ToList();
                animationPathList.Sort((a,b) => { return a.CompareTo(b); });
            
                string[] animationPaths;
                animationPaths = animationPathList.ToArray();
            
                for (int animationIndex = 0; animationIndex < animationPaths.Length; animationIndex++) {
                    string animationPath = animationPaths[animationIndex];
                    EditorUtility.DisplayProgressBar("Converting Animations", animationPath, (float)animationIndex / (float)animationPaths.Length);
                    ConvertToMeshAnimationAsset(animationPath);
                }
                EditorUtility.ClearProgressBar();
                
                // convert audio
                List<string> audioPathList;
                audioPathList = new List<string>(paths);
                audioPathList = audioPathList.Distinct().Where( p => IsMeshAudioPath(p) && File.Exists(p) ).ToList();
                audioPathList.Sort((a,b) => { return a.CompareTo(b); });
                
                string[] audioPaths;
                audioPaths = audioPathList.ToArray();
                
                for (int audioIndex = 0; audioIndex < audioPaths.Length; audioIndex++) {
                    string audioPath = audioPaths[audioIndex];
                    EditorUtility.DisplayProgressBar("Converting Audio", audioPath, (float)audioIndex / (float)audioPaths.Length);
                    ConvertToMeshAudioAsset(audioPath);
                }
                EditorUtility.ClearProgressBar();
                
            } catch {
                
                EditorUtility.ClearProgressBar();
                throw;
                
            }
        }
        
        static void ConvertToMeshAsset(string path) {
			
			System.Type submoduleType;
			submoduleType = SubmoduleMapEditorAdaptor.GetSubmoduleType(path);
			
			if (submoduleType == null) {
				return;
			}
			
			string submodulePath;
			submodulePath = SubmoduleMapEditorAdaptor.GetSubmodulePath(submoduleType);
			
			if (string.IsNullOrEmpty(submodulePath)) {
				return;
			}
            
            // load bytes
            byte[] bytes;
            bytes = System.IO.File.ReadAllBytes(path);
            
            // header length
            int headerLength = 0;
            headerLength += sizeof(System.Single) + sizeof(System.Single); // anchor point
            headerLength += sizeof(System.Single) + sizeof(System.Single); // content size
            headerLength += sizeof(System.Boolean) + sizeof(System.Byte) * 3; // is opaque
            headerLength += sizeof(System.UInt32); // index count
            headerLength += sizeof(System.UInt32); // vertex count
            
            // if the bytes are null or not long enough, the file isn't a legacy mesh animation
            if (bytes == null || bytes.Length < headerLength) {
                EditorUtility.ClearProgressBar();
                return;
            }
            
            // read bytes
            MemoryStream stream;
            stream = new MemoryStream(bytes);
            
            BinaryReader reader;
            reader = new BinaryReader(stream);
            
            // read anchor point
            Vector2 anchorPoint;
            anchorPoint = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            
            // read content size
            Vector2 contentSize;
            contentSize = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            
            // read isOpaque
            bool isOpaque;
            isOpaque = !reader.ReadBoolean();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            
            // read index count
            uint indexCount;
            indexCount = reader.ReadUInt32();
            
            // read vertex count
            uint vertexCount;
            vertexCount = reader.ReadUInt32();
            
            // content length
            int contentLength = 0;
            contentLength += sizeof(System.UInt32) * (int)indexCount; // indicies
            contentLength += sizeof(System.Byte) * (int)vertexCount * 4; // colors
            contentLength += sizeof(System.Single) * (int)vertexCount * 2; // positions
            
            // if the expected length doesn't match the actual length, the file isn't a legacy mesh animation
            if (headerLength + contentLength != bytes.Length) {
                reader.Close();
                stream.Close();
                EditorUtility.ClearProgressBar();
                return;
            }
            
            // read indicies
            int[] indicies = new int[indexCount];
            for (int i = 0; i < indexCount; i++) {
                indicies[i] = checked((int)reader.ReadInt32());
            }
            
            // read vertices
            Color32[] colors;
            colors = new Color32[vertexCount];
            
            Vector3[] vertices;
            vertices = new Vector3[vertexCount];
            
            Vector3 offset;
            offset = new Vector3(contentSize.x * -anchorPoint.x, contentSize.y * -anchorPoint.y, 0.0f);
            
            Vector3 scale;
            scale = Vector3.one;
            
            for (int i = 0; i < vertexCount; i++) {
                
                Vector3 vertex;
                vertex = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0.0f);
                vertex.x = (vertex.x + offset.x) * scale.x;
                vertex.y = (vertex.y + offset.y) * scale.y;
                vertex.z = (vertex.z + offset.z) * scale.z;
                
                Color32 color;
                color = new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                
                vertices[i] = vertex;
                colors[i] = color;
                
            }
            
            reader.Close();
            stream.Close();
            
            // create mesh
            UnityEngine.Mesh mesh;
            mesh = new UnityEngine.Mesh();
            mesh.name = Path.GetFileNameWithoutExtension(path);
            mesh.hideFlags = HideFlags.HideInHierarchy;
            mesh.Clear(false);
            mesh.vertices = vertices;
            mesh.colors32 = colors;
            mesh.triangles = indicies;
            mesh.RecalculateBounds();
            
            string resourcePath;
            resourcePath = Path.Combine(submodulePath, string.Format("_Resources_/Meshes/{0}.asset", Path.GetFileNameWithoutExtension(path)));
            
            string assetPath;
            assetPath = Path.Combine(submodulePath, string.Format("Meshes/{0}.asset", Path.GetFileNameWithoutExtension(path)));
            
            MeshAsset existingAsset;
            existingAsset = (
                AssetDatabase.LoadAssetAtPath(resourcePath, typeof(MeshAsset)) as MeshAsset ?? 
                AssetDatabase.LoadAssetAtPath(assetPath, typeof(MeshAsset)) as MeshAsset
            );
            
            string existingPath;
            existingPath = AssetDatabase.GetAssetPath(existingAsset);
            
            if (existingAsset) {
                
                bool isReadable;
                isReadable = false;
                
                Mesh existingMesh = null;
                foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(existingPath)) {
                    if (existingMesh = obj as Mesh) {
                        break;
                    }
                }
                
                if (existingMesh) {
                    isReadable = existingMesh.isReadable;
                    Object.DestroyImmediate(existingMesh, true);
                    // BUG: 
                    // Calling AssetDatabase.SaveAssets() here somehow destroys `mesh`.
                    // Everything seems to work fine without it.
                    // AssetDatabase.SaveAssets();
                }
                
                existingAsset.name = Path.GetFileNameWithoutExtension(path);
                existingAsset.hideFlags = HideFlags.NotEditable;
                existingAsset.AnchorPoint = anchorPoint;
                existingAsset.ContentSize = contentSize;
                existingAsset.IsOpaque = isOpaque;
                existingAsset.Mesh = mesh;
                
                MeshUtil.SetIsReadable(mesh, isReadable);
                AssetDatabase.AddObjectToAsset(mesh, existingPath);
                AssetDatabase.SaveAssets();
                
            } else {
                
                // create folder
                if (!Directory.Exists(Path.GetDirectoryName(assetPath))) {
                    AssetDatabase.CreateFolder(
						Path.GetDirectoryName(Path.GetDirectoryName(assetPath)), 
						Path.GetFileNameWithoutExtension(Path.GetDirectoryName(assetPath))
					);
                }
                
                MeshAsset asset;
                asset = ScriptableObject.CreateInstance(typeof(MeshAsset)) as MeshAsset;
                asset.name = Path.GetFileNameWithoutExtension(path);
                asset.hideFlags = HideFlags.NotEditable;
                asset.AnchorPoint = anchorPoint;
                asset.ContentSize = contentSize;
                asset.IsOpaque = isOpaque;
                asset.Mesh = mesh;
                
                MeshUtil.SetIsReadable(mesh, false);
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.AddObjectToAsset(mesh, assetPath);
                AssetDatabase.SaveAssets();
                
            }
            
            if (SagoMeshEditor.EditorSettings.Instance.DeleteIntermediateFiles) {
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.SaveAssets();
            
        }
        
        static void ConvertToMeshAnimationAsset(string path) {
			
			System.Type submoduleType;
			submoduleType = SubmoduleMapEditorAdaptor.GetSubmoduleType(path);
			
			if (submoduleType == null) {
				return;
			}
			
			string submodulePath;
			submodulePath = SubmoduleMapEditorAdaptor.GetSubmodulePath(submoduleType);
			
			if (string.IsNullOrEmpty(submodulePath)) {
				return;
			}
            
            TextAsset asset;
            asset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			
			if (asset == null) {
				return;
			}
                
            XmlDocument document;
            document = new XmlDocument();

			try {
            	document.LoadXml(asset.text);
			} catch {
				return;
			}

            XmlNode animationsNode;
            animationsNode = document.SelectSingleNode("/plist/dict/key[text() = 'animations']/following-sibling::dict");
            
            if (animationsNode == null) {
                return;
            }
            
            XmlNode anchorPointNode;
            anchorPointNode = document.SelectSingleNode("/plist/dict/key[text() = 'anchorPoint']/following-sibling::string");
            
            Vector2 anchorPoint;
            anchorPoint = ConvertToVector2(anchorPointNode);
            
            XmlNode contentSizeNode;
            contentSizeNode = document.SelectSingleNode("/plist/dict/key[text() = 'contentSize']/following-sibling::string");
            
            Vector2 contentSize;
            contentSize = ConvertToVector2(contentSizeNode);
            
            foreach (XmlNode keyNode in animationsNode.SelectNodes("key")) {
                
                XmlNode dictNode;
                dictNode = keyNode.NextSibling;
                
                XmlNode delayNode;
                delayNode = dictNode.SelectSingleNode("key[text() = 'delay']/following-sibling::real");
                
                XmlNodeList stringNodes;
                stringNodes = dictNode.SelectNodes("key[text() = 'frames']/following-sibling::array/string");
                
                float interval;
                interval = System.Convert.ToSingle(delayNode.InnerText);
                
                float duration;
                duration = interval * stringNodes.Count;
                
                MeshAsset[] meshes;
                meshes = new MeshAsset[stringNodes.Count];
                
                for (int index = 0; index < stringNodes.Count; index++) {
                    
                    string meshPath;
                    meshPath = null;
                    
                    MeshAsset mesh;
                    mesh = null;
                    
                    if (!mesh) {
                        meshPath = string.Format("{0}/_Resources_/Meshes/{1}.asset", submodulePath, Path.GetFileNameWithoutExtension(stringNodes.Item(index).InnerText));
                        mesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(MeshAsset)) as MeshAsset;
                    }
                    
                    if (!mesh && meshes.Length == 1) {
                        meshPath = string.Format("{0}/Resources/Meshes/{1}_0001.asset", submodulePath, Path.GetFileNameWithoutExtension(stringNodes.Item(index).InnerText));
                        mesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(MeshAsset)) as MeshAsset;
                    }
                    
                    if (!mesh) {
                        meshPath = string.Format("{0}/Meshes/{1}.asset", submodulePath, Path.GetFileNameWithoutExtension(stringNodes.Item(index).InnerText));
                        mesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(MeshAsset)) as MeshAsset;
                    }
                    
                    if (!mesh && meshes.Length == 1) {
                        meshPath = string.Format("{0}/Meshes/{1}_0001.asset", submodulePath, Path.GetFileNameWithoutExtension(stringNodes.Item(index).InnerText));
                        mesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(MeshAsset)) as MeshAsset;
                    }
                    
                    if (!mesh) {
                        EditorUtility.ClearProgressBar();
                        throw new System.Exception(string.Format("Cannot load mesh asset: {0}, {1}, {2}", 
                            stringNodes.Item(index).InnerText,
                            string.Format("{0}/Meshes/{1}.asset", submodulePath, Path.GetFileNameWithoutExtension(stringNodes.Item(index).InnerText)),
                            string.Format("{0}/Meshes/{1}_0001.asset", submodulePath, Path.GetFileNameWithoutExtension(stringNodes.Item(index).InnerText))
                        ));
                    }
                    
                    meshes[index] = mesh;
                    
                }
                
                string animationAssetPath;
                animationAssetPath = string.Format("{0}/MeshAnimations/{1}.asset", submodulePath, keyNode.InnerText);
                
                string animationResourcePath;
                animationResourcePath = string.Format("{0}/_Resources_/MeshAnimations/{1}.asset", submodulePath, keyNode.InnerText);
                
                MeshAnimationAsset existingAsset;
                existingAsset = (
                    AssetDatabase.LoadAssetAtPath(animationResourcePath, typeof(MeshAnimationAsset)) as MeshAnimationAsset ??
                    AssetDatabase.LoadAssetAtPath(animationAssetPath, typeof(MeshAnimationAsset)) as MeshAnimationAsset
                );
                
                MeshAnimationAsset animationAsset;
                animationAsset = ScriptableObject.CreateInstance<MeshAnimationAsset>();
                animationAsset.name = keyNode.InnerText;
                animationAsset.AnchorPoint = anchorPoint;
                animationAsset.Audio = existingAsset ? existingAsset.Audio : null;
                animationAsset.ContentSize = contentSize;
                animationAsset.Duration = duration;
                animationAsset.Meshes = meshes;
                animationAsset.RecalculateBounds();
                animationAsset.RecalculateCrop();
                
                if (existingAsset) {
                    
                    EditorUtility.CopySerialized(animationAsset, existingAsset);
                    AssetDatabase.SaveAssets();
                    
                } else {
                
                    // create folder
                    if (!Directory.Exists(Path.GetDirectoryName(animationAssetPath))) {
                        AssetDatabase.CreateFolder(
							Path.GetDirectoryName(Path.GetDirectoryName(animationAssetPath)),
							Path.GetFileName(Path.GetDirectoryName(animationAssetPath))
						);
                    }
                    
                    AssetDatabase.CreateAsset(animationAsset, animationAssetPath);
                    AssetDatabase.SaveAssets();
                    
                }
                
            }
            
            if (SagoMeshEditor.EditorSettings.Instance.DeleteIntermediateFiles) {
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.SaveAssets();
            
        }
        
        static void ConvertToMeshAudioAsset(string path) {
			
			string submodulePath;
			submodulePath = SubmoduleMapEditorAdaptor.GetSubmodulePath(SubmoduleMapEditorAdaptor.GetSubmoduleType(path)) ?? "Assets";
            
            string animationName = path;
            while (Path.HasExtension(animationName)) {
                animationName = Path.GetFileNameWithoutExtension(animationName);
            }
            
            string animationAssetPath;
            animationAssetPath = string.Format("{0}/MeshAnimations/{1}.asset", submodulePath, animationName);
            
            string animationResourcePath;
            animationResourcePath = string.Format("{0}/_Resources_/MeshAnimations/{1}.asset", submodulePath, animationName);
            
            MeshAnimationAsset animationAsset;
            animationAsset = (
                AssetDatabase.LoadAssetAtPath(animationResourcePath, typeof(MeshAnimationAsset)) as MeshAnimationAsset ?? 
                AssetDatabase.LoadAssetAtPath(animationAssetPath, typeof(MeshAnimationAsset)) as MeshAnimationAsset
            );
            
            if (!animationAsset) {
                Debug.Log("missing animation");
                return;
            }
            
            string animationPath;
            animationPath = AssetDatabase.GetAssetPath(animationAsset);
            
            TextAsset textAsset;
            textAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            
            if (!textAsset) {
                Debug.Log("missing xml");
                return;
            }
            
            XmlDocument document = null;
            try {
                document = new XmlDocument();
                document.LoadXml(textAsset.text);
            } catch {
                Debug.Log("invalid xml");
                return;
            }
            
            MeshAudioAsset audioAsset;
            audioAsset = AssetDatabase.LoadAssetAtPath(animationPath, typeof(MeshAudioAsset)) as MeshAudioAsset;
            if (!audioAsset) {
                audioAsset = ScriptableObject.CreateInstance<MeshAudioAsset>();
                animationAsset.Audio = audioAsset;
                AssetDatabase.AddObjectToAsset(audioAsset, animationAsset);
                AssetDatabase.SaveAssets();
            }
            audioAsset.hideFlags = HideFlags.HideInHierarchy;
            audioAsset.name = animationAsset.name;
            audioAsset.Clear();
            
            bool isComplete;
            isComplete = true;
            
            foreach (XmlNode sound in document.SelectNodes("/range/sound")) {
                
                int index;
                index = System.Convert.ToInt32(sound.Attributes.GetNamedItem("offset").Value);
                
                string name;
                name = sound.Attributes.GetNamedItem("id").Value;
                
                AudioClip audioClip;
                audioClip = FindAudioClip(name);
                
                if (audioClip) {
                    audioAsset.AddAudioClip(index, audioClip);
                } else {
                    Debug.Log("missing audio clip: " + name);
                    isComplete = false;
                }
                
            }
            
            if (isComplete && SagoMeshEditor.EditorSettings.Instance.DeleteIntermediateFiles) {
                AssetDatabase.DeleteAsset(path);
            }
            
            AssetDatabase.SaveAssets();
            
        }
        
        
        static Vector2 ConvertToVector2(XmlNode pointNode) {
            
            Regex pattern;
            pattern = new Regex("{ (.+?), (.+?) }");
            
            Match match;
            match = pattern.Match(pointNode.InnerText);
            
            if (match != null) {
                return new Vector2(
                    System.Convert.ToSingle(match.Groups[1].ToString()), 
                    System.Convert.ToSingle(match.Groups[2].ToString())
                );
            }
            
            return Vector2.zero;
            
        }
        
        static AudioClip FindAudioClip(string id) {
            id = Regex.Replace(id, @"_(wav|aif|aiff)$", string.Empty);
            foreach (string path in AssetDatabase.GetAllAssetPaths()) {
                if (!path.EndsWith(".wav") && !path.EndsWith(".aif") && !path.EndsWith(".aiff")) {
                    continue;
                }
                if (Path.GetFileNameWithoutExtension(path) != id) {
                    continue;
                }
                return AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip;
            }
            return null;
        }
        
    }
    
}
