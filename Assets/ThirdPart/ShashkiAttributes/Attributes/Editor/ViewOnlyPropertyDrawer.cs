using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ViewOnlyAttribute))]
    public sealed class ViewOnlyPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label);

            GUI.enabled = true;
        }
    }
}
