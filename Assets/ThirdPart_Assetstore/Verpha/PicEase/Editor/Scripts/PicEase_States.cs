#if UNITY_EDITOR
using UnityEditor;

namespace Verpha.PicEase
{
    internal static class PicEase_States
    {
        #region Initialization
        public static void Initialize()
        {
            UpdateEditorThemeCache();
        }
        #endregion

        #region Methods
        private static void UpdateEditorThemeCache()
        {
            PicEase_Session.IsProSkin = EditorGUIUtility.isProSkin;
        }
        #endregion
    }
}
#endif