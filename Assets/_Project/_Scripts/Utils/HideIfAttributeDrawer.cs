using EMD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;

namespace EMD
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfAttributeDrawer : PropertyDrawer
    {
        private bool showField = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShowField(property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShowField(property))
                return EditorGUI.GetPropertyHeight(property);

            return 0.0f;
        }

        bool ShouldShowField(SerializedProperty property)
        {
            var so = property.serializedObject;
            var attrb = attribute as HideIfAttribute;

            var boolProp = so.FindProperty(attrb.FieldName);
            if (boolProp != null)
            {
                if (boolProp.propertyType == SerializedPropertyType.Boolean)
                {
                    if (boolProp.boolValue == false)
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