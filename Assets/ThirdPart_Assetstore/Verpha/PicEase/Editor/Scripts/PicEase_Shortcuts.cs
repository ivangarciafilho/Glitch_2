#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Verpha.PicEase
{
    internal static class PicEase_Shortcuts
    {
        #region Shortcuts    
        #pragma warning disable IDE0051
        [Shortcut("PicEase/Open PicEase Window", KeyCode.Alpha1, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void OpenPicEaseWindow() => PicEase_Window.OpenWindow();

        [MenuItem("Edit/Take Screenshot _#&q", false, 182)]
        private static void TakeScreenshotMenu() => PicEase_Operations.TakeQuickScreenshot();
        #endregion
    }
}
#endif