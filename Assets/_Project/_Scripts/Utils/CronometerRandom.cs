using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

[System.Serializable]
public class CronometerRandom
{
    public Vector2 timeMinMax = new Vector2(0f, 0f);

    public bool Ended => _elapsed < Time.time;

    float _elapsed = 0f;
    public float Elapsed => _elapsed;

    public void Tick()
    {
        _elapsed = Time.time + Random.Range(timeMinMax.x, timeMinMax.y);
    }

    public void Reset()
    {
        _elapsed = 0f;
    }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(CronometerRandom))]
public class CronometerRandomPropertyDrawer : PropertyDrawer
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
        
        var timeField = property.FindPropertyRelative("timeMinMax");

        var amountRect = new Rect(position.x, position.y, position.width, position.height);
        EditorGUI.PropertyField(amountRect, timeField, GUIContent.none);

        EditorGUI.EndProperty();
    }
}


#endif