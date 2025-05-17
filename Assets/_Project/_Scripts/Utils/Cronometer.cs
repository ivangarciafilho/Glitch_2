using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

[System.Serializable]
public class Cronometer
{
    public float time = 1f;
    public bool Ended => _elapsed < (unscaledTime ? Time.unscaledTime : Time.time);

    float _elapsed = 0f;
    public float Elapsed => _elapsed;

    public bool unscaledTime;

    public void Tick()
    {
        _elapsed = (unscaledTime ? Time.unscaledTime : Time.time) + time;
    }

    public void Reset()
    {
        _elapsed = 0f;
    }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(Cronometer))]
public class CronometerPropertyDrawer : PropertyDrawer
{
    //public override VisualElement CreatePropertyGUI(SerializedProperty property)
    //{
    //    return base.CreatePropertyGUI(property);
    //    //var container = new VisualElement();

    //    //var timeField = new PropertyField(property.FindPropertyRelative("time"));

    //    //container.Add(timeField);

    //    //return container;
    //}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var timeField = property.FindPropertyRelative("time");

        var amountRect = new Rect(position.x, position.y, position.width, position.height);
        EditorGUI.PropertyField(amountRect, timeField, GUIContent.none);

        EditorGUI.EndProperty();
    }
}


#endif