#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase
{
    internal static class PicEase_File
    {
        #region Methods
        #region Path Management
        private static string GetOrCreateDirectory(string subFolderName)
        {
            string rootPath = GetPicEaseRootPath();
            string fullPath = Path.Combine(rootPath, PicEase_Constants.EditorFolderName, subFolderName);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                AssetDatabase.Refresh();
            }

            return fullPath;
        }

        private static string GetPicEaseRootPath()
        {
            string[] guids = AssetDatabase.FindAssets(PicEase_Constants.AssetName);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Directory.Exists(path) && path.EndsWith(PicEase_Constants.AssetName))
                {
                    return path;
                }
            }

            Debug.LogWarning($"PicEase root path was not found. Defaulting to {Path.Combine(Application.dataPath, PicEase_Constants.AssetName)}.");
            return Path.Combine(Application.dataPath, PicEase_Constants.AssetName);
        }
        #endregion

        #region File Path Getters
        public static string GetSavedDataFilePath(string fileName)
        {
            string fullPath = GetOrCreateDirectory(PicEase_Constants.SavedDataFolderName);
            return Path.Combine(fullPath, fileName);
        }

        public static string GetShaderFilePath(string shaderFileName)
        {
            string shaderFolderPath = GetOrCreateDirectory(PicEase_Constants.ShaderFolderName);
            return Path.Combine(shaderFolderPath, shaderFileName);
        }

        private static string GetPatchNotesFilePath()
        {
            string fullPath = GetOrCreateDirectory(PicEase_Constants.DocumentationFolderName);
            return Path.Combine(fullPath, PicEase_Constants.PatchNotesTextFileName);
        }

        public static string GetLUTFilePath(string lutFileName)
        {
            string lutFolderPath = GetOrCreateDirectory(Path.Combine(PicEase_Constants.ResourcesFolderName, PicEase_Constants.LUTFolderName));
            return Path.Combine(lutFolderPath, lutFileName);
        }
        #endregion

        #region File Handling
        public static string GetPatchNotesData()
        {
            string filePath = GetPatchNotesFilePath();
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"PicEase patch notes file not found at path: {filePath}");
                return "PicEase patch notes file not found.";
            }

            return ReadFileWithLimit(filePath, 100);
        }

        private static string ReadFileWithLimit(string filePath, int maxLines)
        {
            try
            {
                StringBuilder fileContent = new();
                int lineCount = 0;

                using (StreamReader reader = new(filePath))
                {
                    while (!reader.EndOfStream && lineCount < maxLines)
                    {
                        string line = reader.ReadLine();
                        fileContent.AppendLine(line);
                        lineCount++;
                    }
                }

                if (lineCount == maxLines)
                {
                    fileContent.AppendLine("...more");
                }

                return fileContent.ToString();
            }
            catch (IOException e)
            {
                Debug.LogError($"Error reading file at {filePath}: {e.Message}");
                return "Error reading file.";
            }
        }
        #endregion
        #endregion
    }
}
#endif