#if UNITY_EDITOR
using System;
using System.Globalization;
using UnityEngine;

namespace Verpha.PicEase
{
    internal static class PicEase_Color
    {
        #region Properties
        // Dark Theme
        public static readonly Color PrimaryFontColorDark = HexToColor("#fff8ef");
        public static readonly Color PrimaryFontColorFadedDark = HexToColor("#fff8ef80");
        public static readonly Color PrimaryPanelColorTopDark = HexToColor("#020202");
        public static readonly Color PrimaryPanelColorBottomDark = HexToColor("#000000");
        public static readonly Color SecondaryPanelColorDark = HexToColor("#0A0A0AC8");
        public static readonly Color TertiaryPanelColorTopDark = HexToColor("#00000055");
        public static readonly Color TertiaryPanelColorMiddleDark = HexToColor("#00000055");
        public static readonly Color QuartiaryPanelColorDark = HexToColor("#00000080");

        // Light Theme
        public static readonly Color PrimaryFontColorLight = HexToColor("#141414");
        public static readonly Color PrimaryFontColorFadedLight = HexToColor("#14141480");
        public static readonly Color PrimaryPanelColorTopLight = HexToColor("#ededed");
        public static readonly Color PrimaryPanelColorBottomLight = HexToColor("#ffffff");
        public static readonly Color SecondaryPanelColorLight = HexToColor("#F5F5F5C8");
        public static readonly Color TertiaryPanelColorTopLight = HexToColor("#FFFFFF55");
        public static readonly Color TertiaryPanelColorMiddleLight = HexToColor("#FFFFFF55");
        public static readonly Color QuartiaryPanelColorLight = HexToColor("#FFFFFF80");

        // Shared
        public static readonly Color SecondaryFontColor = HexToColor("#f54296");
        public static readonly Color SecondaryFontColorFaded = HexToColor("#FF56AB80");
        public static readonly Color TertiaryFontColor = HexToColor("#4EA8FF");
        public static readonly Color TertiaryFontColorFaded = HexToColor("#4EA8FF80");
        #endregion

        #region Getters
        public static Color GetPrimaryFontColor()
        {
            return PicEase_Session.IsProSkin ? PrimaryFontColorDark : PrimaryFontColorLight;
        }

        public static Color GetPrimaryFontColorFaded()
        {
            return PicEase_Session.IsProSkin ? PrimaryFontColorFadedDark : PrimaryFontColorFadedLight;
        }

        public static Color GetPrimaryPanelColorTop()
        {
            return PicEase_Session.IsProSkin ? PrimaryPanelColorTopDark : PrimaryPanelColorTopLight;
        }

        public static Color GetPrimaryPanelColorBottom()
        {
            return PicEase_Session.IsProSkin ? PrimaryPanelColorBottomDark : PrimaryPanelColorBottomLight;
        }

        public static Color GetSecondaryPanelColor()
        {
            return PicEase_Session.IsProSkin ? SecondaryPanelColorDark : SecondaryPanelColorLight;
        }

        public static Color GetTertiaryPanelColorTop()
        {
            return PicEase_Session.IsProSkin ? TertiaryPanelColorTopDark : TertiaryPanelColorTopLight;
        }

        public static Color GetTertiaryPanelColorMiddle()
        {
            return PicEase_Session.IsProSkin ? TertiaryPanelColorMiddleDark : TertiaryPanelColorMiddleLight;
        }

        public static Color GetQuartiaryPanelColor()
        {
            return PicEase_Session.IsProSkin ? QuartiaryPanelColorDark : QuartiaryPanelColorLight;
        }
        #endregion

        #region Methods
        public static Color HexToColor(string hex)
        {
            try
            {
                hex = hex.Replace("0x", "").Replace("#", "");
                byte a = 255;
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                }
                return new Color32(r, g, b, a);
            }
            catch (Exception ex)
            {
                Debug.LogError("Color parsing failed: " + ex.Message);
                return Color.white;
            }
        }
        #endregion
    }
}
#endif