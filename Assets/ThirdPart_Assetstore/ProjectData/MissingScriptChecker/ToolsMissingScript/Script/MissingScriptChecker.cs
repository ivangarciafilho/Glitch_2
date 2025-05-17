using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;


namespace LLS
{

    public class MissingScriptChecker : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<GameObject> objectsWithMissingScripts = new List<GameObject>();
        private string searchFilter = string.Empty;
        private string statusMessage = "";
        private bool isScanning = false;
        private float scanProgress = 0f;
        private List<GameObject> selectedObjects = new List<GameObject>();
        private Texture2D bannerTexture;
        private bool showAdvancedOptions = false;

        private GUIStyle buttonStyle;
        private GUIStyle toggleStyle;
        private GUIStyle labelStyle;
        private GUIStyle helpBoxStyle;
        private GUIStyle headerStyle;
        private GUIStyle scrollViewStyle;
        private GUIStyle backgroundStyle;

        private static Texture2D customIcon;

        private const string iconPath = "Assets/ProjectData/MissingScriptChecker/ToolsMissingScript/Script/IconMissing.png";
        private const string releaseVersion = "Release version 1.0.0";

        [MenuItem("Tools/Missing Script Checker %m")]
        public static void ShowWindow()
        {
            MissingScriptChecker window = GetWindow<MissingScriptChecker>("Missing Script Checker");
            window.minSize = new Vector2(800, 750);

            window.titleContent = new GUIContent("Missing Script Checker", EditorGUIUtility.IconContent("d_UnityEditor.InspectorWindow").image);

            // Load the icon when the window is opened
            customIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        }

        private void OnEnable()
        {
            LoadPreferences();
            InitializeStyles();
            SetIconForWindow();
        }

        private void OnDisable()
        {
            SavePreferences();
        }

        private void SetIconForWindow()
        {
            if (customIcon != null)
            {
                // Set the icon for the window if loaded successfully
                this.titleContent.image = customIcon;
            }
        }

        private void InitializeStyles()
        {
            buttonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 14,
                fixedHeight = 40,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                hover = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.8f)) },
                active = { background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f, 0.8f)) }
            };

            toggleStyle = new GUIStyle(EditorStyles.toggle)
            {
                fontSize = 14,
                fixedHeight = 30,
                alignment = TextAnchor.MiddleLeft
            };

            labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft
            };

            helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter
            };

            scrollViewStyle = new GUIStyle(EditorStyles.textArea)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            backgroundStyle = new GUIStyle
            {
                normal = { background = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.8f)) }
            };
        }

        private void OnGUI()
        {
            _ = EditorGUILayout.BeginVertical(backgroundStyle);
            if (bannerTexture != null)
            {
                GUILayout.Box(bannerTexture, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            }
            DrawHeader();
            DrawSearchField();
            DrawButtons();
            DrawResults();
            DrawStatusBar();
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader()
        {
            GUILayout.Space(15);

            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            GUIStyle boxStyle = new GUIStyle("box")
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5),
                normal = { background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.15f)) }
            };

            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Missing Script Checker", headerStyle);
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUIStyle helpBoxStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                normal = { background = CreateColoredTexture(1, 1, new Color(0.2f, 0.2f, 0.2f)), textColor = Color.white },
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(10, 10, 10, 10),
                border = new RectOffset(4, 4, 4, 4)
            };

            GUIStyle iconStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fixedWidth = 64,
                fixedHeight = 64,
                margin = new RectOffset(0, 10, 0, 0)
            };

            EditorGUILayout.BeginVertical(helpBoxStyle);
            EditorGUILayout.BeginHorizontal();

            GUILayout.Box(EditorGUIUtility.IconContent("d_ViewToolOrbit").image, iconStyle);

            EditorGUILayout.LabelField("Scan your scene for missing scripts and remove them with a single click.", helpBoxStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            // Display the release version
            GUIStyle releaseStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            GUILayout.Label(releaseVersion, releaseStyle);
        }

        private void DrawSearchField()
        {
            GUILayout.Label("Search:", new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft,
            });

            searchFilter = EditorGUILayout.TextField(searchFilter, new GUIStyle(GUI.skin.textField)
            {
                fontSize = 12,
                padding = new RectOffset(10, 10, 5, 5),
                normal = { textColor = Color.black },
                focused = { background = MakeTex(100, 20, Color.green) },
                hover = { background = MakeTex(100, 20, Color.yellow) },
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 20
            });

            GUILayout.Space(10);
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginVertical();

            GUIStyle buttonStyle = new GUIStyle(this.buttonStyle)
            {
                normal = { background = Create3DButtonTexture(new Color(0f, 0.5f, 0f, 0.5f)) },
                hover = { background = Create3DButtonTexture(new Color(0f, 0.7f, 0f, 0.5f)) },
                active = { background = Create3DButtonTexture(new Color(0f, 0.9f, 0f, 0.5f)) }
            };

            if (GUILayout.Button(new GUIContent("Scan Scene", EditorGUIUtility.IconContent("d_ViewToolOrbit").image, "Scan the scene for missing scripts"), buttonStyle))
            {
                ScanForMissingScripts();
            }

            GUILayout.Space(10);

            buttonStyle = new GUIStyle(this.buttonStyle)
            {
                normal = { background = Create3DButtonTexture(new Color(0.5f, 0f, 0f, 0.5f)) },
                hover = { background = Create3DButtonTexture(new Color(0.7f, 0f, 0f, 0.5f)) },
                active = { background = Create3DButtonTexture(new Color(0.9f, 0f, 0f, 0.5f)) }
            };

            if (GUILayout.Button(new GUIContent("Remove All Missing Scripts", EditorGUIUtility.IconContent("d_TreeEditor.Trash").image, "Remove all missing scripts"), buttonStyle))
            {
                if (EditorUtility.DisplayDialog("Confirm Removal", "Are you sure you want to remove all missing scripts?", "Yes", "No"))
                {
                    RemoveAllMissingScripts();
                }
            }

            GUILayout.Space(10);

            buttonStyle = new GUIStyle(this.buttonStyle)
            {
                normal = { background = Create3DButtonTexture(new Color(0f, 0.5f, 0.5f, 0.5f)) },
                hover = { background = Create3DButtonTexture(new Color(0f, 0.7f, 0.7f, 0.5f)) },
                active = { background = Create3DButtonTexture(new Color(0f, 0.9f, 0.9f, 0.5f)) }
            };

            if (GUILayout.Button(new GUIContent("Export to CSV", EditorGUIUtility.IconContent("d_SaveAs").image, "Export the list of objects with missing scripts to a CSV file"), buttonStyle))
            {
                ExportToCSV();
            }

            GUILayout.Space(10);

            buttonStyle = new GUIStyle(this.buttonStyle)
            {
                normal = { background = Create3DButtonTexture(new Color(0.5f, 0.5f, 0f, 0.5f)) },
                hover = { background = Create3DButtonTexture(new Color(0.7f, 0.7f, 0f, 0.5f)) },
                active = { background = Create3DButtonTexture(new Color(0.9f, 0.9f, 0f, 0.5f)) }
            };

            if (GUILayout.Button(new GUIContent("Scan Prefab Folder", EditorGUIUtility.IconContent("d_Folder Icon").image, "Scan the Prefab folder for missing scripts"), buttonStyle))
            {
                ScanPrefabFolderForMissingScripts();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawResults()
        {
            if (objectsWithMissingScripts.Count == 0)
            {
                EditorGUILayout.HelpBox("No missing scripts found!", MessageType.Info);
                return;
            }

            GUILayout.Label("Objects with Missing Scripts:", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16 });
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, scrollViewStyle, GUILayout.Height(400));

            for (int i = 0; i < objectsWithMissingScripts.Count; i++)
            {
                var obj = objectsWithMissingScripts[i];

                if (!string.IsNullOrWhiteSpace(searchFilter) && !obj.name.ToLower().Contains(searchFilter.ToLower()))
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                if (GUILayout.Button(new GUIContent(obj.name, "Select " + obj.name), GUILayout.Width(200)))
                {
                    Selection.activeGameObject = obj;
                    SceneView.lastActiveSceneView.FrameSelected();
                }

                EditorGUILayout.ObjectField(obj, typeof(GameObject), true);

                EditorGUILayout.LabelField("Missing Scripts", new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 }, GUILayout.Width(150));

                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.3f, 0.6f, 1f);
                if (GUILayout.Button("Details", GUILayout.Width(80)))
                {
                    ShowObjectDetails(obj);
                }

                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                    ScanForMissingScripts();
                }

                GUI.backgroundColor = selectedObjects.Contains(obj) ? new Color(0.3f, 1f, 0.3f) : Color.gray;
                if (GUILayout.Button("Select", GUILayout.Width(80)))
                {
                    if (selectedObjects.Contains(obj))
                    {
                        selectedObjects.Remove(obj);
                    }
                    else
                    {
                        selectedObjects.Add(obj);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (selectedObjects.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();

                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
                if (GUILayout.Button("Remove Selected"))
                {
                    RemoveSelectedMissingScripts();
                }

                GUI.backgroundColor = new Color(0.3f, 1f, 0.3f);
                if (GUILayout.Button("Clear Selection"))
                {
                    selectedObjects.Clear();
                }

                GUI.backgroundColor = defaultColor;

                EditorGUILayout.EndHorizontal();
            }
        }

        private void ScanPrefabFolderForMissingScripts()
        {
            isScanning = true;
            statusMessage = "Scanning Prefab folder...";
            scanProgress = 0f;
            Repaint();
            objectsWithMissingScripts.Clear();

            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            int totalPrefabs = guids.Length;
            int processedPrefabs = 0;
            int repaintInterval = Mathf.Max(1, totalPrefabs / 10); // Repaint every 10% of progress

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    Component[] components = prefab.GetComponents<Component>();
                    if (components.Any(component => component == null))
                    {
                        objectsWithMissingScripts.Add(prefab);
                    }
                }
                processedPrefabs++;
                scanProgress = (float)processedPrefabs / totalPrefabs;

                if (processedPrefabs % repaintInterval == 0)
                {
                    Repaint();
                }
            }

            isScanning = false;
            statusMessage = $"Scan complete. Found {objectsWithMissingScripts.Count} prefabs with missing scripts.";
            Debug.Log(statusMessage);
            Repaint();
        }

        private Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private Texture2D CreateColoredTexture(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private Texture2D Create3DButtonTexture(Color baseColor)
        {
            int width = 100;
            int height = 40;
            Texture2D texture = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float brightness = Mathf.Clamp01((height - y) / (float)height);
                    Color pixelColor = baseColor * brightness;
                    texture.SetPixel(x, y, pixelColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private void ShowObjectDetails(GameObject obj)
        {
            string details = $"Name: {obj.name}\nTag: {obj.tag}\nLayer: {LayerMask.LayerToName(obj.layer)}\nPath: {GetObjectPath(obj)}";
            EditorUtility.DisplayDialog("Object Details", details, "OK");
        }

        private string GetObjectPath(GameObject obj)
        {
            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        private void ScanForMissingScripts()
        {
            isScanning = true;
            statusMessage = "Scanning...";
            scanProgress = 0f;
            Repaint();
            objectsWithMissingScripts.Clear();

            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            int totalObjects = allObjects.Length;
            int processedObjects = 0;
            int repaintInterval = Mathf.Max(1, totalObjects / 10); // Repaint every 10% of progress

            foreach (GameObject obj in allObjects)
            {
                Component[] components = obj.GetComponents<Component>();
                if (components.Any(component => component == null))
                {
                    objectsWithMissingScripts.Add(obj);
                }
                processedObjects++;
                scanProgress = (float)processedObjects / totalObjects;

                if (processedObjects % repaintInterval == 0)
                {
                    Repaint();
                }
            }

            isScanning = false;
            statusMessage = $"Scan complete. Found {objectsWithMissingScripts.Count} objects with missing scripts.";
            Debug.Log(statusMessage);
            Repaint();
        }

        private void RemoveAllMissingScripts()
        {
            int removedCount = 0;

            foreach (var obj in objectsWithMissingScripts)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                removedCount++;
            }

            statusMessage = $"Removed missing scripts from {removedCount} objects.";
            Debug.Log(statusMessage);
            ScanForMissingScripts();
        }

        private void RemoveSelectedMissingScripts()
        {
            int removedCount = 0;

            foreach (var obj in selectedObjects)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                removedCount++;
            }

            selectedObjects.Clear();
            statusMessage = $"Removed missing scripts from {removedCount} selected objects.";
            Debug.Log(statusMessage);
            ScanForMissingScripts();
        }

        private void ExportToCSV()
        {
            string path = EditorUtility.SaveFilePanel("Save Missing Scripts Report", "", "MissingScriptsReport.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            StringBuilder csvContent = new StringBuilder("Object Name, Object Path\n");

            foreach (var obj in objectsWithMissingScripts)
            {
                string objectPath = GetObjectPath(obj);
                csvContent.AppendLine($"{obj.name}, {objectPath}");
            }

            File.WriteAllText(path, csvContent.ToString());
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Export Complete", "The report on missing scripts has been exported successfully!", "OK");
        }

        private void ExportToJSON()
        {
            string path = EditorUtility.SaveFilePanel("Save Missing Scripts Report", "", "MissingScriptsReport.json", "json");
            if (string.IsNullOrEmpty(path)) return;

            List<Dictionary<string, string>> jsonContent = new List<Dictionary<string, string>>();

            foreach (var obj in objectsWithMissingScripts)
            {
                string objectPath = GetObjectPath(obj);
                jsonContent.Add(new Dictionary<string, string>
                {
                    { "Object Name", obj.name },
                    { "Object Path", objectPath }
                });
            }

            string jsonString = JsonUtility.ToJson(new { Objects = jsonContent }, true);
            File.WriteAllText(path, jsonString);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Export Complete", "The report on missing scripts has been exported successfully!", "OK");
        }

        private void DrawStatusBar()
        {
            GUILayout.Label(statusMessage, new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });
            if (isScanning)
            {
                EditorGUILayout.HelpBox("Scanning in progress...", MessageType.Info);
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 20), scanProgress, "Progress");
            }
        }

        private void LoadPreferences()
        {
            searchFilter = EditorPrefs.GetString("MissingScriptChecker_SearchFilter", "");
            showAdvancedOptions = EditorPrefs.GetBool("MissingScriptChecker_ShowAdvancedOptions", false);
        }

        private void SavePreferences()
        {
            EditorPrefs.SetString("MissingScriptChecker_SearchFilter", searchFilter);
            EditorPrefs.SetBool("MissingScriptChecker_ShowAdvancedOptions", showAdvancedOptions);
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
