#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase
{
    internal class PicEase_GUI
    {
        #region Properties
        #region Labels
        private static readonly Lazy<GUIStyle> _titleLabelStyle = new(() => new()
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            padding = new(-50, -50, -50, -50),
            contentOffset = new(0f, 5f)
        });
        public static GUIStyle TitleLabelStyle => _titleLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _headerTitle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 42,
            wordWrap = false,
            contentOffset = new(10f, -4f),
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle HeaderTitleLabelStyle => _headerTitle.Value;

        private static readonly Lazy<GUIStyle> _versionLabelStyle = new(() => new()
        {
            stretchHeight = true,
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 14,
            alignment = TextAnchor.UpperCenter,
            normal = { textColor = PicEase_Color.GetPrimaryFontColorFaded() }
        });
        public static GUIStyle VersionLabelStyle => _versionLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _regularLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 15,
            wordWrap = true,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle RegularLabelStyle => _regularLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _regularCenterLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = { textColor = PicEase_Color.SecondaryFontColor }
        });
        public static GUIStyle RegularCenterLabelStyle => _regularCenterLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _boldLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 19,
            alignment = TextAnchor.UpperLeft,
            wordWrap = false,
            normal = { textColor = PicEase_Color.SecondaryFontColor }
        });
        public static GUIStyle BoldLabelStyle => _boldLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _miniBoldLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 17,
            wordWrap = false,
            normal = { textColor = PicEase_Color.TertiaryFontColor }
        });
        public static GUIStyle MiniBoldLabelStyle => _miniBoldLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _infoLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 13,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = { textColor = PicEase_Color.SecondaryFontColor }
        });
        public static GUIStyle InfoLabelStyle => _infoLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _tabLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.SemiBold,
            fontSize = 22,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle TabLabelStyle => _tabLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _layoutLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 15,
            wordWrap = false,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle LayoutLabelStyle => _layoutLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _buttonLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 13,
            wordWrap = false,
            clipping = TextClipping.Overflow,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle ButtonLabelStyle => _buttonLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _regularLargeLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 16,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = false,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle RegularLargeLabelStyle => _regularLargeLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _assignedLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 16,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = false,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle AssignedLabelStyle => _assignedLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _unassignedLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 16,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = false,
            normal = { textColor = PicEase_Color.GetPrimaryFontColorFaded() }
        });
        public static GUIStyle UnassignedLabelStyle => _unassignedLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorDragAndDropLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEase_Color.SecondaryFontColor }
        });
        public static GUIStyle ImageEditorDragAndDropLabelStyle => _imageEditorDragAndDropLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _mapGeneratorDragAndDropLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEase_Color.TertiaryFontColor }
        });
        public static GUIStyle MapGeneratorDragAndDropLabelStyle => _mapGeneratorDragAndDropLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _categoryLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle CategoryLabelStyle => _categoryLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _previewLabelStyleLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Bold,
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEase_Color.GetPrimaryFontColor() }
        });
        public static GUIStyle PreviewLabelStyle => _previewLabelStyleLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _buttonsDivisorLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 20,
            fixedHeight = 30,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEase_Color.SecondaryFontColor }
        });
        public static GUIStyle ButtonsDivisorLabelStyle => _buttonsDivisorLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _headerButtonsDivisorLabelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.Regular,
            fontSize = 18,
            fixedHeight = 25,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEase_Color.SecondaryFontColor }
        });
        public static GUIStyle HeaderButtonsDivisorLabelStyle => _headerButtonsDivisorLabelStyle.Value;
        #endregion

        #region Panels
        private static readonly Lazy<GUIStyle> _primaryPanelStyle = new(() => new(GUI.skin.box)
        {
            stretchWidth = true,
            stretchHeight = true,
            border = new(-5, -5, -5, -5),
            normal = { background = PicEase_Texture.CreateRadialGradientTexture(512, 512, new Color[] { PicEase_Color.GetPrimaryPanelColorTop(), PicEase_Color.GetPrimaryPanelColorBottom() }) }
        });
        public static GUIStyle PrimaryPanelStyle => _primaryPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _secondaryPanelStyle = new(() => new(GUI.skin.box)
        {
            padding = new(10, 10, 10, 10),
            margin = new(0, 0, 0, 0),
            normal = { background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetSecondaryPanelColor()) }
        });
        public static GUIStyle SecondaryPanelStyle => _secondaryPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _menuButtonsPanelStyle = new(() => new(GUI.skin.box)
        {
            fixedHeight = 60,
            stretchWidth = true,
            normal = { background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetSecondaryPanelColor()) }
        });
        public static GUIStyle MenuButtonsPanelStyle => _menuButtonsPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorDragAndDropPanelStyle = new(() => new(GUI.skin.box)
        {
            padding = new(10, 10, 10, 10),
            normal = { background = PicEase_Texture.CreateGradientTexture(2, 256, new Color[] { PicEase_Color.SecondaryFontColorFaded, PicEase_Color.GetTertiaryPanelColorMiddle(),  PicEase_Color.GetTertiaryPanelColorTop() }) }
        });
        public static GUIStyle ImageEditorDragAndDropPanelStyle => _imageEditorDragAndDropPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _mapGeneratorDragAndDropPanelStyle = new(() => new(GUI.skin.box)
        {
            padding = new(10, 10, 10, 10),
            normal = { background = PicEase_Texture.CreateGradientTexture(2, 256, new Color[] { PicEase_Color.TertiaryFontColorFaded, PicEase_Color.GetTertiaryPanelColorMiddle(), PicEase_Color.GetTertiaryPanelColorTop() }) }
        });
        public static GUIStyle MapGeneratorDragAndDropPanelStyle => _mapGeneratorDragAndDropPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorLeftPanelStyle = new(() => new(GUI.skin.box)
        {
            fixedWidth = 300,
            stretchHeight = true,
            padding = new(10, 10, 10, 10),
            margin = new(10, 10, 10, 10),
            normal = { background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetSecondaryPanelColor()) }
        });
        public static GUIStyle ImageEditorLeftPanelStyle => _imageEditorLeftPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorRightPanelStyle = new(() => new(GUI.skin.box)
        {
            padding = new(10, 10, 10, 12),
            margin = new(10, 10, 10, 10),
            stretchWidth = true,
            stretchHeight = true,
            normal = { background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetSecondaryPanelColor()) }
        });
        public static GUIStyle ImageEditorRightPanelStyle => _imageEditorRightPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorCategoryPanelStyle = new(() => new(GUI.skin.box)
        {
            fixedHeight = 64,
            padding = new(10, 10, 8, 0),
            normal = { background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetQuartiaryPanelColor()) }
        });
        public static GUIStyle ImageEditorCategoryPanelStyle => _imageEditorCategoryPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _menuPopupPanelStyle = new(() => new()
        {
            font = PicEase_Resources.Fonts.SemiBold,
            alignment = TextAnchor.MiddleLeft,
            fontSize = 14,
            fixedHeight = 24,
            fixedWidth = 160,
            stretchWidth = true,
            stretchHeight = true,
            padding = new(5, 5, 10, 10),
            margin = new(1, 1, 1, 1),
            border = new(-5, -5, -5, -20),
            normal =
            {
                textColor = PicEase_Color.GetPrimaryFontColor(),
                background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetPrimaryPanelColorBottom())
            },
            hover = new()
            {
                textColor = PicEase_Color.SecondaryFontColor,
                background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetPrimaryPanelColorBottom())
            },
            active = new()
            {
                textColor = PicEase_Color.SecondaryFontColor,
                background = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetPrimaryPanelColorBottom())
            }
        });
        public static GUIStyle MenuPopupPanelStyle => _menuPopupPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _tooltipPanel = new(() => new(GUI.skin.box)
        {
            padding = new(2, 2, 2, 2),
            wordWrap = true,
            normal = { background = Texture2D.whiteTexture, textColor = Color.black }
        });
        public static GUIStyle TooltipPanel => _tooltipPanel.Value;
        #endregion

        #region Buttons
        private static readonly Lazy<GUIStyle> _menuButtonStyle = new(() => new(GUI.skin.button)
        {
            font = PicEase_Resources.Fonts.SemiBold,
            fontSize = 18,
            fixedHeight = 25,
            alignment = TextAnchor.MiddleCenter,
            normal = new()
            {
                textColor = PicEase_Color.GetPrimaryFontColor(),
                background = PicEase_Texture.ClearTexture
            },
            hover = new()
            {
                textColor = PicEase_Color.SecondaryFontColor,
                background = PicEase_Texture.ClearTexture
            },
            active = new()
            {
                textColor = PicEase_Color.SecondaryFontColorFaded,
                background = PicEase_Texture.ClearTexture
            }
        });
        public static GUIStyle MenuButtonStyle => _menuButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _headerMenuButtonStyle = new(() => new(GUI.skin.button)
        {
            font = PicEase_Resources.Fonts.SemiBold,
            fontSize = 16,
            fixedHeight = 20,
            alignment = TextAnchor.MiddleCenter,
            normal = new()
            {
                textColor = PicEase_Color.GetPrimaryFontColor(),
                background = PicEase_Texture.ClearTexture
            },
            hover = new()
            {
                textColor = PicEase_Color.SecondaryFontColor,
                background = PicEase_Texture.ClearTexture
            },
            active = new()
            {
                textColor = PicEase_Color.SecondaryFontColorFaded,
                background = PicEase_Texture.ClearTexture
            }
        });
        public static GUIStyle HeaderButtonStyle => _headerMenuButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _adjustmentsButtonStyle = new(() => CreateButtonIconStyle(PicEase_Resources.Icons.Adjustments));
        public static GUIStyle AdjustmentsButtonStyle => _adjustmentsButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _colorizeButtonStyle = new(() => CreateButtonIconStyle(PicEase_Resources.Icons.Colorize));
        public static GUIStyle ColorizeButtonStyle => _colorizeButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _filtersButtonStyle = new(() => CreateButtonIconStyle(PicEase_Resources.Icons.Filters));
        public static GUIStyle FiltersButtonStyle => _filtersButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _lutsButtonStyle = new(() => CreateButtonIconStyle(PicEase_Resources.Icons.LUTs));
        public static GUIStyle LUTsButtonStyle => _lutsButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _resetButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 25,
            fixedWidth = 25,
            overflow = new(3, 3, 2, 2),
            margin = new(0, 0, -2, 0),
            normal = { background = PicEase_Resources.Icons.Reset }
        });
        public static GUIStyle ResetButtonStyle => _resetButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _tooltipButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 25,
            fixedWidth = 16,
            overflow = new(9, 7, 2, 2),
            margin = new(0, 0, -2, 0),
            normal = { background = PicEase_Resources.Icons.Tooltip }
        });
        public static GUIStyle TooltipButtonStyle => _tooltipButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _aspectRatioButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 32,
            fixedWidth = 32,
            overflow = new(4, 4, 4, 4),
            margin = new(0, 0, 0, 0),
            normal = { background = PicEase_Resources.Icons.AspectRatio }
        });
        public static GUIStyle AspectRatioButtonStyle => _aspectRatioButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _resolutionRatioButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 28,
            fixedWidth = 28,
            overflow = new(4, 4, 4, 4),
            margin = new(0, 0, 2, 0),
            normal = { background = PicEase_Resources.Icons.ResolutionRatio }
        });
        public static GUIStyle ResolutionRatioButtonStyle => _resolutionRatioButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _promotionalButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 64,
            fixedWidth = 64,
            alignment = TextAnchor.MiddleLeft,
            normal = { background = PicEase_Resources.Promotional.HierarchyDesigner }
        });
        public static GUIStyle PromotionalButtonStyle => _promotionalButtonStyle.Value;
        #endregion

        #region Modifiables
        private static Lazy<GUIStyle> _imageEditorinfoLabelStyle = new(() => CreateImageEditorInfoLabelStyle());
        public static GUIStyle ImageEditorInfoLabelStyle => _imageEditorinfoLabelStyle.Value;
        #endregion
        #endregion

        #region Classes
        private sealed class CustomPopupMenu : PopupWindowContent
        {
            #region Properties
            private readonly Dictionary<string, Action> menuItems;
            #endregion

            #region Constructor
            public CustomPopupMenu(Dictionary<string, Action> items)
            {
                menuItems = items;
            }
            #endregion

            #region Override Methods
            public override void OnGUI(Rect rect)
            {
                GUILayout.BeginVertical();
                foreach (KeyValuePair<string, Action> kvp in menuItems)
                {
                    if (GUILayout.Button(kvp.Key, MenuPopupPanelStyle, GUILayout.Height(MenuPopupPanelStyle.fixedHeight)))
                    {
                        kvp.Value?.Invoke();
                        editorWindow.Close();
                    }
                }
                GUILayout.EndVertical();
            }

            public override Vector2 GetWindowSize()
            {
                return new(MenuPopupPanelStyle.fixedWidth, (menuItems.Count * MenuPopupPanelStyle.fixedHeight) + 6);
            }
            #endregion
        }

        private sealed class TooltipPopup : PopupWindowContent
        {
            #region Properties
            private readonly string tooltipText;
            private static readonly GUIStyle style;
            private static readonly Texture2D backgroundTexture;
            private const float maxWidth = 220f;
            private const int fontSize = 14;
            #endregion

            #region Constructor
            static TooltipPopup()
            {
                backgroundTexture = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetQuartiaryPanelColor());
                style = new(EditorStyles.helpBox)
                {
                    font = PicEase_Resources.Fonts.Regular,
                    fontSize = fontSize,
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    padding = new(2, 2, 2, 3),
                    normal =
                    {
                        textColor = PicEase_Color.GetPrimaryFontColor(),
                        background = backgroundTexture
                    }
                };
            }
            #endregion

            #region Accessor
            public TooltipPopup(string text)
            {
                tooltipText = text;
            }
            #endregion

            #region Override Methods
            public override void OnGUI(Rect rect)
            {
                GUILayout.Label(tooltipText, style);
            }

            public override Vector2 GetWindowSize()
            {
                GUIContent content = new(tooltipText);
                float height = style.CalcHeight(content, maxWidth);
                return new(maxWidth, height);
            }
            #endregion
        }

        private sealed class AspectRatioPopup : PopupWindowContent
        {
            #region Properties
            private static readonly GUIStyle style;
            private static readonly Texture2D backgroundTexture;
            private const float buttonHeight = 25f;
            private const float maxWidth = 120f;
            private static readonly string[] aspectRatios = { "860 x 680", "900 x 700", "1030 x 900", "1200 x 900", "1500 x 900", "1900 x 1000" };
            private static readonly Vector2[] dimensions = { new(860, 680), new(900, 700), new(1030, 900), new(1200, 900), new(1500, 900), new(1900, 1000) };
            private readonly Action<Vector2> onAspectRatioSelected;
            #endregion

            #region Constructor
            static AspectRatioPopup()
            {
                backgroundTexture = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetQuartiaryPanelColor());
                style = new(EditorStyles.helpBox)
                {
                    font = PicEase_Resources.Fonts.Regular,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    padding = new(2, 2, 2, 2),
                    normal =
                    {
                        textColor = PicEase_Color.GetPrimaryFontColor(),
                        background = backgroundTexture
                    }
                };
            }

            public AspectRatioPopup(Action<Vector2> onAspectRatioSelected)
            {
                this.onAspectRatioSelected = onAspectRatioSelected;
            }
            #endregion

            #region Override Methods
            public override void OnGUI(Rect rect)
            {
                for (int i = 0; i < aspectRatios.Length; i++)
                {
                    if (GUILayout.Button(aspectRatios[i], style, GUILayout.Height(buttonHeight)))
                    {
                        onAspectRatioSelected(dimensions[i]);
                        editorWindow.Close();
                    }
                }
            }

            public override Vector2 GetWindowSize()
            {
                float popupHeight = (aspectRatios.Length * buttonHeight) + 15;
                return new(maxWidth, popupHeight);
            }
            #endregion
        }

        private sealed class ResolutionPopup : PopupWindowContent
        {
            #region Properties
            private static readonly GUIStyle style;
            private static readonly Texture2D backgroundTexture;
            private const float buttonHeight = 25f;
            private const float maxWidth = 200f;
            private static readonly string[] resolutionLabels =
            {
            "1K (HD) - 1280 x 720",
            "1K (Full HD) - 1920 x 1080",
            "2K (DCI) - 2048 x 1080",
            "2K (QHD) - 2560 x 1440",
            "4K (UHD) - 3840 x 2160",
            "4K (DCI) - 4096 x 2160",
            "5K - 5120 x 2880",
            "6K - 6144 x 3160",
            "6K - 6144 x 3456",
            "8K (UHD) - 7680 x 4320",
            "8K (DCI) - 8192 x 4320",
            "10K - 10240 x 4320",
            "12K - 12288 x 6480",
            "16K - 16360 x 8640"
            };
            private static readonly Vector2[] resolutionDimensions =
            {
            new(1280, 720),
            new(1920, 1080),
            new(2048, 1080),
            new(2560, 1440),
            new(3840, 2160),
            new(4096, 2160),
            new(5120, 2880),
            new(6144, 3160),
            new(6144, 3456),
            new(7680, 4320),
            new(8192, 4320),
            new(10240, 4320),
            new(12288, 6480),
            new(16360, 8640)
            };
            private readonly Action<Vector2> onResolutionSelected;
            #endregion

            #region Constructor
            static ResolutionPopup()
            {
                backgroundTexture = PicEase_Texture.CreateTexture(2, 2, PicEase_Color.GetQuartiaryPanelColor());
                style = new GUIStyle(EditorStyles.helpBox)
                {
                    font = PicEase_Resources.Fonts.Regular,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    padding = new RectOffset(2, 2, 2, 2),
                    normal =
                    {
                        textColor = PicEase_Color.GetPrimaryFontColor(),
                        background = backgroundTexture
                    }
                };
            }

            public ResolutionPopup(Action<Vector2> onResolutionSelected)
            {
                this.onResolutionSelected = onResolutionSelected;
            }
            #endregion

            #region Override Methods
            public override void OnGUI(Rect rect)
            {
                for (int i = 0; i < resolutionLabels.Length; i++)
                {
                    if (GUILayout.Button(resolutionLabels[i], style, GUILayout.Height(buttonHeight)))
                    {
                        onResolutionSelected(resolutionDimensions[i]);
                        editorWindow.Close();
                    }
                }
            }

            public override Vector2 GetWindowSize()
            {
                float popupHeight = (resolutionLabels.Length * buttonHeight) + 30;
                return new Vector2(maxWidth, popupHeight);
            }
            #endregion
        }
        #endregion

        #region Helpers
        private static GUIStyle CreateButtonIconStyle(Texture2D icon)
        {
            return new(GUI.skin.button)
            {
                fixedHeight = 32,
                fixedWidth = 32,
                normal = { background = icon }
            };
        }
        #endregion

        #region Methods
        public static float DrawFloatSlider(string label, float value, float leftValue, float rightValue, float defaultValue)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(label, RegularLabelStyle, GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            float newValue = EditorGUILayout.Slider(value, leftValue, rightValue);
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            return newValue;
        }

        public static bool DrawToggle(string label,  bool value, bool defaultValue)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(label, RegularLabelStyle, GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            bool newValue = EditorGUILayout.Toggle(value);
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            return newValue;
        }

        public static int DrawIntSlider(string label, float labelWidth, int value, int leftValue, int rightValue, int defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            int newValue = EditorGUILayout.IntSlider(value, leftValue, rightValue, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
                GUIUtility.keyboardControl = 0;
            }

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static int DrawIntSliderVertical(string label, float labelWidth, int value, int leftValue, int rightValue, int defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(label, RegularLabelStyle, GUILayout.ExpandWidth(true));

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.BeginHorizontal();
            int newValue = EditorGUILayout.IntSlider(value, leftValue, rightValue, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            return newValue;
        }

        public static int DrawIntField(string label, float labelWidth, int value, int defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));

            EditorGUILayout.BeginHorizontal();
            int newValue = EditorGUILayout.IntField(value);
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static string DrawStringField(string label, float labelWidth, string value, string defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));

            EditorGUILayout.BeginHorizontal();
            string newValue = EditorGUILayout.TextField(value);
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static bool DrawToggle(string label, float labelWidth, bool value, bool defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            bool newValue = EditorGUILayout.Toggle(value, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static int DrawPopup(string label, float labelWidth, int selectedIndex, string[] options, int defaultValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));

            int newIndex = EditorGUILayout.Popup(selectedIndex, options, GUILayout.ExpandWidth(true));
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newIndex = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newIndex;
        }

        public static Color DrawColorField(string label, float labelWidth, Color value, Color defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            Color newColor = EditorGUILayout.ColorField(value, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newColor = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newColor;
        }

        public static T DrawEnumPopup<T>(string label, float labelWidth, T selectedEnum, T defaultEnum, bool showTooltip = false, string tooltipText = "") where T : Enum
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            T newEnum = (T)EditorGUILayout.EnumPopup(selectedEnum, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newEnum = defaultEnum;
            }

            EditorGUILayout.EndHorizontal();
            return newEnum;
        }

        public static T DrawObjectField<T>(string label, float labelWidth, T currentValue, T defaultValue, bool showTooltip = false, string tooltipText = "") where T : UnityEngine.Object
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltipPopup(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            T newValue = (T)EditorGUILayout.ObjectField(currentValue, typeof(T), false, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static bool DrawButtonIcon(string label, GUIStyle buttonStyle)
        {
            float width = buttonStyle.fixedWidth;

            EditorGUILayout.BeginVertical(GUILayout.Width(width));
            bool isClicked = GUILayout.Button(string.Empty, buttonStyle, GUILayout.Width(width), GUILayout.Height(buttonStyle.fixedHeight));
            GUILayout.Space(2);
            EditorGUILayout.LabelField(label, ButtonLabelStyle, GUILayout.Width(width));
            EditorGUILayout.EndVertical();

            return isClicked;
        }

        public static void DrawAspectRatioButton(Action<Vector2> onAspectRatioSelected)
        {
            DrawAspectRatioPopup(onAspectRatioSelected);
        }

        public static void DrawResolutionButton(Action<Vector2> onResolutionSelected)
        {
            DrawResolutionPopup(onResolutionSelected);
        }
        #endregion

        #region Operations
        public static void DrawPopupButton(string buttonText, GUIStyle buttonStyle, float buttonHeight, Dictionary<string, Action> menuItems)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(new(buttonText), buttonStyle, GUILayout.Height(buttonHeight), GUILayout.ExpandWidth(false));
            if (GUI.Button(buttonRect, buttonText, buttonStyle))
            {
                Rect popupRect = new(buttonRect.x + 6, buttonRect.y + buttonRect.height + 4, 0, 0);
                PopupWindow.Show(popupRect, new CustomPopupMenu(menuItems));
            }
            GUILayout.Space(2);
        }

        public static void DrawTooltipPopup(string tooltipText)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(TooltipButtonStyle.fixedWidth, TooltipButtonStyle.fixedHeight, TooltipButtonStyle, GUILayout.ExpandWidth(false));
            if (GUI.Button(buttonRect, GUIContent.none, TooltipButtonStyle))
            {
                Rect popupRect = new(buttonRect.x, buttonRect.y + buttonRect.height + 4, 0, 0);
                PopupWindow.Show(popupRect, new TooltipPopup(tooltipText));
            }
            GUILayout.Space(2);
        }

        private static void DrawAspectRatioPopup(Action<Vector2> onAspectRatioSelected)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(AspectRatioButtonStyle.fixedWidth, AspectRatioButtonStyle.fixedHeight, AspectRatioButtonStyle, GUILayout.ExpandWidth(false));
            if (GUI.Button(buttonRect, GUIContent.none, AspectRatioButtonStyle))
            {
                Rect popupRect = new(buttonRect.x, buttonRect.y + buttonRect.height + 4, 0, 0);
                PopupWindow.Show(popupRect, new AspectRatioPopup(onAspectRatioSelected));
            }
            GUILayout.Space(2);
        }

        private static void DrawResolutionPopup(Action<Vector2> onResolutionSelected)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(ResolutionRatioButtonStyle.fixedWidth, ResolutionRatioButtonStyle.fixedHeight, ResolutionRatioButtonStyle, GUILayout.ExpandWidth(false));
            if (GUI.Button(buttonRect, GUIContent.none, ResolutionRatioButtonStyle))
            {
                Rect popupRect = new(buttonRect.x, buttonRect.y + buttonRect.height + 4, 0, 0);
                PopupWindow.Show(popupRect, new ResolutionPopup(onResolutionSelected));
            }
            GUILayout.Space(2);
        }
        #endregion

        #region Modifiables
        private static GUIStyle CreateImageEditorInfoLabelStyle()
        {
            return new()
            {
                font = PicEase_Resources.Fonts.Regular,
                fontSize = PicEase_Settings.ImageEditorInfoLabelFontSize,
                fixedHeight = PicEase_Window.ImageInfoLabelHeight,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = PicEase_Settings.ImageEditorInfoLabelFontColor }
            };
        }

        public static void RefreshImageEditorInfoLabelStyle()
        {
            _imageEditorinfoLabelStyle = new(() => CreateImageEditorInfoLabelStyle());
        }
        #endregion
    }
}
#endif