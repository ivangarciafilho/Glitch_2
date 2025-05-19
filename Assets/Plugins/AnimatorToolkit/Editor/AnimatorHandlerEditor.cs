using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BFS;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatorHandler))]
public class AnimatorHandlerEditor : Editor
{
	private struct EventData
	{
		public AnimatorEventType m_Type;
		public string m_State;

		public EventData(AnimatorEventType i_Type, string i_State)
		{
			m_Type = i_Type;
			m_State = i_State;
		}
	}

	private Animator m_Animator;
	private AnimatorHandler m_AnimatorHandler;

	private int m_PreviousEventsCount;
	private AnimatorEvent m_EventToRemove;
	private float m_StateLength;
	private List<Dictionary<string, List<AnimatorEvent>> > m_EventsByName = new List<Dictionary<string, List<AnimatorEvent>> > ();
	private List<List<string>> m_StateNames;
	private List<string> m_FunctionNames;

	#region Get/Set
	private bool ShowAll
	{
		get { return m_AnimatorHandler.m_ShowAll; }
		set { m_AnimatorHandler.m_ShowAll = value; }
	}

	private bool FilterEmpty
	{
		get { return m_AnimatorHandler.m_FilterEmpty; }
		set { m_AnimatorHandler.m_FilterEmpty = value; }
	}

	private int TypesToShow
	{
		get { return m_AnimatorHandler.m_TypesToShow; }
		set { m_AnimatorHandler.m_TypesToShow = value; }
	}

	private string CurrentState
	{
		get { return m_AnimatorHandler.m_CurrentState; }
		set { m_AnimatorHandler.m_CurrentState = value; }
	}

	private int CurrentLayer
	{
		get { return m_AnimatorHandler.m_CurrentLayer; }
		set { m_AnimatorHandler.m_CurrentLayer = value; }
	}
	#endregion

	#region Editor Methods
	public override void OnInspectorGUI()
	{
		m_AnimatorHandler = (AnimatorHandler)target;
		
		if (m_StateNames == null || m_AnimatorHandler.m_EventsChanged || m_PreviousEventsCount != m_AnimatorHandler.Events.Count)
		{
			Setup();
			m_AnimatorHandler.m_EventsChanged = false;
		}

		m_PreviousEventsCount = m_AnimatorHandler.Events.Count;

		if (m_Animator != null && m_Animator.isActiveAndEnabled)
		{
			AnimatorCheck();
		}

		EditorGUILayout.Space();

		ShowLayer();

		if (m_StateNames == null || m_StateNames[CurrentLayer].Count == 0)
		{
			EditorGUILayout.LabelField("No states available for layer: " + CurrentLayer + "!");
			return;
		}

		ShowState();
		ShowMisc();
		ShowEvents();

		if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
		{
			Repaint();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(m_AnimatorHandler);
		}
	}
	#endregion

	#region Check
	private void AnimatorCheck()
	{
		// HACK For some reason, the animator stops corresponding and needs to be reset that way. Must find out why
		// However, it doesn't seem to fix all cases...
		if (m_Animator.layerCount == 0)
		{
			m_Animator.gameObject.SetActive(false);
			m_Animator.gameObject.SetActive(true);
		}
	}
	#endregion

	#region Show
	private void ShowLayer()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField("Layer Selection", EditorStyles.centeredGreyMiniLabel);
		CurrentLayer = EditorGUILayout.IntField("Layer", CurrentLayer);
		CurrentLayer = Mathf.Clamp(CurrentLayer, 0, m_StateNames.Count - 1);
		EditorGUILayout.LabelField("Name", m_Animator.TryGetLayerName(CurrentLayer));
		EditorGUILayout.EndVertical();
		if (EditorGUI.EndChangeCheck())
		{
			CurrentState = null;
		}
	}

	private void ShowState()
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField("State Selection", EditorStyles.centeredGreyMiniLabel);

		EditorGUI.BeginChangeCheck();
		ShowAll = EditorGUILayout.Toggle("Show All States", ShowAll);
		if (EditorGUI.EndChangeCheck()&& ShowAll)
		{
			CurrentState = null;
		}

		if (ShowAll)
		{
			FilterEmpty = EditorGUILayout.Toggle(new GUIContent("Filter Empty", "If true, only states with events will be displayed"), FilterEmpty);
			ShowEventTypes();
		}
		else
		{
			ShowEventTypes();
			if (string.IsNullOrEmpty(CurrentState))
			{
				CurrentState = m_StateNames[CurrentLayer][0];
				m_StateLength = m_Animator.GetLengthByState(CurrentState, CurrentLayer);
				GUI.changed = true;
			}

			int index = m_StateNames[CurrentLayer].IndexOf(CurrentState);
			EditorGUI.BeginChangeCheck();
			index = EditorGUILayout.Popup("Current State", index, m_StateNames[CurrentLayer].ToArray(), EditorStyles.toolbarDropDown);
			if (index >= 0)
			{
				CurrentState = m_StateNames[CurrentLayer][index];
				EditorGUILayout.LabelField("State Length", m_StateLength.ToString("F2")+ " (s)");
			}
			if (EditorGUI.EndChangeCheck())
			{
				m_StateLength = m_Animator.GetLengthByState(CurrentState, CurrentLayer);
			}
		}

		EditorGUILayout.EndVertical();
	}

	private void ShowMisc()
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField("Misc", EditorStyles.centeredGreyMiniLabel);

		if (m_AnimatorHandler != null && m_AnimatorHandler.StatesAtRuntime && m_Animator.runtimeAnimatorController != null)
		{
			ControllerData controllerData = AssetDatabase.LoadAssetAtPath<ControllerData>(AssetDatabase.GetAssetPath(m_Animator.runtimeAnimatorController));
			m_AnimatorHandler.StatesAtRuntime = controllerData != null;
		}

		if (m_Animator.runtimeAnimatorController == null)
		{
			EditorGUILayout.LabelField("No Animator Controller assigned in the animator");
		}
		else
		{
			EditorGUI.BeginChangeCheck();
			m_AnimatorHandler.StatesAtRuntime = EditorGUILayout.Toggle(new GUIContent("States At Runtime", "If true, it will use the ControllerData in the Animator's controller asset in order to get the states' name at runtime"), m_AnimatorHandler.StatesAtRuntime);
			if (EditorGUI.EndChangeCheck())
			{
				if (m_AnimatorHandler.StatesAtRuntime)
				{
					m_AnimatorHandler.Reset();
				}
			}
		}

		EditorGUILayout.EndVertical();
	}

	private void ShowEventTypes()
	{
		string[] eventTypes = System.Enum.GetNames(typeof(AnimatorEventType));
		TypesToShow = EditorGUILayout.MaskField(new GUIContent("Show Event Types", "A mask field where you can select event types to filter"), TypesToShow, eventTypes);
	}

	private void ShowEvents()
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField("Animator Events", EditorStyles.centeredGreyMiniLabel);

		if (m_EventsByName != null && m_EventsByName[CurrentLayer].Count > 0)
		{
			if (ShowAll)
			{
				bool eventFound = false;
				foreach (KeyValuePair<string, List<AnimatorEvent>> kvp in m_EventsByName[CurrentLayer])
				{
					if (FilterEmpty && kvp.Value.Count == 0)
					{
						continue;
					}
					eventFound = true;
					EditorGUILayout.BeginVertical(GUI.skin.box);
					ShowEventsList(kvp.Key, kvp.Value);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
				}
				if (!eventFound)
				{
					GUI.color = AnimatorPreviewer.LightBlue;
					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.LabelField("No Event");
					EditorGUILayout.EndVertical();
				}
			}
			else
			{
				ShowEventsList(CurrentState, m_EventsByName[CurrentLayer][CurrentState]);
				EditorGUILayout.Space();
			}

			if (m_EventToRemove != null)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Cannot remove event while application is playing!");
				}
				else
				{
					m_AnimatorHandler.RemoveEvent(m_EventToRemove);
					m_EventToRemove = null;
					EditorUtility.SetDirty(m_AnimatorHandler);
				}
			}
		}

		EditorGUILayout.EndVertical();
	}

	private void ShowEventsList(string i_State, List<AnimatorEvent> i_Events)
	{
		EditorGUILayout.LabelField(i_State, EditorStyles.centeredGreyMiniLabel);
		int eventsCount = i_Events.Count;
		if (eventsCount == 0)
		{
			GUI.color = AnimatorPreviewer.LightBlue;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.LabelField("No Event");
			EditorGUILayout.EndVertical();
		}
		else
		{
			EditorGUI.indentLevel++;

			bool needUpdate = false;

			for (int i = 0; i < eventsCount; i++)
			{
				if ((TypesToShow & (int)i_Events[i].m_EventType)!= 0)
				{
					GUI.color = AnimatorPreviewer.IsFunctionValid(i_Events[i].m_Sender, i_Events[i].m_Function)? AnimatorPreviewer.LightGreen : AnimatorPreviewer.LightRed;
					EditorGUILayout.BeginVertical(GUI.skin.box);
					AnimatorEvent toRemove = AnimatorPreviewer.ShowEvent(i_Events[i]);
					if (m_EventToRemove == null && toRemove != null)
					{
						m_EventToRemove = toRemove;
					}

					if (i_Events[i].m_HasChanged)
					{
						needUpdate = true;
						i_Events[i].m_HasChanged = false;
						if (Application.isPlaying)
						{
							m_AnimatorHandler.Setup();
						}
						else
						{
							AnimatorPreviewer.SetAllDirty(m_AnimatorHandler);
						}
					}

					EditorGUILayout.EndVertical();
				}
			}

			if (needUpdate)
			{
				UpdateEventsDictionary();
			}
			EditorGUI.indentLevel--;
		}

		GUI.color = Color.green;
		if (GUILayout.Button("Add Event", EditorStyles.toolbarButton))
		{
			if (Application.isPlaying)
			{
				Debug.LogError("Cannot add event while application is playing!");
				return;
			}

			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Enter State"), false, AddAnimatorEvent, new EventData(AnimatorEventType.EnterState, i_State));
			menu.AddItem(new GUIContent("Exit State"), false, AddAnimatorEvent, new EventData(AnimatorEventType.ExitState, i_State));
			menu.AddItem(new GUIContent("Custom"), false, AddAnimatorEvent, new EventData(AnimatorEventType.Custom, i_State));
			menu.ShowAsContext();
		}
		GUI.color = Color.white;
	}
	#endregion

	#region Context Menu
	private void AddAnimatorEvent(object i_EventData)
	{
		EventData eventData = (EventData)i_EventData;
		switch (eventData.m_Type)
		{
			case AnimatorEventType.EnterState:
				m_AnimatorHandler.AddEvent(AnimatorEventType.EnterState, eventData.m_State, -1f, CurrentLayer);
				break;
			case AnimatorEventType.ExitState:
				m_AnimatorHandler.AddEvent(AnimatorEventType.ExitState, eventData.m_State, 2f, CurrentLayer);
				break;
			case AnimatorEventType.Custom:
				m_AnimatorHandler.AddEvent(AnimatorEventType.Custom, eventData.m_State, 0f, CurrentLayer);
				break;
		}
	}
	#endregion

	#region Setup
	private void Setup()
	{
		m_Animator = m_AnimatorHandler.GetComponent<Animator>();
		m_StateNames = m_Animator.GetAllStateNamesByLayers();
		for (int i = 0; i < m_StateNames.Count; i++)
		{
			m_StateNames[i].Sort();
		}

		m_EventsByName.Clear();
		List<AnimatorEvent> events = m_AnimatorHandler.Events;

		// Setup Events Dictionary (For display purpose)
		for (int layer = 0; layer < m_StateNames.Count; layer++)
		{
			m_EventsByName.Add(new Dictionary<string, List<AnimatorEvent>>());
			for (int i = 0; i < m_StateNames[layer].Count; i++)
			{
				if (!m_EventsByName[layer].ContainsKey(m_StateNames[layer][i]))
				{
					m_EventsByName[layer].Add(m_StateNames[layer][i], new List<AnimatorEvent>());
				}
			}
		}

		events = events
			.OrderBy(x => x.m_NormalizedTime)
			.ToList();

		int count = events.Count;
		for (int i = events.Count-1; i >= 0; i--)
		{
			if (!m_EventsByName[events[i].m_Layer].ContainsKey(events[i].m_StateName))
			{
				if (m_AnimatorHandler.RemoveEvent(events[i]))
				{
					Debug.LogError("Removing event with state name (" + events[i].m_StateName + ") since the state doesn't exist anymore. Have you rename or delete a state in the controller?");
					events.RemoveAt(i);
				}
			}
			else
			{
				m_EventsByName[events[i].m_Layer][events[i].m_StateName].Add(events[i]);
			}
		}

		UpdateEventsDictionary();
		if (!string.IsNullOrEmpty(CurrentState))
		{
			m_StateLength = m_Animator.GetLengthByState(CurrentState, CurrentLayer);
		}

		if (!Application.isPlaying)
		{			
			foreach (AnimatorEvent animEvent in m_AnimatorHandler.Events)
			{
				animEvent.EditorSetup();
				
			}
		}
	}

	private void UpdateEventsDictionary()
	{
		// Sort Dictionary's keys
		m_EventsByName.SelectMany(x => x)
			.ToList()
			.OrderByDescending(kp => kp.Value)
			.Select(kp => kp.Key);

		Sort();
	}

	private void Sort()
	{
		// Sort Dictionary's lists
		for (int layer = 0; layer < m_EventsByName.Count; layer++)
		{
			Dictionary<string, List<AnimatorEvent>> copy = new Dictionary<string, List<AnimatorEvent>>(m_EventsByName[layer]);
			foreach (KeyValuePair<string, List<AnimatorEvent>> kvp in m_EventsByName[layer])
			{
				copy[kvp.Key] = kvp.Value
					.OrderBy(x => x.m_NormalizedTime)
					.ThenBy(x => x.m_Function)
					.ThenBy(x => x.m_UniqueID)
					.ToList();
			}
			m_EventsByName[layer] = copy;
		}
	}
	#endregion
}