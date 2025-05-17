#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase
{
    internal class PicEase_Session : ScriptableSingleton<PicEase_Session>
    {
        #region Properties
        #region General
        public bool IsInitialized = false;
        #endregion

        #region PicEase_States
        public static bool IsProSkin = false;
        #endregion

        #region PicEase_Window
        public bool IsPatchNotesLoaded = false;
        public string PatchNotesContent = string.Empty;
        public bool IsComputeShaderImageProcessingLoaded = false;
        public bool IsComputeShaderMapGeneratorLoaded = false;
        public ComputeShader ComputeShaderImage = null;
        public ComputeShader ComputeShaderMap = null;
        #endregion

        #region PicEase_Shared_Texture
        public Font FallbackFont = null;
        public Texture2D FallbackTexture2D = null;
        public Texture3D FallbackTexture3D = null;
        #endregion
        #endregion
    }
}
#endif