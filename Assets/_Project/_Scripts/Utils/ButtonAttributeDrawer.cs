using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;

namespace EMD
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ButtonAttributeDrawer : PropertyDrawer
    {
        private bool showField = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        bool ShouldShowField(SerializedProperty property)
        {
            var so = property.serializedObject;
            var attrb = attribute as ShowIfAttribute;

            var boolProp = so.FindProperty(attrb.FieldName);
            if (boolProp != null)
            {
                if (boolProp.propertyType == SerializedPropertyType.Boolean)
                {
                    if (boolProp.boolValue == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}
#endif