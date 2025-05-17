using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Shashki.Attributes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public sealed class ShashkiAttributesEditor : UnityEditor.Editor
    {
        public static List<FieldInfo> AllSerializedFieldsInScript { get => _allFields; set => _allFields = value; }

        private FoldoutAttribute _foldoutAttribute;

        private static List<FieldInfo> _allFields = new();

        private UnityEngine.Object targetObject;
        private Type targetType;


        private void OnEnable()
        {
            targetObject = serializedObject.targetObject;
            targetType = targetObject.GetType();

            _allFields.Clear();
            _allFields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            MethodAsButtonEditor.buttonsStorage?.Clear();
            ViewOnlyEditor.ViewOnlyFieldsPaths?.Clear();
        }

        public override void OnInspectorGUI()
        {
            if (target is not MonoBehaviour or ScriptableObject)
            {
                DrawDefaultInspector();
                return;
            }

            if (_allFields.Count > 0)
                if (GetAttribute<ColorizeScriptTitleAttribute>(targetObject) != null)
                    ColorizeScriptTitleHandler.DrawTitle(target);
                else DrawScriptField();

            if (GetAttribute<SearchableAttribute>(targetObject) != null)
                SearchableEditor.DrawSearchField(target, serializedObject);

            var currentProperty = serializedObject.GetIterator();
            currentProperty.NextVisible(true);

            if (SearchableEditor.searchQuery.Trim() == string.Empty || SearchableEditor.searchQuery.Trim() == SearchableEditor.SEARCH_FIELD_DEFAULT_TEXT)
            {

                if (_allFields.Count > 0)
                {
                    while (currentProperty.NextVisible(false))
                    {
                        serializedObject.Update();
                        var field = _allFields?.FirstOrDefault(f => f.Name == currentProperty.name);
                        var foldout = field?.GetCustomAttribute<FoldoutAttribute>();
                        _foldoutAttribute = foldout != null ? foldout : _foldoutAttribute;

                        if (field?.GetCustomAttribute<FoldoutAttribute>() != null)
                        {
                            FoldoutDrawer.serializedObject = serializedObject;
                            EditorGUILayout.PropertyField(currentProperty, true);
                        }
                        else if (_foldoutAttribute != null && _foldoutAttribute.FieldsNames.Contains(currentProperty.name)) ;
                        else if (ViewOnlyEditor.ViewOnlyFieldsPaths.Contains(currentProperty.propertyPath)) ;
                        else if (field?.GetCustomAttribute<ExtendedRangeAttribute>() != null)
                        {
                            EditorGUILayout.Space(ExtendedRangeDrawer.PROPERTY_SPACE);
                            EditorGUILayout.PropertyField(currentProperty, true);
                            EditorGUILayout.Space(ExtendedRangeDrawer.PROPERTY_SPACE);
                        }
                        else EditorGUILayout.PropertyField(currentProperty, true);

                        serializedObject.ApplyModifiedProperties();
                    }
                }
                else DrawDefaultInspector();


                ViewOnlyEditor.DrawViewOnlyFields(serializedObject);

                MethodAsButtonEditor.DrawButtons(serializedObject);
            }
        }

        private void DrawScriptField()
        {
            GUI.enabled = false;

            UnityEngine.Object targetScript = target as MonoBehaviour;
            if (targetScript is null) targetScript = target as ScriptableObject;
            if (targetScript is null) return;

            MonoScript script = (targetScript as MonoBehaviour) != null
                ? MonoScript.FromMonoBehaviour(targetScript as MonoBehaviour)
                : (targetScript as ScriptableObject) != null
                ? MonoScript.FromScriptableObject(targetScript as ScriptableObject)
                : throw new ArgumentException(nameof(target));

            EditorGUILayout.ObjectField("Script", script, script.GetType());
            EditorGUILayout.Space(7);

            GUI.enabled = true;
        }

        public static T GetAttribute<T>(UnityEngine.Object target) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(target.GetType(), typeof(T));
        }

        public static IEnumerable<T> GetAttributes<T>(UnityEngine.Object target) where T : Attribute
        {
            return (IEnumerable<T>)Attribute.GetCustomAttributes(target.GetType(), typeof(T));
        }
    }

    public class ScriptsRenameWatcher : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                if (movedAssets[i].EndsWith(".cs"))
                {
                    Debug.Log($"Script renamed from: <color=cyan>{movedFromAssetPaths[i]}</color> to: <color=green>{movedAssets[i]}</color>");
                }
            }
        }
    }
}
