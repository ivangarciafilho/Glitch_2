#if UNITY_EDITOR
namespace Verpha.PicEase
{
    internal static class PicEase_Constants
    {
        #region Properties
        #region Asset Info
        public const string AssetName = "PicEase";
        #endregion

        #region Menu Info
        public const string MenuLocation = "Tools/" + AssetName;
        public const int MenuPriority = 0;
        #endregion

        #region Format Info
        public const string DateFormat = "yyyy-MM-dd_hhmmtt";
        #endregion

        #region Folder Names
        public const string EditorFolderName = "Editor";
        public const string DocumentationFolderName = "Documentation";
        public const string ResourcesFolderName = "Resources";
        public const string LUTFolderName = "LUTs";
        public const string SavedDataFolderName = "Saved Data";
        public const string ShaderFolderName = "Shaders";
        #endregion

        #region File Names
        public const string ImageComputeShader = "PicEase_Image.compute";
        public const string MapComputeShader = "PicEase_Map.compute";
        public const string PatchNotesTextFileName = "PicEase Patch Notes.txt";
        public const string SettingsTextFileName = "PicEase_SavedData_Settings.json";
        public const string CustomFilterTextFileName = "PicEase_SavedData_CustomFilters.json";
        #endregion
        #endregion
    }
}
#endif