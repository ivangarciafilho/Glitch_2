#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase
{
    [Serializable]
    internal class Filter
    {
        public string Name;
        public float Vibrance;
        public float Saturation;
        public float Temperature;
        public float Tint;
        public float Hue;
        public float Gamma;
        public float Brightness;
        public float Exposure;
        public float Contrast;
        public float Black;
        public float White;
        public float Shadow;
        public float Highlight;
        public float Bloom;
        public float Sharpen;
        public float Clarity;
        public float Smooth;
        public float Blur;
        public float Grain;
        public float Pixelate;
        public float SobelEdge;

        public Filter(string name, float vibrance, float saturation, float temperature, float tint, float hue, float gamma, float brightness, float exposure, float contrast, float black, float white, float shadow, float highlight, float bloom, float sharpen, float clarity, float smooth, float blur, float grain, float pixelate, float sobelEdge)
        {
            Name = name;
            Vibrance = vibrance;
            Saturation = saturation;
            Temperature = temperature;
            Tint = tint;
            Hue = hue;
            Gamma = gamma;
            Brightness = brightness;
            Exposure = exposure;
            Contrast = contrast;
            Black = black;
            White = white;
            Shadow = shadow;
            Highlight = highlight;
            Bloom = bloom;
            Sharpen = sharpen;
            Clarity = clarity;
            Smooth = smooth;
            Blur = blur;
            Grain = grain;
            Pixelate = pixelate;
            SobelEdge = sobelEdge;
        }

        public void ApplyTo(PicEase_Image manager)
        {
            manager.VibranceMagnitude = Vibrance;
            manager.SaturationMagnitude = Saturation;
            manager.TemperatureMagnitude = Temperature;
            manager.TintMagnitude = Tint;
            manager.HueMagnitude = Hue;
            manager.GammaMagnitude = Gamma;
            manager.BrightnessMagnitude = Brightness;
            manager.ExposureMagnitude = Exposure;
            manager.ContrastMagnitude = Contrast;
            manager.BlackMagnitude = Black;
            manager.WhiteMagnitude = White;
            manager.ShadowMagnitude = Shadow;
            manager.HighlightMagnitude = Highlight;
            manager.BloomMagnitude = Bloom;
            manager.SharpenMagnitude = Sharpen;
            manager.ClarityMagnitude = Clarity;
            manager.SmoothMagnitude = Smooth;
            manager.BlurMagnitude = Blur;
            manager.GrainMagnitude = Grain;
            manager.PixelateMagnitude = Pixelate;
            manager.SobelEdgeMagnitude = SobelEdge;
        }
    }

    internal static class PicEase_Filters
    {
        #region Properties
        [Serializable]
        private class FilterListWrapper
        {
            public List<Filter> filters;

            public FilterListWrapper(List<Filter> filters)
            {
                this.filters = filters;
            }
        }

        private static readonly Dictionary<string, Filter> builtinFilters = new()
        {
            { "Aqua", new("Aqua", 20f, 30f, -80f, 0f, 0f, 0f, 10f, 5f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Black and White I", new("Black and White I", 0f, -100f, 0f, 0f, 0f, 0f, 20f, 5f, 5f, 0f, 10f, -2f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Black and White II", new("Black and White II", 0f, -100f, 0f, 0f, 0f, 0f, 10f, 5f, -10f, 5f, 5f, -5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Black and White III", new("Black and White III", -50f, -85f, 0f, 0f, 0f, 0f, 0f, 20f, 6f, 4f, 10f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Black and White IV", new("Black and White IV", 0f, -100f, 0f, 0f, 0f, 0f, 10f, 5f, 10f, 5f, 2f, -5f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Black and White V", new("Black and White V", 0f, -100f, 0f, 0f, 0f, -30f, 10f, 5f, 15f, 5f, 2f, -5f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Bleach", new("Bleach", -20f, -5f, 0f, 0f, 0f, 0f, 5f, 20f, -5f, -10f, 60f, 50f, 5f, 0f, 0f, 0f, 50f, 0f, 0f, 0f, 0f) },
            { "Cinematic I", new("Cinematic I", -60f, -20f, -15f, 0f, 0f, 0f, 5f, 65f, -10f, -10f, -20f, -40f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Cinematic II", new("Cinematic II", -50, -5f, -100f, 0f, 0f, 0f, 5f, 50f, -25f, -10f, -20f, -40f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Cinematic III", new("Cinematic III", -70f, -10f, 10f, 0f, 0f, 0f, 0f, 0f, -15f, 15f, 15f, -55f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Cinematic IV", new("Cinematic IV", -30f, -10f, -80f, 20f, 0f, 0f, -10f, 5f, 40f, -5f, 15f, -50f, 10f, 0f, 10f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Cinematic V", new("Cinematic V", -40f, -50f, 40f, 0f, 0f, 0f, 5f, 30f, -20f, -5f, -20f, -45f, 10f, 0f, 10f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Cinematic VI", new("Cinematic VI", 25f, 10f, -40f, -20f, 10f, 0f, -25f, 5f, 40f, -5f, 15f, -50f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Cinematic VII", new("Cinematic VII", -50, -5f, -10f, 0f, 0f, -15f, 5f, 50f, -25f, -10f, -20f, -40f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Contrast I", new("Contrast I", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 15f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Contrast II", new("Contrast II", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 5f, 50f, 0f, 0f, -5f, 30f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Clear", new("Clear", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 40f, 10f, 0f, 0f, 5f, 0f, 0f, 20f, 5f, 40f, 0f, 0f, 0f, 15f) },
            { "Cool", new("Cool", -10f, 20f, -40f, 0f, 0f, 0f, 5f, 5f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Cool Blues", new("Cool Blues", -10f, 10f, -100f, 0f, 0f, 0f, 5f, -20f, 50f, -5f, 45f, 30f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Dark I", new("Dark I", -100f, 0f, -20f, 0f, 0f, 0f, 0f, 20f, 10f, 5f, 10f, -60f, 2f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Dark II", new("Dark II", -100f, -30f, 0f, 0f, 0f, 0f, -15f, -5f, 10f, 10f, 20f, -80f, -25f, 0f, 0f, 0f, 0f, 10f, -20f, 0f, 0f) },
            { "Film I", new("Film I", -15f, -30f, 50f, 0f, 5f, 0f, 5f, -10f, -10f, 8f, 8f, 15f, 0f, 0f, 0f, 0f, 20f, 0f, 20f, 0f, 0f) },
            { "Film II", new("Film II", -20f, -20f, -10f, 5f, 0f, 0f, -5f, 10f, -30f, -20f, 15f, -40f, -5f, 0f, -10f, 0f, 0f, 0f, -30f, 0f, 0f) },
            { "Film III", new("Film III", 0f, -80f, 30f, -5f, 0f, 0f, 0f, 10f, -25f, -5f, -10f, 0f, -5f, 0f, 0f, 0f, 0f, 0f, 30f, 0f, 0f) },
            { "Film IV", new("Film IV", -20f, -20f, 35f, 5f, 0f, -5f, -5f, 10f, -30f, -5f, 15f, -40f, -5f, 0f, -10f, 0f, 0f, 0f, -60f, 0f, 0f) },
            { "Forest I", new("Forest I", 10f, 30f, 25f, 12f, 0f, 0f, 0f, 20f, 25f, 0f, 10f, -10f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Forest II", new("Forest II", 30f, 15f, -20f, 10f, 0f, 0f, 5f, 10f, 25f, 0f, 5f, 5f, 20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Horror I", new("Horror I", -25f, -35f, 10f, 0f, 0f, 0f, -20f, -65f, 55f, 0f, 70f, -40f, 0f, 0f, 0f, 0f, 0f, 0f, -70f, 0f, 0f) },
            { "Horror II", new("Horror II", -25f, -35f, 10f, 0f, 0f, 0f, 0f, -65f, 55f, 0f, 70f, -40f, 0f, -25f, 0f, 0f, 0f, 0f, -70f, 0f, 0f) },
            { "Horror III", new("Horror III", -25f, -35f, 10f, 0f, 0f, 0f, 0f, -65f, 30f, 5f, 60f, -50f, 0f, -25f, 0f, 0f, 0f, 0f, 100f, 0f, -20f) },
            { "Night Vision", new("Night Vision", -25f, 0f, 0f, 100f, 0f, 0f, -15f, 85f, 25f, -20f, 5f, 10f, 2f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Pixel I", new("Pixel I", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -100f, 0f) },
            { "Pixel II", new("Pixel II", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -50f, 0f) },
            { "Pixel III", new("Pixel III", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -25f, 0f) },
            { "Retro I", new("Retro I", 50f, 50f, 50f, 0f, 0f, 0f, 0f, 0f, -5f, 40f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -25f, 100f) },
            { "Retro II", new("Retro II", 50f, 50f, 50f, 0f, 0f, 0f, 0f, 0f, -5f, 45f, 100f, -60f, 0f, 0f, 0f, 0f, 0f, -50f, 0f, -10f, 100f) },
            { "Sepia I", new("Sepia I", -10f, -50f, 100f, 0f, 0f, 0f, 0f, 25f, 15f, -5f, 2f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Sepia II", new("Sepia II", -15f, -60f, 100f, -5f, 0f, 0f, 0f, 30f, 30f, 0f, 10f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Sketch I", new("Sketch I", 0f, -100f, 0f, 0f, 0f, 0f, -10f, -10f, 100f, 15f, 100f, 100f, 0f, 2f, -10f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Sketch II", new("Sketch II", 0f, -100f, 0f, 0f, 0f, 0f, -10f, -10f, 100f, 40f, 90f, 100f, -10f, 0f, -10f, 5f, 0f, -50f, 0f, 0f, 0f) },
            { "Sketch III", new("Sketch III", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 100f, 0f, -100f, 100f, 0f, 0f, 0f, 0f, 0f, 80f, 00f, 0f, 0f, 100f) },
            { "Sketch IV", new("Sketch IV", 0f, -100f, 0f, 0f, 0f, 0f, -10f, 0f, -100f, 0f, 0f, -100f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -90f) },
            { "Soft Blur", new("Soft Blur", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 10f, -80f, 0f, 0f, 0f) },
            { "Sunset I", new("Sunset I", 25f, 15f, 75f, -10f, 0f, 0f, 20f, 30f, 15f, 0f, -5f, 10f, 0f, 5f, 0f, 0f, 30f, 0f, 0f, 0f, 0f) },
            { "Sunset II", new("Sunset II", 30f, 20f, 80f, -15f, 5f, 0f, 25f, 35f, 20f, -5f, -10f, 15f, 0f, 5f, 0f, 0f, 40f, 00f, 0f, 0f, 0f) },
            { "Underworld", new("Underworld", 20f, -25f, -60f, -10f, -25f, 0f, -15f, 65f, 65f, 5f, 15f, 30f, 5f, -30f, 0f, -20f, 5f, 0f, 0f, 0f, 0f) },
            { "Vintage I", new("Vintage I", 0f, -40f, 50f, 0f, 0f, 0f, 0f, 30f, 10f, 0f, 10f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 20f, 0f, 0f) },
            { "Vintage II", new("Vintage II", -10f, -35f, 45f, 0f, 0f, 0f, 0f, 25f, 8f, 0f, 8f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 18f, 0f, 0f) },
            { "Vivid I", new("Vivid I", 50f, 50f, 0f, 0f, 0f, 0f, 10f, 20f, 5f, 0f, 0f, 5f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Vivid II", new("Vivid II", 40f, 80f, 0f, 0f, 0f, 0f, 0f, 80f, 0f, 0f, 0f, 5f, 0f, 0f, 0f, 0f, 10f, 0f, 0f, 0f, -10f) },
            { "Vivid III", new("Vivid III", 40f, 50f, 10f, 0f, 0f, 0f, 15f, 15f, 15f, 0f, 0f, 0f, 0f, 0f, 5f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Vivid IV", new("Vivid IV", 30f, 40f, 5f, 0f, 0f, 0f, 10f, 10f, 20f, 0f, 0f, 0f, 0f, 0f, 10f, 0f, 0f, 0f, 0f, 0f, 0f) },
            { "Warm", new("Warm", 10f, 15f, 40f, 0f, 0f, 0f, 10f, 10f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f) },
        };

        public static readonly Dictionary<string, List<string>> FilterGroups = new()
        {
            {
                "Custom", new()
            },
            {
                "Black & White", new() { "Black and White I", "Black and White II", "Black and White III", "Black and White IV", "Black and White V" }
            },
            {
                "Cinematic", new() { "Cinematic I", "Cinematic II", "Cinematic III", "Cinematic IV", "Cinematic V", "Cinematic VI", "Cinematic VII" }
            },
            {
                "Film", new() { "Film I", "Film II", "Film III", "Film IV" }
            },
            {
                "Horror", new() { "Dark I", "Dark II", "Horror I", "Horror II", "Horror III", "Underworld", }
            },
            {
                "Nature", new() { "Forest I", "Forest II", "Sunset I", "Sunset II" }
            },
            {
                "Retro", new() { "Pixel I", "Pixel II", "Pixel III", "Retro I", "Retro II" }
            },
            {
                "Sketch", new() { "Sketch I", "Sketch II", "Sketch III", "Sketch IV" }
            },
            {
                "Tuning", new() { "Contrast I", "Contrast II", "Clear", "Soft Blur", }
            },
            {
                "Various", new() { "Aqua", "Bleach", "Cool", "Cool Blues", "Night Vision", "Warm", }
            },
            {
                "Vintage", new() { "Sepia I", "Sepia II", "Vintage I", "Vintage II" }
            },
            {
                "Vivid", new() { "Vivid I", "Vivid II", "Vivid III", "Vivid IV", }
            },
        };
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadSettings();
        }
        #endregion

        #region Accessors
        public static List<Filter> CustomFilters { get; private set; } = new();
        #endregion

        #region Methods
        public static void ApplyBuiltInFilter(PicEase_Image manager, string filterName)
        {
            if (builtinFilters.TryGetValue(filterName, out Filter filter))
            {
                filter.ApplyTo(manager);
                manager.ApplyAdjustments();
            }
            else
            {
                Debug.LogWarning($"Filter '{filterName}' not found.");
            }
        }

        public static void ApplyCustomFilter(PicEase_Image manager, Filter customFilter)
        {
            if (customFilter != null)
            {
                customFilter.ApplyTo(manager);
                manager.ApplyAdjustments();
            }
            else
            {
                Debug.LogWarning("Filter is null.");
            }
        }
        #endregion

        #region Save and Load
        public static void SaveSettings()
        {
            string dataFilePath = PicEase_File.GetSavedDataFilePath(PicEase_Constants.CustomFilterTextFileName);
            FilterListWrapper wrapper = new(CustomFilters);
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(dataFilePath, json);
            AssetDatabase.Refresh();
        }

        public static void LoadSettings()
        {
            string dataFilePath = PicEase_File.GetSavedDataFilePath(PicEase_Constants.CustomFilterTextFileName);
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                FilterListWrapper wrapper = JsonUtility.FromJson<FilterListWrapper>(json);
                CustomFilters = wrapper.filters ?? new();
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private static void SetDefaultSettings()
        {
            CustomFilters = new();
        }
        #endregion
    }
}
#endif