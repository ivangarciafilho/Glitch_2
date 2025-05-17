#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;

namespace Verpha.PicEase
{
    internal static class PicEase_Operations
    {
        #region Methods
        #region Scene Screenshot
        public static void TakeQuickScreenshot()
        {
            Camera cam = Camera.main;
            if (cam == null && Camera.allCamerasCount > 0)
            {
                cam = Camera.allCameras[0];
            }
            if (cam == null)
            {
                Debug.LogError("No camera available for quick screenshot.");
                return;
            }

            int width = PicEase_Settings.SceneScreenshotDefaultImageWidth;
            int height = PicEase_Settings.SceneScreenshotDefaultImageHeight;
            bool hdrExport = false;

            RenderTexture renderTexture = null;
            Texture2D quickScreenshot = null;
            try
            {
                renderTexture = new(width, height, 24, hdrExport ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
                cam.targetTexture = renderTexture;
                cam.Render();
                RenderTexture.active = renderTexture;

                quickScreenshot = new(renderTexture.width, renderTexture.height, TextureFormat.RGB9e5Float, false);
                quickScreenshot.ReadPixels(new(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                quickScreenshot.Apply();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to capture quick screenshot: {ex.Message}");
                return;
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
                    UnityEngine.Object.DestroyImmediate(renderTexture);
                }
            }

            string exportDirectory = PicEase_Settings.SceneScreenshotDefaultExportDirectory;
            if (string.IsNullOrEmpty(exportDirectory))
            {
                exportDirectory = Application.dataPath;
            }
            else if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            PicEase_Converter.ImageFormat exportImageFormat = PicEase_Settings.SceneScreenshotDefaultExportImageFormat;
            string extension = PicEase_Converter.GetExtension(exportImageFormat);

            string baseFileName = $"PicEase_QuickScreenshot_{DateTime.Now.ToString(PicEase_Constants.DateFormat)}";
            string fileName = $"{baseFileName}.{extension}";
            string path = Path.Combine(exportDirectory, fileName);

            int count = 1;
            while (File.Exists(path))
            {
                fileName = $"{baseFileName}_{count}.{extension}";
                path = Path.Combine(exportDirectory, fileName);
                count++;
            }

            byte[] bytes = PicEase_Converter.EncodeImage(quickScreenshot, exportImageFormat);
            try
            {
                File.WriteAllBytes(path, bytes);
                Debug.Log($"Quick screenshot exported to {path}");
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to export quick screenshot: {ex.Message}");
            }
            UnityEngine.Object.DestroyImmediate(quickScreenshot);
        }
        #endregion

        #region Computer Shader Performance
        public static void EnsureComputeShaderProperties(string shaderFileName, PicEase_Enums.PrecisionMode precisionModeSettings, PicEase_Enums.ThreadGroupSize threadGroupSizeSettings)
        {
            string computeShaderPath = PicEase_File.GetShaderFilePath(shaderFileName);
            if (string.IsNullOrEmpty(computeShaderPath))
            {
                Debug.LogError("Compute shader path is null or empty.");
                return;
            }

            string[] shaderLines = File.ReadAllLines(computeShaderPath);

            string currentPrecision = null;
            int currentThreadGroupSizeX = 0;
            int currentThreadGroupSizeY = 0;
            int currentThreadGroupSizeZ = 0;

            for (int i = 0; i < shaderLines.Length; i++)
            {
                string line = shaderLines[i];
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//") || trimmedLine.StartsWith("/*")) continue;

                if (currentPrecision == null)
                {
                    Match matchAnyPrecision = Regex.Match(trimmedLine, @"\b(half|float)([234])?\b");
                    if (matchAnyPrecision.Success)
                    {
                        currentPrecision = matchAnyPrecision.Groups[1].Value;
                        continue;
                    }
                }

                Match matchNumThreads = Regex.Match(trimmedLine, @"\[\s*numthreads\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*\]");
                if (matchNumThreads.Success)
                {
                    currentThreadGroupSizeX = int.Parse(matchNumThreads.Groups[1].Value);
                    currentThreadGroupSizeY = int.Parse(matchNumThreads.Groups[2].Value);
                    currentThreadGroupSizeZ = int.Parse(matchNumThreads.Groups[3].Value);
                    continue;
                }
            }

            string desiredPrecision = null;
            if (precisionModeSettings == PicEase_Enums.PrecisionMode.Full)
            {
                desiredPrecision = "float";
            }
            else if (precisionModeSettings == PicEase_Enums.PrecisionMode.Half)
            {
                desiredPrecision = "half";
            }

            int desiredThreadGroupSizeX, desiredThreadGroupSizeY, desiredThreadGroupSizeZ = 1;
            switch (threadGroupSizeSettings)
            {
                case PicEase_Enums.ThreadGroupSize.EightByEight:
                    desiredThreadGroupSizeX = 8;
                    desiredThreadGroupSizeY = 8;
                    break;
                case PicEase_Enums.ThreadGroupSize.SixteenBySixteen:
                    desiredThreadGroupSizeX = 16;
                    desiredThreadGroupSizeY = 16;
                    break;
                case PicEase_Enums.ThreadGroupSize.ThirtyTwoByThirtyTwo:
                    desiredThreadGroupSizeX = 32;
                    desiredThreadGroupSizeY = 32;
                    break;
                default:
                    desiredThreadGroupSizeX = 16;
                    desiredThreadGroupSizeY = 16;
                    break;
            }

            bool needsUpdate = false;
            if (currentPrecision != desiredPrecision)
            {
                needsUpdate = true;
            }

            if (currentThreadGroupSizeX != desiredThreadGroupSizeX || currentThreadGroupSizeY != desiredThreadGroupSizeY || currentThreadGroupSizeZ != desiredThreadGroupSizeZ)
            {
                needsUpdate = true;
            }

            if (!needsUpdate)
            {
                return;
            }

            StringBuilder modifiedShader = new();
            for (int i = 0; i < shaderLines.Length; i++)
            {
                string line = shaderLines[i];
                string modifiedLine = line;

                if (currentPrecision != desiredPrecision)
                {
                    if (currentPrecision == "float" && desiredPrecision == "half")
                    {
                        modifiedLine = ReplacePrecision(modifiedLine, "float", "half");
                    }
                    else if (currentPrecision == "half" && desiredPrecision == "float")
                    {
                        modifiedLine = ReplacePrecision(modifiedLine, "half", "float");
                    }
                }

                Match matchNumThreads = Regex.Match(line, @"\[\s*numthreads\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*\]");
                if (matchNumThreads.Success)
                {
                    modifiedLine = $"[numthreads({desiredThreadGroupSizeX}, {desiredThreadGroupSizeY}, {desiredThreadGroupSizeZ})]";
                }

                modifiedShader.AppendLine(modifiedLine);
            }

            File.WriteAllText(computeShaderPath, modifiedShader.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            UpdateComputeShaderInstanceSessionCache(computeShaderPath);
        }

        private static string ReplacePrecision(string line, string fromPrecision, string toPrecision)
        {
            string[] patterns = { $"{fromPrecision}4", $"{fromPrecision}3", $"{fromPrecision}2", fromPrecision };
            foreach (string pattern in patterns)
            {
                string replacement = pattern.Replace(fromPrecision, toPrecision);
                line = Regex.Replace(line, $@"\b{pattern}\b", replacement);
            }
            return line;
        }

        private static void UpdateComputeShaderInstanceSessionCache(string shaderPath)
        {
            ComputeShader updatedComputeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(shaderPath);
            if (updatedComputeShader != null)
            {
                if(shaderPath == PicEase_Constants.ImageComputeShader)
                {
                    PicEase_Session.instance.ComputeShaderImage = updatedComputeShader;
                    PicEase_Session.instance.IsComputeShaderImageProcessingLoaded = true;
                }
                else
                {
                    PicEase_Session.instance.ComputeShaderMap = updatedComputeShader;
                    PicEase_Session.instance.IsComputeShaderMapGeneratorLoaded = true;
                }
            }
            else
            {
                Debug.LogError("Failed to reload the compute shader after modification.");
            }
        }
        #endregion
        #endregion
    }
}
#endif