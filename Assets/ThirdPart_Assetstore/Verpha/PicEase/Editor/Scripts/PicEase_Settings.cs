#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase
{
    internal static class PicEase_Settings
    {
        #region Properties
        [System.Serializable]
        private class Settings
        {
            public FilterMode imageEditorDefaultTextureFilterMode = FilterMode.Bilinear;
            public int imageEditorDefaultTextureAnisoLevel = 1;
            public PicEase_Converter.ExportImageFormat imageEditorExportImageFormat = PicEase_Converter.ExportImageFormat.OriginalImage;
            public PicEase_Converter.ImageFormat imageEditorDefaultExportImageFormat = PicEase_Converter.ImageFormat.PNG;
            public bool imageEditorSameExportPath = true;
            public string imageEditorDefaultExportDirectory = "";
            public PicEase_Enums.BehaviourMode imageEditorBehaviourMode = PicEase_Enums.BehaviourMode.Synced;
            public PicEase_Enums.ThreadGroupSize imageEditorThreadGroupSize = PicEase_Enums.ThreadGroupSize.SixteenBySixteen;
            public PicEase_Enums.PrecisionMode imageEditorPrecisionMode = PicEase_Enums.PrecisionMode.Full;
            public bool imageEditorEnableImageInfoDisplay = true;
            public int imageEditorInfoLabelFontSize = 12;
            public Color imageEditorInfoLabelFontColor = Color.white;
            public Color imageEditorInfoBackgroundColor = new(0f, 0f, 0f, 0.5f);
            public bool imageEditorEnableDebugLogForExportedFiles = true;
            public FilterMode mapGeneratorDefaultTextureFilterMode = FilterMode.Bilinear;
            public int mapGeneratorDefaultTextureAnisoLevel = 1;
            public PicEase_Converter.ExportImageFormat mapGeneratorExportImageFormat = PicEase_Converter.ExportImageFormat.OriginalImage;
            public PicEase_Converter.ImageFormat mapGeneratorDefaultExportImageFormat = PicEase_Converter.ImageFormat.PNG;
            public bool mapGeneratorSameExportPath = true;
            public string mapGeneratorDefaultExportDirectory = "";
            public PicEase_Enums.BehaviourMode mapGeneratorBehaviourMode = PicEase_Enums.BehaviourMode.Synced;
            public PicEase_Enums.ThreadGroupSize mapGeneratorThreadGroupSize = PicEase_Enums.ThreadGroupSize.SixteenBySixteen;
            public PicEase_Enums.PrecisionMode mapGeneratorPrecisionMode = PicEase_Enums.PrecisionMode.Full;
            public bool mapGeneratorEnableImageInfoDisplay = true;
            public bool mapGeneratorEnableDebugLogForExportedFiles = true;
            public int sceneScreenshotDefaultImageWidth = 1920;
            public int sceneScreenshotDefaultImageHeight = 1080;
            public PicEase_Converter.ImageFormat sceneScreenshotDefaultExportImageFormat = PicEase_Converter.ImageFormat.PNG;
            public string sceneScreenshotDefaultExportDirectory = "";
        }
        private static Settings settings = new();
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadSettings();

            PicEase_Operations.EnsureComputeShaderProperties(PicEase_Constants.ImageComputeShader, ImageEditorPrecisionMode, ImageEditorThreadGroupSize);
            PicEase_Operations.EnsureComputeShaderProperties(PicEase_Constants.MapComputeShader, MapGeneratorPrecisionMode, MapGeneratorThreadGroupSize);
        }
        #endregion

        #region Accessors
        #region Image Editor
        public static FilterMode ImageEditorDefaultImportTextureFilterMode
        {
            get => settings.imageEditorDefaultTextureFilterMode;
            set
            {
                if (settings.imageEditorDefaultTextureFilterMode != value)
                {
                    settings.imageEditorDefaultTextureFilterMode = value;
                }
            }
        }

        public static int ImageEditorDefaultImportTextureAnisoLevel
        {
            get => settings.imageEditorDefaultTextureAnisoLevel;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, 16);
                if (settings.imageEditorDefaultTextureAnisoLevel != clampedValue)
                {
                    settings.imageEditorDefaultTextureAnisoLevel = clampedValue;
                }
            }
        }

        public static PicEase_Converter.ExportImageFormat ImageEditorExportImageFormat
        {
            get => settings.imageEditorExportImageFormat;
            set
            {
                if (settings.imageEditorExportImageFormat != value)
                {
                    settings.imageEditorExportImageFormat = value;
                }
            }
        }

        public static PicEase_Converter.ImageFormat ImageEditorDefaultExportImageFormat
        {
            get => settings.imageEditorDefaultExportImageFormat;
            set
            {
                if (settings.imageEditorDefaultExportImageFormat != value)
                {
                    settings.imageEditorDefaultExportImageFormat = value;
                }
            }
        }

        public static bool ImageEditorSameExportPath
        {
            get => settings.imageEditorSameExportPath;
            set
            {
                if (settings.imageEditorSameExportPath != value)
                {
                    settings.imageEditorSameExportPath = value;
                }
            }
        }

        public static string ImageEditorDefaultExportDirectory
        {
            get => settings.imageEditorDefaultExportDirectory;
            set
            {
                if (settings.imageEditorDefaultExportDirectory != value)
                {
                    settings.imageEditorDefaultExportDirectory = value;
                }
            }
        }

        public static PicEase_Enums.BehaviourMode ImageEditorBehaviourMode
        {
            get => settings.imageEditorBehaviourMode;
            set
            {
                if (settings.imageEditorBehaviourMode != value)
                {
                    settings.imageEditorBehaviourMode = value;
                }
            }
        }

        public static PicEase_Enums.ThreadGroupSize ImageEditorThreadGroupSize
        {
            get => settings.imageEditorThreadGroupSize;
            set
            {
                if (settings.imageEditorThreadGroupSize != value)
                {
                    settings.imageEditorThreadGroupSize = value;
                }
            }
        }

        public static PicEase_Enums.PrecisionMode ImageEditorPrecisionMode
        {
            get => settings.imageEditorPrecisionMode;
            set
            {
                if (settings.imageEditorPrecisionMode != value)
                {
                    settings.imageEditorPrecisionMode = value;
                }
            }
        }
        
        public static bool ImageEditorEnableImageInfoDisplay
        {
            get => settings.imageEditorEnableImageInfoDisplay;
            set
            {
                if (settings.imageEditorEnableImageInfoDisplay != value)
                {
                    settings.imageEditorEnableImageInfoDisplay = value;
                }
            }
        }

        public static int ImageEditorInfoLabelFontSize
        {
            get => settings.imageEditorInfoLabelFontSize;
            set
            {
                int clampedValue = Mathf.Clamp(value, 7, 21);
                if (settings.imageEditorInfoLabelFontSize != clampedValue)
                {
                    settings.imageEditorInfoLabelFontSize = clampedValue;
                    PicEase_GUI.RefreshImageEditorInfoLabelStyle();
                }
            }
        }

        public static Color ImageEditorInfoLabelFontColor
        {
            get => settings.imageEditorInfoLabelFontColor;
            set
            {
                if (settings.imageEditorInfoLabelFontColor != value)
                {
                    settings.imageEditorInfoLabelFontColor = value;
                    PicEase_GUI.RefreshImageEditorInfoLabelStyle();
                }
            }
        }

        public static Color ImageEditorInfoBackgroundColor
        {
            get => settings.imageEditorInfoBackgroundColor;
            set
            {
                if (settings.imageEditorInfoBackgroundColor != value)
                {
                    settings.imageEditorInfoBackgroundColor = value;
                    PicEase_GUI.RefreshImageEditorInfoLabelStyle();
                }
            }
        }

        public static bool ImageEditorEnableDebugLogForExportedFiles
        {
            get => settings.imageEditorEnableDebugLogForExportedFiles;
            set
            {
                if (settings.imageEditorEnableDebugLogForExportedFiles != value)
                {
                    settings.imageEditorEnableDebugLogForExportedFiles = value;
                }
            }
        }
        #endregion

        #region Map Generator
        public static FilterMode MapGeneratorDefaultImportTextureFilterMode
        {
            get => settings.mapGeneratorDefaultTextureFilterMode;
            set
            {
                if (settings.mapGeneratorDefaultTextureFilterMode != value)
                {
                    settings.mapGeneratorDefaultTextureFilterMode = value;
                }
            }
        }

        public static int MapGeneratorDefaultImportTextureAnisoLevel
        {
            get => settings.mapGeneratorDefaultTextureAnisoLevel;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, 16);
                if (settings.mapGeneratorDefaultTextureAnisoLevel != clampedValue)
                {
                    settings.mapGeneratorDefaultTextureAnisoLevel = clampedValue;
                }
            }
        }

        public static PicEase_Converter.ExportImageFormat MapGeneratorExportImageFormat
        {
            get => settings.mapGeneratorExportImageFormat;
            set
            {
                if (settings.mapGeneratorExportImageFormat != value)
                {
                    settings.mapGeneratorExportImageFormat = value;
                }
            }
        }

        public static PicEase_Converter.ImageFormat MapGeneratorDefaultExportImageFormat
        {
            get => settings.mapGeneratorDefaultExportImageFormat;
            set
            {
                if (settings.mapGeneratorDefaultExportImageFormat != value)
                {
                    settings.mapGeneratorDefaultExportImageFormat = value;
                }
            }
        }

        public static bool MapGeneratorSameExportPath
        {
            get => settings.mapGeneratorSameExportPath;
            set
            {
                if (settings.mapGeneratorSameExportPath != value)
                {
                    settings.mapGeneratorSameExportPath = value;
                }
            }
        }

        public static string MapGeneratorDefaultExportDirectory
        {
            get => settings.mapGeneratorDefaultExportDirectory;
            set
            {
                if (settings.mapGeneratorDefaultExportDirectory != value)
                {
                    settings.mapGeneratorDefaultExportDirectory = value;
                }
            }
        }

        public static PicEase_Enums.BehaviourMode MapGeneratorBehaviourMode
        {
            get => settings.mapGeneratorBehaviourMode;
            set
            {
                if (settings.mapGeneratorBehaviourMode != value)
                {
                    settings.mapGeneratorBehaviourMode = value;
                }
            }
        }

        public static PicEase_Enums.ThreadGroupSize MapGeneratorThreadGroupSize
        {
            get => settings.mapGeneratorThreadGroupSize;
            set
            {
                if (settings.mapGeneratorThreadGroupSize != value)
                {
                    settings.mapGeneratorThreadGroupSize = value;
                }
            }
        }

        public static PicEase_Enums.PrecisionMode MapGeneratorPrecisionMode
        {
            get => settings.mapGeneratorPrecisionMode;
            set
            {
                if (settings.mapGeneratorPrecisionMode != value)
                {
                    settings.mapGeneratorPrecisionMode = value;
                }
            }
        }

        public static bool MapGeneratorEnableImageInfoDisplay
        {
            get => settings.mapGeneratorEnableImageInfoDisplay;
            set
            {
                if (settings.mapGeneratorEnableImageInfoDisplay != value)
                {
                    settings.mapGeneratorEnableImageInfoDisplay = value;
                }
            }
        }

        public static bool MapGeneratorEnableDebugLogForExportedFiles
        {
            get => settings.mapGeneratorEnableDebugLogForExportedFiles;
            set
            {
                if (settings.mapGeneratorEnableDebugLogForExportedFiles != value)
                {
                    settings.mapGeneratorEnableDebugLogForExportedFiles = value;
                }
            }
        }
        #endregion

        #region Scene Screenshot
        public static int SceneScreenshotDefaultImageWidth
        {
            get => settings.sceneScreenshotDefaultImageWidth;
            set
            {
                if (settings.sceneScreenshotDefaultImageWidth != value)
                {
                    settings.sceneScreenshotDefaultImageWidth = value;
                }
            }
        }

        public static int SceneScreenshotDefaultImageHeight
        {
            get => settings.sceneScreenshotDefaultImageHeight;
            set
            {
                if (settings.sceneScreenshotDefaultImageHeight != value)
                {
                    settings.sceneScreenshotDefaultImageHeight = value;
                }
            }
        }

        public static PicEase_Converter.ImageFormat SceneScreenshotDefaultExportImageFormat
        {
            get => settings.sceneScreenshotDefaultExportImageFormat;
            set
            {
                if (settings.sceneScreenshotDefaultExportImageFormat != value)
                {
                    settings.sceneScreenshotDefaultExportImageFormat = value;
                }
            }
        }

        public static string SceneScreenshotDefaultExportDirectory
        {
            get => settings.sceneScreenshotDefaultExportDirectory;
            set
            {
                if (settings.sceneScreenshotDefaultExportDirectory != value)
                {
                    settings.sceneScreenshotDefaultExportDirectory = value;
                }
            }
        }
        #endregion
        #endregion

        #region Save and Load
        public static void SaveSettings()
        {
            string dataFilePath = PicEase_File.GetSavedDataFilePath(PicEase_Constants.SettingsTextFileName);
            string json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(dataFilePath, json);
            AssetDatabase.Refresh();
        }

        public static void LoadSettings()
        {
            string dataFilePath = PicEase_File.GetSavedDataFilePath(PicEase_Constants.SettingsTextFileName);
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                Settings loadedSettings = JsonUtility.FromJson<Settings>(json);
                settings = loadedSettings;
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private static void SetDefaultSettings()
        {
            settings = new()
            {
                imageEditorDefaultTextureFilterMode = FilterMode.Bilinear,
                imageEditorDefaultTextureAnisoLevel = 1,
                imageEditorExportImageFormat = PicEase_Converter.ExportImageFormat.OriginalImage,
                imageEditorDefaultExportImageFormat = PicEase_Converter.ImageFormat.PNG,
                imageEditorSameExportPath = true,
                imageEditorDefaultExportDirectory = "",
                imageEditorBehaviourMode = PicEase_Enums.BehaviourMode.Synced,
                imageEditorThreadGroupSize = PicEase_Enums.ThreadGroupSize.SixteenBySixteen,
                imageEditorPrecisionMode = PicEase_Enums.PrecisionMode.Full,
                imageEditorEnableImageInfoDisplay = true,
                imageEditorInfoLabelFontSize = 12,
                imageEditorInfoLabelFontColor = Color.white,
                imageEditorInfoBackgroundColor = new(0f, 0f, 0f, 0.5f),
                imageEditorEnableDebugLogForExportedFiles = true,
                mapGeneratorDefaultTextureFilterMode = FilterMode.Bilinear,
                mapGeneratorDefaultTextureAnisoLevel = 1,
                mapGeneratorExportImageFormat = PicEase_Converter.ExportImageFormat.OriginalImage,
                mapGeneratorDefaultExportImageFormat = PicEase_Converter.ImageFormat.PNG,
                mapGeneratorSameExportPath = true,
                mapGeneratorEnableImageInfoDisplay = true,
                mapGeneratorDefaultExportDirectory = "",
                mapGeneratorBehaviourMode = PicEase_Enums.BehaviourMode.Synced,
                mapGeneratorThreadGroupSize = PicEase_Enums.ThreadGroupSize.SixteenBySixteen,
                mapGeneratorPrecisionMode = PicEase_Enums.PrecisionMode.Full,
                mapGeneratorEnableDebugLogForExportedFiles = true,
                sceneScreenshotDefaultImageWidth = 1920,
                sceneScreenshotDefaultImageHeight = 1080,
                sceneScreenshotDefaultExportImageFormat = PicEase_Converter.ImageFormat.PNG,
                sceneScreenshotDefaultExportDirectory = ""
            };
        }
        #endregion
    }
}
#endif