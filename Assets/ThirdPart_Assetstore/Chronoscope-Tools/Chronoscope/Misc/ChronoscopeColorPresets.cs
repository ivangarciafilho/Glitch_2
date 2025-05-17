using System.Collections.Generic;
using UnityEngine;
using ChronoscopeTools;

namespace ChronoscopeToolsInternal
{
    public static class ChronoscopeColorPresets
    {
        public enum PresetSchemes
        {
            Default,
            Dark,
            DarkBlue,
            DarkOrange,
            Yellow,
            DarkGreen,
            GreenScreen,
            InfraRed,
        }

        public static Dictionary<PresetSchemes, ChronoscopeColorSet> Presets = new Dictionary<PresetSchemes, ChronoscopeColorSet>()
    {
        { PresetSchemes.Default, new ChronoscopeColorSet()
            {
                areaColor = new Color(0.85f, 0.85f, 0.85f, 1.0f),
                headerColor = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                valueColor = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                majorColor = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                minorColor = new Color(0.55f, 0.55f, 0.55f, 1.0f),
                markerBackgroundColor = new Color(0.4f, 0.4f, 0.4f, 0.329f),
                markerLineColor = new Color(1.0f, 0.0f, 0.0f, 1.0f),
                eventMarkerColor = new Color(1.0f, 0.0f, 0.0f, 0.85f),
                eventAreaBackground = new Color(0.85f, 0.85f, 0.85f, 0.75f)
            }
        },
        { PresetSchemes.Dark, new ChronoscopeColorSet()
            {
                areaColor = new Color(0.08f, 0.08f, 0.08f, 1.0f),
                headerColor = new Color(0.83f, 0.83f, 0.83f, 1.0f),
                valueColor = new Color(0.83f, 0.83f, 0.83f, 1.0f),
                majorColor = new Color(0.83f, 0.83f, 0.83f, 1.0f),
                minorColor = new Color(0.55f, 0.55f, 0.55f, 1.0f),
                markerBackgroundColor = new Color(0.72f, 0.72f, 0.72f, 0.329f),
                markerLineColor = new Color(1.0f, 0.0f, 0.0f, 1.0f),
                eventMarkerColor = new Color(1.0f, 0.0f, 0.0f, 0.85f),
                eventAreaBackground = new Color(0.08f, 0.08f, 0.08f, 0.75f)
            }
        },
        { PresetSchemes.DarkBlue, new ChronoscopeColorSet()
            {
                areaColor = new Color(0.145f, 0.164f, 0.250f, 1f),
                headerColor = new Color(0.392f, 0.5415f, 0.741f, 1f),
                valueColor = new Color(0.701f, 0.764f, 0.847f, 1f),
                majorColor = new Color(0.701f, 0.764f, 0.847f, 1f),
                minorColor = new Color(0.349f, 0.411f, 0.494f, 1f),
                markerBackgroundColor = new Color(0.760f, 0.796f, 0.858f, 0.329f),
                markerLineColor = new Color(1f, 0f, 0f, 1f),
                eventMarkerColor = new Color(1f, 0f, 0f, 0.85f),
                eventAreaBackground = new Color(0.145f, 0.164f, 0.250f, 0.75f)
            }
        },
        { PresetSchemes.DarkOrange, new ChronoscopeColorSet()
            {
                areaColor = new Color(0.149f, 0.149f, 0.149f, 1f),
                headerColor = new Color(1f, 0.501f, 0.250f, 1f),
                valueColor = new Color(0.749f, 0.749f, 0.749f, 1f),
                majorColor = new Color(0.749f, 0.749f, 0.749f, 1f),
                minorColor = new Color(0.501f, 0.501f, 0.501f, 1f),
                markerBackgroundColor = new Color(0.839f, 0.839f, 0.839f, 0.329f),
                markerLineColor = new Color(1f, 0f, 0f, 1f),
                eventMarkerColor = new Color(1f, 0f, 0f, 0.85f),
                eventAreaBackground = new Color(0.149f, 0.149f, 0.149f, 0.75f)
            }
        },
        { PresetSchemes.Yellow, new ChronoscopeColorSet()
            {
                areaColor = new Color(0.325f, 0.0274f, 0.0274f, 1f),
                headerColor = new Color(0.945f, 1f, 0f, 1f),
                valueColor = new Color(1f, 0.972f, 0f, 1f),
                majorColor = new Color(1f, 0.972f, 0f, 1f),
                minorColor = new Color(0.721f, 0.682f, 0.184f, 1f),
                markerBackgroundColor = new Color(0.984f, 1f, 0f, 0.329f),
                markerLineColor = new Color(1f, 0f, 0f, 1f),
                eventMarkerColor = new Color(1f, 0f, 0f, 0.85f),
                eventAreaBackground = new Color(0.325f, 0.0274f, 0.0274f, 0.75f)
            }
        },
        { PresetSchemes.DarkGreen, new ChronoscopeColorSet()
            {
                areaColor = new Color(0.149f, 0.149f, 0.149f, 1f),
                headerColor = new Color(0.239f, 0.874f, 0.321f, 1f),
                valueColor = new Color(0.749f, 0.749f, 0.749f, 1f),
                majorColor = new Color(0.749f, 0.749f, 0.749f, 1f),
                minorColor = new Color(0.501f, 0.501f, 0.501f, 1f),
                markerBackgroundColor = new Color(0.839f, 0.839f, 0.839f, 0.329f),
                markerLineColor = new Color(1f, 0f, 0f, 1f),
                eventMarkerColor = new Color(1f, 0f, 0f, 0.85f),
                eventAreaBackground = new Color(0.149f, 0.149f, 0.149f, 0.75f)
            }
        },
        { PresetSchemes.GreenScreen, new ChronoscopeColorSet()
            {
                areaColor = new Color(0f, 0.066f, 0.011f, 1f),
                headerColor = new Color(0f, 0.941f, 0.12f, 1f),
                valueColor = new Color(0f, 0.858f, 0.0431f, 1f),
                majorColor = new Color(0f, 0.721f, 0.0352f, 1f),
                minorColor = new Color(0f, 0.411f, 0.0196f, 1f),
                markerBackgroundColor = new Color(0.090f, 0.772f, 0f, 0.329f),
                markerLineColor = new Color(0.047f, 0.662f, 0.050f, 1f),
                eventMarkerColor = new Color(0f, 0.85f, 0.00f, 0.85f),
                eventAreaBackground = new Color(0f, 0.066f, 0.011f, 0.75f)
            }
        },
        { PresetSchemes.InfraRed, new ChronoscopeColorSet()
            {
                areaColor = new Color(0.152f, 0f, 0f, 1f),
                headerColor = new Color(0.941f, 0f, 0f, 1f),
                valueColor = new Color(0.858f, 0f, 0f, 1f),
                majorColor = new Color(0.858f, 0f, 0f, 1f),
                minorColor = new Color(0.560f, 0f, 0f, 1f),
                markerBackgroundColor = new Color(0.772f, 0f, 0f, 0.329f),
                markerLineColor = new Color(1f, 0f, 0f, 1f),
                eventMarkerColor = new Color(1f, 0f, 0f, 0.85f),
                eventAreaBackground = new Color(0.152f, 0f, 0f, 0.75f)
            }
        },
    };
    } 
}
