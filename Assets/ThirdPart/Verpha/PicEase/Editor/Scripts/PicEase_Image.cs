#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Verpha.PicEase
{
    internal class PicEase_Image : IDisposable
    {
        #region Properties
        #region Core
        private readonly PicEase_Enums.BehaviourMode behaviourMode;
        private readonly Texture2D originalImage;
        private readonly ComputeShader computeShader;
        private readonly EditorWindow editorWindow;
        private readonly TextureImporterSettings originalImageImporterSettings;
        private readonly int originalMaxTextureSize;
        private readonly TextureImporterCompression originalTextureCompression;

        private RenderTexture resultTexture;
        private Texture2D readbackTexture;

        private readonly int mainKernelHandle;
        private readonly int threadGroupsX;
        private readonly int threadGroupsY;

        private bool isReadbackInProgress = false;
        #endregion

        #region Adjustments
        public float VibranceMagnitude { get; set; } = 0f;
        public float SaturationMagnitude { get; set; } = 0f;
        public float TemperatureMagnitude { get; set; } = 0f;
        public float TintMagnitude { get; set; } = 0f;
        public float HueMagnitude { get; set; } = 0f;
        public float GammaMagnitude { get; set; } = 0f;
        public float BrightnessMagnitude { get; set; } = 0f;
        public float ExposureMagnitude { get; set; } = 0f;
        public float ContrastMagnitude { get; set; } = 0f;
        public float BlackMagnitude { get; set; } = 0f;
        public float WhiteMagnitude { get; set; } = 0f;
        public float ShadowMagnitude { get; set; } = 0f;
        public float HighlightMagnitude { get; set; } = 0f;
        public float BloomMagnitude { get; set; } = 0f;
        public float SharpenMagnitude { get; set; } = 0f;
        public float ClarityMagnitude { get; set; } = 0f;
        public float SmoothMagnitude { get; set; } = 0f;
        public float BlurMagnitude { get; set; } = 0f;
        public float GrainMagnitude { get; set; } = 0f;
        public float PixelateMagnitude { get; set; } = 0f;
        public float SobelEdgeMagnitude { get; set; } = 0f;

        private float prevVibranceMagnitude;
        private float prevSaturationMagnitude;
        private float prevTemperatureMagnitude;
        private float prevTintMagnitude;
        private float prevHueMagnitude;
        private float prevGammaMagnitude;
        private float prevBrightnessMagnitude;
        private float prevExposureMagnitude;
        private float prevContrastMagnitude;
        private float prevBlackMagnitude;
        private float prevWhiteMagnitude;
        private float prevShadowMagnitude;
        private float prevHighlightMagnitude;
        private float prevBloomMagnitude;
        private float prevSharpenMagnitude;
        private float prevClarityMagnitude;
        private float prevSmoothMagnitude;
        private float prevBlurMagnitude;
        private float prevGrainMagnitude;
        private float prevPixelateMagnitude;
        private float prevSobelEdgeMagnitude;
        #endregion

        #region Colorize
        private const int maxColorReplacements = 20;

        private readonly List<Color> sourceColors = new();
        private readonly List<Color> targetColors = new();

        private bool hasTemporaryColorReplacement = false;
        private Color temporarySourceColor;
        private Color temporaryTargetColor;

        public float ColorizeThreshold { get; set; } = 0.1f;
        public float ColorizeSmoothness { get; set; } = 0.1f;

        private int prevEffectiveColorReplacementCount;
        private float prevColorizeThreshold;
        private float prevColorizeSmoothness;
        #endregion

        #region LUT
        public Texture3D CustomLUT = null;
        public string LUTName { get; set; } = null;
        public bool UseLUT { get; set; } = false;
        public float LUTBlend { get; set; } = 0f;

        private string prevLUTName = null;
        private bool prevUseLUT = false;
        private float prevLUTBlend = 0f;

        private int lutSize = 32;

        private static readonly Dictionary<string, Texture3D> defaultLUTs = new()
        {
            { "None", PicEase_Texture.FallbackTexture3D },
            { "Adventure I", PicEase_Resources.LUTs.AdventureI },
            { "Adventure II", PicEase_Resources.LUTs.AdventureII },
            { "Adventure III", PicEase_Resources.LUTs.AdventureIII },
            { "Ambient I", PicEase_Resources.LUTs.AmbientI },
            { "Ambient II", PicEase_Resources.LUTs.AmbientII },
            { "Cinema I", PicEase_Resources.LUTs.CinemaI },
            { "Cinema II", PicEase_Resources.LUTs.CinemaII },
            { "Cinema III", PicEase_Resources.LUTs.CinemaIII },
            { "Cinema IV", PicEase_Resources.LUTs.CinemaIV },
            { "Cinematic I", PicEase_Resources.LUTs.CinematicI },
            { "Cinematic II", PicEase_Resources.LUTs.CinematicII },
            { "Cinematic III", PicEase_Resources.LUTs.CinematicIII },
            { "Cinematic IV", PicEase_Resources.LUTs.CinematicIV },
            { "HDR", PicEase_Resources.LUTs.HDR },
            { "Hollywood I", PicEase_Resources.LUTs.HollywoodI },
            { "Hollywood II", PicEase_Resources.LUTs.HollywoodII },
            { "Noir I", PicEase_Resources.LUTs.NoirI },
            { "Noir II", PicEase_Resources.LUTs.NoirII },
            { "Teal", PicEase_Resources.LUTs.Teal },
            { "Vintage I", PicEase_Resources.LUTs.VintageI },
        };
        #endregion
        #endregion

        #region Constructor
        public PicEase_Image(Texture2D originalImage, ComputeShader shader, EditorWindow window, TextureImporterSettings originalImporterSettings, int originalMaxTextureSize, TextureImporterCompression originalTextureCompression)
        {
            this.behaviourMode = PicEase_Settings.ImageEditorBehaviourMode;
            this.originalImage = originalImage != null ? originalImage : throw new ArgumentNullException(nameof(originalImage));
            this.computeShader = shader != null ? shader : throw new ArgumentNullException(nameof(shader));
            this.editorWindow = window;
            this.originalImageImporterSettings = originalImporterSettings;
            this.originalMaxTextureSize = originalMaxTextureSize;
            this.originalTextureCompression = originalTextureCompression;

            resultTexture = new(originalImage.width, originalImage.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            resultTexture.Create();
            resultTexture.filterMode = originalImage.filterMode;

            Graphics.Blit(originalImage, resultTexture);

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                readbackTexture = new(resultTexture.width, resultTexture.height, TextureFormat.RGBAFloat, false)
                {
                    filterMode = originalImage.filterMode
                };
            }

            mainKernelHandle = computeShader.FindKernel("CSMain");
            computeShader.SetTexture(mainKernelHandle, "InputTexture", originalImage);
            computeShader.SetTexture(mainKernelHandle, "Result", resultTexture);

            PicEase_Enums.ThreadGroupSize threadGroupSize = PicEase_Settings.ImageEditorThreadGroupSize;
            float divisionFactor = threadGroupSize switch
            {
                PicEase_Enums.ThreadGroupSize.EightByEight => 8.0f,
                PicEase_Enums.ThreadGroupSize.SixteenBySixteen => 16.0f,
                PicEase_Enums.ThreadGroupSize.ThirtyTwoByThirtyTwo => 32.0f,
                _ => 16.0f,
            };
            threadGroupsX = Mathf.CeilToInt(originalImage.width / divisionFactor);
            threadGroupsY = Mathf.CeilToInt(originalImage.height / divisionFactor);

            ResetComputeShaderParameters();
            CacheAdjustmentsParameters();
            CacheColorizeParameters();
            CacheLUTParameters();
        }
        #endregion

        #region Accessors
        public Texture GetResultTexture()
        {
            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                return readbackTexture != null ? readbackTexture : originalImage;
            }
            return resultTexture;
        }

        public Texture2D GetFinalTexture2D()
        {
            ApplyAdjustments();
            ApplyColorize();
            ApplyLUT();

            RenderTexture tempRT = RenderTexture.GetTemporary(resultTexture.width, resultTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(resultTexture, tempRT);

            Texture2D exportTexture = new(resultTexture.width, resultTexture.height, originalImage.format, false)
            {
                alphaIsTransparency = originalImage.alphaIsTransparency,
                wrapMode = originalImage.wrapMode,
                filterMode = originalImage.filterMode,
            };

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = tempRT;
            exportTexture.ReadPixels(new(0, 0, tempRT.width, tempRT.height), 0, 0);
            exportTexture.Apply();
            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(tempRT);

            return exportTexture;
        }
        #endregion

        #region Methods
        #region Adjustments
        public void ApplyAdjustments()
        {
            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                if (isReadbackInProgress) return;
                isReadbackInProgress = true;
            }

            SetShaderFloatIfChanged("VibranceMagnitude", VibranceMagnitude, ref prevVibranceMagnitude);
            SetShaderFloatIfChanged("SaturationMagnitude", SaturationMagnitude, ref prevSaturationMagnitude);
            SetShaderFloatIfChanged("TemperatureMagnitude", TemperatureMagnitude, ref prevTemperatureMagnitude);
            SetShaderFloatIfChanged("TintMagnitude", TintMagnitude, ref prevTintMagnitude);
            SetShaderFloatIfChanged("HueMagnitude", HueMagnitude, ref prevHueMagnitude);
            SetShaderFloatIfChanged("GammaMagnitude", GammaMagnitude, ref prevGammaMagnitude);
            SetShaderFloatIfChanged("BrightnessMagnitude", BrightnessMagnitude, ref prevBrightnessMagnitude);
            SetShaderFloatIfChanged("ExposureMagnitude", ExposureMagnitude, ref prevExposureMagnitude);
            SetShaderFloatIfChanged("ContrastMagnitude", ContrastMagnitude, ref prevContrastMagnitude);
            SetShaderFloatIfChanged("BlackMagnitude", BlackMagnitude, ref prevBlackMagnitude);
            SetShaderFloatIfChanged("WhiteMagnitude", WhiteMagnitude, ref prevWhiteMagnitude);
            SetShaderFloatIfChanged("ShadowMagnitude", ShadowMagnitude, ref prevShadowMagnitude);
            SetShaderFloatIfChanged("HighlightMagnitude", HighlightMagnitude, ref prevHighlightMagnitude);
            SetShaderFloatIfChanged("PixelateMagnitude", PixelateMagnitude, ref prevPixelateMagnitude);
            SetShaderFloatIfChanged("SharpenMagnitude", SharpenMagnitude, ref prevSharpenMagnitude);
            SetShaderFloatIfChanged("ClarityMagnitude", ClarityMagnitude, ref prevClarityMagnitude);
            SetShaderFloatIfChanged("BloomMagnitude", BloomMagnitude, ref prevBloomMagnitude);
            SetShaderFloatIfChanged("SmoothMagnitude", SmoothMagnitude, ref prevSmoothMagnitude);
            SetShaderFloatIfChanged("BlurMagnitude", BlurMagnitude, ref prevBlurMagnitude);
            SetShaderFloatIfChanged("GrainMagnitude", GrainMagnitude, ref prevGrainMagnitude);
            SetShaderFloatIfChanged("SobelEdgeMagnitude", SobelEdgeMagnitude, ref prevSobelEdgeMagnitude);

            computeShader.Dispatch(mainKernelHandle, threadGroupsX, threadGroupsY, 1);

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                AsyncGPUReadback.Request(resultTexture, 0, TextureFormat.RGBAFloat, OnCompleteReadback);
            }
            else
            {
                editorWindow.Repaint();
            }
        }
        #endregion

        #region Colorize
        public void ApplyColorize()
        {
            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                if (isReadbackInProgress) return;
                isReadbackInProgress = true;
            }

            int effectiveColorReplacementCount = sourceColors.Count + (hasTemporaryColorReplacement ? 1 : 0);

            SetShaderIntIfChanged("ColorReplacementCount", effectiveColorReplacementCount, ref prevEffectiveColorReplacementCount);
            SetShaderFloatIfChanged("ColorizeThreshold", ColorizeThreshold, ref prevColorizeThreshold);
            SetShaderFloatIfChanged("ColorizeSmoothness", ColorizeSmoothness, ref prevColorizeSmoothness);

            Vector4[] sourceColorArray = new Vector4[maxColorReplacements];
            Vector4[] targetColorArray = new Vector4[maxColorReplacements];

            for (int i = 0; i < sourceColors.Count; i++)
            {
                sourceColorArray[i] = sourceColors[i];
                targetColorArray[i] = targetColors[i];
            }

            if (hasTemporaryColorReplacement && sourceColors.Count < maxColorReplacements)
            {
                sourceColorArray[sourceColors.Count] = temporarySourceColor;
                targetColorArray[sourceColors.Count] = temporaryTargetColor;
            }

            computeShader.SetVectorArray("SourceColors", sourceColorArray);
            computeShader.SetVectorArray("TargetColors", targetColorArray);

            computeShader.Dispatch(mainKernelHandle, threadGroupsX, threadGroupsY, 1);

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                AsyncGPUReadback.Request(resultTexture, 0, TextureFormat.RGBAFloat, OnCompleteReadback);
            }
            else
            {
                editorWindow.Repaint();
            }
        }

        public void AddColorReplacement(Color source, Color target)
        {
            if (sourceColors.Count < maxColorReplacements)
            {
                sourceColors.Add(source);
                targetColors.Add(target);
            }
            else
            {
                Debug.LogWarning("Maximum (Total: " + maxColorReplacements + ")" + " number of color replacements reached.");
            }
        }

        public void RemoveColorReplacementAt(int index)
        {
            if (index >= 0 && index < sourceColors.Count)
            {
                sourceColors.RemoveAt(index);
                targetColors.RemoveAt(index);
            }
        }

        public void SetTemporaryColorReplacement(Color source, Color target)
        {
            hasTemporaryColorReplacement = true;
            temporarySourceColor = source;
            temporaryTargetColor = target;
        }

        public void ClearTemporaryColorReplacement()
        {
            hasTemporaryColorReplacement = false;
        }

        public void UpdateColorReplacementAt(int index, Color source, Color target)
        {
            if (index >= 0 && index < sourceColors.Count)
            {
                sourceColors[index] = source;
                targetColors[index] = target;
            }
        }

        public int GetColorReplacementCount()
        {
            return sourceColors.Count;
        }

        public Color GetSourceColor(int index)
        {
            return sourceColors[index];
        }

        public Color GetTargetColor(int index)
        {
            return targetColors[index];
        }

        public List<Color> GetRandomColors(int count)
        {
            List<Color> randomColors = new(count);

            for (int i = 0; i < count; i++)
            {
                int x = UnityEngine.Random.Range(0, originalImage.width);
                int y = UnityEngine.Random.Range(0, originalImage.height);
                randomColors.Add(originalImage.GetPixel(x, y));
            }
            return randomColors;
        }

        #endregion

        #region LUT
        public void ApplyLUT()
        {
            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                if (isReadbackInProgress) return;
                isReadbackInProgress = true;
            }

            bool lutNameChanged = (LUTName != prevLUTName);
            bool lutBlendChanged = (Mathf.Abs(LUTBlend - prevLUTBlend) > 1e-6f);
            bool useLUTChanged = (UseLUT != prevUseLUT);

            if (!lutNameChanged && !lutBlendChanged && !useLUTChanged)
            {
                if (behaviourMode == PicEase_Enums.BehaviourMode.Synced) isReadbackInProgress = false;
                return;
            }

            if (!UseLUT || string.IsNullOrEmpty(LUTName) || !defaultLUTs.ContainsKey(LUTName))
            {
                computeShader.SetInt("UseLUT", 0);
                SetShaderFloatIfChanged("LUTBlend", 0f, ref prevLUTBlend);
                computeShader.SetTexture(mainKernelHandle, "LUTTexture", PicEase_Texture.FallbackTexture3D);
            }
            else
            {
                Texture3D currentTexture = defaultLUTs[LUTName];
                computeShader.SetInt("UseLUT", 1);
                computeShader.SetInt("LUTSize", lutSize);
                SetShaderFloatIfChanged("LUTBlend", LUTBlend, ref prevLUTBlend);
                computeShader.SetTexture(mainKernelHandle, "LUTTexture", currentTexture);
            }

            computeShader.Dispatch(mainKernelHandle, threadGroupsX, threadGroupsY, 1);

            CacheLUTParameters();

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                AsyncGPUReadback.Request(resultTexture, 0, TextureFormat.RGBAFloat, OnCompleteReadback);
            }
            else
            {
                editorWindow.Repaint();
            }
        }

        public void SelectLUT(string lutName, float blend = 1f)
        {
            if (!defaultLUTs.ContainsKey(lutName))
            {
                Debug.LogWarning($"LUT '{lutName}' not found in defaultLUTs.");
                return;
            }

            if (lutName != "None" && LUTName != lutName && Mathf.Approximately(blend, 0f))
            {
                blend = 1f;
            }

            Texture3D selectedTex = defaultLUTs[lutName];
            int dimension = selectedTex.width;
            if (selectedTex.height != dimension || selectedTex.depth != dimension)
            {
                Debug.LogWarning($"LUT '{lutName}' is not a perfect cube, but using dimension={dimension} anyway.");
            }

            LUTName = lutName;
            UseLUT = true;
            lutSize = dimension;
            LUTBlend = Mathf.Clamp01(blend);
        }

        public void SetCustomUserLUT(Texture3D customTexture)
        {
            if (defaultLUTs.ContainsKey("Custom LUT"))
            {
                defaultLUTs.Remove("Custom LUT");
            }

            if (customTexture == null) return;

            defaultLUTs.Add("Custom LUT", customTexture);
        }

        public static Texture3D GetDefaultLUT(string lutName)
        {
            if (defaultLUTs.TryGetValue(lutName, out Texture3D lut))
            {
                return lut;
            }
            return null;
        }

        public static List<string> GetAvailableLUTs()
        {
            return new List<string>(defaultLUTs.Keys);
        }

        public void ResetLUT()
        {
            CustomLUT = null;
            if (defaultLUTs.ContainsKey("Custom LUT")) defaultLUTs.Remove("Custom LUT");

            LUTName = "None";
            UseLUT = false;
            LUTBlend = 0f;

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                if (isReadbackInProgress) return;
                isReadbackInProgress = true;
            }

            computeShader.SetInt("UseLUT", 0);
            computeShader.SetInt("LUTSize", 0);
            computeShader.SetFloat("LUTBlend", 0f);
            computeShader.SetTexture(mainKernelHandle, "LUTTexture", PicEase_Texture.FallbackTexture3D);

            computeShader.Dispatch(mainKernelHandle, threadGroupsX, threadGroupsY, 1);

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                AsyncGPUReadback.Request(resultTexture, 0, TextureFormat.RGBAFloat, OnCompleteReadback);
            }
            else
            {
                editorWindow.Repaint();
            }
        }

        private void ResetLUTValues()
        {
            CustomLUT = null;
            LUTName = "None";
            UseLUT = false;
            lutSize = 0;
            LUTBlend = 0f;
        }
        #endregion

        #region Operations
        private void SetShaderFloatIfChanged(string propertyName, float currentValue, ref float previousValue)
        {
            if (currentValue != previousValue)
            {
                computeShader.SetFloat(propertyName, currentValue);
                previousValue = currentValue;
            }
        }

        private void SetShaderIntIfChanged(string propertyName, int currentValue, ref int previousValue)
        {
            if (currentValue != previousValue)
            {
                computeShader.SetInt(propertyName, currentValue);
                previousValue = currentValue;
            }
        }

        private void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            isReadbackInProgress = false;

            if (request.hasError)
            {
                Debug.LogError("GPU readback error detected.");
                return;
            }

            readbackTexture.LoadRawTextureData(request.GetData<byte>());
            readbackTexture.Apply();

            EditorApplication.delayCall += () =>
            {
                editorWindow.Repaint();
            };
        }

        public void SetTextureImporterSettings(string relativePath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
            if (importer != null)
            {
                if (originalImageImporterSettings != null)
                {
                    importer.SetTextureSettings(originalImageImporterSettings);
                    importer.maxTextureSize = originalMaxTextureSize;
                    importer.textureCompression = originalTextureCompression;
                    importer.SaveAndReimport();
                }
            }
        }
        #endregion

        #region Reset and Cache
        public void ResetImageEffects()
        {
            ResetMagnitudeValues();
            ResetComputeShaderParameters();
            ApplyAdjustments();
            ResetColorizeValues();
            ApplyColorize();
            ResetLUT();
            Graphics.Blit(originalImage, resultTexture);
        }

        private void ResetMagnitudeValues()
        {
            VibranceMagnitude = 0f;
            SaturationMagnitude = 0f;
            TemperatureMagnitude = 0f;
            TintMagnitude = 0f;
            HueMagnitude = 0f;
            GammaMagnitude = 0f;
            BrightnessMagnitude = 0f;
            ExposureMagnitude = 0f;
            ContrastMagnitude = 0f;
            BlackMagnitude = 0f;
            WhiteMagnitude = 0f;
            ShadowMagnitude = 0f;
            HighlightMagnitude = 0f;
            PixelateMagnitude = 0f;
            SharpenMagnitude = 0f;
            ClarityMagnitude = 0f;
            BloomMagnitude = 0f;
            SmoothMagnitude = 0f;
            BlurMagnitude = 0f;
            GrainMagnitude = 0f;
            SobelEdgeMagnitude = 0f;
        }

        private void ResetColorizeValues()
        {
            sourceColors.Clear();
            targetColors.Clear();
            ClearTemporaryColorReplacement();
        }

        private void ResetComputeShaderParameters()
        {
            computeShader.SetFloat("VibranceMagnitude", VibranceMagnitude);
            computeShader.SetFloat("SaturationMagnitude", SaturationMagnitude);
            computeShader.SetFloat("TemperatureMagnitude", TemperatureMagnitude);
            computeShader.SetFloat("TintMagnitude", TintMagnitude);
            computeShader.SetFloat("HueMagnitude", HueMagnitude);
            computeShader.SetFloat("GammaMagnitude", GammaMagnitude);
            computeShader.SetFloat("BrightnessMagnitude", BrightnessMagnitude);
            computeShader.SetFloat("ExposureMagnitude", ExposureMagnitude);
            computeShader.SetFloat("ContrastMagnitude", ContrastMagnitude);
            computeShader.SetFloat("BlackMagnitude", BlackMagnitude);
            computeShader.SetFloat("WhiteMagnitude", WhiteMagnitude);
            computeShader.SetFloat("ShadowMagnitude", ShadowMagnitude);
            computeShader.SetFloat("HighlightMagnitude", HighlightMagnitude);
            computeShader.SetFloat("PixelateMagnitude", PixelateMagnitude);
            computeShader.SetFloat("SharpenMagnitude", SharpenMagnitude);
            computeShader.SetFloat("ClarityMagnitude", ClarityMagnitude);
            computeShader.SetFloat("BloomMagnitude", BloomMagnitude);
            computeShader.SetFloat("SmoothMagnitude", SmoothMagnitude);
            computeShader.SetFloat("BlurMagnitude", BlurMagnitude);
            computeShader.SetFloat("GrainMagnitude", GrainMagnitude);
            computeShader.SetFloat("SobelEdgeMagnitude", SobelEdgeMagnitude);

            computeShader.SetInt("ColorReplacementCount", 0);
            computeShader.SetFloat("ColorizeThreshold", 0f);
            computeShader.SetFloat("ColorizeSmoothness", 0f);
            Vector4[] emptyColorArray = new Vector4[maxColorReplacements];
            computeShader.SetVectorArray("SourceColors", emptyColorArray);
            computeShader.SetVectorArray("TargetColors", emptyColorArray);

            computeShader.SetTexture(mainKernelHandle, "LUTTexture", PicEase_Texture.FallbackTexture3D);
            computeShader.SetInt("UseLUT", 0);
            computeShader.SetInt("LUTSize", 0);
            computeShader.SetFloat("LUTBlend", 0f);
        }

        private void CacheAdjustmentsParameters()
        {
            prevVibranceMagnitude = VibranceMagnitude;
            prevSaturationMagnitude = SaturationMagnitude;
            prevTemperatureMagnitude = TemperatureMagnitude;
            prevTintMagnitude = TintMagnitude;
            prevHueMagnitude = HueMagnitude;
            prevGammaMagnitude = GammaMagnitude;
            prevBrightnessMagnitude = BrightnessMagnitude;
            prevExposureMagnitude = ExposureMagnitude;
            prevContrastMagnitude = ContrastMagnitude;
            prevBlackMagnitude = BlackMagnitude;
            prevWhiteMagnitude = WhiteMagnitude;
            prevShadowMagnitude = ShadowMagnitude;
            prevHighlightMagnitude = HighlightMagnitude;
            prevPixelateMagnitude = PixelateMagnitude;
            prevSharpenMagnitude = SharpenMagnitude;
            prevClarityMagnitude = ClarityMagnitude;
            prevBloomMagnitude = BloomMagnitude;
            prevSmoothMagnitude = SmoothMagnitude;
            prevBlurMagnitude = BlurMagnitude;
            prevGrainMagnitude = GrainMagnitude;
            prevSobelEdgeMagnitude = SobelEdgeMagnitude;
        }

        private void CacheColorizeParameters()
        {
            prevEffectiveColorReplacementCount = sourceColors.Count + (hasTemporaryColorReplacement ? 1 : 0);
            prevColorizeThreshold = ColorizeThreshold;
            prevColorizeSmoothness = ColorizeSmoothness;
        }

        private void CacheLUTParameters()
        {
            prevLUTName = LUTName;
            prevUseLUT = UseLUT;
            prevLUTBlend = LUTBlend;
        }
        #endregion
        #endregion

        #region IDisposable
        public void Dispose()
        {
            ResetMagnitudeValues();
            ResetColorizeValues();
            ResetLUTValues();
            ResetComputeShaderParameters();

            if (resultTexture != null)
            {
                if (RenderTexture.active == resultTexture) { RenderTexture.active = null; }
                resultTexture.Release();
                UnityEngine.Object.DestroyImmediate(resultTexture);
                resultTexture = null;
            }

            if (readbackTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(readbackTexture);
                readbackTexture = null;
            }
        }
        #endregion
    }
}
#endif