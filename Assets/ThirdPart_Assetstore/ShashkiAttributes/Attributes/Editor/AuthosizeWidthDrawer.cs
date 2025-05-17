using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(AuthosizeWidthAttribute))]
    public sealed class AuthosizeWidthDrawer : PropertyDrawer
    {
        private const float MIN_LABEL_WIDTH = 75f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float labelWidth = EditorGUIUtility.labelWidth;

            float dynamicLabelWidth = CalculateDynamicLabelWidth(label.text);
            EditorGUIUtility.labelWidth = dynamicLabelWidth;

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndProperty();
        }

        private float CalculateDynamicLabelWidth(string label)
        {
            float calculatedWidth = EditorStyles.label.CalcSize(new GUIContent(label)).x;

            return Mathf.Max(MIN_LABEL_WIDTH, calculatedWidth + 20f);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, label, true);
    }
}
