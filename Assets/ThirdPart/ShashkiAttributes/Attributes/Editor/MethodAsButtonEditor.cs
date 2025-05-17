using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace Shashki.Attributes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    public sealed class MethodAsButtonEditor : UnityEditor.Editor
    {
        public readonly static List<MethodButtonsStorage> buttonsStorage = new();

        public static void DrawButtons(SerializedObject serializedObject)
        {
            serializedObject.Update();

            var targetObject = serializedObject.targetObject;
            List<MethodAsButtonAttribute> methodButtons = new();
            List<MethodInfo> methodsWithButtonAttribute = new();

            var allMethods = targetObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var method in allMethods)
            {
                var buttonAttribute = method.GetCustomAttribute<MethodAsButtonAttribute>();

                if (buttonAttribute != null)
                {
                    methodButtons.Add(buttonAttribute);
                    methodsWithButtonAttribute.Add(method);
                }
            }

            EditorGUILayout.Space(5);

            if (methodButtons.Count > 0)
            {
                DrawLabel();

                for (int i = 0; i < methodButtons.Count; i++)
                {
                    string name = (methodButtons[i].buttonName.Trim() != string.Empty) ? methodButtons[i].buttonName : methodsWithButtonAttribute[i].Name;

                    var buttonStyle = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = (int)(methodButtons[i].buttonHeight / 1.5f),
                    };

                    if (buttonsStorage?.Select(b => b.name).Contains(name) == false)
                    {
                        buttonsStorage.Add(new() 
                        { 
                            name = name, 
                            methodInfo = methodsWithButtonAttribute[i],
                            style = buttonStyle,
                            options = new[] { GUILayout.Height(methodButtons[i].buttonHeight) }
                        });
                    }

                    if (GUILayout.Button(name, buttonStyle, GUILayout.Height(methodButtons[i].buttonHeight)))
                    {
                        var parameters = methodsWithButtonAttribute[i].GetParameters();
                        methodsWithButtonAttribute[i].Invoke(targetObject, parameters);
                    }
                }
            }


            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawLabel()
        {
            EditorGUILayout.LabelField("", new GUIStyle(GUI.skin.horizontalSlider));
            EditorGUILayout.LabelField("Methods", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 18 });
            EditorGUILayout.Space(8);
        }

        public struct MethodButtonsStorage
        {
            public string name;
            public MethodInfo methodInfo;
            public GUIStyle style;
            public IList<GUILayoutOption> options;
        }
    }
}
