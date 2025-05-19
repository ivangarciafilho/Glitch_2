#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BFS;
using UnityEditor;
using UnityEngine;

/// <summary>
/// (Editor-Only) Class managing how AnimatorEvents are drawn
/// </summary>
public static class AnimatorEventDrawer
{
	private static AnimatorEvent m_Event;
	private static MethodInfo m_MethodInfo;
	private static List<string> m_MethodNames = new List<string>();
	private static GameObject m_PreviousSender;
	private static Dictionary<string, List<MethodInfo>> m_Methods;

	public static void Draw(AnimatorEvent i_Event)
	{
		if (m_Methods == null || m_PreviousSender != i_Event.m_Sender)
		{
			EditorSetup(i_Event);
		}

		m_Event = i_Event;

		ShowOptions();

		ShowSender();
		ShowFunctions();

		ProcessKeyInputs();

		if (GUI.changed)
		{
			if(Application.isPlaying)
			{
				Debug.LogWarning("Modifications might not work and won't be saved at runtime!");
			}
			m_Event.m_HasChanged = true;
		}
		m_PreviousSender = i_Event.m_Sender;
	}

	public static void EditorSetup(AnimatorEvent i_Event, bool i_Force = false)
	{
		UpdateMethodInfos(i_Event);
		i_Event.UpdateTime(false);
		if(i_Event.m_EventType == 0)
		{
			i_Event.m_EventType = AnimatorEventType.EnterState;
		}
	}

	private static void ShowOptions()
	{
		GUI.color = AnimatorPreviewer.LightOrange;
		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUI.color = Color.white;
		m_Event.m_ShowOptions = EditorGUILayout.Foldout(m_Event.m_ShowOptions, "Options", true);
		if (m_Event.m_ShowOptions)
		{
			EditorGUI.indentLevel++;
			ShowEventType();

			if (m_Event.IsEnterStateEvent)
			{
				m_Event.m_Option1 = EditorGUILayout.Toggle(new GUIContent("Wait For Transition", "If true, the event will only be triggered as soon as the transition has been completed"), m_Event.m_Option1);
			}
			else if (m_Event.IsExitStateEvent)
			{
				m_Event.m_Option1 = EditorGUILayout.Toggle(new GUIContent("Call On Exit Only", "If true, the event will only be triggered when exiting the state and not at the end of the state. If false, it will only be trigger at the end of the state"), m_Event.m_Option1);
			}
			else if (m_Event.IsCustomEvent)
			{
				ShowEditableFloatField(new GUIContent("Norm. Time", "(Normalized Time) When will the event be called in normalized time (0%-100%)"), 0f, 100f, ref m_Event.m_EditNormalizedTime, ref m_Event.m_TempNormValue, m_Event.m_EditFixedTime, "{0}%", OnNormalizedTimeUpdate);
				ShowEditableFloatField(new GUIContent("Fixed Time", "When will the event be called in fixed time (0s-[clip.Length]s)"), 0f, m_Event.m_StateLength, ref m_Event.m_EditFixedTime, ref m_Event.m_TempFixedValue, m_Event.m_EditNormalizedTime, "{0}s", OnFixedTimeUpdate);
				m_Event.m_Option1 = EditorGUILayout.Toggle(new GUIContent("Call In Enter Transition", "Call the event while transitioning to the state? If true, it will be called if the normalized time is higher than the event's normalized time while in transition"), m_Event.m_Option1);
				m_Event.m_Option2 = EditorGUILayout.Toggle(new GUIContent("Call On Exit State", "If true, it also calls the event on exit state if the normalized time is less than the event's normalized time"), m_Event.m_Option2);
				m_Event.m_Option3 = EditorGUILayout.Toggle(new GUIContent("Call Only Once", "If true, it calls the event once even if the state is looping"), m_Event.m_Option3);
			}

			EditorGUI.indentLevel--;
		}
		EditorGUILayout.EndVertical();
	}

	private static void ShowEventType()
	{
		AnimatorEventType lastType = m_Event.m_EventType;
		m_Event.m_EventType = (AnimatorEventType)EditorGUILayout.EnumPopup("Event Type", m_Event.m_EventType, EditorStyles.toolbarDropDown);
		if (lastType != m_Event.m_EventType)
		{
			m_Event.m_Option1 = m_Event.m_EventType == AnimatorEventType.EnterState ? false : true;
			m_Event.m_Option3 = false;
			if (m_Event.IsEnterStateEvent)
			{
				m_Event.m_NormalizedTime = -1f;
			}
			else if (m_Event.IsExitStateEvent)
			{
				m_Event.m_NormalizedTime = 2f;
			}
			else
			{
				m_Event.m_NormalizedTime = 0f;
			}
			m_Event.UpdateTime();
		}
	}

	private static void ShowEditableFloatField(GUIContent i_Content, float i_MinValue, float i_MaxValue, ref bool i_Edit, ref float i_Value, bool i_DisabledCondition, string i_ValueFormat, System.Action i_OnUpdate = null)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginDisabledGroup(i_DisabledCondition);
		if (i_Edit)
		{
			i_Value = EditorGUILayout.Slider(i_Content, i_Value, i_MinValue, i_MaxValue);
		}
		else
		{
			string formattedValue = string.Format(i_ValueFormat, i_Value.ToString("F2"));
			EditorGUILayout.LabelField(i_Content, new GUIContent(formattedValue));
		}

		string buttonName = i_Edit ? "Update" : "Edit";
		if (GUILayout.Button(buttonName, EditorStyles.toolbarButton)|| (i_Edit && m_Event.m_ForceUpdate))
		{
			i_Edit = !i_Edit;
			if (!i_Edit && i_OnUpdate != null)
			{
				i_OnUpdate();
			}
			m_Event.m_ForceUpdate = false;
			GUI.FocusControl("");

		}
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
	}

	private static void ShowSender()
	{
		EditorGUI.BeginChangeCheck();
		GUI.SetNextControlName("Sender");
		m_Event.m_Sender = (GameObject)EditorGUILayout.ObjectField("Sender", m_Event.m_Sender, typeof(GameObject), true);
		if (EditorGUI.EndChangeCheck())
		{
			UpdateMethodInfos(m_Event);
		}
	}

	private static void ShowFunctions()
	{
		if (m_Event.m_Sender == null)
		{
			return;
		}
		if (m_Methods == null || m_Methods.Count == 0)
		{
			EditorGUILayout.LabelField("No Function found!");
		}
		else
		{
			int functionIndex = m_MethodNames.IndexOf(m_Event.m_Function);
			EditorGUI.BeginChangeCheck();
			functionIndex = EditorGUILayout.Popup("Function", functionIndex, m_MethodNames.ToArray(), EditorStyles.toolbarDropDown);
			if (EditorGUI.EndChangeCheck())
			{
				m_Event.m_HasChanged = true;
			}
			if (functionIndex >= 0)
			{
				m_Event.m_Function = m_MethodNames[functionIndex];
				m_MethodInfo = m_Methods.
				SelectMany(x => x.Value).
				ToList().
				First(y => y.Name == m_Event.m_Function);
			}

			if (m_MethodInfo != null)
			{
				ParameterInfo[] paramInfos = m_MethodInfo.GetParameters();
				if (paramInfos.Length == 1)
				{
					DrawParameter(paramInfos[0]);
				}
				else
				{
					m_Event.m_ParameterType = TypesEnum.NONE;
					m_Event.m_ObjectReferenceParameter = null;
				}

				ShowInvokeButton();
			}
			else
			{
				EditorGUILayout.LabelField("No Parameters");
			}
		}
	}

	private static void ShowInvokeButton()
	{
		if (GUILayout.Button("Invoke", EditorStyles.toolbarButton))
		{
			m_Event.Invoke();
		}
	}

	private static void DrawParameter(ParameterInfo i_ParamInfo)
	{
		GUI.SetNextControlName("Parameter");
		if (i_ParamInfo.ParameterType.IsEnum)
		{
			m_Event.m_ParameterType = TypesEnum.INT;
			string[] enumValues = System.Enum.GetNames(i_ParamInfo.ParameterType);
			m_Event.m_IntValue = EditorGUILayout.Popup("Enum Value", m_Event.m_IntValue, enumValues);
		}
		else if (i_ParamInfo.ParameterType == typeof(int))
		{
			m_Event.m_ParameterType = TypesEnum.INT;
			m_Event.m_IntValue = EditorGUILayout.IntField("Int Value", m_Event.m_IntValue);
		}
		else if (i_ParamInfo.ParameterType == typeof(float))
		{
			m_Event.m_ParameterType = TypesEnum.FLOAT;
			m_Event.m_FloatValue = EditorGUILayout.FloatField("Float Value", m_Event.m_FloatValue);
		}
		else if (i_ParamInfo.ParameterType == typeof(string))
		{
			m_Event.m_ParameterType = TypesEnum.STRING;
			m_Event.m_StringValue = EditorGUILayout.TextField("String Value", m_Event.m_StringValue);
		}
		else if (i_ParamInfo.ParameterType.IsSubclassOf(typeof(UnityEngine.Object)))
		{
			m_Event.m_ParameterType = TypesEnum.OBJECT;
			m_Event.m_ObjectReferenceParameter = EditorGUILayout.ObjectField("Object Value", (UnityEngine.Object)m_Event.m_ObjectReferenceParameter, i_ParamInfo.ParameterType, true);
			if (m_Event.m_ObjectReferenceParameter != null && i_ParamInfo.ParameterType.GetInterface("IDrawable")!= null)
			{
				i_ParamInfo.ParameterType.InvokeMember("Draw",
					BindingFlags.DeclaredOnly |
					BindingFlags.Public | BindingFlags.NonPublic |
					BindingFlags.Instance | BindingFlags.InvokeMethod, null, m_Event.m_ObjectReferenceParameter, null);
			}
		}
		else
		{
			m_Event.m_ParameterType = TypesEnum.NONE;
			EditorGUILayout.LabelField("Type (" + i_ParamInfo.ParameterType.ToString()+ ") not supported");
			m_Event.m_ObjectReferenceParameter = null;
		}
	}

	private static void UpdateMethodInfos(AnimatorEvent i_Event)
	{
		m_Methods = i_Event.m_Sender == null ? null : i_Event.m_Sender.GetAllMethods(typeof(AnimatorEventAttribute), 1);
		if (m_Methods != null)
		{
			m_MethodNames.Clear();
			foreach (KeyValuePair<string, List<MethodInfo>> kvp in m_Methods)
			{
				foreach (MethodInfo methodInfo in kvp.Value)
				{
					if (!m_MethodNames.Contains(methodInfo.Name))
					{
						//string customName = kvp.Key + "/" + methodInfo.Name;
						m_MethodNames.Add(methodInfo.Name);
					}
					else
					{
						Debug.LogError("The following method name has been found multiple times -> " +
							methodInfo.Name + ".\nConsider changing method names");
					}
				}
			}
		}
	}

	private static void OnNormalizedTimeUpdate()
	{
		m_Event.m_NormalizedTime = m_Event.m_TempNormValue * 0.01f;
		m_Event.UpdateTime();
	}
	private static void OnFixedTimeUpdate()
	{
		m_Event.m_NormalizedTime = m_Event.m_TempFixedValue / m_Event.m_StateLength;
		m_Event.UpdateTime();
	}

	private static void ProcessKeyInputs()
	{
		if ((m_Event.m_EditFixedTime || m_Event.m_EditNormalizedTime)&&
			(Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
		{
			m_Event.m_ForceUpdate = true;
		}
	}
}
#endif