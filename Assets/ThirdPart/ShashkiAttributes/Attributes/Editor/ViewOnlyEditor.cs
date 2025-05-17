using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace Shashki.Attributes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true, isFallback = true)]
    public sealed class ViewOnlyEditor : UnityEditor.Editor
    {
        public static List<string> ViewOnlyFieldsPaths => _viewOnlyFields;

        private static List<string> _viewOnlyFields = new();

        public static void DrawViewOnlyFields(SerializedObject serializedObject)
        {
            serializedObject.Update();

            var targetObject = serializedObject.targetObject;
            HashSet<string> viewOnlyFields = new();
            var fields = targetObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var viewOnlyAttribure = field.GetCustomAttribute<ViewOnlyAttribute>();

                if (viewOnlyAttribure != null)
                {
                    viewOnlyFields.Add(field.Name);
                }
            }

            if (viewOnlyFields.Count > 0)
            {
                DrawLabel();

                var current = serializedObject.GetIterator();
                current.NextVisible(true);

                while (current.NextVisible(false))
                {
                    if (viewOnlyFields.Contains(current.name))
                    {
                        if (_viewOnlyFields.Contains(current.propertyPath) == false)
                            _viewOnlyFields.Add(current.propertyPath);

                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(current, true);
                        GUI.enabled = true;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawLabel()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("", new GUIStyle(GUI.skin.horizontalSlider));
            EditorGUILayout.LabelField("View Only fields",
                new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 15
                });
            EditorGUILayout.Space(8);
        }
    }
}
