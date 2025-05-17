using System;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute), true)]
    public sealed class IndentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indentAttribute = attribute as IndentAttribute;

            EditorGUI.indentLevel += indentAttribute.indentLevel;

            EditorGUI.PropertyField(position, property, label);

            EditorGUI.indentLevel -= indentAttribute.indentLevel;
        }
    }
}
