using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(FoldoutAttribute), true)]
    public sealed class FoldoutDrawer : PropertyDrawer
    {
        public static SerializedObject serializedObject { get; set; }

        private Dictionary<string, bool> _foldoutStates = new();

        private bool _isExpanded = false;

        private SerializedProperty _currentProperty, _fieldProperty;
        private FoldoutAttribute _foldoutAttribute;
        private GUIStyle _foldoutStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SetStyles();

            _foldoutAttribute = (FoldoutAttribute)attribute;
            _currentProperty = property;

            if (!_foldoutStates.TryGetValue(_foldoutAttribute.Title, out _isExpanded))
            {
                _isExpanded = false;
            }
            try
            {

                DrawFoldout();
            }
            catch { }
        }

        private void DrawFoldout()
        {
            serializedObject.Update();

            Color originalColor = GUI.color;
            GUI.color = _foldoutAttribute.backColor;

            _isExpanded = EditorGUILayout.Foldout(_isExpanded, _foldoutAttribute.Title, true, _foldoutStyle);

            GUI.color = originalColor;

            _foldoutStates[_foldoutAttribute.Title] = _isExpanded;

            if (_isExpanded)
            {
                GUI.color = _foldoutAttribute.backColor;

                Rect rect = new Rect(GetBackRect().x, GetBackRect().y, GetBackRect().width, GetBackRect().height + _foldoutAttribute.FieldsNames.Length * 2);
                EditorGUI.DrawRect(rect, _foldoutAttribute.backColor != Color.white ?
                    new Color(_foldoutAttribute.backColor.r * 0.22f,
                    _foldoutAttribute.backColor.g * 0.22f,
                    _foldoutAttribute.backColor.b * 0.22f,
                    0.1f) :
                    new Color(0.22f, 0.22f, 0.22f));


                EditorGUI.indentLevel++;

                if (_foldoutAttribute.FieldsNames.Contains(_currentProperty.name) == false)
                {
                    if (Attribute.GetCustomAttribute(fieldInfo, typeof(ExtendedRangeAttribute)) != null)
                        EditorGUILayout.Space(ExtendedRangeDrawer.SPACE_IN_FOLDOUT);
                    EditorGUILayout.PropertyField(_currentProperty, true);
                }

                foreach (string fieldName in _foldoutAttribute.FieldsNames)
                {
                    _fieldProperty = _currentProperty.serializedObject.FindProperty(fieldName);

                    if (_fieldProperty != null)
                    {
                        var fi = ShashkiAttributesEditor.AllSerializedFieldsInScript.First(fi => fi.Name == _fieldProperty.name);
                        if (Attribute.GetCustomAttribute(fi, typeof(ExtendedRangeAttribute)) != null)
                            EditorGUILayout.Space(ExtendedRangeDrawer.SPACE_IN_FOLDOUT);
                        EditorGUILayout.PropertyField(_fieldProperty, true);
                    }
                    else
                    {
                        Debug.LogError($"Field '{fieldName}' not found in '{_currentProperty.serializedObject.targetObject.GetType()}'.");
                    }
                }

                GUI.color = originalColor;

                EditorGUI.indentLevel--;

                serializedObject.ApplyModifiedProperties();
            }
        }

        private Rect GetBackRect()
        {
            Rect startRect = GUILayoutUtility.GetLastRect();

            float totalHeight = 0;

            if (_foldoutAttribute.FieldsNames.Contains(_currentProperty.name) == false)
                totalHeight += EditorGUI.GetPropertyHeight(_currentProperty);

            foreach (string fieldName in _foldoutAttribute.FieldsNames)
            {
                _fieldProperty = _currentProperty.serializedObject.FindProperty(fieldName);

                if (_fieldProperty != null)
                {
                    var fi = ShashkiAttributesEditor.AllSerializedFieldsInScript.First(fi => fi.Name == _fieldProperty.name);

                    if (fi != null && Attribute.GetCustomAttribute(fi, typeof(ExtendedRangeAttribute)) != null)
                        totalHeight += ExtendedRangeDrawer.TOTAL_HEIGHT;
                    else
                        totalHeight += EditorGUI.GetPropertyHeight(_fieldProperty);
                }
            }

            return new Rect(startRect.x, startRect.y + startRect.height + 1, startRect.width, totalHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        private void SetStyles()
        {
            _foldoutStyle = new(EditorStyles.foldoutHeader)
            {
                fontStyle = FontStyle.Bold
            };
        }
    }
}