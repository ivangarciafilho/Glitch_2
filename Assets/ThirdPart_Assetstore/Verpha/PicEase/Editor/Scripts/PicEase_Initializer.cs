#if UNITY_EDITOR
using UnityEditor;

namespace Verpha.PicEase
{
    [InitializeOnLoad]
    internal static class PicEase_Initializer
    {
        static PicEase_Initializer()
        {
            PicEase_States.Initialize();
            if (!PicEase_Session.instance.IsInitialized)
            {
                PicEase_Settings.Initialize();
                PicEase_Filters.Initialize();
                PicEase_Session.instance.IsInitialized = true;
            }
        }
    }
}
#endif