using System;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(Colorize), true)]
    public sealed class ColorizeDrawer : PropertyDrawer
    {
        private Colorize colorize { get; set; }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            colorize = attribute as Colorize;


            switch (colorize.target)
            {
                case ColorizeTarget.Name:
                    Color orig = GUI.color;
                    SetStyles();

                    EditorGUI.PropertyField(position, property, label, true);

                    GUI.color = orig;
                    break;
                case ColorizeTarget.Back:
                    EditorGUI.DrawRect(position, colorize.color);

                    EditorGUI.PropertyField(position, property, label, true);
                    break;

                default: throw new ArgumentException(property.serializedObject.targetObject.name);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private void SetStyles()
        {
            colorize = attribute as Colorize;

            if (colorize.hexColor != string.Empty)
            {
                if (ColorUtility.TryParseHtmlString(colorize.hexColor, out Color color) == false)
                    throw new ArgumentException(nameof(colorize.hexColor));

                GUI.color = new Color(color.r + 0.22f, color.g + 0.22f, color.b + 0.22f);
            }
            else
            {
                GUI.color = colorize.color;
            }
        }
    }
}
