#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BFS;
using UnityEditor;
using UnityEngine;

public class AnimatorPreviewer : EditorWindow
{
	public static Color LightRed = new Color(1f, 0.8f, 0.8f);
	public static Color LightGreen = new Color(0.8f, 1f, 0.8f);
	public static Color LightBlue = new Color(0.8f, 0.8f, 1f);
	public static Color LightCyan = new Color(0.8f, 1f, 1f);
	public static Color LightMagenta = new Color(1f, 0.8f, 1f);
	public static Color LightOrange = new Color(1f, 0.85f, 0.65f);

	private const float STATE_ZONE_WIDTH = 25f;
	private const float PROGRESS_BAR_HEIGHT = 40f;
	private const float MOUSE_UP_POSITION_THRESHOLD = 5f;
	private const float MOUSE_DOWN_POSITION_THRESHOLD = 285f;

	private static bool m_NeedUpdate;

	private bool m_IsPlaying;
	private bool m_IsPaused;
	private bool m_Loop;
	private bool m_CanDragPreview;
	private AnimatorEventType m_SelectedStateZone = AnimatorEventType.Custom;
	private AnimatorEventType m_PreviousZone = AnimatorEventType.Custom;
	private int m_SelectedEvent = -1;
	private float m_Speed = 1f;
	private float m_UpdateTimer;
	private double m_LastTime;
	private float m_RightClickNormalizedTime;
	private float m_StateLength;
	private int m_CurrentLayer;
	private string m_StateToPreview;

	private Rect m_EnterStateRect;
	private Rect m_ExitStateRect;
	private Vector2 m_ScrollPos;

	private AnimatorEventType m_SelectedEventType;
	private Animator m_Animator;
	private AnimatorHandler m_AnimatorHandler;

	private GUIContent m_ZoomIcon;
	private GUIContent m_AnimationMarkerIcon;

	private List<List<string>> m_StateNames;
	private List<AnimatorEvent> m_CurrentEvents = new List<AnimatorEvent>();
	//private List<AnimatorEvent> m_EventsSnapshots;

	private static GUIStyle m_StateFontStyle;

	[MenuItem("Tools/Animator Toolkit/Animator Previewer...", false, 300)]
	private static void Init()
	{
		EditorWindow window = EditorWindow.GetWindow<AnimatorPreviewer>("Animator Previewer", true);
		window.Show();
	}

	#region Getters	
	private static GUIStyle StateFontStyle
	{
		get
		{
			if (m_StateFontStyle == null)
			{
				m_StateFontStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
				m_StateFontStyle.fontSize = 10;
				m_StateFontStyle.normal.textColor = Color.black;
				m_StateFontStyle.alignment = TextAnchor.UpperCenter;
			}
			return m_StateFontStyle;
		}
	}
	private float NormalizedTime
	{
		get
		{
			if (m_Animator == null)
			{
				return 0f;
			}
			return Mathf.Clamp01(m_Animator.GetCurrentAnimatorStateInfo(m_CurrentLayer).normalizedTime);
		}
	}
	#endregion

	#region Static
	public static void SetAllDirty(AnimatorHandler i_AnimatorHandler, bool i_SaveAll = false)
	{
		if (!Application.isPlaying)
		{
			EditorUtility.SetDirty(i_AnimatorHandler);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
			if (i_SaveAll)
			{
				AssetDatabase.SaveAssets();
				UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
			}
		}
	}

	public static bool IsFunctionValid(GameObject i_GameObject, string i_FunctionName)
	{
		if (i_GameObject == null)
		{
			return false;
		}
		List<string> functionNames = i_GameObject.GetAllMethodNames(typeof(AnimatorEventAttribute), 1);
		return !string.IsNullOrEmpty(i_FunctionName)&& functionNames.Contains(i_FunctionName);
	}

	public static AnimatorEvent ShowEvent(AnimatorEvent i_Event)
	{
		AnimatorEvent toRemove = null;
		EditorGUILayout.BeginHorizontal();
		string foldoutName = string.IsNullOrEmpty(i_Event.m_Function)? "No Function Selected " : i_Event.m_Function;
		if (i_Event.IsCustomEvent)
		{
			foldoutName += " (" + (i_Event.m_NormalizedTime * 100f).ToString("F0")+ "%, " +
				i_Event.FixedTime.ToString("F2")+ "s)";
		}
		else
		{
			foldoutName += "(" + i_Event.m_EventType.ToString()+ ")";
		}
		i_Event.m_Foldout = EditorGUILayout.Foldout(i_Event.m_Foldout, foldoutName, true);

		GUI.color = Color.red;
		if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20f)))
		{
			toRemove = i_Event;
		}
		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();

		if (i_Event.m_Foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginVertical();
			i_Event.Draw();
			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;
		}

		return toRemove;
	}
	#endregion

	#region GUI
	private void OnGUI()
	{
		if (ShowAnimator())
		{
			if (!m_Animator.isActiveAndEnabled)
			{
				GUI.color = Color.red;
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField("Animator's gameObject must be activated!");
				EditorGUILayout.EndVertical();
				GUI.color = Color.white;
				return;
			}
			if (m_StateNames == null)
			{
				m_StateNames = m_Animator == null ? null : m_Animator.GetAllStateNamesByLayers();
				for (int i = 0; i < m_StateNames.Count; i++)
				{
					m_StateNames[i].Sort();
				}
			}

			if (m_AnimatorHandler == null)
			{
				SetupHandler();
			}

			ShowLayer();

			bool eventSelected = false;
			bool canShowStates = false;
			EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(135f + PROGRESS_BAR_HEIGHT));
			canShowStates = ShowStates();
			if (canShowStates)
			{
				ShowOptions();

				GUILayout.Space(5f);
				eventSelected = ShowPreviewBar();
			}
			EditorGUILayout.EndVertical();

			m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
			EditorGUILayout.BeginVertical(GUI.skin.box);

			// FIX A bug ocurring when adding an event in the ENTER or EXIT without being selected at first
			if (Event.current.type != EventType.Ignore)
			{
				if (m_SelectedStateZone != AnimatorEventType.Custom && m_PreviousZone == m_SelectedStateZone)
				{
					ShowStateEvents();
				}
				else if (m_SelectedEvent != -1 && canShowStates && !eventSelected && m_CurrentEvents.Count > 0)
				{
					ShowSelectedEvent();
				}
				else
				{
					if (m_AnimatorHandler == null || m_CurrentEvents.Count == 0)
					{
						EditorGUILayout.LabelField("No animator events. To add one, right-click in the bar above!");
					}
					else
					{
						EditorGUILayout.LabelField("No animator event selected! You can also select 'Enter' or 'Exit'");
					}
				}
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndScrollView();

			m_PreviousZone = m_SelectedStateZone;

			if (GUI.changed && m_AnimatorHandler != null)
			{
				EditorUtility.SetDirty(m_AnimatorHandler);
			}

			if (Application.isPlaying && m_AnimatorHandler != null)
			{
				UpdateHandler();
				Repaint();
			}

			AnimatorCheck();
		}

		if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
		{
			Repaint();
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

	#region Handler
	private void SetupHandler()
	{
		if (m_Animator != null && m_AnimatorHandler == null)
		{
			UpdateHandler();
			if (m_AnimatorHandler != null)
			{
				if (m_SelectedEvent >= m_AnimatorHandler.Events.Count)
				{
					m_SelectedEvent = -1;
					m_SelectedStateZone = AnimatorEventType.Custom;
				}
			}
		}
	}

	private void UpdateHandler()
	{
		if (m_Animator != null)
		{
			m_AnimatorHandler = m_Animator.GetComponent<AnimatorHandler>();
		}
	}
	#endregion

	#region Show
	private bool ShowAnimator()
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);

		EditorGUILayout.LabelField("Select an animator from an object in the scene");
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginHorizontal();
		m_Animator = (Animator)EditorGUILayout.ObjectField(m_Animator, typeof(Animator), true);
		if (m_Animator != null)
		{
			string invalidMessage = null;
			if (!m_Animator.gameObject.activeSelf)
			{
				invalidMessage = "The animator's gameObject must be active!";
			}
			else if (!m_Animator.gameObject.scene.IsValid())
			{
				invalidMessage = "The animator needs to be from an object in the scene!";
			}

			if (!string.IsNullOrEmpty(invalidMessage))
			{
				EditorUtility.DisplayDialog("Invalid Selection", invalidMessage, "ok");
				m_SelectedEvent = -1;
				m_Animator = null;
				GUI.changed = false;
				m_SelectedStateZone = AnimatorEventType.Custom;
			}
		}

		if (EditorGUI.EndChangeCheck()|| m_NeedUpdate)
		{
			m_NeedUpdate = false;
			m_StateToPreview = null;
			m_StateNames = m_Animator == null ? null : m_Animator.GetAllStateNamesByLayers();
			UpdateHandler();
			if (m_Animator != null && m_AnimatorHandler != null)
			{
				m_AnimatorHandler.Setup();
				for (int i = 0; i < m_StateNames.Count; i++)
				{
					m_StateNames[i].Sort();
				}
			}
		}

		if (m_Animator != null)
		{
			if (GUILayout.Button(m_ZoomIcon, EditorStyles.toolbarButton, GUILayout.Width(25f)))
			{
				EditorGUIUtility.PingObject(m_Animator);
				GameObject selectedGO = Selection.activeGameObject;
				Selection.activeGameObject = m_Animator.gameObject;
				SceneView.lastActiveSceneView.FrameSelected();
				Selection.activeGameObject = selectedGO;
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

		return m_Animator != null;
	}

	private void ShowLayer()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginVertical(GUI.skin.box);
		m_CurrentLayer = EditorGUILayout.IntField("Layer", m_CurrentLayer);
		m_CurrentLayer = Mathf.Clamp(m_CurrentLayer, 0, m_StateNames.Count - 1);
		EditorGUILayout.LabelField("Name", m_Animator.TryGetLayerName(m_CurrentLayer));
		EditorGUILayout.EndVertical();
		if (EditorGUI.EndChangeCheck())
		{
			m_StateToPreview = null;
		}
	}

	private bool ShowStates()
	{
		if (m_StateNames == null || m_StateNames[m_CurrentLayer].Count == 0)
		{
			m_StateToPreview = null;
			EditorGUILayout.LabelField("No States found in the animator!");
			return false;
		}

		EditorGUI.BeginChangeCheck();
		if (string.IsNullOrEmpty(m_StateToPreview))
		{
			m_StateToPreview = m_StateNames[m_CurrentLayer][0];
			GUI.changed = true;
		}

		int index = m_StateNames[m_CurrentLayer].IndexOf(m_StateToPreview);
		index = EditorGUILayout.Popup("State To Preview", index, m_StateNames[m_CurrentLayer].ToArray(), EditorStyles.toolbarDropDown);
		if (index >= 0)
		{
			m_StateToPreview = m_StateNames[m_CurrentLayer][index];
			EditorGUILayout.LabelField("State Length", m_StateLength.ToString("F2")+ " (s)");
		}
		if (EditorGUI.EndChangeCheck())
		{
			m_SelectedEvent = -1;
			m_SelectedStateZone = AnimatorEventType.Custom;
			UpdateEvents();
			m_StateLength = m_Animator.GetLengthByState(m_StateToPreview, m_CurrentLayer);
			m_Animator.Play(m_StateToPreview, m_CurrentLayer, 0f);
		}

		return true;
	}

	private void UpdateEvents()
	{
		if (m_AnimatorHandler != null)
		{
			m_CurrentEvents = GetEventsByStateName(m_AnimatorHandler.Events, m_StateToPreview, m_CurrentLayer);
			if (m_CurrentEvents != null)
			{
				foreach (AnimatorEvent animEvent in m_CurrentEvents)
				{
					animEvent.EditorSetup();
				}
			}
		}
	}

	private void ShowOptions()
	{
		if (m_CurrentLayer == 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUI.color = (m_IsPlaying && !m_IsPaused)? Color.cyan : Color.green;
			string playState = (m_IsPlaying && !m_IsPaused)? "Pause" : "Play";
			if (GUILayout.Button(playState, EditorStyles.toolbarButton))
			{
				if (!m_IsPlaying || m_IsPaused)
				{
					m_IsPlaying = true;
					m_IsPaused = false;

					SetNormalizedTime(NormalizedTime >= 1f && !m_Loop ? 0f : NormalizedTime, true);
				}
				else
				{
					m_IsPaused = true;
				}
			}

			GUILayout.Space(2f);

			GUI.color = m_IsPlaying ? Color.red : Color.grey;
			EditorGUI.BeginDisabledGroup(!m_IsPlaying);
			if (GUILayout.Button("Stop", EditorStyles.toolbarButton))
			{
				Stop();
			}
			EditorGUI.EndDisabledGroup();

			GUI.color = Color.white;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginHorizontal(GUI.skin.box);
			EditorGUILayout.LabelField("Speed", GUILayout.Width(40f));
			m_Speed = EditorGUILayout.Slider(m_Speed, 0.01f, 1f);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GUI.skin.box);
			EditorGUILayout.LabelField("Loop?", GUILayout.Width(37f));
			m_Loop = EditorGUILayout.Toggle(m_Loop, GUILayout.Width(20f));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GUI.skin.box);
			if (!Application.isPlaying)
			{
				if (m_AnimatorHandler == null)
				{
					EditorGUILayout.LabelField("No AnimatorHandler on Animator");
				}
				else
				{
					EditorGUILayout.LabelField("Test Events In Edit Mode?", GUILayout.Width(150f));
					m_AnimatorHandler.m_ExecuteInEditMode = EditorGUILayout.Toggle(m_AnimatorHandler.m_ExecuteInEditMode);
				}
			}
			else
			{
				EditorGUILayout.LabelField("Preview while playing can create unexpected behaviours. Use it carefully");
			}
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			GUI.color = Color.red;
			EditorGUILayout.BeginHorizontal(GUI.skin.box);
			GUI.color = Color.white;
			EditorGUILayout.LabelField("Preview only works for "+m_Animator.GetLayerName(0)+"/Layer 0 for now");
			EditorGUILayout.EndHorizontal();
		}
	}

	private bool ShowPreviewBar()
	{
		Rect rect = EditorGUILayout.GetControlRect();

		if (Event.current.type != EventType.Layout)
		{
			rect.x += STATE_ZONE_WIDTH;
			rect.width -= STATE_ZONE_WIDTH * 2f;
			rect.y -= 5f;
			rect.height = PROGRESS_BAR_HEIGHT;
			EditorGUI.ProgressBar(rect, NormalizedTime, "");
		}

		ShowStateZone(rect, true);
		ShowStateZone(rect, false);

		Rect barTitleRect = rect;
		barTitleRect.y += 10f;

		GUIStyle centeredLabel = GUI.skin.GetStyle("Label");
		centeredLabel.alignment = TextAnchor.UpperCenter;
		EditorGUI.LabelField(barTitleRect, "(" + (NormalizedTime * 100f).ToString("F2")+ "%)", centeredLabel);

		bool eventSelected = ShowAnimatorEvents(rect);
		if (!eventSelected)
		{
			ProcessBarClick(rect);
		}

		return eventSelected;
	}

	private void ShowStateZone(Rect i_SourceRect, bool i_IsEnterState)
	{
		Rect frame = i_SourceRect;
		frame.width = STATE_ZONE_WIDTH;
		frame.x = i_IsEnterState ?
			(frame.x - STATE_ZONE_WIDTH):
			(frame.x + i_SourceRect.width);
		if (i_IsEnterState)
		{
			m_EnterStateRect = frame;
		}
		else
		{
			m_ExitStateRect = frame;
		}
		EditorGUI.DrawRect(frame, Color.grey);
		frame.x += 1f;
		frame.y += 1f;
		frame.width -= 2f;
		frame.height -= 2f;
		EditorGUI.DrawRect(frame, i_IsEnterState ?
			(m_SelectedStateZone == AnimatorEventType.EnterState ? Color.cyan : LightCyan):
			(m_SelectedStateZone == AnimatorEventType.ExitState ? Color.magenta : LightMagenta));

		frame.y += 40f;
		int count = i_IsEnterState ?
			m_CurrentEvents.FindAll(x => x.IsEnterStateEvent).Count :
			m_CurrentEvents.FindAll(x => x.IsExitStateEvent).Count;

		EditorGUI.LabelField(frame, new GUIContent(count.ToString(), "Number of events in " + (i_IsEnterState ? "Enter" : "Exit")+ " state)"), StateFontStyle);

		Rect stateRect = i_SourceRect;
		stateRect.x = i_IsEnterState ?
			(stateRect.x - i_SourceRect.width * 0.5f - 2f):
			(stateRect.x + i_SourceRect.width * 0.5f + STATE_ZONE_WIDTH - 5f);

		Vector2 pivotPoint = new Vector2(stateRect.x + stateRect.width * 0.5f, stateRect.y + stateRect.height * 0.5f);
		GUIUtility.RotateAroundPivot(-90f, pivotPoint);
		GUI.Label(stateRect, i_IsEnterState ? "ENTER" : "EXIT", StateFontStyle);
		GUIUtility.RotateAroundPivot(90f, pivotPoint);
	}

	private void ShowStateEvents()
	{
		List<AnimatorEvent> stateEvents = m_CurrentEvents
			.Where(x => x.m_EventType == m_SelectedStateZone)
			.OrderBy(x => x.m_NormalizedTime)
			.ThenBy(x => x.m_Function)
			.ThenBy(x => x.m_UniqueID)
			.ToList();

		if (stateEvents.Count == 0)
		{
			EditorGUILayout.LabelField("No animator events. To add one, right-click in the bar above!");
			return;
		}

		EditorGUILayout.LabelField(m_SelectedStateZone == AnimatorEventType.EnterState ? "Enter State Events" : "Exit State Events");
		for (int i = 0; i < stateEvents.Count; i++)
		{
			GUI.color = IsFunctionValid(stateEvents[i].m_Sender, stateEvents[i].m_Function)? LightGreen : LightRed;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			AnimatorEvent toRemove = ShowEvent(stateEvents[i]);
			if (toRemove != null)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Cannot remove event while application is playing!");
				}
				else
				{
					m_AnimatorHandler.RemoveEvent(toRemove);
					i--;
					UpdateEvents();
				}
			}
			EditorGUILayout.EndVertical();
		}
	}

	private void ShowSelectedEvent()
	{
		if (m_SelectedEvent >= 0 && m_SelectedEvent < m_CurrentEvents.Count)
		{
			EditorGUILayout.LabelField("Selected Event");
			m_CurrentEvents[m_SelectedEvent].Draw();
			if (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl())&&
				Event.current.type != EventType.Layout &&
				(Event.current.keyCode == KeyCode.Delete ||
					Event.current.keyCode == KeyCode.Backspace))
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Cannot remove event while application is playing!");
					return;
				}
				m_AnimatorHandler.RemoveEvent(m_CurrentEvents[m_SelectedEvent]);
				m_CurrentEvents.RemoveAt(m_SelectedEvent);
				m_SelectedEvent = -1;
				UpdateEvents();
				Repaint();
			}
		}
	}

	private bool ShowAnimatorEvents(Rect i_Rect)
	{
		if (m_AnimatorHandler != null && !string.IsNullOrEmpty(m_StateToPreview))
		{
			Vector2 mousePosition = Event.current.mousePosition;
			Rect iconRect = i_Rect;
			iconRect.x += 1f;
			iconRect.width = m_AnimationMarkerIcon.image.width + 2f;
			iconRect.height = m_AnimationMarkerIcon.image.height;
			i_Rect.width -= m_AnimationMarkerIcon.image.width;
			for (int i = 0; i < m_CurrentEvents.Count; i++)
			{
				if (m_CurrentEvents[i].IsCustomEvent)
				{
					GUI.color = i == m_SelectedEvent ? Color.blue : Color.white;
					i_Rect.x = m_CurrentEvents[i].m_NormalizedTime * i_Rect.width;
					i_Rect.x += STATE_ZONE_WIDTH + m_AnimationMarkerIcon.image.width;
					i_Rect.x = Mathf.Clamp(i_Rect.x, 5f, i_Rect.x);
					iconRect.x = i_Rect.x;
					EditorGUI.LabelField(i_Rect, m_AnimationMarkerIcon);
					bool mouseOnIcon = iconRect.Contains(mousePosition);

					if (Event.current.type == EventType.MouseDown)
					{
						if (mouseOnIcon)
						{
							m_SelectedEvent = i;
							m_SelectedStateZone = AnimatorEventType.Custom;
							GUI.color = Color.white;
							GUI.FocusControl("");
							// TODO Undo function
							//m_EventsSnapshots = new List<AnimatorEvent>();
							//for (int j = 0; j < m_CurrentEvents.Count; j++)
							//{
							//	m_EventsSnapshots.Add(new AnimatorEvent(m_CurrentEvents[j]));
							//}
							//
							//Undo.undoRedoPerformed -= OnUndo;
							//Undo.undoRedoPerformed += OnUndo;
							Repaint();
							return true;
						}
						Repaint();
					}
				}

				// Show ToolTip?
				//if (mouseOnIcon)
				//{
				//	GUI.tooltip = string.IsNullOrEmpty(m_CurrentEvents[i].m_Message)? "No Function Selected" : m_CurrentEvents[i].m_Message;
				//	iconRect.width = 150f;
				//	iconRect.height = 20f;
				//	GUI.Label(iconRect, GUI.tooltip);
				//}
			}
			GUI.color = Color.white;

			// Check if mouse inputs can be triggered
			if (mousePosition.y > MOUSE_UP_POSITION_THRESHOLD && mousePosition.y < MOUSE_DOWN_POSITION_THRESHOLD)
			{
				if (Event.current.type == EventType.MouseDown)
				{
					if (!m_EnterStateRect.Contains(mousePosition)&& !m_ExitStateRect.Contains(mousePosition))
					{
						m_SelectedStateZone = AnimatorEventType.Custom;
					}
					m_SelectedEvent = -1;
					GUI.FocusControl("");
					Repaint();
				}
				else if (m_SelectedEvent != -1 && Event.current.type == EventType.MouseDrag)
				{
					m_CurrentEvents[m_SelectedEvent].m_NormalizedTime = Mathf.Clamp01((mousePosition.x - STATE_ZONE_WIDTH - 8f)/ i_Rect.width);
					m_CurrentEvents[m_SelectedEvent].UpdateTime();
					AnimatorPreviewer.SetAllDirty(m_AnimatorHandler);
					Repaint();
				}
			}
		}

		return false;
	}

	private void OnUndo()
	{
		Undo.undoRedoPerformed -= OnUndo;
		//if (m_EventsSnapshots != null && m_AnimatorHandler != null)
		//{
		//	m_AnimatorHandler.CopyEvents(m_EventsSnapshots);
		//}
		EditorUtility.SetDirty(m_AnimatorHandler);
		Repaint();
	}

	private void ProcessBarClick(Rect i_Rect)
	{
		Vector2 mousePosition = Event.current.mousePosition;
		if (m_SelectedEvent == -1 || m_CanDragPreview)
		{
			OnProgressBarInput(i_Rect, OnProgressCenterLeftMouse, OnProgressCenterRightMouse, true);
			OnProgressBarInput(m_EnterStateRect, OnEnterStateLeftMouse, OnEnterStateRightMouse, false);
			OnProgressBarInput(m_ExitStateRect, OnExitStateLeftMouse, OnExitStateRightMouse, false);
		}

		if (m_CanDragPreview && Event.current.isMouse && Event.current.type == EventType.MouseUp)
		{
			m_CanDragPreview = false;
		}
	}

	private void OnProgressBarInput(Rect i_Rect, System.Action<Rect> i_OnLeftMouse, System.Action<Rect> i_OnRightMouse, bool i_EnableMouseDrag = false)
	{
		Vector2 mousePosition = Event.current.mousePosition;
		if (i_Rect.Contains(mousePosition)|| (m_CanDragPreview && i_EnableMouseDrag))
		{
			if (Event.current.isMouse)
			{
				if (Event.current.button != 1)
				{
					if ((i_EnableMouseDrag && Event.current.type == EventType.MouseDrag)|| Event.current.clickCount >= 1)
					{
						if (i_OnLeftMouse != null)
						{
							i_OnLeftMouse(i_Rect);
						}
					}
				}
				else if (Event.current.button == 1)
				{
					if (Event.current.type == EventType.MouseUp)
					{
						if (i_OnRightMouse != null)
						{
							i_OnRightMouse(i_Rect);
						}
					}
				}
			}
		}
	}
	#endregion

	#region Progress Bar Inputs
	private void OnProgressCenterLeftMouse(Rect i_Rect)
	{
		GUI.FocusControl("");
		m_CanDragPreview = true;
		m_IsPaused = true;
		SetNormalizedTime((Event.current.mousePosition.x - i_Rect.x)/ i_Rect.width, true);
		Repaint();
	}

	private void OnProgressCenterRightMouse(Rect i_Rect)
	{
		m_SelectedEventType = AnimatorEventType.Custom;
		m_RightClickNormalizedTime = (Event.current.mousePosition.x - i_Rect.x)/ i_Rect.width;
		ShowAnimatorEvent(i_Rect);
	}

	private void OnEnterStateLeftMouse(Rect i_Rect)
	{
		m_SelectedStateZone = AnimatorEventType.EnterState;
		GUI.FocusControl("");
		m_IsPaused = true;
		Repaint();
	}

	private void OnEnterStateRightMouse(Rect i_Rect)
	{
		m_SelectedEventType = AnimatorEventType.EnterState;
		m_RightClickNormalizedTime = -1f;
		ShowAnimatorEvent(i_Rect);
	}

	private void OnExitStateLeftMouse(Rect i_Rect)
	{
		m_SelectedStateZone = AnimatorEventType.ExitState;
		GUI.FocusControl("");
		m_IsPaused = true;
		Repaint();
	}

	private void OnExitStateRightMouse(Rect i_Rect)
	{
		m_SelectedEventType = AnimatorEventType.ExitState;
		m_RightClickNormalizedTime = 2f;
		ShowAnimatorEvent(i_Rect);
	}
	#endregion

	#region Context Menu
	private void ShowAnimatorEvent(Rect i_Rect)
	{
		GenericMenu menu = new GenericMenu();

		menu.AddItem(new GUIContent("Add Animator Event"), false, AddAnimatorEvent);
		menu.ShowAsContext();
	}

	private void AddAnimatorEvent()
	{
		if (Application.isPlaying)
		{
			Debug.LogError("Cannot add event while application is playing!");
			return;
		}

		if (m_AnimatorHandler == null)
		{
			if (Application.isPlaying)
			{
				Debug.LogError("AnimatorHandler automatically added to animator's gameObject at runtime. It won't be saved!");
			}
			else
			{
				Debug.LogWarning("AnimatorHandler automatically added to animator's gameObject!");
			}
			m_AnimatorHandler = m_Animator.gameObject.AddComponent<AnimatorHandler>();
		}
		else
		{
			UpdateHandler();
		}
		m_AnimatorHandler.Setup();

		AnimatorEvent animatorEvent = m_AnimatorHandler.AddEvent(m_SelectedEventType, m_StateToPreview, m_RightClickNormalizedTime, m_CurrentLayer);

		UpdateEvents();
		if (m_SelectedEventType == AnimatorEventType.EnterState)
		{
			m_SelectedStateZone = AnimatorEventType.EnterState;
		}
		else if (m_SelectedEventType == AnimatorEventType.ExitState)
		{
			m_SelectedStateZone = AnimatorEventType.ExitState;
		}
		else
		{
			m_SelectedStateZone = AnimatorEventType.Custom;
			for (int i = 0; i < m_CurrentEvents.Count; i++)
			{
				if (m_CurrentEvents[i] == animatorEvent)
				{
					m_SelectedEvent = i;
					break;
				}
			}
		}
		UpdateEvents();
	}
	#endregion

	#region Editor Methods
	private void OnFocus()
	{
		SetupHandler();
		if (m_AnimatorHandler != null && !string.IsNullOrEmpty(m_StateToPreview)&& !Application.isPlaying)
		{
			UpdateEvents();
			m_AnimatorHandler.Setup();
		}
	}

	private void OnLostFocus()
	{
		if (m_Animator != null && m_Animator.isActiveAndEnabled)
		{
			Stop();
		}
	}

	private void OnEnable()
	{
		m_AnimationMarkerIcon = EditorGUIUtility.IconContent("Animation.EventMarker");
		m_ZoomIcon = EditorGUIUtility.IconContent("ViewToolZoom");
		m_ZoomIcon.tooltip = "Zoom on the animator's gameObject in the scene and";
	}

	private void Update()
	{
		if (m_Animator != null && m_IsPlaying && !m_IsPaused)
		{
			Play();
		}
	}
	#endregion

	#region Animator
	private void Play()
	{
		float delta = (float)(EditorApplication.timeSinceStartup - m_LastTime);
		m_UpdateTimer += delta * m_Speed;
		m_Animator.CrossFadeInFixedTime(m_StateToPreview, 0f, m_CurrentLayer, m_UpdateTimer);
		m_Animator.Update(0.0001f); // Need to force a small time, otherwise, it could stop playing

		if (m_AnimatorHandler != null && m_AnimatorHandler.m_ExecuteInEditMode)
		{
			m_AnimatorHandler.ProcessLayer(m_CurrentLayer);
		}

		// HACK With this, we can play any animation clip but we need to activate animation preview...
		/*
		AnimationMode.StartAnimationMode();
		if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
		{
			AnimationClip clip = m_Animator.GetClipByState(m_StateToPreview, m_CurrentLayer);

			AnimationMode.BeginSampling();
			AnimationMode.SampleAnimationClip(m_Animator.gameObject, clip, m_UpdateTimer);
			AnimationMode.EndSampling();
			if (m_UpdateTimer > clip.length)
			{
				m_UpdateTimer = 0f;
			}
			SceneView.RepaintAll();
		}
		*/

		if (NormalizedTime >= 1f)
		{
			if (m_Loop)
			{
				SetNormalizedTime();
			}
			else
			{
				m_IsPlaying = false;
			}
		}
		Repaint();

		m_LastTime = EditorApplication.timeSinceStartup;
	}

	private void Stop()
	{
		SetNormalizedTime(0f);
		m_IsPlaying = false;
		m_IsPaused = false;
	}

	private void SetNormalizedTime(float i_NormalizedTime = 0f, bool i_ForcePlay = false)
	{
		if (m_Animator != null)
		{
			if (!Application.isPlaying || i_ForcePlay)
			{
				m_Animator.Play(m_StateToPreview, m_CurrentLayer, i_NormalizedTime);
				m_Animator.Update(0.0001f); // Need to force a small time, otherwise, it could stop playing
			}
			m_UpdateTimer = m_Animator.GetCurrentFixedTime(m_CurrentLayer);
			m_LastTime = EditorApplication.timeSinceStartup;
		}
	}
	#endregion

	#region Misc
	public static void ForceUpdate()
	{
		m_NeedUpdate = true;
	}

	private static List<AnimatorEvent> GetEventsByStateName(List<AnimatorEvent> i_Events, string i_StateName, int i_Layer = 0)
	{
		List<AnimatorEvent> eventsByName = new List<AnimatorEvent>();
		if (!string.IsNullOrEmpty(i_StateName))
		{
			for (int i = 0; i < i_Events.Count; i++)
			{
				if (i_Events[i].m_Layer == i_Layer && i_Events[i].m_StateName.ToLower()== i_StateName.ToLower())
				{
					eventsByName.Add(i_Events[i]);
				}
			}
		}
		else
		{
			Debug.LogWarning("AnimatorPreviewer -> GetEventsByStateName: i_StateName is Null or Empty!");
		}
		return eventsByName;
	}
	#endregion
}
#endif