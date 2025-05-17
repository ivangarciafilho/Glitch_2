#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Verpha.PicEase
{
    internal static class PicEase_Resources
    {
        private static class ResourceNames
        {
            #region Fonts
            internal const string FontBold = "PicEase Font Bold";
            internal const string FontRegular = "PicEase Font Regular";
            internal const string FontSemiBold = "PicEase Font Semi Bold";
            #endregion

            #region Icons
            internal const string IconAdjustments = "PicEase Icon Adjustments";
            internal const string IconColorize = "PicEase Icon Colorize";
            internal const string IconFilters = "PicEase Icon Filters";
            internal const string IconLUTs = "PicEase Icon LUTs";
            internal const string IconReset = "PicEase Icon Reset";
            internal const string IconTooltip = "PicEase Icon Tooltip";
            internal const string IconAspectRatio = "PicEase Icon Aspect Ratio";
            internal const string IconResolutionRatio = "PicEase Icon Resolution Ratio";
            #endregion

            #region Graphics
            // Dark Theme
            internal const string GraphicsTitleDark = "PicEase Graphics Title Dark";
            internal const string GraphicsOverlayDark = "PicEase Graphics Overlay Dark";

            // Light Theme
            internal const string GraphicsTitleLight = "PicEase Graphics Title Light";
            internal const string GraphicsOverlayLight = "PicEase Graphics Overlay Light";
            #endregion

            #region Promotional
            internal const string PromotionalHierarchyDesigner = "PicEase Promotional Hierarchy Designer";
            #endregion

            #region LUTs
            internal const string LUTAdventureI = "PicEase LUT Adventure I";
            internal const string LUTAdventureII = "PicEase LUT Adventure II";
            internal const string LUTAdventureIII = "PicEase LUT Adventure III";
            internal const string LUTAmbientI = "PicEase LUT Ambient I";
            internal const string LUTAmbientII = "PicEase LUT Ambient II";
            internal const string LUTCinemaI = "PicEase LUT Cinema I";
            internal const string LUTCinemaII = "PicEase LUT Cinema II";
            internal const string LUTCinemaIII = "PicEase LUT Cinema III";
            internal const string LUTCinemaIV = "PicEase LUT Cinema IV";
            internal const string LUTCinematicI = "PicEase LUT Cinematic I";
            internal const string LUTCinematicII = "PicEase LUT Cinematic II";
            internal const string LUTCinematicIII = "PicEase LUT Cinematic III";
            internal const string LUTCinematicIV = "PicEase LUT Cinematic IV";
            internal const string LUTHDR = "PicEase LUT HDR";
            internal const string LUTHollywoodI = "PicEase LUT Hollywood I";
            internal const string LUTHollywoodII = "PicEase LUT Hollywood II";
            internal const string LUTNoirI = "PicEase LUT Noir I";
            internal const string LUTNoirII = "PicEase LUT Noir II";
            internal const string LUTTeal = "PicEase LUT Teal";
            internal const string LUTVintageI = "PicEase LUT Vintage I";
            #endregion
        }

        #region Classes
        internal static class Fonts
        {
            private static readonly Lazy<Font> _bold = new(() => PicEase_Texture.LoadFont(ResourceNames.FontBold));
            public static Font Bold => _bold.Value;

            private static readonly Lazy<Font> _regular = new(() => PicEase_Texture.LoadFont(ResourceNames.FontRegular));
            public static Font Regular => _regular.Value;

            private static readonly Lazy<Font> _semibold = new(() => PicEase_Texture.LoadFont(ResourceNames.FontSemiBold));
            public static Font SemiBold => _semibold.Value;
        }

        internal static class Icons
        {
            private static readonly Lazy<Texture2D> _adjustments = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconAdjustments));
            public static Texture2D Adjustments => _adjustments.Value;

            private static readonly Lazy<Texture2D> _colorize = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconColorize));
            public static Texture2D Colorize => _colorize.Value;

            private static readonly Lazy<Texture2D> _filters = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconFilters));
            public static Texture2D Filters => _filters.Value;

            private static readonly Lazy<Texture2D> _luts = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconLUTs));
            public static Texture2D LUTs => _luts.Value;

            private static readonly Lazy<Texture2D> _reset = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconReset));
            public static Texture2D Reset => _reset.Value;

            private static readonly Lazy<Texture2D> _tooltip = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconTooltip));
            public static Texture2D Tooltip => _tooltip.Value;

            private static readonly Lazy<Texture2D> _aspectRatio = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconAspectRatio));
            public static Texture2D AspectRatio => _aspectRatio.Value; 
            
            private static readonly Lazy<Texture2D> _resolutionRatio = new(() => PicEase_Texture.LoadTexture(ResourceNames.IconResolutionRatio));
            public static Texture2D ResolutionRatio => _resolutionRatio.Value;
        }

        internal static class Graphics
        {
            private static readonly Lazy<Texture2D> _title = new(() => PicEase_Texture.LoadTexture(PicEase_Session.IsProSkin ? ResourceNames.GraphicsTitleDark : ResourceNames.GraphicsTitleLight));
            public static Texture2D Title => _title.Value;

            private static readonly Lazy<Texture2D> _overlay = new(() => PicEase_Texture.LoadTexture(PicEase_Session.IsProSkin ? ResourceNames.GraphicsOverlayDark : ResourceNames.GraphicsOverlayLight));
            public static Texture2D Overlay => _overlay.Value;
        }

        internal static class Promotional
        {
            private static readonly Lazy<Texture2D> _hierarchyDesigner = new(() => PicEase_Texture.LoadTexture(ResourceNames.PromotionalHierarchyDesigner));
            public static Texture2D HierarchyDesigner => _hierarchyDesigner.Value;
        }

        internal static class LUTs
        {
            private static readonly Lazy<Texture3D> _adventureI = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTAdventureI));
            public static Texture3D AdventureI => _adventureI.Value;

            private static readonly Lazy<Texture3D> _adventureII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTAdventureII));
            public static Texture3D AdventureII => _adventureII.Value;

            private static readonly Lazy<Texture3D> _adventureIII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTAdventureIII));
            public static Texture3D AdventureIII => _adventureIII.Value;

            private static readonly Lazy<Texture3D> _ambientI = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTAmbientI));
            public static Texture3D AmbientI => _ambientI.Value;

            private static readonly Lazy<Texture3D> _ambientII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTAmbientII));
            public static Texture3D AmbientII => _ambientII.Value;

            private static readonly Lazy<Texture3D> _cinemaI = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinemaI));
            public static Texture3D CinemaI => _cinemaI.Value;

            private static readonly Lazy<Texture3D> _cinemaII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinemaII));
            public static Texture3D CinemaII => _cinemaII.Value;

            private static readonly Lazy<Texture3D> _cinemaIII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinemaIII));
            public static Texture3D CinemaIII => _cinemaIII.Value;

            private static readonly Lazy<Texture3D> _cinemaIV = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinemaIV));
            public static Texture3D CinemaIV => _cinemaIV.Value;

            private static readonly Lazy<Texture3D> _cinematicI = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinematicI));
            public static Texture3D CinematicI => _cinematicI.Value;

            private static readonly Lazy<Texture3D> _cinematicII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinematicII));
            public static Texture3D CinematicII => _cinematicII.Value;

            private static readonly Lazy<Texture3D> _cinematicIII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinematicIII));
            public static Texture3D CinematicIII => _cinematicIII.Value;

            private static readonly Lazy<Texture3D> _cinematicIV = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTCinematicIV));
            public static Texture3D CinematicIV => _cinematicIV.Value;

            private static readonly Lazy<Texture3D> _hdr = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTHDR));
            public static Texture3D HDR => _hdr.Value;

            private static readonly Lazy<Texture3D> _hollywoodI = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTHollywoodI));
            public static Texture3D HollywoodI => _hollywoodI.Value;

            private static readonly Lazy<Texture3D> _hollywoodII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTHollywoodII));
            public static Texture3D HollywoodII => _hollywoodII.Value;

            private static readonly Lazy<Texture3D> _noirI = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTNoirI));
            public static Texture3D NoirI => _noirI.Value;

            private static readonly Lazy<Texture3D> _noirII = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTNoirII));
            public static Texture3D NoirII => _noirII.Value;

            private static readonly Lazy<Texture3D> _teal = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTTeal));
            public static Texture3D Teal => _teal.Value;

            private static readonly Lazy<Texture3D> _vintageI = new(() => PicEase_Texture.LoadTexture3D(ResourceNames.LUTVintageI));
            public static Texture3D VintageI => _vintageI.Value;
        }
        #endregion
    }
}
#endif