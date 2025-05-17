using UnityEditor;

using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute), true)]
    public sealed class MinMaxRangeDrawer : PropertyDrawer
    {
        private const float FIELD_WIDTH = 50;
        private const float SLIDER_SPACE = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return;

            var minMaxAttribute = attribute as MinMaxRangeAttribute ?? new MinMaxRangeAttribute(0f, 1f);
            
            var minProperty = property.FindPropertyRelative(
                property.propertyType switch
                {
                    SerializedPropertyType.Vector2 => nameof(Vector2.x),
                    SerializedPropertyType.Vector2Int => nameof(Vector2Int.x),
                    _ => nameof(MinMaxPair.Min)
                });
            var maxProperty = property.FindPropertyRelative(
                 property.propertyType switch
                 {
                     SerializedPropertyType.Vector2 => nameof(Vector2.y),
                     SerializedPropertyType.Vector2Int => nameof(Vector2Int.y),
                     _ => nameof(MinMaxPair.Max)
                 });

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var min = minProperty.floatValue;
            var max = maxProperty.floatValue;

            var left = new Rect(position.x, position.y, FIELD_WIDTH, position.height);
            var right = new Rect(position.x + position.width - left.width, position.y, FIELD_WIDTH, position.height);
            var slider = new Rect(SLIDER_SPACE + left.xMax, position.y, position.width - SLIDER_SPACE - left.width - right.width - SLIDER_SPACE, position.height);
            min = Mathf.Clamp(
                EditorGUI.FloatField(left, min),
                minMaxAttribute.Min,
                max);
            max = Mathf.Clamp(
                EditorGUI.FloatField(right, max),
                min,
                minMaxAttribute.Max);

            EditorGUI.MinMaxSlider(slider, GUIContent.none, ref min, ref max, minMaxAttribute.Min, minMaxAttribute.Max);

            minProperty.floatValue = min;
            maxProperty.floatValue = max;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects) return 0f;

            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
