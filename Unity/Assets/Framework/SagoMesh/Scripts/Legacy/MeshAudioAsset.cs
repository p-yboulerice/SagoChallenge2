namespace SagoEngine {
    
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MeshAudioAsset : ScriptableObject {
        
        // ================================================================= //
        // Variables
        // ================================================================= //
        
        [NonSerialized]
        private Dictionary<int,MeshAudioAssetBinding> m_BindingDictionary;
        
        [SerializeField]
        private List<MeshAudioAssetBinding> m_BindingList;
        
        // ================================================================= //
        // Public Methods
        // ================================================================= //
        
        public bool AddAudioClip(int frame, AudioClip audioClip) {
            UpdateBindingDictionary();
            if (!m_BindingDictionary.ContainsKey(frame)) {
                m_BindingDictionary[frame] = new MeshAudioAssetBinding();
                m_BindingDictionary[frame].AudioClips = new List<AudioClip>();
                m_BindingDictionary[frame].Frame = frame;
            }
            if (!m_BindingDictionary[frame].AudioClips.Contains(audioClip)) {
                m_BindingDictionary[frame].AudioClips.Add(audioClip);
                UpdateBindingList();
                return true;
            }
            return false;
        }
        
        public int[] GetFrames() {
            UpdateBindingDictionary();
            List<int> indices;
            indices = new List<int>(m_BindingDictionary.Keys);
            indices.Sort( (a,b) => { return a.CompareTo(b); });
            return indices.ToArray();
        }
        
        public AudioClip[] GetAudioClips(int frame) {
            UpdateBindingDictionary();
            if (m_BindingDictionary.ContainsKey(frame)) {
                return m_BindingDictionary[frame].AudioClips.ToArray();
            }
            return null;
        }
        
        public bool RemoveAudioClip(int frame, AudioClip audioClip) {
            UpdateBindingDictionary();
            if (m_BindingDictionary.ContainsKey(frame)) {
                if (m_BindingDictionary[frame].AudioClips.Remove(audioClip)) {
                    if (m_BindingDictionary[frame].AudioClips.Count == 0) {
                        m_BindingDictionary.Remove(frame);
                    }
                    UpdateBindingList();
                    return true;
                }
            }
            return false;
        }
        
        public void Clear() {
            m_BindingDictionary = null;
            m_BindingList = null;
        }
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        private void UpdateBindingDictionary() {
            if (m_BindingDictionary == null) {
                m_BindingDictionary = new Dictionary<int,MeshAudioAssetBinding>();
                if (m_BindingList != null) {
                    foreach (MeshAudioAssetBinding binding in m_BindingList) {
                        m_BindingDictionary[binding.Frame] = binding;
                    }
                }
            }
        }
        
        private void UpdateBindingList() {
            if (m_BindingDictionary == null) {
                m_BindingList = null;
            } else {
                m_BindingList = new List<MeshAudioAssetBinding>(m_BindingDictionary.Values);
                #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }
        }
        
    }
    
    [Serializable]
    public class MeshAudioAssetBinding {
        
        // ================================================================= //
        // Variables
        // ================================================================= //
        
        [SerializeField]
        private List<AudioClip> m_AudioClips;
        
        [SerializeField]
        private int m_Frame;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public List<AudioClip> AudioClips {
            get { return m_AudioClips; }
            set { m_AudioClips = value; }
        }
        
        public int Frame {
            get { return m_Frame; }
            set { m_Frame = value; }
        }
        
    }
    
}