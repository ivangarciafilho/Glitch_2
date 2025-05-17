#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Verpha.PicEase
{
    internal class PicEase_Map : IDisposable
    {
        #region Properties
        #region Core
        private readonly PicEase_Enums.BehaviourMode behaviourMode;
        private readonly Texture2D originalDiffuseMap;
        private readonly ComputeShader computeShader;
        private readonly EditorWindow editorWindow;

        private readonly RenderTexture inputTexture;
        private RenderTexture resultHeightMap;
        private RenderTexture resultNormalMap;
        private Texture2D readbackHeightMap;
        private Texture2D readbackNormalMap;

        private readonly int mainKernelHandle;
        private readonly int threadGroupsX;
        private readonly int threadGroupsY;

        private bool isReadbackInProgress = false;
        #endregion

        #region Normal
        public bool NormalInvertX { get; set; } = false;
        public bool NormalInvertY { get; set; } = false;
        public float NormalStrengthXMagnitude { get; set; } = 1.0f;
        public float NormalStrengthYMagnitude { get; set; } = 2.0f;
        public float NormalZBiasMagnitude { get; set; } = 1.0f;
        public float NormalSmoothnessMagnitude { get; set; } = 0.5f;
        public float NormalThresholdMagnitude { get; set; } = 0.025f;

        private bool prevNormalInvertX;
        private bool prevNormalInvertY;
        private float prevNormalStrengthXMagnitude;
        private float prevNormalStrengthYMagnitude;
        private float prevNormalZBiasMagnitude;
        private float prevNormalSmoothnessMagnitude;
        private float prevNormalThresholdMagnitude;
        #endregion

        #region Height
        public bool HeightInvert { get; set; } = false;
        public float HeightBlackLevelMagnitude { get; set; } = 0.0f;
        public float HeightWhiteLevelMagnitude { get; set; } = 0.0f;
        public float HeightGammaMagnitude { get; set; } = 1.0f;
        public float HeightAmplitudeMagnitude { get; set; } = 1.0f;
        public float HeightBaseLevelMagnitude { get; set; } = 0.5f;
        public float HeightContrastMagnitude { get; set; } = 0.8f;
        public float HeightGrayScaleMagnitude { get; set; } = 0.0f;
        public float HeightSmoothnessMagnitude { get; set; } = 0.5f;
        public float HeightBlurMagnitude { get; set; } = 0.0f;

        private bool prevHeightInvert;
        private float prevHeightBlackLevelMagnitude;
        private float prevHeightWhiteLevelMagnitude;
        private float prevHeightGammaMagnitude;
        private float prevHeightAmplitudeMagnitude;
        private float prevHeightBaseLevelMagnitude;
        private float prevHeightContrastMagnitude;
        private float prevHeightGrayScaleMagnitude;
        private float prevHeightSmoothnessMagnitude;
        private float prevHeightBlurMagnitude;
        #endregion
        #endregion

        #region Constructor
        public PicEase_Map(Texture2D diffuseMap, ComputeShader shader, EditorWindow window)
        {
            this.behaviourMode = PicEase_Settings.MapGeneratorBehaviourMode;
            this.originalDiffuseMap = diffuseMap != null ? diffuseMap : throw new ArgumentNullException(nameof(diffuseMap));
            this.computeShader = shader != null ? shader : throw new ArgumentNullException(nameof(shader));
            this.editorWindow = window != null ? window : throw new ArgumentNullException(nameof(window));

            inputTexture = new(originalDiffuseMap.width, originalDiffuseMap.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            inputTexture.Create();
            inputTexture.filterMode = originalDiffuseMap.filterMode;

            Graphics.Blit(originalDiffuseMap, inputTexture);

            resultHeightMap = new(originalDiffuseMap.width, originalDiffuseMap.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            resultHeightMap.Create();

            resultNormalMap = new(originalDiffuseMap.width, originalDiffuseMap.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            resultNormalMap.Create();

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                readbackHeightMap = new(originalDiffuseMap.width, originalDiffuseMap.height, TextureFormat.RGBAFloat, false, true)
                {
                    filterMode = diffuseMap.filterMode
                };
                readbackNormalMap = new(originalDiffuseMap.width, originalDiffuseMap.height, TextureFormat.RGBAFloat, false, true)
                {
                    filterMode = diffuseMap.filterMode
                };
            }

            mainKernelHandle = computeShader.FindKernel("CSMain");
            computeShader.SetTexture(mainKernelHandle, "InputTexture", inputTexture);
            computeShader.SetTexture(mainKernelHandle, "HeightMap", resultHeightMap);
            computeShader.SetTexture(mainKernelHandle, "NormalMap", resultNormalMap);

            PicEase_Enums.ThreadGroupSize threadGroupSize = PicEase_Settings.MapGeneratorThreadGroupSize;
            float divisionFactor = threadGroupSize switch
            {
                PicEase_Enums.ThreadGroupSize.EightByEight => 8.0f,
                PicEase_Enums.ThreadGroupSize.SixteenBySixteen => 16.0f,
                PicEase_Enums.ThreadGroupSize.ThirtyTwoByThirtyTwo => 32.0f,
                _ => 16.0f,
            };
            threadGroupsX = Mathf.CeilToInt(originalDiffuseMap.width / divisionFactor);
            threadGroupsY = Mathf.CeilToInt(originalDiffuseMap.height / divisionFactor);

            ResetComputeShaderParameters();
            CacheAdjustmentsParameters();
        }
        #endregion

        #region Accessors
        public Texture GetNormalMap()
        {
            return behaviourMode == PicEase_Enums.BehaviourMode.Synced ? readbackNormalMap : resultNormalMap;
        }

        public Texture GetHeightMap()
        {
            return behaviourMode == PicEase_Enums.BehaviourMode.Synced ? readbackHeightMap : resultHeightMap;
        }

        public Texture2D GetFinalNormalMap2D()
        {
            ApplyAdjustments();

            RenderTexture tempRT = RenderTexture.GetTemporary(resultNormalMap.width, resultNormalMap.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
            Graphics.Blit(resultNormalMap, tempRT);

            Texture2D exportTexture = new(resultNormalMap.width, resultNormalMap.height, TextureFormat.RGBA32, false)
            {
                alphaIsTransparency = true,
                wrapMode = originalDiffuseMap.wrapMode,
                filterMode = originalDiffuseMap.filterMode,
            };

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = tempRT;
            exportTexture.ReadPixels(new(0, 0, tempRT.width, tempRT.height), 0, 0);
            exportTexture.Apply();
            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(tempRT);

            return exportTexture;
        }

        public Texture2D GetFinalHeightMap2D()
        {
            ApplyAdjustments();

            RenderTexture tempRT = RenderTexture.GetTemporary(resultHeightMap.width, resultHeightMap.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
            Graphics.Blit(resultHeightMap, tempRT);

            Texture2D exportTexture = new(resultHeightMap.width, resultHeightMap.height, TextureFormat.RGBA32, false)
            {
                alphaIsTransparency = true,
                wrapMode = originalDiffuseMap.wrapMode,
                filterMode = originalDiffuseMap.filterMode,
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
        public void ApplyAdjustments()
        {
            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                if (isReadbackInProgress) return;
                isReadbackInProgress = true;
            }

            SetShaderBoolIfChanged("NormalInvertX", NormalInvertX, ref prevNormalInvertX);
            SetShaderBoolIfChanged("NormalInvertY", NormalInvertY, ref prevNormalInvertY);
            SetShaderFloatIfChanged("NormalStrengthXMagnitude", NormalStrengthXMagnitude, ref prevNormalStrengthXMagnitude);
            SetShaderFloatIfChanged("NormalStrengthYMagnitude", NormalStrengthYMagnitude, ref prevNormalStrengthYMagnitude);
            SetShaderFloatIfChanged("NormalZBiasMagnitude", NormalZBiasMagnitude, ref prevNormalZBiasMagnitude);
            SetShaderFloatIfChanged("NormalSmoothnessMagnitude", NormalSmoothnessMagnitude, ref prevNormalSmoothnessMagnitude);
            SetShaderFloatIfChanged("NormalThresholdMagnitude", NormalThresholdMagnitude, ref prevNormalThresholdMagnitude);
            SetShaderBoolIfChanged("HeightInvert", HeightInvert, ref prevHeightInvert);
            SetShaderFloatIfChanged("HeightBlackLevelMagnitude", HeightBlackLevelMagnitude, ref prevHeightBlackLevelMagnitude);
            SetShaderFloatIfChanged("HeightWhiteLevelMagnitude", HeightWhiteLevelMagnitude, ref prevHeightWhiteLevelMagnitude);
            SetShaderFloatIfChanged("HeightGammaMagnitude", HeightGammaMagnitude, ref prevHeightGammaMagnitude);
            SetShaderFloatIfChanged("HeightAmplitudeMagnitude", HeightAmplitudeMagnitude, ref prevHeightAmplitudeMagnitude);
            SetShaderFloatIfChanged("HeightBaseLevelMagnitude", HeightBaseLevelMagnitude, ref prevHeightBaseLevelMagnitude);
            SetShaderFloatIfChanged("HeightContrastMagnitude", HeightContrastMagnitude, ref prevHeightContrastMagnitude);
            SetShaderFloatIfChanged("HeightGrayScaleMagnitude", HeightGrayScaleMagnitude, ref prevHeightGrayScaleMagnitude);
            SetShaderFloatIfChanged("HeightSmoothnessMagnitude", HeightSmoothnessMagnitude, ref prevHeightSmoothnessMagnitude);
            SetShaderFloatIfChanged("HeightBlurMagnitude", HeightBlurMagnitude, ref prevHeightBlurMagnitude);

            computeShader.Dispatch(mainKernelHandle, threadGroupsX, threadGroupsY, 1);

            if (behaviourMode == PicEase_Enums.BehaviourMode.Synced)
            {
                AsyncGPUReadback.Request(resultNormalMap, 0, TextureFormat.RGBAFloat, OnCompleteNormalMapReadback);
                AsyncGPUReadback.Request(resultHeightMap, 0, TextureFormat.RGBAFloat, OnCompleteHeightMapReadback);
            }
            else
            {
                editorWindow.Repaint();
            }
        }

        private void SetShaderBoolIfChanged(string propertyName, bool currentValue, ref bool previousValue)
        {
            if (currentValue != previousValue)
            {
                computeShader.SetBool(propertyName, currentValue);
                previousValue = currentValue;
            }
        }

        private void SetShaderFloatIfChanged(string propertyName, float currentValue, ref float previousValue)
        {
            if (currentValue != previousValue)
            {
                computeShader.SetFloat(propertyName, currentValue);
                previousValue = currentValue;
            }
        }

        private void OnCompleteNormalMapReadback(AsyncGPUReadbackRequest request)
        {
            if (readbackNormalMap == null) return;
            isReadbackInProgress = false;

            if (request.hasError)
            {
                Debug.LogError("GPU readback error detected.");
                return;
            }

            readbackNormalMap.LoadRawTextureData(request.GetData<byte>());
            readbackNormalMap.Apply();

            EditorApplication.delayCall += () =>
            {
                editorWindow.Repaint();
            };
        }

        private void OnCompleteHeightMapReadback(AsyncGPUReadbackRequest request)
        {
            if (readbackHeightMap == null) return;
            isReadbackInProgress = false;

            if (request.hasError)
            {
                Debug.LogError("GPU readback error detected.");
                return;
            }

            readbackHeightMap.LoadRawTextureData(request.GetData<byte>());
            readbackHeightMap.Apply();

            EditorApplication.delayCall += () =>
            {
                editorWindow.Repaint();
            };
        }

        public void ResetMapData()
        {
            ResetComputeShaderParameters();
            ResetMagnitudeValues();
        }

        private void ResetMagnitudeValues()
        {
            NormalInvertX = false;
            NormalInvertY = false;
            NormalStrengthXMagnitude = 1f;
            NormalStrengthYMagnitude = 2f;
            NormalZBiasMagnitude = 1f;
            NormalSmoothnessMagnitude = 0.5f;
            NormalThresholdMagnitude = 0.025f;
            HeightInvert = false;
            HeightBlackLevelMagnitude = 0.0f;
            HeightWhiteLevelMagnitude = 0.0f;
            HeightGammaMagnitude = 1f;
            HeightAmplitudeMagnitude = 1f;
            HeightBaseLevelMagnitude = 0.5f;
            HeightContrastMagnitude = 0.8f;
            HeightGrayScaleMagnitude = 0.0f;
            HeightSmoothnessMagnitude = 0.5f;
            HeightBlurMagnitude = 0.0f;
        }

        private void ResetComputeShaderParameters()
        {
            computeShader.SetBool("NormalInvertX", NormalInvertX);
            computeShader.SetBool("NormalInvertY", NormalInvertY);
            computeShader.SetFloat("NormalStrengthXMagnitude", NormalStrengthXMagnitude);
            computeShader.SetFloat("NormalStrengthYMagnitude", NormalStrengthYMagnitude);
            computeShader.SetFloat("NormalZBiasMagnitude", NormalZBiasMagnitude);
            computeShader.SetFloat("NormalSmoothnessMagnitude", NormalSmoothnessMagnitude);
            computeShader.SetFloat("NormalThresholdMagnitude", NormalThresholdMagnitude);
            computeShader.SetFloat("HeightBlackLevelMagnitude", HeightBlackLevelMagnitude);
            computeShader.SetFloat("HeightWhiteLevelMagnitude", HeightWhiteLevelMagnitude);
            computeShader.SetFloat("HeightGammaMagnitude", HeightGammaMagnitude);
            computeShader.SetFloat("HeightAmplitudeMagnitude", HeightAmplitudeMagnitude);
            computeShader.SetFloat("HeightBaseLevelMagnitude", HeightBaseLevelMagnitude);
            computeShader.SetFloat("HeightContrastMagnitude", HeightContrastMagnitude);
            computeShader.SetFloat("HeightGrayScaleMagnitude", HeightGrayScaleMagnitude);
            computeShader.SetFloat("HeightSmoothnessMagnitude", HeightSmoothnessMagnitude);
            computeShader.SetFloat("HeightBlurMagnitude", HeightBlurMagnitude);
        }

        private void CacheAdjustmentsParameters()
        {
            prevNormalInvertX = NormalInvertX;
            prevNormalInvertY = NormalInvertY;
            prevNormalStrengthXMagnitude = NormalStrengthXMagnitude;
            prevNormalStrengthYMagnitude = NormalStrengthYMagnitude;
            prevNormalZBiasMagnitude = NormalZBiasMagnitude;
            prevNormalSmoothnessMagnitude = NormalSmoothnessMagnitude;
            prevNormalThresholdMagnitude = NormalThresholdMagnitude;
            prevHeightInvert = HeightInvert;
            prevHeightBlackLevelMagnitude = HeightBlackLevelMagnitude;
            prevHeightWhiteLevelMagnitude = HeightWhiteLevelMagnitude;
            prevHeightGammaMagnitude = HeightGammaMagnitude;
            prevHeightAmplitudeMagnitude = HeightAmplitudeMagnitude;
            prevHeightBaseLevelMagnitude = HeightBaseLevelMagnitude;
            prevHeightContrastMagnitude = HeightContrastMagnitude;
            prevHeightSmoothnessMagnitude = HeightSmoothnessMagnitude;
            prevHeightBlurMagnitude = HeightBlurMagnitude;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            ResetMapData();

            if (resultHeightMap != null)
            {
                if (RenderTexture.active == resultHeightMap) { RenderTexture.active = null; }
                resultHeightMap.Release();
                UnityEngine.Object.DestroyImmediate(resultHeightMap);
                resultHeightMap = null;
            }

            if (resultNormalMap != null)
            {
                if (RenderTexture.active == resultNormalMap) { RenderTexture.active = null; }
                resultNormalMap.Release();
                UnityEngine.Object.DestroyImmediate(resultNormalMap);
                resultNormalMap = null;
            }

            if (readbackHeightMap != null)
            {
                UnityEngine.Object.DestroyImmediate(readbackHeightMap);
                readbackHeightMap = null;
            }

            if (readbackNormalMap != null)
            {
                UnityEngine.Object.DestroyImmediate(readbackNormalMap);
                readbackNormalMap = null;
            }
        }
        #endregion
    }
}
#endif