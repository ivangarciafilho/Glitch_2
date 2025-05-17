using UnityEditor;

using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ExtendedRangeAttribute), true)]
    public sealed class ExtendedRangeDrawer : PropertyDrawer
    {
        public const float TOTAL_HEIGHT = (SPACE_IN_FOLDOUT * 2 + PROPERTY_SPACE) * 2;
        public const float PROPERTY_SPACE = 3.5f;
        public const float SPACE_IN_FOLDOUT = 7;

        private const int FIELD_WIDTH = 50;
        private const float SLIDER_HORIZONTAL_SPACE = 5;

        private Rect _minRect, _maxRect, _sliderRect, _valueRect;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var extendedRange = attribute as ExtendedRangeAttribute;

            GUILayout.Space(PROPERTY_SPACE);
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            _minRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight / 2, FIELD_WIDTH, position.height);
            _maxRect = new Rect(position.x + position.width - _minRect.width, _minRect.y, FIELD_WIDTH, position.height);
            _valueRect = new Rect((_maxRect.xMax - _minRect.xMax) / 2 + position.x, position.y - EditorGUIUtility.singleLineHeight / 2, FIELD_WIDTH, position.height);
            _sliderRect = new Rect(SLIDER_HORIZONTAL_SPACE + _minRect.xMax, _maxRect.y, position.width - SLIDER_HORIZONTAL_SPACE - _minRect.width - _maxRect.width - SLIDER_HORIZONTAL_SPACE, position.height);

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (extendedRange.CanEditLimitsInInspector)
                        EditorGUI.IntField(_minRect, (int)extendedRange.Min);
                    else
                        EditorGUI.LabelField(_minRect, ((int)extendedRange.Min).ToString());

                    GUILayout.BeginHorizontal();

                    GUILayout.Space(_sliderRect.x);

                    EditorGUI.BeginChangeCheck();

                    var intValue = EditorGUI.IntField(_valueRect, (int)Mathf.Clamp(property.intValue, extendedRange.Min, extendedRange.Max));

                    if (EditorGUI.EndChangeCheck())
                        property.intValue = (int)Mathf.Clamp(intValue, extendedRange.Min, extendedRange.Max);
                    else
                        property.intValue = (int)GUI.HorizontalSlider(_sliderRect, property.intValue, extendedRange.Min, extendedRange.Max);

                    GUILayout.EndHorizontal();
                    if (extendedRange.CanEditLimitsInInspector)
                        EditorGUI.IntField(_maxRect, (int)extendedRange.Max);
                    else
                        EditorGUI.LabelField(_maxRect, ((int)extendedRange.Max).ToString());
                    break;

                case SerializedPropertyType.Float:
                    if (extendedRange.CanEditLimitsInInspector)
                        EditorGUI.FloatField(_minRect, extendedRange.Min);
                    else
                        EditorGUI.LabelField(_minRect, extendedRange.Min.ToString());
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(_sliderRect.x);

                    EditorGUI.BeginChangeCheck();

                    var floatValue = EditorGUI.FloatField(_valueRect, Mathf.Clamp(property.floatValue, extendedRange.Min, extendedRange.Max));

                    if (EditorGUI.EndChangeCheck())
                        property.floatValue = Mathf.Clamp(floatValue, extendedRange.Min, extendedRange.Max);
                    else
                        property.floatValue = GUI.HorizontalSlider(_sliderRect, property.floatValue, extendedRange.Min, extendedRange.Max);

                    GUILayout.EndHorizontal();
                    if (extendedRange.CanEditLimitsInInspector)
                        EditorGUI.FloatField(_maxRect, extendedRange.Max);
                    else
                        EditorGUI.LabelField(_maxRect, extendedRange.Max.ToString());
                    break;
            }

            EditorGUI.EndProperty();
            GUILayout.Space(PROPERTY_SPACE);
        }
    }
}
