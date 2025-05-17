#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Verpha.PicEase
{
    internal class PicEase_Window : EditorWindow
    {
        #region Properties
        #region General
        #region Layout
        private Rect screenRect;
        private const float editorWindowMinX =600;
        private const float editorWindowMinY = 400;
        private Vector2 scrollHomeBody;
        private Vector2 scrollAboutWelcomePanel;
        private Vector2 scrollAboutPatchNotes;
        private Vector2 scrollAboutPromotional;
        private Vector2 scrollSettingsFile;
        private Vector2 scrollSettingsShortcuts;
        private Vector2 scrollSettingsImageEditor;
        private Vector2 scrollSettingsMapGenerator;
        private Vector2 scrollSettingsSceneScreenshot;
        private Vector2 scrollImageEditorAdjustments;
        private Vector2 scrollImageEditorColorize;
        private Vector2 scrollImageEditorFilters;
        private Vector2 scrollImageEditorLUTs;
        private Vector2 scrollMapGeneratorAdjustments;
        private const float primaryButtonHeight = 30f;
        private const float secondaryButtonHeight = 25f;
        private const float spacingMenuButtons = 2f;
        private const float labelWidthSettings = 205f;
        private const float labelWidthSceneScreenshot = 100f;
        private const float labelWidthColorize = 92f;
        private const float labelWidthLUT = 105f;
        #endregion

        #region View Mode
        private enum ViewMode { Home, About, Settings, ImageEditor, MapGenerator, SceneScreenshot }
        private ViewMode currentViewMode = ViewMode.Home;

        private Dictionary<string, Action> settingsMenuItems;
        #endregion
        #endregion

        #region Home
        private const float titleAspectRatio = 0.5f;
        private const float titleWidthPercentage = 0.8f;
        private const float titleMinWidth = 512f;
        private const float titleMaxWidth = 1024f;
        private const float titleMinHeight = 150f;
        private const float titleMaxHeight = 512f;
        private const string buttonsDivisor = "▪";
        #endregion

        #region About
        private const string imageEditorText =
            "The Image Editor allows you to easily modify and enhance images, such as textures, sprites, or any other type of image.\n";

        private const string mapGeneratorText =
            "The Map Generator enables you to create additional maps, such as normal maps and height maps, using a diffuse/albedo map as the source.\n";

        private const string sceneScreenshotText =
            "The Scene Screenshot allows you to quickly capture screenshots from any scene's camera view.\n";

        private const string shortcutText =
            "PicEase utilizes UnityEditor.ShortcutManagement to manage its shortcuts.\n\n" +
            "You can customize PicEase's shortcuts in the Unity Shortcuts window (Top Bar > Edit > Shortcuts...).\n";

        private const string savedDataText =
            "Settings and custom Filters are saved in the 'Saved Data' folder (located at: .../Assets/.../PicEase/Editor/Saved Data) as .json files.\n\n" +
            "To export PicEase's data to another project, simply copy and paste the .json files into the other project's saved data folder, and then restart the editor.\n";

        private const string additionalNotesText =
            "PicEase is currently in development, and more features and improvements are coming soon.\n\n" +
            "PicEase is an editor-only tool, meaning it won’t be included in your build.\n" +
            "It also has no dependencies, such as packages, external libraries, prefabs, managers and so on.\n\n" +
            "If you like PicEase, please rate it on the Asset Store.\n\n" +
            "If you have any questions, would like to report a bug, or have suggestions for improvements, you may email me at: VerphaSuporte@outlook.com";

        private string patchNotes = string.Empty;
        #endregion

        #region Settings
        private enum SettingsPanel { ImageEditor, MapGenerator, SceneScreenshot, Shortcuts }
        private SettingsPanel currentSettingsPanel = SettingsPanel.ImageEditor;

        #region Image Editor
        private FilterMode imageEditorDefaultTextureFilterMode;
        private int imageEditorDefaultTextureAnisoLevel;
        private PicEase_Converter.ExportImageFormat imageEditorExportImageFormat;
        private PicEase_Converter.ImageFormat imageEditorDefaultExportImageFormat;
        private bool imageEditorSameExportPath;
        private string imageEditorDefaultExportDirectory;
        PicEase_Enums.BehaviourMode imageEditorBehaviourMode;
        PicEase_Enums.ThreadGroupSize imageEditorThreadGroupSize;
        PicEase_Enums.PrecisionMode imageEditorPrecisionMode;
        private bool imageEditorEnableImageInfoDisplay;
        private int imageEditorInfoLabelFontSize;
        private Color imageEditorInfoLabelFontColor;
        private Color imageEditorInfoBackgroundColor;
        private bool imageEditorEnableDebugLogForExportedFiles;
        #endregion

        #region Map Generator
        private FilterMode mapGeneratorDefaultTextureFilterMode;
        private int mapGeneratorDefaultTextureAnisoLevel;
        private PicEase_Converter.ExportImageFormat mapGeneratorExportImageFormat;
        private PicEase_Converter.ImageFormat mapGeneratorDefaultExportImageFormat;
        private bool mapGeneratorSameExportPath;
        private string mapGeneratorDefaultExportDirectory;
        PicEase_Enums.BehaviourMode mapGeneratorBehaviourMode;
        PicEase_Enums.ThreadGroupSize mapGeneratorThreadGroupSize;
        PicEase_Enums.PrecisionMode mapGeneratorPrecisionMode;
        public bool mapGeneratorEnableImageInfoDisplay;
        public bool mapGeneratorEnableDebugLogForExportedFiles;
        #endregion

        #region Scene Screenshot
        private int sceneScreenshotDefaultImageWidth;
        private int sceneScreenshotDefaultImageHeight;
        private PicEase_Converter.ImageFormat sceneScreenshotDefaultExportImageFormat;
        private string sceneScreenshotDefaultExportDirectory;
        #endregion

        #region Shortcuts
        private const float minorShortcutCommandLabelWidth = 200;
        private const float minorShortcutLabelWidth = 400;
        private readonly List<string> minorShortcutIdentifiers = new()
        {
            "PicEase/Open PicEase Window",
            "PicEase/Take Screenshot",
        };
        private readonly Dictionary<string, string> minorShortcutTooltips = new()
        {
            { "PicEase/Open PicEase Window", "Opens the PicEase window." },
            { "PicEase/Take Screenshot", "Takes a quick screenshot, primarily designed for use during play mode, but not exclusively.\n\nNote: Uses the currently active/main camera. Quick screenshot uses all the default Scene Screenshot file settings' values. Screenshots are exported to the Scene Screenshot's Default Export Directory. If the directory is not valid, they are saved to Application.dataPath.\n\n(This shortcut's keybind is uneditable.)\n" },
        };
        #endregion
        #endregion

        #region Image Editor
        private enum ImageEditorCategory { Adjustments, Colorize, Filters, LUTs }
        private ImageEditorCategory currentImageEditorCategory = ImageEditorCategory.Adjustments;
        private string cachedCategoryLabel;

        private Texture2D originalImage;
        private TextureImporterSettings originalImageImporterSettings;
        private int originalMaxTextureSize;
        private TextureImporterCompression originalTextureCompression;
        private ComputeShader imageComputeShader;
        private PicEase_Image imageManager;

        #region Adjustments
        private const float adjustmentSliderDefaultValue = 0f;
        private const float adjustmentSliderMinValue = -100f;
        private const float adjustmentSliderMaxValue = 100f;
        #endregion

        #region Colorize
        private int autoDetectCount = 3;
        private Color colorizeSourceColor = Color.white;
        private Color colorizeTargetColor = Color.white;
        #endregion

        #region Filters
        private enum FilterViewState { Main, Group }
        private FilterViewState filterViewState = FilterViewState.Main;
        private string selectedFilterGroup = null;
        private string filterNameInput = "";
        #endregion

        #region LUTs
        #endregion

        private bool isComparing = false;
        private bool isComparingDoOnce = false;
        private string currentLoadedImageName = "Image";
        private string currentImagePath = string.Empty;
        private string currentImageFormat = "Unknown";
        private int currentImageWidth = 0;
        private int currentImageHeight = 0;
        private string currentImageDirectory = string.Empty;
        private PicEase_Converter.ImageFormat imageEditorCurrentExportFormat;

        private const float imageInfoLabelPadding = 2f;
        internal const float ImageInfoLabelHeight = 30f;
        #endregion

        #region Map Generator
        private enum MapGeneratorMapView { Normal, Height }
        private MapGeneratorMapView currentMapView = MapGeneratorMapView.Normal;
        private Texture2D diffuseMap;
        private ComputeShader mapComputeShader;
        private PicEase_Map mapManager;
        private string currentLoadedDiffuseMapName = "Image";
        private string currentDiffuseMapFormat = "Unknown";
        private int currentDiffuseMapWidth = 0;
        private int currentDiffuseMapHeight = 0;
        private string currentDiffuseMapPath = string.Empty;
        private string currentDiffuseMapDirectory = string.Empty;
        private PicEase_Converter.ImageFormat mapGeneratorCurrentExportFormat;
        #endregion

        #region Scene Screenshot
        private Camera[] availableCameras;
        private string[] cameraNames;
        private int selectedCameraIndex = 0;

        private Texture2D sceneScreenshot;
        private Texture2D exportScreenshot;

        private int sceneScreenshotCurrentImageWidth;
        private int sceneScreenshotCurrentImageHeight;

        private bool hdrExport = false;
        private PicEase_Converter.ImageFormat sceneScreenshotCurrentExportFormat;
        #endregion
        #endregion

        [MenuItem(PicEase_Constants.MenuLocation, false, PicEase_Constants.MenuPriority)]
        public static void OpenWindow()
        {
            PicEase_Window editorWindow = GetWindow<PicEase_Window>(PicEase_Constants.AssetName);
            editorWindow.minSize = new(editorWindowMinX, editorWindowMinY);
        }

        #region Initialization
        private void OnEnable()
        {
            InitializeHome();
            InitializeMenus();
            InitializeSettings();
            InitializeImageEditor();
            InitializeMapGenerator();
            InitializeSceneScreenshot();
        }

        private void InitializeHome()
        {
            if (!PicEase_Session.instance.IsPatchNotesLoaded)
            {
                patchNotes = PicEase_File.GetPatchNotesData();
                PicEase_Session.instance.PatchNotesContent = patchNotes;
                PicEase_Session.instance.IsPatchNotesLoaded = true;
            }
            else
            {
                patchNotes = PicEase_Session.instance.PatchNotesContent;
            }
        }

        private void InitializeMenus()
        {
            if (settingsMenuItems == null)
            {
                settingsMenuItems = new()
                {
                    { "Image Editor", () => { SelectSettingsImageEditorPanel(); } },
                    { "Map Generator", () => { SelectSettingsMapGeneratorPanel(); } },
                    { "Scene Screenshot", () => { SelectSettingsSceneScreenshotPanel(); } },
                    { "Shortcuts", () => { SelectSettingsShortcutsPanel(); } }
                };
            }     
        }

        private void InitializeSettings()
        {
            PicEase_Settings.LoadSettings();
            imageEditorDefaultTextureFilterMode = PicEase_Settings.ImageEditorDefaultImportTextureFilterMode;
            imageEditorDefaultTextureAnisoLevel = PicEase_Settings.ImageEditorDefaultImportTextureAnisoLevel;
            imageEditorExportImageFormat = PicEase_Settings.ImageEditorExportImageFormat;
            imageEditorDefaultExportImageFormat = PicEase_Settings.ImageEditorDefaultExportImageFormat;
            imageEditorSameExportPath = PicEase_Settings.ImageEditorSameExportPath;
            imageEditorDefaultExportDirectory = PicEase_Settings.ImageEditorDefaultExportDirectory;
            imageEditorBehaviourMode = PicEase_Settings.ImageEditorBehaviourMode;
            imageEditorThreadGroupSize = PicEase_Settings.ImageEditorThreadGroupSize;
            imageEditorPrecisionMode = PicEase_Settings.ImageEditorPrecisionMode;
            imageEditorEnableImageInfoDisplay = PicEase_Settings.ImageEditorEnableImageInfoDisplay;
            imageEditorInfoLabelFontSize = PicEase_Settings.ImageEditorInfoLabelFontSize;
            imageEditorInfoLabelFontColor = PicEase_Settings.ImageEditorInfoLabelFontColor;
            imageEditorInfoBackgroundColor = PicEase_Settings.ImageEditorInfoBackgroundColor;
            imageEditorEnableDebugLogForExportedFiles = PicEase_Settings.ImageEditorEnableDebugLogForExportedFiles;
            mapGeneratorDefaultTextureFilterMode = PicEase_Settings.MapGeneratorDefaultImportTextureFilterMode;
            mapGeneratorDefaultTextureAnisoLevel = PicEase_Settings.MapGeneratorDefaultImportTextureAnisoLevel;
            mapGeneratorExportImageFormat = PicEase_Settings.MapGeneratorExportImageFormat;
            mapGeneratorDefaultExportImageFormat = PicEase_Settings.MapGeneratorDefaultExportImageFormat;
            mapGeneratorSameExportPath = PicEase_Settings.MapGeneratorSameExportPath;
            mapGeneratorDefaultExportDirectory = PicEase_Settings.MapGeneratorDefaultExportDirectory;
            mapGeneratorBehaviourMode = PicEase_Settings.MapGeneratorBehaviourMode;
            mapGeneratorThreadGroupSize = PicEase_Settings.MapGeneratorThreadGroupSize;
            mapGeneratorPrecisionMode = PicEase_Settings.MapGeneratorPrecisionMode;
            mapGeneratorEnableImageInfoDisplay = PicEase_Settings.MapGeneratorEnableImageInfoDisplay;
            mapGeneratorEnableDebugLogForExportedFiles = PicEase_Settings.MapGeneratorEnableDebugLogForExportedFiles;
            sceneScreenshotDefaultImageWidth = PicEase_Settings.SceneScreenshotDefaultImageWidth;
            sceneScreenshotDefaultImageHeight = PicEase_Settings.SceneScreenshotDefaultImageHeight;
            sceneScreenshotDefaultExportImageFormat = PicEase_Settings.SceneScreenshotDefaultExportImageFormat;
            sceneScreenshotDefaultExportDirectory = PicEase_Settings.SceneScreenshotDefaultExportDirectory;
        }

        private void InitializeImageEditor()
        {
            if (PicEase_Session.instance.ComputeShaderImage == null)
            {
                string shaderPath = PicEase_File.GetShaderFilePath(PicEase_Constants.ImageComputeShader);
                imageComputeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(shaderPath);
                PicEase_Session.instance.ComputeShaderImage = imageComputeShader;
                PicEase_Session.instance.IsComputeShaderImageProcessingLoaded = true;
            }
            else
            {
                imageComputeShader = PicEase_Session.instance.ComputeShaderImage;
            }

            if (imageManager != null)
            {
                imageManager.ResetImageEffects();
            }

            UpdateImageEditorInitializedFields();
            PicEase_Filters.Initialize();
        }

        private void InitializeMapGenerator()
        {
            LoadMapComputeShader();

            if (mapManager != null)
            {
                mapManager.ResetMapData();
            }
        }

        private void InitializeSceneScreenshot()
        {
            UpdateSceneScreenshotInitializedFields();
        }
        #endregion

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(PicEase_GUI.PrimaryPanelStyle);
            screenRect = new(0, 0, Screen.width, Screen.height);

            #region Header
            if (currentViewMode != ViewMode.Home)
            {
                DrawHeader();
            }
            #endregion

            #region Body
            EditorGUILayout.BeginVertical();
            switch (currentViewMode)
            {
                case ViewMode.Home:
                    DrawHomeTab();
                    break;
                case ViewMode.About:
                    DrawAboutTab();
                    break;
                case ViewMode.Settings:
                    DrawSettingsTab();
                    break;
                case ViewMode.ImageEditor:
                    DrawImageEditorTab();
                    break;
                case ViewMode.MapGenerator:
                    DrawMapGeneratorTab();
                    break;
                case ViewMode.SceneScreenshot:
                    DrawSceneScreenshotTab();
                    break;
            }
            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.EndVertical();
        }

        #region Methods
        #region General
        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(PicEase_GUI.MenuButtonsPanelStyle);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("PICEASE", PicEase_GUI.HeaderTitleLabelStyle);
            GUILayout.Space(18);

            EditorGUILayout.BeginVertical();
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("HOME", PicEase_GUI.HeaderButtonStyle))
            {
                SelectHomeTab();
            }

            GUILayout.Space(spacingMenuButtons);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.HeaderButtonsDivisorLabelStyle);
            GUILayout.Space(spacingMenuButtons);

            if (GUILayout.Button("ABOUT", PicEase_GUI.HeaderButtonStyle))
            {
                SelectAboutTab();
            }

            GUILayout.Space(spacingMenuButtons);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.HeaderButtonsDivisorLabelStyle);
            GUILayout.Space(spacingMenuButtons);

            PicEase_GUI.DrawPopupButton("SETTINGS ▾", PicEase_GUI.HeaderButtonStyle, 25, settingsMenuItems);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(1);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("IMAGE EDITOR", PicEase_GUI.HeaderButtonStyle))
            {
                SelectImageEditorTab();
            }

            GUILayout.Space(spacingMenuButtons);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.HeaderButtonsDivisorLabelStyle);
            GUILayout.Space(spacingMenuButtons);

            if (GUILayout.Button("MAP GENERATOR", PicEase_GUI.HeaderButtonStyle))
            {
                SelectMapGeneratorTab();
            }

            GUILayout.Space(spacingMenuButtons);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.HeaderButtonsDivisorLabelStyle);
            GUILayout.Space(spacingMenuButtons);

            if (GUILayout.Button("SCENE SCREENSHOT", PicEase_GUI.HeaderButtonStyle))
            {
                SelectSceneScreenshotTab();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void SelectHomeTab()
        {
            SwitchViewMode(ViewMode.Home);
        }

        private void SelectAboutTab()
        {
            SwitchViewMode(ViewMode.About);
        }

        private void SelectMapGeneratorTab()
        {
            SwitchViewMode(ViewMode.MapGenerator, UpdateImageEditorCategoryLabel);
        }

        private void SelectImageEditorTab()
        {
            SwitchViewMode(ViewMode.ImageEditor, UpdateImageEditorCategoryLabel);
        }

        private void SelectSceneScreenshotTab()
        {
            SwitchViewMode(ViewMode.SceneScreenshot, RefreshCameraList);
        }

        private void SwitchViewMode(ViewMode newViewMode, Action extraAction = null)
        {
            if (currentViewMode == newViewMode) return;

            extraAction?.Invoke();
            currentViewMode = newViewMode;
        }
        #endregion

        #region Home
        private void DrawHomeTab()
        {
            DrawOverlay();

            scrollHomeBody = EditorGUILayout.BeginScrollView(scrollHomeBody, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.Space(20);
            GUILayout.FlexibleSpace();

            #region Title
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawAssetTitle();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Menu Buttons
            EditorGUILayout.BeginVertical();
            DrawHomeMenuButtons();
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.FlexibleSpace();
            EditorGUILayout.Space(5);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawOverlay()
        {
            GUI.DrawTexture(screenRect, PicEase_Resources.Graphics.Overlay, ScaleMode.ScaleAndCrop);
        }

        private void DrawAssetTitle()
        {
            float labelWidth = position.width * titleWidthPercentage;
            float labelHeight = labelWidth * titleAspectRatio;

            labelWidth = Mathf.Clamp(labelWidth, titleMinWidth, titleMaxWidth);
            labelHeight = Mathf.Clamp(labelHeight, titleMinHeight, titleMaxHeight);

            GUILayout.Label(PicEase_Resources.Graphics.Title, PicEase_GUI.TitleLabelStyle, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
        }

        private void DrawHomeMenuButtons()
        {
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("HOME", PicEase_GUI.MenuButtonStyle))
            {
                SelectHomeTab();
            }

            GUILayout.Space(4);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.ButtonsDivisorLabelStyle);
            GUILayout.Space(4);

            if (GUILayout.Button("ABOUT", PicEase_GUI.MenuButtonStyle))
            {
                SelectAboutTab();
            }

            GUILayout.Space(4);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.ButtonsDivisorLabelStyle);
            GUILayout.Space(4);

            PicEase_GUI.DrawPopupButton("SETTINGS ▾", PicEase_GUI.MenuButtonStyle, 25, settingsMenuItems);
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(spacingMenuButtons);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("IMAGE EDITOR", PicEase_GUI.MenuButtonStyle))
            {
                SelectImageEditorTab();
            }

            GUILayout.Space(4);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.ButtonsDivisorLabelStyle);
            GUILayout.Space(4);

            if (GUILayout.Button("MAP GENERATOR", PicEase_GUI.MenuButtonStyle))
            {
                SelectMapGeneratorTab();
            }

            GUILayout.Space(4);
            GUILayout.Label($"{buttonsDivisor}", PicEase_GUI.ButtonsDivisorLabelStyle);
            GUILayout.Space(4);

            if (GUILayout.Button("SCENE SCREENSHOT", PicEase_GUI.MenuButtonStyle))
            {
                SelectSceneScreenshotTab();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
        }
        #endregion

        #region About
        private void DrawAboutTab()
        {
            EditorGUILayout.BeginHorizontal();
            DrawWelcomePanel();
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical();
            DrawPatchNotesPanel();
            GUILayout.Space(5);
            DrawPromotionalPanel();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawWelcomePanel()
        {
            scrollAboutWelcomePanel = EditorGUILayout.BeginScrollView(scrollAboutWelcomePanel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.Label("Features Breakdown", PicEase_GUI.BoldLabelStyle);

            GUILayout.Label("Image Editor", PicEase_GUI.MiniBoldLabelStyle);
            GUILayout.Label(imageEditorText, PicEase_GUI.RegularLabelStyle);

            GUILayout.Label("Map Generator", PicEase_GUI.MiniBoldLabelStyle);
            GUILayout.Label(mapGeneratorText, PicEase_GUI.RegularLabelStyle);

            GUILayout.Label("Scene Screenshot", PicEase_GUI.MiniBoldLabelStyle);
            GUILayout.Label(sceneScreenshotText, PicEase_GUI.RegularLabelStyle);

            GUILayout.Label("Shortcuts", PicEase_GUI.MiniBoldLabelStyle);
            GUILayout.Label(shortcutText, PicEase_GUI.RegularLabelStyle);

            GUILayout.Label("Saved Data", PicEase_GUI.MiniBoldLabelStyle);
            GUILayout.Label(savedDataText, PicEase_GUI.RegularLabelStyle);

            GUILayout.Label("Additional Notes", PicEase_GUI.BoldLabelStyle);
            GUILayout.Label(additionalNotesText, PicEase_GUI.RegularLabelStyle);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawPatchNotesPanel()
        {
            scrollAboutPatchNotes = EditorGUILayout.BeginScrollView(scrollAboutPatchNotes, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("Patch Notes", PicEase_GUI.BoldLabelStyle);
            GUILayout.Label(patchNotes, PicEase_GUI.RegularLabelStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawPromotionalPanel()
        {
            scrollAboutPromotional = EditorGUILayout.BeginScrollView(scrollAboutPromotional, GUILayout.MinHeight(125), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("My Other Assets", PicEase_GUI.BoldLabelStyle);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(string.Empty, PicEase_GUI.PromotionalButtonStyle))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/hierarchy-designer-273928");
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(2);
            GUILayout.Label("Hierarchy Designer", PicEase_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);
            GUILayout.Label("An editor tool to enhance your hierarchy window.", PicEase_GUI.RegularLabelStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region Settings
        private void DrawSettingsTab()
        {
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle);

            #region Body
            switch (currentSettingsPanel)
            {
                case SettingsPanel.ImageEditor:
                    DrawSettingsImageEditorPanel();
                    break;
                case SettingsPanel.MapGenerator:
                    DrawSettingsMapGeneratorPanel();
                    break;
                case SettingsPanel.SceneScreenshot:
                    DrawSettingsSceneScreenshotPanel();
                    break;
                case SettingsPanel.Shortcuts:
                    DrawSettingsShortcutsFilePanel();
                    break;
            }
            #endregion

            if(currentSettingsPanel == SettingsPanel.Shortcuts)
            {
                if (GUILayout.Button("Open Shortcut Manager", GUILayout.Height(primaryButtonHeight)))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Shortcuts...");
                }
            }
            if (GUILayout.Button("Save All Settings", GUILayout.Height(primaryButtonHeight)))
            {
                SaveSettings();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSettingsImageEditorPanel()
        {
            scrollSettingsImageEditor = EditorGUILayout.BeginScrollView(scrollSettingsImageEditor, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("Image Editor", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("File Settings", PicEase_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(5);
            imageEditorDefaultTextureFilterMode = PicEase_GUI.DrawEnumPopup("Default Texture Filter Mode", labelWidthSettings, imageEditorDefaultTextureFilterMode, FilterMode.Bilinear, true, "The texture filter mode value applied to images loaded from outside your Unity Project.");
            imageEditorDefaultTextureAnisoLevel = PicEase_GUI.DrawIntSlider("Default Texture Aniso Level", labelWidthSettings, imageEditorDefaultTextureAnisoLevel, 0, 16, 1, true, "The aniso level value applied to images loaded from outside your Unity Project.");
            imageEditorExportImageFormat = PicEase_GUI.DrawEnumPopup("Export File Format Type", labelWidthSettings, imageEditorExportImageFormat, PicEase_Converter.ExportImageFormat.OriginalImage, true, "Export Values' Image Format Behavior:\n\nOriginal: Initializes the export image format field to match the file format of the original image (e.g., PNG, JPG).\n\nSettings: Initializes the export image format field with the default value specified in the export settings.");
            imageEditorDefaultExportImageFormat = PicEase_GUI.DrawEnumPopup("Default Export File Format", labelWidthSettings, imageEditorDefaultExportImageFormat, PicEase_Converter.ImageFormat.PNG, true, "The default initialized value of the Image Format in the Export Field.");
            imageEditorSameExportPath = PicEase_GUI.DrawToggle("Same Export Path", labelWidthSettings, imageEditorSameExportPath, true, true, "The directory will open in the same path as the input (original) image that was loaded.\n\nNote: This overrides the default export directory.");
            imageEditorDefaultExportDirectory = PicEase_GUI.DrawStringField("Default Export Directory", labelWidthSettings, imageEditorDefaultExportDirectory, "", true, @"The default directory initialized for the exported image. When exporting an image, this directory will be set as the default path if valid (e.g., D:\...\ProjectName\Assets).");

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Performance Settings", PicEase_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(5);
            imageEditorBehaviourMode = PicEase_GUI.DrawEnumPopup("Behaviour Mode", labelWidthSettings, imageEditorBehaviourMode, PicEase_Enums.BehaviourMode.Synced, true, "Choose the Behaviour Mode for image processing:\n\n" +
                "• Synced – GPU and CPU are synchronized. Image processing uses AsyncGPUReadbackRequest, which prevents freezes during readback but introduces a slight FPS latency, particularly noticeable on high-resolution images and lower-end devices.\n\n" +
                "• Unsynced – Uses the previous behavior. Image processing may cause occasional minor freezes, with minimal to no FPS latency.\n\n" +
                "*Re-open the PicEase window for this change to take effect.*");
            imageEditorThreadGroupSize = PicEase_GUI.DrawEnumPopup("Thread Group Size", labelWidthSettings, imageEditorThreadGroupSize, PicEase_Enums.ThreadGroupSize.SixteenBySixteen, true, "Adjust the number of threads used for processing (8x8, 16x16, 32x32). More threads can improve performance on powerful hardware, while fewer threads may reduce resource usage.\n\n*Re-open the PicEase window for this change to take effect.*");
            imageEditorPrecisionMode = PicEase_GUI.DrawEnumPopup("Compute Precision Mode", labelWidthSettings, imageEditorPrecisionMode, PicEase_Enums.PrecisionMode.Full, true, "Choose the precision mode for image processing:\n\n" +
                "• Half precision (half) offers faster performance and uses less memory but may result in reduced color accuracy or subtle artifacts in fine adjustments, especially in complex images.\n\n" +
                "• Full precision (float) provides the highest color accuracy and quality, ensuring smoother blending and more precise adjustments, though it may come with a slight performance cost, particularly with large images.\n\n" +
                "*Re-open the PicEase window for this change to take effect.*");
            
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Miscellaneous Settings", PicEase_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(5);
            imageEditorEnableImageInfoDisplay = PicEase_GUI.DrawToggle("Enable Image Info Display", labelWidthSettings, imageEditorEnableImageInfoDisplay, true, true, "Display image info (name, dimensions, format, and source) while in compare mode.");
            imageEditorInfoLabelFontSize = PicEase_GUI.DrawIntSlider("Image Info Label Font Size", labelWidthSettings, imageEditorInfoLabelFontSize, 7, 21, 12, true, "The label font size for the image information displayed in compare mode.");
            imageEditorInfoLabelFontColor = PicEase_GUI.DrawColorField("Image Info Label Font Color", labelWidthSettings, imageEditorInfoLabelFontColor, Color.white, true, "The label font color for the image information displayed in compare mode.");
            imageEditorInfoBackgroundColor = PicEase_GUI.DrawColorField("Image Info Background Color", labelWidthSettings, imageEditorInfoBackgroundColor, new(0f, 0f, 0f, 0.25f), true, "The background color for the image information displayed in compare mode.");
            imageEditorEnableDebugLogForExportedFiles = PicEase_GUI.DrawToggle("Enable Debug.Log for Exports", labelWidthSettings, imageEditorEnableDebugLogForExportedFiles, true, true, "Display a Debug.Log message showing the directory where the edited image was exported.");
            
            EditorGUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        private void DrawSettingsMapGeneratorPanel()
        {
            scrollSettingsMapGenerator = EditorGUILayout.BeginScrollView(scrollSettingsMapGenerator, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("Map Generator", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("File Settings", PicEase_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(5);
            mapGeneratorDefaultTextureFilterMode = PicEase_GUI.DrawEnumPopup("Default Texture Filter Mode", labelWidthSettings, mapGeneratorDefaultTextureFilterMode, FilterMode.Bilinear, true, "The texture filter mode value applied to images loaded from outside your Unity Project.");
            mapGeneratorDefaultTextureAnisoLevel = PicEase_GUI.DrawIntSlider("Default Texture Aniso Level", labelWidthSettings, mapGeneratorDefaultTextureAnisoLevel, 0, 16, 1, true, "The aniso level value applied to images loaded from outside your Unity Project.");
            mapGeneratorExportImageFormat = PicEase_GUI.DrawEnumPopup("Export File Format Type", labelWidthSettings, mapGeneratorExportImageFormat, PicEase_Converter.ExportImageFormat.OriginalImage, true, "Export Values' Image Format Behavior:\n\nOriginal: Initializes the export image format field to match the file format of the original image (e.g., PNG, JPG).\n\nSettings: Initializes the export image format field with the default value specified in the export settings.");
            mapGeneratorDefaultExportImageFormat = PicEase_GUI.DrawEnumPopup("Default Export File Format", labelWidthSettings, mapGeneratorDefaultExportImageFormat, PicEase_Converter.ImageFormat.PNG, true, "The default initialized value of the Image Format in the Export Field.");
            mapGeneratorSameExportPath = PicEase_GUI.DrawToggle("Same Export Path", labelWidthSettings, mapGeneratorSameExportPath, true, true, "The directory will open in the same path as the input (original) image that was loaded.\n\nNote: This overrides the default export directory.");
            mapGeneratorDefaultExportDirectory = PicEase_GUI.DrawStringField("Default Export Directory", labelWidthSettings, mapGeneratorDefaultExportDirectory, "", true, @"The default directory initialized for the exported image. When exporting an image, this directory will be set as the default path if valid (e.g., D:\...\ProjectName\Assets).");

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Performance Settings", PicEase_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(5);
            mapGeneratorBehaviourMode = PicEase_GUI.DrawEnumPopup("Behaviour Mode", labelWidthSettings, mapGeneratorBehaviourMode, PicEase_Enums.BehaviourMode.Synced, true, "Choose the Behaviour Mode for map generation:\n\n" +
                "• Synced – GPU and CPU are synchronized. Image processing uses AsyncGPUReadbackRequest, which prevents freezes during readback but introduces a slight FPS latency, particularly noticeable on high-resolution images and lower-end devices.\n\n" +
                "• Unsynced – Uses the previous behavior. Image processing may cause occasional minor freezes, with minimal to no FPS latency.\n\n" +
                "*Re-open the PicEase window for this change to take effect.*");
            mapGeneratorThreadGroupSize = PicEase_GUI.DrawEnumPopup("Thread Group Size", labelWidthSettings, mapGeneratorThreadGroupSize, PicEase_Enums.ThreadGroupSize.SixteenBySixteen, true, "Adjust the number of threads used for processing (8x8, 16x16, 32x32). More threads can improve performance on powerful hardware, while fewer threads may reduce resource usage.\n\n*Re-open the PicEase window for this change to take effect.*");
            mapGeneratorPrecisionMode = PicEase_GUI.DrawEnumPopup("Compute Precision Mode", labelWidthSettings, mapGeneratorPrecisionMode, PicEase_Enums.PrecisionMode.Full, true, "Choose the precision mode for map generation:\n\n" +
                "• Half precision (half) offers faster performance and uses less memory but may result in reduced color accuracy or subtle artifacts in fine adjustments, especially in complex maps.\n\n" +
                "• Full precision (float) provides the highest color accuracy and quality, ensuring smoother blending and more precise adjustments, though it may come with a slight performance cost, particularly with large maps.\n\n" +
                "*Re-open the PicEase window for this change to take effect.*");
            
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Miscellaneous Settings", PicEase_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(5);
            mapGeneratorEnableImageInfoDisplay = PicEase_GUI.DrawToggle("Enable Image Info Display", labelWidthSettings, mapGeneratorEnableImageInfoDisplay, true, true, "Display image info (i.e., name, dimensions, format, and path).");
            mapGeneratorEnableDebugLogForExportedFiles = PicEase_GUI.DrawToggle("Enable Debug.Log for Exports", labelWidthSettings, mapGeneratorEnableDebugLogForExportedFiles, true, true, "Display a Debug.Log message showing the directory where the generated map was exported.");
            
            EditorGUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        private void DrawSettingsSceneScreenshotPanel()
        {
            scrollSettingsSceneScreenshot = EditorGUILayout.BeginScrollView(scrollSettingsSceneScreenshot, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("Scene Screenshot", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("File Settings", PicEase_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(5);
            sceneScreenshotDefaultImageWidth = PicEase_GUI.DrawIntField("Default Image Width", labelWidthSettings, sceneScreenshotDefaultImageWidth, 1920, true, "The default initialized value of the Image Width in the Input Field.");
            sceneScreenshotDefaultImageHeight = PicEase_GUI.DrawIntField("Default Image Height", labelWidthSettings, sceneScreenshotDefaultImageHeight, 1080, true, "The default initialized value of the Image Height in the Input Field.");
            sceneScreenshotDefaultExportImageFormat = PicEase_GUI.DrawEnumPopup("Default Export Image Format", labelWidthSettings, sceneScreenshotDefaultExportImageFormat, PicEase_Converter.ImageFormat.PNG, true, "The default initialized value of the Image Format in the Input Field.");
            sceneScreenshotDefaultExportDirectory = PicEase_GUI.DrawStringField("Default Export Directory", labelWidthSettings, sceneScreenshotDefaultExportDirectory, "", true, @"The default directory initialized for the exported screenshot. When exporting a screenshot, this directory will be set as the default path if valid (e.g., C:\Users\...\Desktop).");
            
            EditorGUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        private void DrawSettingsShortcutsFilePanel()
        {
            scrollSettingsShortcuts = EditorGUILayout.BeginScrollView(scrollSettingsShortcuts, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("Shortcuts", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(10);

            foreach (string shortcutId in minorShortcutIdentifiers)
            {
                string[] parts = shortcutId.Split('/');
                string commandName = parts[parts.Length - 1];
                minorShortcutTooltips.TryGetValue(shortcutId, out string tooltipText);

                EditorGUILayout.BeginHorizontal();

                if (!string.IsNullOrEmpty(tooltipText))
                    PicEase_GUI.DrawTooltipPopup(tooltipText);
                GUILayout.Space(4);
                GUILayout.Label(commandName + ":", PicEase_GUI.RegularLargeLabelStyle, GUILayout.Width(minorShortcutCommandLabelWidth));

                bool hasKeyCombination = false;
                try
                {
                    ShortcutBinding currentBinding = ShortcutManager.instance.GetShortcutBinding(shortcutId);
                    foreach (KeyCombination kc in currentBinding.keyCombinationSequence)
                    {
                        if (!hasKeyCombination)
                        {
                            hasKeyCombination = true;
                            GUILayout.Label(kc.ToString(), PicEase_GUI.AssignedLabelStyle, GUILayout.MinWidth(minorShortcutLabelWidth));
                        }
                        else
                        {
                            GUILayout.Label(" + " + kc.ToString(), PicEase_GUI.AssignedLabelStyle, GUILayout.MinWidth(minorShortcutLabelWidth));
                        }
                    }
                }
                catch (ArgumentException)
                {
                    string displayName = shortcutId;
                    if (shortcutId == "PicEase/Take Screenshot") displayName = "Shift + Alt + Q";
                    GUILayout.Label(displayName, PicEase_GUI.AssignedLabelStyle, GUILayout.MinWidth(minorShortcutLabelWidth));
                    hasKeyCombination = true;
                }

                if (!hasKeyCombination)
                {
                    GUILayout.Label("unassigned shortcut", PicEase_GUI.UnassignedLabelStyle, GUILayout.MinWidth(minorShortcutLabelWidth));
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.HelpBox("To modify minor shortcuts, please go to: Edit/Shortcuts.../PicEase.\n You can click the button below for quick access, then in the category section, search for PicEase.", MessageType.Info);
        }

        private void SaveSettings()
        {
            PicEase_Settings.ImageEditorDefaultImportTextureFilterMode = imageEditorDefaultTextureFilterMode;
            PicEase_Settings.ImageEditorDefaultImportTextureAnisoLevel = imageEditorDefaultTextureAnisoLevel;
            PicEase_Settings.ImageEditorExportImageFormat = imageEditorExportImageFormat;
            PicEase_Settings.ImageEditorDefaultExportImageFormat = imageEditorDefaultExportImageFormat;
            PicEase_Settings.ImageEditorSameExportPath = imageEditorSameExportPath;
            PicEase_Settings.ImageEditorDefaultExportDirectory = imageEditorDefaultExportDirectory;
            PicEase_Settings.ImageEditorBehaviourMode = imageEditorBehaviourMode;
            PicEase_Settings.ImageEditorThreadGroupSize = imageEditorThreadGroupSize;
            PicEase_Settings.ImageEditorPrecisionMode = imageEditorPrecisionMode;
            PicEase_Settings.ImageEditorEnableImageInfoDisplay = imageEditorEnableImageInfoDisplay;
            PicEase_Settings.ImageEditorInfoLabelFontSize = imageEditorInfoLabelFontSize;
            PicEase_Settings.ImageEditorInfoLabelFontColor = imageEditorInfoLabelFontColor;
            PicEase_Settings.ImageEditorInfoBackgroundColor = imageEditorInfoBackgroundColor;
            PicEase_Settings.ImageEditorEnableDebugLogForExportedFiles = imageEditorEnableDebugLogForExportedFiles;
            PicEase_Settings.MapGeneratorDefaultImportTextureFilterMode = mapGeneratorDefaultTextureFilterMode;
            PicEase_Settings.MapGeneratorDefaultImportTextureAnisoLevel = mapGeneratorDefaultTextureAnisoLevel;
            PicEase_Settings.MapGeneratorExportImageFormat = mapGeneratorExportImageFormat;
            PicEase_Settings.MapGeneratorDefaultExportImageFormat = mapGeneratorDefaultExportImageFormat;
            PicEase_Settings.MapGeneratorSameExportPath = mapGeneratorSameExportPath;
            PicEase_Settings.MapGeneratorDefaultExportDirectory = mapGeneratorDefaultExportDirectory;
            PicEase_Settings.MapGeneratorBehaviourMode = mapGeneratorBehaviourMode;
            PicEase_Settings.MapGeneratorThreadGroupSize = mapGeneratorThreadGroupSize;
            PicEase_Settings.MapGeneratorPrecisionMode = mapGeneratorPrecisionMode;
            PicEase_Settings.MapGeneratorEnableImageInfoDisplay = mapGeneratorEnableImageInfoDisplay;
            PicEase_Settings.MapGeneratorEnableDebugLogForExportedFiles = mapGeneratorEnableDebugLogForExportedFiles;
            PicEase_Settings.SceneScreenshotDefaultImageWidth = sceneScreenshotDefaultImageWidth;
            PicEase_Settings.SceneScreenshotDefaultImageHeight = sceneScreenshotDefaultImageHeight;
            PicEase_Settings.SceneScreenshotDefaultExportImageFormat = sceneScreenshotDefaultExportImageFormat;
            PicEase_Settings.SceneScreenshotDefaultExportDirectory = sceneScreenshotDefaultExportDirectory;
            PicEase_Settings.SaveSettings();

            PicEase_Operations.EnsureComputeShaderProperties(PicEase_Constants.ImageComputeShader, imageEditorPrecisionMode, imageEditorThreadGroupSize);
            PicEase_Operations.EnsureComputeShaderProperties(PicEase_Constants.MapComputeShader, mapGeneratorPrecisionMode, mapGeneratorThreadGroupSize);

            UpdateImageEditorInitializedFields();
            UpdateSceneScreenshotInitializedFields();
        }

        private void UpdateImageEditorInitializedFields()
        {
            imageEditorCurrentExportFormat = imageEditorDefaultExportImageFormat;
        }

        private void UpdateMapGenratorInitializedFields()
        {
            mapGeneratorCurrentExportFormat = mapGeneratorDefaultExportImageFormat;
        }

        private void UpdateSceneScreenshotInitializedFields()
        {
            sceneScreenshotCurrentImageWidth = sceneScreenshotDefaultImageWidth;
            sceneScreenshotCurrentImageHeight = sceneScreenshotDefaultImageHeight;
            sceneScreenshotCurrentExportFormat = sceneScreenshotDefaultExportImageFormat;
        }

        private void SelectSettingsImageEditorPanel()
        {
            SwitchSettingsPanelMode(SettingsPanel.ImageEditor);
        }

        private void SelectSettingsMapGeneratorPanel()
        {
            SwitchSettingsPanelMode(SettingsPanel.MapGenerator);
        }

        private void SelectSettingsSceneScreenshotPanel()
        {
            SwitchSettingsPanelMode(SettingsPanel.SceneScreenshot);
        }

        private void SelectSettingsShortcutsPanel()
        {
            SwitchSettingsPanelMode(SettingsPanel.Shortcuts);
        }

        private void SwitchSettingsPanelMode(SettingsPanel newSettingsPanel, Action extraAction = null)
        {
            SwitchViewMode(ViewMode.Settings);
            if (currentSettingsPanel == newSettingsPanel) return;

            extraAction?.Invoke();
            currentSettingsPanel = newSettingsPanel;
        }
        #endregion

        #region Image Editor
        private void DrawImageEditorTab()
        {
            if (originalImage == null)
            {
                InsertImage();
            }
            else
            {
                if (imageManager == null)
                {
                    imageManager = new(originalImage, imageComputeShader, this, originalImageImporterSettings, originalMaxTextureSize, originalTextureCompression);
                }

                EditorGUILayout.BeginHorizontal();

                #region Left Panel
                EditorGUILayout.BeginVertical(PicEase_GUI.ImageEditorLeftPanelStyle);

                #region Header
                DrawImageEditorCategoriesButtons();
                #endregion

                #region Body
                EditorGUILayout.Space(4);
                GUILayout.Label(cachedCategoryLabel, PicEase_GUI.CategoryLabelStyle);
                EditorGUILayout.Space(2);

                switch (currentImageEditorCategory)
                {
                    case ImageEditorCategory.Adjustments:
                        DrawImageAdjustValueFields();
                        break;
                    case ImageEditorCategory.Colorize:
                        DrawImageColorizeFields();
                        break;
                    case ImageEditorCategory.Filters:
                        DrawImageFiltersFields();
                        break;
                    case ImageEditorCategory.LUTs:
                        DrawImageLUTsFields();
                        break;
                }
                #endregion

                EditorGUILayout.Space(6);
                GUILayout.FlexibleSpace();

                #region Footer
                DrawImageEditorButtons();
                #endregion

                EditorGUILayout.EndVertical();
                #endregion

                #region Right Panel
                DrawPreviewImage();
                #endregion

                EditorGUILayout.EndHorizontal();
            }
        }

        private void InsertImage()
        {
            GUILayout.BeginVertical(PicEase_GUI.ImageEditorDragAndDropPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.Space(5);
                GUILayout.Label("IMAGE EDITOR", PicEase_GUI.TabLabelStyle, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("Drag & Drop Your Image Here", PicEase_GUI.ImageEditorDragAndDropLabelStyle, GUILayout.ExpandWidth(true));
                        GUILayout.Space(5);
                        if (GUILayout.Button("Upload Image From Device", GUILayout.Height(primaryButtonHeight), GUILayout.ExpandWidth(true)))
                        {
                            string path = EditorUtility.OpenFilePanel("PicEase Image Editor - Load Image", "", PicEase_Converter.GetImageFormatString());
                            LoadImageFromPath(path, false);
                        }
                    }
                    GUILayout.Space(5);
                    GUILayout.Space(50);
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();

            Rect dropArea = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                if (dropArea.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is Texture2D texture)
                            {
                                string path = AssetDatabase.GetAssetPath(texture);
                                LoadImageFromPath(path, !string.IsNullOrEmpty(path));
                                break;
                            }
                        }
                    }
                    Event.current.Use();
                }
            }
        }

        private void LoadImageFromPath(string path, bool isFromAssetDatabase)
        {
            if (!string.IsNullOrEmpty(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                string extension = Path.GetExtension(path);

                Texture2D tex;
                bool isTextureLoaded;

                if (extension == ".tga" || extension == ".TGA")
                {
                    tex = PicEase_Converter.LoadImageTGA(fileData);
                    isTextureLoaded = tex != null;
                }
                else if (extension.Equals(".tif", StringComparison.OrdinalIgnoreCase) || extension.Equals(".tiff", StringComparison.OrdinalIgnoreCase))
                {
                    tex = PicEase_Converter.LoadImageTIFF(fileData);
                    isTextureLoaded = tex != null;
                }
                else
                {
                    tex = new(2, 2, TextureFormat.RGBA32, false);
                    isTextureLoaded = tex.LoadImage(fileData, false);
                }

                if (isTextureLoaded)
                {
                    if (isFromAssetDatabase)
                    {
                        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (importer != null)
                        {
                            tex.filterMode = importer.filterMode;
                            tex.anisoLevel = importer.anisoLevel;

                            originalImageImporterSettings = new();
                            importer.ReadTextureSettings(originalImageImporterSettings);
                            originalMaxTextureSize = importer.maxTextureSize;
                            originalTextureCompression = importer.textureCompression;
                        }
                        else
                        {
                            tex.filterMode = PicEase_Settings.ImageEditorDefaultImportTextureFilterMode;
                            tex.anisoLevel = PicEase_Settings.ImageEditorDefaultImportTextureAnisoLevel;

                            originalImageImporterSettings = null;
                            originalMaxTextureSize = 2048;
                            originalTextureCompression = TextureImporterCompression.Compressed;
                        }
                    }
                    else
                    {
                        tex.filterMode = PicEase_Settings.ImageEditorDefaultImportTextureFilterMode;
                        tex.anisoLevel = PicEase_Settings.ImageEditorDefaultImportTextureAnisoLevel;

                        originalImageImporterSettings = null;
                        originalMaxTextureSize = 2048;
                        originalTextureCompression = TextureImporterCompression.Compressed;
                    }
                    tex.Apply();

                    ClearImageEditor();

                    originalImage = tex;

                    imageManager = new(originalImage, imageComputeShader, this, originalImageImporterSettings, originalMaxTextureSize, originalTextureCompression);
                    imageManager.ResetImageEffects();

                    currentLoadedImageName = Path.GetFileNameWithoutExtension(path);
                    currentImagePath = isFromAssetDatabase ? path : Path.GetFullPath(path);
                    currentImageFormat = extension.TrimStart('.').ToUpper();
                    currentImageWidth = tex.width;
                    currentImageHeight = tex.height;
                    currentImageDirectory = Path.GetDirectoryName(currentImagePath);
                    if (imageEditorExportImageFormat == PicEase_Converter.ExportImageFormat.OriginalImage)
                    {
                        imageEditorCurrentExportFormat = PicEase_Converter.GetImageFormatFromExtensionImage(extension);
                    }

                    Repaint();
                }
                else
                {
                    Debug.LogError("Failed to load image. Most likely an unsupported file format.");
                    DestroyImmediate(tex);
                }
            }
        }

        private void DrawImageEditorCategoriesButtons()
        {
            EditorGUILayout.BeginHorizontal(PicEase_GUI.ImageEditorCategoryPanelStyle);
            GUILayout.FlexibleSpace();

            if (PicEase_GUI.DrawButtonIcon("Adjustments", PicEase_GUI.AdjustmentsButtonStyle))
            {
                SelectAdjustmentCategory();
            }
            GUILayout.Space(42);
            if (PicEase_GUI.DrawButtonIcon(" Colorize", PicEase_GUI.ColorizeButtonStyle))
            {
                SelectColorizeCategory();
            }
            GUILayout.Space(34);
            if (PicEase_GUI.DrawButtonIcon(" Filters", PicEase_GUI.FiltersButtonStyle))
            {
                SelectFiltersCategory();
            }
            GUILayout.Space(33);
            if (PicEase_GUI.DrawButtonIcon("LUTs", PicEase_GUI.LUTsButtonStyle))
            {
                SelectLUTsCategory();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawImageAdjustValueFields()
        {
            scrollImageEditorAdjustments = EditorGUILayout.BeginScrollView(scrollImageEditorAdjustments, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region Color
            GUILayout.Label("Color", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageManager.VibranceMagnitude = PicEase_GUI.DrawFloatSlider("Vibrance", imageManager.VibranceMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.SaturationMagnitude = PicEase_GUI.DrawFloatSlider("Saturation", imageManager.SaturationMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.TemperatureMagnitude = PicEase_GUI.DrawFloatSlider("Temperature", imageManager.TemperatureMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.TintMagnitude = PicEase_GUI.DrawFloatSlider("Tint", imageManager.TintMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.HueMagnitude = PicEase_GUI.DrawFloatSlider("Hue", imageManager.HueMagnitude, -180, 180, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Light
            GUILayout.Label("Light", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageManager.GammaMagnitude = PicEase_GUI.DrawFloatSlider("Gamma", imageManager.GammaMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.BrightnessMagnitude = PicEase_GUI.DrawFloatSlider("Brightness", imageManager.BrightnessMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.ExposureMagnitude = PicEase_GUI.DrawFloatSlider("Exposure", imageManager.ExposureMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.ContrastMagnitude = PicEase_GUI.DrawFloatSlider("Contrast", imageManager.ContrastMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.BlackMagnitude = PicEase_GUI.DrawFloatSlider("Black", imageManager.BlackMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.WhiteMagnitude = PicEase_GUI.DrawFloatSlider("White", imageManager.WhiteMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.ShadowMagnitude = PicEase_GUI.DrawFloatSlider("Shadow", imageManager.ShadowMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.HighlightMagnitude = PicEase_GUI.DrawFloatSlider("Highlight", imageManager.HighlightMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.BloomMagnitude = PicEase_GUI.DrawFloatSlider("Bloom", imageManager.BloomMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Detail
            GUILayout.Label("Detail", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageManager.SharpenMagnitude = PicEase_GUI.DrawFloatSlider("Sharpen", imageManager.SharpenMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.ClarityMagnitude = PicEase_GUI.DrawFloatSlider("Clarity", imageManager.ClarityMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.SmoothMagnitude = PicEase_GUI.DrawFloatSlider("Smooth", imageManager.SmoothMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.BlurMagnitude = PicEase_GUI.DrawFloatSlider("Blur", imageManager.BlurMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.GrainMagnitude = PicEase_GUI.DrawFloatSlider("Grain", imageManager.GrainMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Style
            GUILayout.Label("Style", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageManager.PixelateMagnitude = PicEase_GUI.DrawFloatSlider("Pixelate", imageManager.PixelateMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageManager.SobelEdgeMagnitude = PicEase_GUI.DrawFloatSlider("Sobel Edge", imageManager.SobelEdgeMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            EditorGUILayout.EndScrollView();
        }

        private void DrawImageColorizeFields()
        {
            scrollImageEditorColorize = EditorGUILayout.BeginScrollView(scrollImageEditorColorize, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region Input
            GUILayout.Label("Input", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            colorizeSourceColor = PicEase_GUI.DrawColorField("Source Color", labelWidthColorize, colorizeSourceColor, Color.white);
            EditorGUI.BeginChangeCheck();
            colorizeTargetColor = PicEase_GUI.DrawColorField("Target Color", labelWidthColorize, colorizeTargetColor, Color.white);
            if (EditorGUI.EndChangeCheck())
            {
                imageManager.SetTemporaryColorReplacement(colorizeSourceColor, colorizeTargetColor);
                imageManager.ApplyColorize();
            }

            EditorGUILayout.Space(2);
            if (GUILayout.Button("Apply Target Color", GUILayout.Height(secondaryButtonHeight)))
            {
                imageManager.AddColorReplacement(colorizeSourceColor, colorizeTargetColor);
                imageManager.ClearTemporaryColorReplacement();
                imageManager.ApplyColorize();
            }
            #endregion

            EditorGUILayout.Space(10);

            #region Auto-Detect
            GUILayout.Label("Color Detection", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            autoDetectCount = PicEase_GUI.DrawIntSliderVertical("Detection Amount", labelWidthColorize, autoDetectCount, 1, 20, 3);
            if (GUILayout.Button("Auto-Detect Colors", GUILayout.Height(secondaryButtonHeight)))
            {
                List<Color> randomColors = imageManager.GetRandomColors(autoDetectCount);
                foreach (Color c in randomColors)
                {
                    imageManager.AddColorReplacement(c, c);
                }
                imageManager.ApplyColorize();
            }
            #endregion

            EditorGUILayout.Space(10);

            #region Global Modifiers
            GUILayout.Label("Global Modifiers", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageManager.ColorizeThreshold = PicEase_GUI.DrawFloatSlider("Threshold", imageManager.ColorizeThreshold, 0f, 1f, 0.1f);
            imageManager.ColorizeSmoothness = PicEase_GUI.DrawFloatSlider("Smoothness", imageManager.ColorizeSmoothness, 0f, 1f, 0.1f);
            if (EditorGUI.EndChangeCheck())
            {
                imageManager.ApplyColorize();
            }
            EditorGUILayout.Space(6);

            if (imageManager.GetColorReplacementCount() > 0)
            {
                GUILayout.Label("Color Replacements List", PicEase_GUI.BoldLabelStyle);
                EditorGUILayout.Space(2);

                for (int i = 0; i < imageManager.GetColorReplacementCount(); i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{i + 1})", GUILayout.Width(15));

                    EditorGUI.BeginChangeCheck();
                    Color newSourceColor = EditorGUILayout.ColorField(imageManager.GetSourceColor(i), GUILayout.ExpandWidth(true));
                    GUILayout.Label(" →", PicEase_GUI.RegularLabelStyle, GUILayout.Width(20));
                    Color newTargetColor = EditorGUILayout.ColorField(imageManager.GetTargetColor(i), GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        imageManager.UpdateColorReplacementAt(i, newSourceColor, newTargetColor);
                        imageManager.ApplyColorize();
                    }

                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        imageManager.RemoveColorReplacementAt(i);
                        imageManager.ApplyColorize();
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            #endregion

            EditorGUILayout.EndScrollView();
        }

        private void DrawImageFiltersFields()
        {
            scrollImageEditorFilters = EditorGUILayout.BeginScrollView(scrollImageEditorFilters, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            switch (filterViewState)
            {
                case FilterViewState.Main:
                    DrawMainFiltersView();
                    break;

                case FilterViewState.Group:
                    DrawGroupFiltersView();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawMainFiltersView()
        {
            #region Create
            GUILayout.Label("Create", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            filterNameInput = PicEase_GUI.DrawStringField("Filter Name", 85, filterNameInput, "", true, "The name of the custom filter you want to create. The custom filter will save the current values of each adjustment (i.e., Color, Light, Detail, and Style).\n\nNote: When a custom filter is created, a new .json file named PicEase_SavedData_CustomFilters is generated and stored in the 'Saved Data' folder. All custom filters are saved in this file.");

            if (GUILayout.Button("Create", GUILayout.Height(secondaryButtonHeight)))
            {
                if (!string.IsNullOrEmpty(filterNameInput))
                {
                    string createdName = filterNameInput;
                    bool filterExists = false;
                    foreach (Filter filter in PicEase_Filters.CustomFilters)
                    {
                        if (string.Equals(filter.Name, filterNameInput, StringComparison.OrdinalIgnoreCase))
                        {
                            filterExists = true;
                            break;
                        }
                    }

                    if (filterExists)
                    {
                        bool overwrite = EditorUtility.DisplayDialog("Overwrite Filter?", $"A custom filter named '{filterNameInput}' already exists. Overwrite it?", "Yes", "No");
                        if (overwrite)
                        {
                            for (int i = 0; i < PicEase_Filters.CustomFilters.Count; i++)
                            {
                                if (string.Equals(PicEase_Filters.CustomFilters[i].Name, filterNameInput, StringComparison.OrdinalIgnoreCase))
                                {
                                    PicEase_Filters.CustomFilters[i] = CreateNewFilter(filterNameInput);
                                    PicEase_Filters.SaveSettings();
                                    filterNameInput = "";
                                    GUIUtility.keyboardControl = 0;
                                    Repaint();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        PicEase_Filters.CustomFilters.Add(CreateNewFilter(filterNameInput));
                        PicEase_Filters.SaveSettings();
                        EditorUtility.DisplayDialog("Custom Filter Created!", $"Filter '{createdName}' was created successfully and added to the Custom Filter group.", "OK");
                        filterNameInput = "";
                        GUIUtility.keyboardControl = 0;
                        Repaint();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Filter Name!", "Please enter a valid filter name.", "OK");
                }
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Groups
            GUILayout.Label("Groups", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            foreach (KeyValuePair<string, List<string>> groupEntry in PicEase_Filters.FilterGroups)
            {
                string groupName = groupEntry.Key;
                if (GUILayout.Button(groupName, GUILayout.Height(secondaryButtonHeight)))
                {
                    selectedFilterGroup = groupName;
                    filterViewState = FilterViewState.Group;
                }
            }
            #endregion
        }

        private void DrawGroupFiltersView()
        {
            if (string.IsNullOrEmpty(selectedFilterGroup))
            {
                filterViewState = FilterViewState.Main;
                return;
            }

            GUILayout.Label($"Group: {selectedFilterGroup}", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            if (GUILayout.Button("← Back", GUILayout.Height(secondaryButtonHeight)))
            {
                filterViewState = FilterViewState.Main;
                return;
            }

            EditorGUILayout.Space(4);

            if (selectedFilterGroup == "Custom")
            {
                if (PicEase_Filters.CustomFilters.Count == 0)
                {
                    EditorGUILayout.LabelField("No custom filters found.");
                    return;
                }

                for (int i = 0; i < PicEase_Filters.CustomFilters.Count; i++)
                {
                    Filter customFilter = PicEase_Filters.CustomFilters[i];
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(customFilter.Name, GUILayout.Height(secondaryButtonHeight)))
                    {
                        PicEase_Filters.ApplyCustomFilter(imageManager, customFilter);
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(60), GUILayout.Height(secondaryButtonHeight)))
                    {
                        bool delete = EditorUtility.DisplayDialog("Delete Filter?", $"Are you sure you want to delete the custom filter '{customFilter.Name}'?", "Yes", "No");
                        if (delete)
                        {
                            PicEase_Filters.CustomFilters.RemoveAt(i);
                            PicEase_Filters.SaveSettings();
                            Repaint();
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                if (PicEase_Filters.FilterGroups.TryGetValue(selectedFilterGroup, out List<string> filtersInGroup))
                {
                    foreach (string filterName in filtersInGroup)
                    {
                        if (GUILayout.Button(filterName, GUILayout.Height(secondaryButtonHeight)))
                        {
                            PicEase_Filters.ApplyBuiltInFilter(imageManager, filterName);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No filters found in this group.");
                }
            }
        }

        private void DrawImageLUTsFields()
        {
            scrollImageEditorLUTs = EditorGUILayout.BeginScrollView(scrollImageEditorLUTs, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.Label("Input", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            Texture3D currentCustomLUT = imageManager.CustomLUT;
            string currentLUTName = imageManager.LUTName ?? "None";
            float currentBlend = imageManager.LUTBlend;

            List<string> availableLUTs = PicEase_Image.GetAvailableLUTs();
            if (availableLUTs.Count == 0)
            {
                GUILayout.Label("No LUTs found!", PicEase_GUI.RegularLabelStyle);
            }
            else
            {
                int currentIndex = availableLUTs.IndexOf(currentLUTName);
                if (currentIndex < 0) currentIndex = 0;

                #region Input
                EditorGUI.BeginChangeCheck();
                Texture3D newCustomLUT = PicEase_GUI.DrawObjectField("Current LUT", labelWidthLUT, currentCustomLUT, null);
                EditorGUILayout.Space(4);
                int newIndex = PicEase_GUI.DrawPopup("LUT Selection", labelWidthLUT, currentIndex, availableLUTs.ToArray(), 0);
                EditorGUILayout.Space(4);
                float newBlend = PicEase_GUI.DrawFloatSlider("Blend", currentBlend, 0f, 1f, 0f);

                if (EditorGUI.EndChangeCheck())
                {
                    string newLUTName = availableLUTs[newIndex];
                    bool changedPopupOrBlend = (newLUTName != currentLUTName) || !Mathf.Approximately(newBlend, currentBlend);

                    if (changedPopupOrBlend)
                    {
                        imageManager.SelectLUT(newLUTName, newBlend);

                        if (newLUTName == "None")
                        {
                            imageManager.CustomLUT = null;
                        }
                        else
                        {
                            imageManager.CustomLUT = PicEase_Image.GetDefaultLUT(newLUTName);
                        }

                        imageManager.ApplyLUT();
                    }

                    if (newCustomLUT != currentCustomLUT)
                    {
                        imageManager.CustomLUT = newCustomLUT;

                        if (newCustomLUT == null)
                        {
                            imageManager.SelectLUT("None", 0f);
                            imageManager.ApplyLUT();
                        }
                        else
                        {
                            string foundKey = null;
                            foreach (string lutName in availableLUTs)
                            {
                                Texture3D builtInTex = PicEase_Image.GetDefaultLUT(lutName);
                                if (builtInTex == newCustomLUT)
                                {
                                    foundKey = lutName;
                                    break;
                                }
                            }

                            if (!string.IsNullOrEmpty(foundKey) && foundKey != "None")
                            {
                                imageManager.SelectLUT(foundKey, newBlend);
                                imageManager.ApplyLUT();
                            }
                            else
                            {
                                imageManager.SetCustomUserLUT(newCustomLUT);
                                imageManager.SelectLUT("Custom LUT", newBlend);
                                imageManager.ApplyLUT();
                            }
                        }
                    }
                }
                #endregion

                EditorGUILayout.Space(6);

                if (GUILayout.Button("Reset LUT", GUILayout.Height(secondaryButtonHeight)))
                {
                    imageManager.ResetLUT();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawImageEditorButtons()
        {
            #region Hold To Compare Button
            Rect buttonRect = GUILayoutUtility.GetRect(new("Hold To Compare"), GUI.skin.button, GUILayout.Height(primaryButtonHeight));
            if (Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition))
            {
                if (!isComparing)
                {
                    isComparing = true;
                    Repaint();
                    isComparingDoOnce = true;
                }
                Event.current.Use();
            }
            if (Event.current.type == EventType.MouseUp && isComparing)
            {
                isComparing = false;
                if (isComparingDoOnce)
                {
                    Repaint();
                    isComparingDoOnce = false;
                }
            }
            GUI.Button(buttonRect, "Hold To Compare");
            #endregion

            if (GUILayout.Button("Reset All Values", GUILayout.Height(primaryButtonHeight)))
            {
                ResetImageEditorEffects();
            }
            if (GUILayout.Button("Clear & Load New Image", GUILayout.Height(primaryButtonHeight)))
            {
                ClearImageEditor();
            }
        }

        private void DrawPreviewImage()
        {
            if (originalImage == null) return;

            EditorGUILayout.BeginVertical(PicEase_GUI.ImageEditorRightPanelStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(5);
            PicEase_GUI.DrawAspectRatioButton(SetAspectRatio);
            GUILayout.FlexibleSpace();
            GUILayout.Label(isComparing ? "Original Image" : "Preview Image", PicEase_GUI.PreviewLabelStyle);
            EditorGUILayout.Space(30);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(2);

            Texture imageToDisplay = isComparing ? originalImage : imageManager.GetResultTexture();
            if (imageToDisplay != null)
            {
                Rect panelRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                float imageAspect = (float)imageToDisplay.width / imageToDisplay.height;
                float panelAspect = panelRect.width / panelRect.height;
                float displayWidth, displayHeight;

                if (imageAspect > panelAspect)
                {
                    displayWidth = panelRect.width;
                    displayHeight = displayWidth / imageAspect;
                }
                else
                {
                    displayHeight = panelRect.height;
                    displayWidth = displayHeight * imageAspect;
                }

                float offsetX = (panelRect.width - displayWidth) * 0.5f;
                float offsetY = (panelRect.height - displayHeight) * 0.5f;
                Rect imageRect = new(panelRect.x + offsetX, panelRect.y + offsetY, displayWidth, displayHeight);

                GUI.DrawTexture(imageRect, imageToDisplay, ScaleMode.ScaleToFit, true);

                if (imageEditorEnableImageInfoDisplay && isComparing)
                {
                    Rect labelRect = new(imageRect.x + imageInfoLabelPadding, imageRect.y + imageInfoLabelPadding, imageRect.width - 2 * imageInfoLabelPadding, ImageInfoLabelHeight);

                    Color originalColor = GUI.color;
                    GUI.color = imageEditorInfoBackgroundColor;
                    GUI.DrawTexture(labelRect, EditorGUIUtility.whiteTexture);
                    GUI.color = originalColor;

                    string infoText = $"Name: {currentLoadedImageName}  •  " +
                                      $"Dimensions: {currentImageWidth} x {currentImageHeight}  •  " +
                                      $"Format: {currentImageFormat}  •  " +
                                      $"Source: {currentImagePath}";

                    GUI.Label(labelRect, infoText, PicEase_GUI.ImageEditorInfoLabelStyle);
                }

                EditorGUILayout.Space(2);
                DrawImageEditorExportFields();
            }

            EditorGUILayout.EndVertical();
        }

        private void SetAspectRatio(Vector2 dimensions)
        {
            Rect currentPosition = position;
            position = new(currentPosition.x, currentPosition.y, dimensions.x, dimensions.y);
            minSize = dimensions;
            maxSize = dimensions;

            Repaint();

            EditorApplication.delayCall += () =>
            {
                minSize = new(editorWindowMinX, editorWindowMinY);
                maxSize = new(7680, 2160);
            };
        }

        private void ClearImageEditor()
        {
            if (imageManager != null)
            {
                imageManager.Dispose();
                imageManager = null;
            }

            if (originalImage != null)
            {
                DestroyImmediate(originalImage);
                originalImage = null;
            }

            currentImagePath = string.Empty;
            currentImageFormat = "Unknown";
            currentImageWidth = 0;
            currentImageHeight = 0;
            currentImageDirectory = string.Empty;

            Repaint();
        }

        private void ResetImageEditorEffects()
        {
            imageManager.ResetImageEffects();
        }

        private void DrawImageEditorExportFields()
        {
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle);
            GUILayout.Label("Export Values", PicEase_GUI.BoldLabelStyle);

            imageEditorCurrentExportFormat = PicEase_GUI.DrawEnumPopup("Image Format", 100, imageEditorCurrentExportFormat, imageEditorDefaultExportImageFormat);
            GUILayout.Space(4);
            if (GUILayout.Button("Export Image", GUILayout.Height(primaryButtonHeight)))
            {
                ExportEditedImage();
            }
            EditorGUILayout.EndVertical();
        }

        private void ExportEditedImage()
        {
            string exportDirectory;
            if (PicEase_Settings.ImageEditorSameExportPath)
            {
                exportDirectory = currentImageDirectory;
            }
            else
            {
                exportDirectory = Directory.Exists(PicEase_Settings.ImageEditorDefaultExportDirectory) ? PicEase_Settings.ImageEditorDefaultExportDirectory : "";
            }
            string extension = PicEase_Converter.GetExtension(imageEditorCurrentExportFormat);
            string path = EditorUtility.SaveFilePanel("PicEase Image Editor - Export Edited Image", exportDirectory, $"{currentLoadedImageName} Edited {DateTime.Now.ToString(PicEase_Constants.DateFormat)}.{extension}", extension);

            if (!string.IsNullOrEmpty(path))
            {
                Texture2D exportTexture = imageManager.GetFinalTexture2D();
                byte[] bytes = PicEase_Converter.EncodeImage(exportTexture, imageEditorCurrentExportFormat);
                try
                {
                    File.WriteAllBytes(path, bytes);
                    if (imageEditorEnableDebugLogForExportedFiles)
                    {
                        Debug.Log($"PicEase edited image exported to {path}");
                    }
                    RefreshAssetDatabase(path);

                    if (originalImageImporterSettings != null && path.StartsWith(Application.dataPath))
                    {
                        string relativePath = "Assets" + path[Application.dataPath.Length..];
                        EditorApplication.delayCall += () =>
                        {
                            imageManager.SetTextureImporterSettings(relativePath);
                        };
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to export edited image: {ex.Message}");
                }
            }
        }

        private Filter CreateNewFilter(string name)
        {
            return new(
                name,
                imageManager.VibranceMagnitude,
                imageManager.SaturationMagnitude,
                imageManager.TemperatureMagnitude,
                imageManager.TintMagnitude,
                imageManager.HueMagnitude,
                imageManager.GammaMagnitude,
                imageManager.BrightnessMagnitude,
                imageManager.ExposureMagnitude,
                imageManager.ContrastMagnitude,
                imageManager.BlackMagnitude,
                imageManager.WhiteMagnitude,
                imageManager.ShadowMagnitude,
                imageManager.HighlightMagnitude,
                imageManager.BloomMagnitude,
                imageManager.SharpenMagnitude,
                imageManager.ClarityMagnitude,
                imageManager.BlurMagnitude,
                imageManager.SmoothMagnitude,
                imageManager.GrainMagnitude,
                imageManager.PixelateMagnitude,
                imageManager.SobelEdgeMagnitude
            );
        }

        private void SelectAdjustmentCategory()
        {
            SwitchImageEditorCategory(ImageEditorCategory.Adjustments);
        }

        private void SelectColorizeCategory()
        {
            SwitchImageEditorCategory(ImageEditorCategory.Colorize);
        }

        private void SelectFiltersCategory()
        {
            SwitchImageEditorCategory(ImageEditorCategory.Filters);
        }

        private void SelectLUTsCategory()
        {
            SwitchImageEditorCategory(ImageEditorCategory.LUTs);
        }

        private void SwitchImageEditorCategory(ImageEditorCategory newImageEditorCategory, Action extraAction = null)
        {
            if (currentImageEditorCategory == newImageEditorCategory) return;

            extraAction?.Invoke();
            currentImageEditorCategory = newImageEditorCategory;
            UpdateImageEditorCategoryLabel();
        }

        private void UpdateImageEditorCategoryLabel()
        {
            cachedCategoryLabel = currentImageEditorCategory.ToString();
        }
        #endregion

        #region Map Generator
        private void DrawMapGeneratorTab()
        {
            if (diffuseMap == null)
            {
                InsertDiffuseMap();
            }
            else
            {
                if (mapManager == null)
                {
                    mapManager = new(diffuseMap, mapComputeShader, this);
                }

                EditorGUILayout.BeginHorizontal();

                #region Left Panel
                EditorGUILayout.BeginVertical(PicEase_GUI.ImageEditorLeftPanelStyle);

                #region Header
                EditorGUILayout.Space(4);
                GUILayout.Label("Map Generator", PicEase_GUI.CategoryLabelStyle);
                EditorGUILayout.Space(2);
                GUILayout.Label("Diffuse Map (Input Image)", PicEase_GUI.RegularCenterLabelStyle);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(diffuseMap, GUILayout.Width(256), GUILayout.Height(256));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(4);
                DrawImageInfo();         
                DrawMapViewButtons();
                EditorGUILayout.Space(10);
                #endregion

                #region Body
                DrawMapAdjustValueFields();
                #endregion

                EditorGUILayout.Space(6);
                GUILayout.FlexibleSpace();

                #region Footer
                if (GUILayout.Button("Reset All Values", GUILayout.Height(primaryButtonHeight)))
                {
                    ResetEffects();
                }
                if (GUILayout.Button("Clear & Load New Image", GUILayout.Height(primaryButtonHeight)))
                {
                    ClearMapGenerator();
                    Repaint();
                }
                #endregion

                EditorGUILayout.EndVertical();
                #endregion

                #region Right Panel
                DrawGeneratedMaps();
                #endregion

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawImageInfo()
        {
            if (mapGeneratorEnableImageInfoDisplay)
            {
                GUILayout.Label($"Name: {currentLoadedDiffuseMapName}", PicEase_GUI.RegularLabelStyle);
                GUILayout.Label($"Format: {currentDiffuseMapFormat}", PicEase_GUI.RegularLabelStyle);
                GUILayout.Label($"Dimensions: {currentDiffuseMapWidth} x {currentDiffuseMapHeight}", PicEase_GUI.RegularLabelStyle);
                GUILayout.Label($"Path: {currentDiffuseMapPath}", PicEase_GUI.RegularLabelStyle);
                EditorGUILayout.Space(4);
            }
        }

        private void DrawMapAdjustValueFields()
        {
            scrollMapGeneratorAdjustments = EditorGUILayout.BeginScrollView(scrollMapGeneratorAdjustments, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region Normal Map
            GUILayout.Label("Normal Map", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            mapManager.NormalInvertX = PicEase_GUI.DrawToggle("Invert X", mapManager.NormalInvertX, false);
            mapManager.NormalInvertY = PicEase_GUI.DrawToggle("Invert Y", mapManager.NormalInvertY, false);
            mapManager.NormalStrengthXMagnitude = PicEase_GUI.DrawFloatSlider("Strength X", mapManager.NormalStrengthXMagnitude, 0.0f, 10.0f, 1.0f);
            mapManager.NormalStrengthYMagnitude = PicEase_GUI.DrawFloatSlider("Strength Y", mapManager.NormalStrengthYMagnitude, 0.0f, 10.0f, 2.0f);
            mapManager.NormalZBiasMagnitude = PicEase_GUI.DrawFloatSlider("Z-Bias", mapManager.NormalZBiasMagnitude, -1.0f, 5.0f, 1.0f);
            mapManager.NormalSmoothnessMagnitude = PicEase_GUI.DrawFloatSlider("Smoothness", mapManager.NormalSmoothnessMagnitude, 0.0f, 1f, 0.5f);
            mapManager.NormalThresholdMagnitude = PicEase_GUI.DrawFloatSlider("Threshold", mapManager.NormalThresholdMagnitude, 0.0f, 1f, 0.025f);
            if (EditorGUI.EndChangeCheck())
            {
                mapManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Height Map
            GUILayout.Label("Height Map", PicEase_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);
            EditorGUI.BeginChangeCheck();
            mapManager.HeightInvert = PicEase_GUI.DrawToggle("Invert", mapManager.HeightInvert, false);
            mapManager.HeightBlackLevelMagnitude = PicEase_GUI.DrawFloatSlider("Black Level", mapManager.HeightBlackLevelMagnitude, 0.0f, 1.0f, 0.0f);
            mapManager.HeightWhiteLevelMagnitude = PicEase_GUI.DrawFloatSlider("White Level", mapManager.HeightWhiteLevelMagnitude, 0.0f, 1.0f, 0.0f);
            mapManager.HeightGammaMagnitude = PicEase_GUI.DrawFloatSlider("Gamma", mapManager.HeightGammaMagnitude, 0.0f, 1.0f, 3.0f);
            mapManager.HeightAmplitudeMagnitude = PicEase_GUI.DrawFloatSlider("Amplitude", mapManager.HeightAmplitudeMagnitude, 0.0f, 1.0f, 1.0f);
            mapManager.HeightBaseLevelMagnitude = PicEase_GUI.DrawFloatSlider("Base Level", mapManager.HeightBaseLevelMagnitude, 0.0f, 1f, 0.5f);
            mapManager.HeightContrastMagnitude = PicEase_GUI.DrawFloatSlider("Contrast", mapManager.HeightContrastMagnitude, 0.0f, 1.0f, 0.8f);
            mapManager.HeightGrayScaleMagnitude = PicEase_GUI.DrawFloatSlider("Gray Scale", mapManager.HeightGrayScaleMagnitude, -100.0f, 100.0f, 0.0f);
            mapManager.HeightSmoothnessMagnitude = PicEase_GUI.DrawFloatSlider("Smoothness", mapManager.HeightSmoothnessMagnitude, 0.0f, 1.0f, 0.5f);
            mapManager.HeightBlurMagnitude = PicEase_GUI.DrawFloatSlider("Blur", mapManager.HeightBlurMagnitude, 0.0f, 10.0f, 0.0f);
            if (EditorGUI.EndChangeCheck())
            {
                mapManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            EditorGUILayout.EndScrollView();
        }

        private void InsertDiffuseMap()
        {
            GUILayout.BeginVertical(PicEase_GUI.MapGeneratorDragAndDropPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.Space(5);
                GUILayout.Label("MAP GENERATOR", PicEase_GUI.TabLabelStyle, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("Drag & Drop Your Diffuse Map Here", PicEase_GUI.MapGeneratorDragAndDropLabelStyle, GUILayout.ExpandWidth(true));
                        GUILayout.Space(5);
                        if (GUILayout.Button("Upload Diffuse Map From Device", GUILayout.Height(primaryButtonHeight), GUILayout.ExpandWidth(true)))
                        {
                            string path = EditorUtility.OpenFilePanel("Select Diffuse Map", "", PicEase_Converter.GetImageFormatString());
                            LoadDiffuseMapFromPath(path, false);
                        }
                        GUILayout.Space(5);
                    }
                    GUILayout.Space(50);
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();

            Rect dropArea = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                if (dropArea.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is Texture2D texture)
                            {
                                string path = AssetDatabase.GetAssetPath(texture);
                                LoadDiffuseMapFromPath(path, !string.IsNullOrEmpty(path));
                                break;
                            }
                        }
                    }
                    Event.current.Use();
                }
            }
        }

        private void LoadDiffuseMapFromPath(string path, bool isFromAssetDatabase)
        {
            if (!string.IsNullOrEmpty(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                string extension = Path.GetExtension(path);
                bool isTextureLoaded;

                Texture2D tex;
                if (extension.Equals(".tga", StringComparison.OrdinalIgnoreCase))
                {
                    tex = PicEase_Converter.LoadImageTGA(fileData);
                    isTextureLoaded = tex != null;
                }
                else if (extension.Equals(".tif", StringComparison.OrdinalIgnoreCase) || extension.Equals(".tiff", StringComparison.OrdinalIgnoreCase))
                {
                    tex = PicEase_Converter.LoadImageTIFF(fileData);
                    isTextureLoaded = tex != null;
                }
                else
                {
                    tex = new(2, 2, TextureFormat.RGBA32, false);
                    isTextureLoaded = tex.LoadImage(fileData, false);
                }

                if (isTextureLoaded)
                {
                    if (isFromAssetDatabase)
                    {
                        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (importer != null)
                        {
                            tex.filterMode = importer.filterMode;
                            tex.anisoLevel = importer.anisoLevel;
                        }
                        else
                        {
                            tex.filterMode = PicEase_Settings.MapGeneratorDefaultImportTextureFilterMode;
                            tex.anisoLevel = PicEase_Settings.MapGeneratorDefaultImportTextureAnisoLevel;
                        }
                    }
                    else
                    {
                        tex.filterMode = PicEase_Settings.MapGeneratorDefaultImportTextureFilterMode;
                        tex.anisoLevel = PicEase_Settings.MapGeneratorDefaultImportTextureAnisoLevel;
                    }
                    tex.Apply();

                    ClearMapGenerator();

                    diffuseMap = tex;

                    LoadMapComputeShader();

                    if (mapComputeShader != null)
                    {
                        mapManager = new(diffuseMap, mapComputeShader, this);
                        mapManager.ApplyAdjustments();
                    }
                    else
                    {
                        Debug.LogError("Map Compute Shader not found");
                    }

                    currentLoadedDiffuseMapName = Path.GetFileNameWithoutExtension(path);
                    currentDiffuseMapFormat = extension.TrimStart('.').ToUpper();
                    currentDiffuseMapWidth = tex.width;
                    currentDiffuseMapHeight = tex.height;
                    currentDiffuseMapPath = isFromAssetDatabase ? path : Path.GetFullPath(path);
                    currentDiffuseMapDirectory = Path.GetDirectoryName(currentDiffuseMapPath);
                    if (mapGeneratorExportImageFormat == PicEase_Converter.ExportImageFormat.OriginalImage)
                    {
                        mapGeneratorCurrentExportFormat = PicEase_Converter.GetImageFormatFromExtensionMap(extension);
                    }

                    Repaint();
                }
                else
                {
                    Debug.LogError("Failed to load diffuse map. Unsupported file format or corrupt file.");
                    if (tex != null) DestroyImmediate(tex);
                }
            }
        }

        private void DrawGeneratedMaps()
        {
            if (diffuseMap == null) return;

            EditorGUILayout.BeginVertical(PicEase_GUI.ImageEditorRightPanelStyle);                       
            switch (currentMapView)
            {
                case MapGeneratorMapView.Normal:
                    DrawNormalMapView();
                    break;
                case MapGeneratorMapView.Height:
                    DrawHeightMapView();
                    break;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawMapViewButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Normal Map View", GUILayout.Height(secondaryButtonHeight)))
            {
                currentMapView = MapGeneratorMapView.Normal;
            }
            if (GUILayout.Button("Height Map View", GUILayout.Height(secondaryButtonHeight)))
            {
                currentMapView = MapGeneratorMapView.Height;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNormalMapView()
        {
            GUILayout.Label("Generated Normal Map", PicEase_GUI.PreviewLabelStyle);
            EditorGUILayout.Space(4);
            DrawScaledTexture(mapManager.GetNormalMap());
            EditorGUILayout.Space(4);
            if (GUILayout.Button("Export Normal Map", GUILayout.Height(secondaryButtonHeight)))
            {
                ExportGeneratedMapTexture(mapManager.GetFinalNormalMap2D(), $"{currentLoadedDiffuseMapName}" + " Normal");
            }
        }

        private void DrawHeightMapView()
        {
            GUILayout.Label("Generated Height Map", PicEase_GUI.PreviewLabelStyle);
            EditorGUILayout.Space(4);
            DrawScaledTexture(mapManager.GetHeightMap());
            EditorGUILayout.Space(4);
            if (GUILayout.Button("Export Height Map", GUILayout.Height(secondaryButtonHeight)))
            {
                ExportGeneratedMapTexture(mapManager.GetFinalHeightMap2D(), $"{currentLoadedDiffuseMapName}" + " Height");
            }
        }

        private void DrawScaledTexture(Texture texture)
        {
            if(texture == null) return;

            Rect panelRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            float imageAspect = (float)texture.width / texture.height;
            float panelAspect = panelRect.width / panelRect.height;
            float displayWidth, displayHeight;

            if (imageAspect > panelAspect)
            {
                displayWidth = panelRect.width;
                displayHeight = displayWidth / imageAspect;
            }
            else
            {
                displayHeight = panelRect.height;
                displayWidth = displayHeight * imageAspect;
            }

            float offsetX = (panelRect.width - displayWidth) * 0.5f;
            float offsetY = (panelRect.height - displayHeight) * 0.5f;
            Rect imageRect = new(panelRect.x + offsetX, panelRect.y + offsetY, displayWidth, displayHeight);

            GUI.DrawTexture(imageRect, texture, ScaleMode.ScaleToFit, true);
        }

        private void ExportGeneratedMapTexture(Texture2D texture, string defaultName)
        {
            string exportDirectory;
            if (PicEase_Settings.MapGeneratorSameExportPath)
            {
                exportDirectory = currentDiffuseMapDirectory;
            }
            else
            {
                exportDirectory = Directory.Exists(PicEase_Settings.MapGeneratorDefaultExportDirectory) ? PicEase_Settings.MapGeneratorDefaultExportDirectory : "";
            }
            string extension = PicEase_Converter.GetExtension(mapGeneratorCurrentExportFormat);
            string path = EditorUtility.SaveFilePanel("PicEase Map Generator - Export Generated Texture", exportDirectory, defaultName, extension);

            if (!string.IsNullOrEmpty(path))
            {
                byte[] bytes = PicEase_Converter.EncodeImage(texture, mapGeneratorCurrentExportFormat);
                try
                {
                    File.WriteAllBytes(path, bytes);
                    if (mapGeneratorEnableDebugLogForExportedFiles)
                    {
                        Debug.Log($"PicEase generated texture exported to {path}");
                    }
                    RefreshAssetDatabase(path);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to export edited image: {ex.Message}");
                }
            }
        }

        private void LoadMapComputeShader()
        {
            if (PicEase_Session.instance.ComputeShaderMap == null)
            {
                string shaderPath = PicEase_File.GetShaderFilePath(PicEase_Constants.MapComputeShader);
                mapComputeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(shaderPath);
                PicEase_Session.instance.ComputeShaderMap = mapComputeShader;
                PicEase_Session.instance.IsComputeShaderMapGeneratorLoaded = true;
            }
            else
            {
                mapComputeShader = PicEase_Session.instance.ComputeShaderMap;
            }
        }

        private void ResetEffects()
        {
            mapManager.ResetMapData();
            mapManager.ApplyAdjustments();
        }

        private void ClearMapGenerator()
        {
            if (mapManager != null)
            {
                mapManager.Dispose();
                mapManager = null;
            }

            if(diffuseMap != null)
            {
                DestroyImmediate(diffuseMap);
                diffuseMap = null;
            }

            currentDiffuseMapFormat = "Unknown";
            currentDiffuseMapWidth = 0;
            currentDiffuseMapHeight = 0;
            currentDiffuseMapPath = string.Empty;
            currentDiffuseMapDirectory = string.Empty;
        }
        #endregion

        #region Scene Screenshot
        private void DrawSceneScreenshotTab()
        {
            #region Header
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scene Screenshot", PicEase_GUI.BoldLabelStyle, GUILayout.Width(150));
            GUILayout.Space(5);
            PicEase_GUI.DrawResolutionButton((Vector2 res) => { sceneScreenshotCurrentImageWidth = (int)res.x; sceneScreenshotCurrentImageHeight = (int)res.y; });

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh Cameras List", GUILayout.Height(secondaryButtonHeight), GUILayout.ExpandWidth(false), GUILayout.Width(140)))
            {
                RefreshCameraList();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            DrawScreenshotFields();

            EditorGUILayout.EndVertical();
            #endregion

            #region Body
            if (sceneScreenshot != null)
            {
                GUILayout.Space(5);
                DrawScreenshotPreview();
                GUILayout.Space(5);
                DrawScreenshotExportFields();
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
            #endregion
        }

        private void RefreshCameraList()
        {
            int cameraCount = Camera.allCamerasCount;
            availableCameras = new Camera[cameraCount];
            Camera.GetAllCameras(availableCameras);

            cameraNames = new string[cameraCount];
            for (int i = 0; i < cameraCount; i++)
            {
                cameraNames[i] = availableCameras[i].name;
            }

            selectedCameraIndex = 0;
        }

        private void DrawScreenshotFields()
        {
            if (availableCameras.Length > 0)
            {
                selectedCameraIndex = PicEase_GUI.DrawPopup("Camera", labelWidthSceneScreenshot, selectedCameraIndex, cameraNames, 0);
                sceneScreenshotCurrentImageWidth = Mathf.Min(16384, PicEase_GUI.DrawIntField("Image Width", labelWidthSceneScreenshot, sceneScreenshotCurrentImageWidth, sceneScreenshotDefaultImageWidth));
                sceneScreenshotCurrentImageHeight = Mathf.Min(16384, PicEase_GUI.DrawIntField("Image Height", labelWidthSceneScreenshot, sceneScreenshotCurrentImageHeight, sceneScreenshotDefaultImageHeight));

                GUILayout.Space(4);
                if (GUILayout.Button("Take Screenshot", GUILayout.Height(primaryButtonHeight)))
                {
                    if (sceneScreenshotCurrentImageWidth >= 4096 || sceneScreenshotCurrentImageHeight >= 4096)
                    {
                        bool confirm = EditorUtility.DisplayDialog(
                            "High Resolution Warning!",
                            "A resolution of 4096 or higher may cause freezing and could crash on lower-end devices. Are you sure you want to continue?",
                            "Yes", "No"
                        );

                        if (!confirm)
                        {
                            return;
                        }
                    }

                    CaptureSceneScreenshotForPreview();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No cameras were found in the scene.", MessageType.Warning);
            }
        }

        private void CaptureSceneScreenshotForPreview()
        {
            if (availableCameras.Length > 0 && selectedCameraIndex < availableCameras.Length)
            {
                Camera cam = availableCameras[selectedCameraIndex];
                if (cam != null)
                {
                    RenderTexture renderTexture = null;
                    try
                    {
                        renderTexture = new(sceneScreenshotCurrentImageWidth, sceneScreenshotCurrentImageHeight, 24, RenderTextureFormat.DefaultHDR);
                        cam.targetTexture = renderTexture;
                        cam.Render();

                        RenderTexture.active = renderTexture;
                        sceneScreenshot = new(renderTexture.width, renderTexture.height, TextureFormat.RGB9e5Float, false);
                        sceneScreenshot.ReadPixels(new(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                        sceneScreenshot.Apply();

                        Repaint();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to capture screenshot: {ex.Message}");
                    }
                    finally
                    {
                        if (cam != null)
                        {
                            cam.targetTexture = null;
                        }
                        if (RenderTexture.active == renderTexture)
                        {
                            RenderTexture.active = null;
                        }
                        if (renderTexture != null)
                        {
                            DestroyImmediate(renderTexture);
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("PicEase Scene Screenshot", "The selected camera is null.", "OK");
                }
            }
        }

        private void DrawScreenshotPreview()
        {
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle);
            GUILayout.Label("Screenshot Preview", PicEase_GUI.BoldLabelStyle);
            Rect panelRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.DrawTexture(panelRect, sceneScreenshot, ScaleMode.ScaleToFit, true);
            EditorGUILayout.EndVertical();
        }

        private void DrawScreenshotExportFields()
        {
            EditorGUILayout.BeginVertical(PicEase_GUI.SecondaryPanelStyle);
            GUILayout.Label("Export Values", PicEase_GUI.BoldLabelStyle);

            sceneScreenshotCurrentExportFormat = PicEase_GUI.DrawEnumPopup("Image Format", 100, sceneScreenshotCurrentExportFormat, sceneScreenshotDefaultExportImageFormat);
            GUILayout.Space(4);
            hdrExport = PicEase_GUI.DrawToggle("Enhance Image with HDR", 170, hdrExport, false);
            GUILayout.Space(4);

            if (GUILayout.Button("Export Screenshot", GUILayout.Height(primaryButtonHeight)))
            {
                ExportScreenshot();
            }
            EditorGUILayout.EndVertical();
        }

        private void ExportScreenshot()
        {
            string exportDirectory = Directory.Exists(PicEase_Settings.SceneScreenshotDefaultExportDirectory) ? PicEase_Settings.SceneScreenshotDefaultExportDirectory : "";
            string extension = PicEase_Converter.GetExtension(sceneScreenshotCurrentExportFormat);
            string path = EditorUtility.SaveFilePanel("Export Screenshot", exportDirectory, $"PicEase_Screenshot_{DateTime.Now.ToString(PicEase_Constants.DateFormat)}.{extension}", extension);

            if (!string.IsNullOrEmpty(path))
            {
                exportScreenshot = hdrExport ? sceneScreenshot : CaptureSceneScreenshotForExport();
                if (exportScreenshot != null)
                {
                    byte[] bytes = PicEase_Converter.EncodeImage(exportScreenshot, sceneScreenshotCurrentExportFormat);
                    try
                    {
                        File.WriteAllBytes(path, bytes);
                        Debug.Log($"PicEase Screenshot exported to {path}");
                        RefreshAssetDatabase(path);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to export screenshot: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to capture screenshot for export.");
                }
            }
        }

        private Texture2D CaptureSceneScreenshotForExport()
        {
            if (availableCameras.Length > 0 && selectedCameraIndex < availableCameras.Length)
            {
                Camera cam = availableCameras[selectedCameraIndex];
                if (cam != null)
                {
                    RenderTexture renderTexture = null;
                    try
                    {
                        RenderTextureFormat format = hdrExport ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
                        renderTexture = new(sceneScreenshotCurrentImageWidth, sceneScreenshotCurrentImageHeight, 24, format);
                        cam.targetTexture = renderTexture;
                        cam.Render();

                        RenderTexture.active = renderTexture;
                        Texture2D exportScreenshot = new(renderTexture.width, renderTexture.height, TextureFormat.RGB9e5Float, false);
                        exportScreenshot.ReadPixels(new(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                        exportScreenshot.Apply();

                        return exportScreenshot;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to capture screenshot for export: {ex.Message}");
                        return null;
                    }
                    finally
                    {
                        if (cam != null)
                        {
                            cam.targetTexture = null;
                        }
                        if (RenderTexture.active == renderTexture)
                        {
                            RenderTexture.active = null;
                        }
                        if (renderTexture != null)
                        {
                            DestroyImmediate(renderTexture);
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("PicEase Scene Screenshot", "The selected camera is null.", "OK");
                    return null;
                }
            }
            return null;
        }

        private void ClearSceneScreenshot()
        {
            availableCameras = null;
            cameraNames = null;
            selectedCameraIndex = 0;
            if (sceneScreenshot != null) DestroyImmediate(sceneScreenshot);
            if (exportScreenshot != null) DestroyImmediate(exportScreenshot);
        }
        #endregion

        #region Shared Operations
        private void RefreshAssetDatabase(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                AssetDatabase.Refresh();
            }
        }
        #endregion
        #endregion

        private void OnDestroy()
        {
            ClearImageEditor();
            ClearMapGenerator();
            ClearSceneScreenshot();
        }
    }
}
#endif