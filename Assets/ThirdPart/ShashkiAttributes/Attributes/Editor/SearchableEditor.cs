using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true, isFallback = true)]
    public sealed class SearchableEditor : UnityEditor.Editor
    {
        public const string SEARCH_FIELD_DEFAULT_TEXT = "Search...";
        private const int SPACE = 7;

        public static string searchQuery => _searchQuery;

        public IEnumerator<SerializedProperty> displayedFields => _displayedFields;
        public IEnumerator<MethodAsButtonEditor.MethodButtonsStorage> displayedMethods => _displayedMethods;

        private static string _searchQuery = "";
        private static GUIStyle _placeholderStyle = null;
        private static GUIContent _searchIcon = null;

        private static IEnumerator<SerializedProperty> _displayedFields;
        private static IEnumerator<MethodAsButtonEditor.MethodButtonsStorage> _displayedMethods;

        public static void DrawSearchField(Object target, SerializedObject serializedObject)
        {
            serializedObject.Update();

            if (target.GetType().GetCustomAttributes(typeof(SearchableAttribute), true).Length > 0)
            {
                if (_placeholderStyle == null)
                {
                    _placeholderStyle = new GUIStyle(EditorStyles.textField);
                    _placeholderStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                }

                if (_searchIcon == null)
                {
                    _searchIcon = EditorGUIUtility.IconContent("Search Icon");
                }

                EditorGUILayout.Space(SPACE);
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(_searchIcon, GUILayout.Width(20), GUILayout.Height(20));

                Rect textFieldRect = GUILayoutUtility.GetRect(200, EditorGUIUtility.singleLineHeight);

                _searchQuery = EditorGUI.TextField(
                    new Rect(textFieldRect.x, textFieldRect.y, textFieldRect.width, textFieldRect.height),
                    string.IsNullOrEmpty(_searchQuery) ? SEARCH_FIELD_DEFAULT_TEXT : _searchQuery,
                    string.IsNullOrEmpty(_searchQuery) ? _placeholderStyle : EditorStyles.textField
                );

                if (_searchQuery == SEARCH_FIELD_DEFAULT_TEXT)
                {
                    _searchQuery = "";
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(SPACE);

                DisplayFilteredFields(target);

                DisplayFilteredButtons(target);

                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void DisplayFilteredFields(Object target)
        {
            List<string> fields = new List<string>();
            var serializedObject = new SerializedObject(target);

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            while (property.NextVisible(false))
            {
                if (_searchQuery.Trim() != string.Empty && property.displayName.IndexOf(_searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    EditorGUILayout.PropertyField(property, true);

                    if(fields.Contains(property.displayName) == false) fields.Add(property.displayName);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DisplayFilteredButtons(Object target)
        {
            if (MethodAsButtonEditor.buttonsStorage is null || MethodAsButtonEditor.buttonsStorage.Count == 0) return;

            var methodButtonsStorage = MethodAsButtonEditor.buttonsStorage.Where((b, i) => _searchQuery.Trim() != string.Empty && b.name.IndexOf(_searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0);

            if (methodButtonsStorage.Count() == 0) return;

            EditorGUILayout.LabelField("", new GUIStyle(GUI.skin.horizontalSlider));

            _displayedMethods = (IEnumerator<MethodAsButtonEditor.MethodButtonsStorage>)methodButtonsStorage;

            foreach (var item in methodButtonsStorage)
            {
                if (GUILayout.Button(item.name, item.style, item.options.ToArray()))
                {
                    var parameters = item.methodInfo.GetParameters();
                    item.methodInfo.Invoke(target, parameters);
                }
            }
        }
    }
}
