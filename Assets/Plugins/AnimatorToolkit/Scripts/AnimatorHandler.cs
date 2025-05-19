using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BFS;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class AnimatorHandler : MonoBehaviour
{
	private struct LayerInfo
	{
		public float m_NormalizedTime;
		public float m_NextNormalizedTime;
		public int m_Hash;
		public int m_NextHash;
		public bool m_IsTransitioning;
	}

	[SerializeField] private List<AnimatorEvent> m_Events = new List<AnimatorEvent>();
	[SerializeField] private ControllerData m_ControllerData;

	[SerializeField] private bool m_StatesAtRuntime;

	private bool m_Initialized;
	private LayerInfo[] m_LayerInfos;
	private Animator m_Animator;
	private Lookup<int, AnimatorEvent> m_EventsGroup;

#if UNITY_EDITOR
	public bool m_ShowAll;
	public bool m_FilterEmpty = true;
	public bool m_ExecuteInEditMode = false;
	public int m_TypesToShow = ~0;
	public int m_CurrentLayer;
	public string m_CurrentState;
	[HideInInspector] public bool m_EventsChanged = false;
#endif

	#region Public Getters
	public bool StatesAtRuntime
	{
		get { return m_StatesAtRuntime; }
		set { m_StatesAtRuntime = value; }
	}

	public string GetCurrentStateName(int i_Layer)
	{
		return GetStateName(i_Layer, Anim.GetCurrentAnimatorStateInfo(i_Layer).shortNameHash);
	}

	public string GetNextStateName(int i_Layer)
	{
		return GetStateName(i_Layer, Anim.GetNextAnimatorStateInfo(i_Layer).shortNameHash);
	}

	public List<AnimatorEvent> Events
	{
		get { return m_Events; }
	}
	#endregion

	#region Private Getters
	private string GetStateName(int i_Layer, int i_HashName)
	{
		if (!m_StatesAtRuntime)
		{
			Debug.LogWarning("Couldn't GetStateName since StatesAtRuntime has been disabled!");
			return "";
		}
		if (m_ControllerData != null)
		{
			return m_ControllerData.GetStateName(i_Layer, i_HashName);
		}
		Debug.LogError("Couldn't GetStateName since Controller Data hasn't been found/destroyed");
		return "";
	}

	private Animator Anim
	{
		get
		{
			if (m_Animator == null)
			{
				m_Animator = GetComponent<Animator>();
			}

			return m_Animator;
		}
	}
	#endregion

	#region Mono Methods
	private void Awake()
	{
		Setup();
	}

	private void LateUpdate()
	{
		for (int layer = 0; layer < m_LayerInfos.Length; layer++)
		{
			ProcessLayer(layer);
		}
	}

#if UNITY_EDITOR
	private void OnEnable()
	{
		for (int i = 0; i < m_Events.Count; i++)
		{
			AnimatorEventDrawer.EditorSetup(m_Events[i], true);
		}
	}

	private void OnValidate()
	{
		for (int i = 0; i < m_Events.Count; i++)
		{
			AnimatorEventDrawer.EditorSetup(m_Events[i], true);
		}
	}

	public void Reset()
	{
		if (Anim.runtimeAnimatorController != null && m_ControllerData == null)
		{
			m_ControllerData = ControllerData.AddOrGet(Anim.runtimeAnimatorController);
		}
		m_StatesAtRuntime = m_ControllerData != null;
	}
#endif
	#endregion

	#region Process
	public void ProcessLayer(int i_Layer)
	{
#if UNITY_EDITOR
		if (m_LayerInfos == null)
		{
			Setup();
			return;
		}
		if (!m_Animator.isInitialized || m_LayerInfos.Length == 0)
		{
			Debug.LogWarning("Can't Process layer since the animator stopped responding! Try to toggle the animator's gameObject On/Off");
			return;
		}
		//Debug.Log("Layer " + i_Layer + "'s current state = " + GetCurrentStateName(i_Layer));
#endif
		AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(i_Layer);
		AnimatorStateInfo nextStateInfo = m_Animator.GetNextAnimatorStateInfo(i_Layer);

		float normalizedTime = stateInfo.normalizedTime;
		int hash = stateInfo.shortNameHash;
		int nextHash = nextStateInfo.shortNameHash;

		// Check events in current state
		if (m_EventsGroup.Contains(hash))
		{
			foreach (AnimatorEvent animEvent in m_EventsGroup[hash])
			{
				if (CanTriggerEvent(animEvent, stateInfo, nextStateInfo, i_Layer))
				{
					animEvent.Invoke();
				}
			}
		}
		// Check events in next state
		if (m_EventsGroup.Contains(nextHash))
		{
			foreach (AnimatorEvent animEvent in m_EventsGroup[nextHash])
			{
				if (CanTriggerNextEvent(animEvent, nextStateInfo, i_Layer))
				{
					animEvent.Invoke();
				}
			}
		}
		m_LayerInfos[i_Layer].m_NormalizedTime = normalizedTime;
		m_LayerInfos[i_Layer].m_NextNormalizedTime = nextStateInfo.normalizedTime;
		m_LayerInfos[i_Layer].m_Hash = hash;
		m_LayerInfos[i_Layer].m_NextHash = nextHash;
		m_LayerInfos[i_Layer].m_IsTransitioning = m_Animator.IsInTransition(i_Layer);
	}

	private bool CanTriggerEvent(AnimatorEvent i_Event, AnimatorStateInfo i_CurrentInfo, AnimatorStateInfo i_NextInfo, int i_Layer)
	{
		if (i_Event.m_Layer == i_Layer)
		{
			bool isEntering = m_LayerInfos[i_Layer].m_Hash != i_CurrentInfo.shortNameHash && i_CurrentInfo.shortNameHash != 0;
			bool isExiting = m_LayerInfos[i_Layer].m_NextHash != i_NextInfo.shortNameHash && i_NextInfo.shortNameHash != 0;

			switch (i_Event.m_EventType)
			{
				case AnimatorEventType.EnterState:
					return m_LayerInfos[i_Layer].m_Hash == 0 ||
						(!i_Event.m_Option1 &&
							isEntering &&
							!m_LayerInfos[i_Layer].m_IsTransitioning)||
						(i_Event.m_Option1 &&
							m_LayerInfos[i_Layer].m_IsTransitioning &&
							!m_Animator.IsInTransition(i_Layer));

				case AnimatorEventType.ExitState:
					return m_LayerInfos[i_Layer].m_NextHash != i_Event.m_NameHash &&
						((!i_Event.m_Option1 &&
								m_LayerInfos[i_Layer].m_NormalizedTime < 1f &&
								i_CurrentInfo.normalizedTime >= 1f)||
							((i_Event.m_Option1 || i_CurrentInfo.normalizedTime < 1f)&&
								isExiting));

				case AnimatorEventType.Custom:
					// Check if the event can be called on exit state
					if (i_Event.m_Option2 &&
						i_CurrentInfo.normalizedTime < i_Event.m_NormalizedTime &&
						m_LayerInfos[i_Layer].m_NextHash != i_Event.m_NameHash &&
						isExiting)
					{
						return true;
					}
					float layerTime = m_LayerInfos[i_Layer].m_NormalizedTime;
					if (layerTime > 1f && (i_Event.m_Option3 || !i_CurrentInfo.loop))
					{
						return false;
					}
					if (m_LayerInfos[i_Layer].m_Hash != i_CurrentInfo.shortNameHash)
					{
						return false;
					}
					float clampedNormalized = i_CurrentInfo.normalizedTime - (int)i_CurrentInfo.normalizedTime;
					float clampedLayerNormalized = layerTime - (int)layerTime;

					return IsInEventThreshold(i_Event, clampedNormalized, clampedLayerNormalized, i_CurrentInfo.loop);
			}
		}

		return false;
	}

	private bool CanTriggerNextEvent(AnimatorEvent i_Event, AnimatorStateInfo i_NextInfo, int i_Layer)
	{
		if (i_Event.m_Layer == i_Layer && i_Event.m_EventType != AnimatorEventType.ExitState)
		{
			bool isInTransition = m_Animator.IsInTransition(i_Layer)&&
				i_NextInfo.shortNameHash == i_Event.m_NameHash;

			switch (i_Event.m_EventType)
			{
				case AnimatorEventType.EnterState:
					return isInTransition &&
						!m_LayerInfos[i_Layer].m_IsTransitioning &&
						!i_Event.m_Option1;
				case AnimatorEventType.Custom:
					// Check if the event can be called while in transition
					return isInTransition &&
						i_Event.m_Option1 &&
						IsInEventThreshold(i_Event, i_NextInfo.normalizedTime, m_LayerInfos[i_Layer].m_NextNormalizedTime, i_NextInfo.loop);
			}
		}

		return false;
	}
	#endregion

	#region Comparison
	private bool IsInEventThreshold(AnimatorEvent i_Event, float i_SourceTime, float i_LastTime, bool i_IsLoop = true)
	{
		// That means there's a loop
		if (i_LastTime > i_SourceTime)
		{
			if (i_Event.m_NormalizedTime >= i_LastTime)
			{
				return true;
			}
			if (i_Event.m_Option3)
			{
				return false;
			}
			if (i_IsLoop)
			{
				i_LastTime = 0f;
			}
		}
		return i_SourceTime >= i_Event.m_NormalizedTime && i_LastTime <= i_Event.m_NormalizedTime;
	}
	#endregion

	#region Setup
	public void Setup()
	{
		m_Animator = GetComponent<Animator>();
		if (Application.isPlaying && !m_Initialized)
		{
			m_Initialized = true;
			m_EventsGroup = (Lookup<int, AnimatorEvent>)m_Events.ToLookup<AnimatorEvent, int, AnimatorEvent>(p => p.m_NameHash, p => p);
			m_LayerInfos = new LayerInfo[m_Animator.layerCount];
		}
	}
	#endregion

	#region Events Management
	public AnimatorEvent AddEvent(AnimatorEvent i_Event)
	{
		m_Events.Add(i_Event);
#if UNITY_EDITOR
		i_Event.UpdateTime();
		m_EventsChanged = true;
		Setup();
		AnimatorPreviewer.SetAllDirty(this);
#endif
		return i_Event;
	}

	public AnimatorEvent AddEvent(AnimatorEventType i_EventType, string i_StateName, float i_NormalizedTime, int i_Layer)
	{
		AnimatorEvent animatorEvent = new AnimatorEvent();
#if UNITY_EDITOR
		int maxIndex = m_Events.Count == 0 ? 0 : m_Events.Max(x => x.m_UniqueID)+ 1;
		animatorEvent.m_UniqueID = maxIndex;
#endif
		animatorEvent.m_EventType = i_EventType;
		animatorEvent.m_Layer = i_Layer;
		animatorEvent.m_Option1 = animatorEvent.m_EventType == AnimatorEventType.EnterState ? false : true;
		animatorEvent.m_Sender = Anim.gameObject;
		animatorEvent.m_StateName = i_StateName;
		animatorEvent.m_NormalizedTime = i_NormalizedTime;
#if UNITY_EDITOR
		animatorEvent.m_StateLength = Anim.GetLengthByState(i_StateName, i_Layer);
		animatorEvent.m_NameHash = Anim.GetNameHashByState(i_StateName, i_Layer);
#endif
		return AddEvent(animatorEvent);
	}

	public void RemoveEventAt(int i_Index)
	{
		if (i_Index >= 0 && i_Index < m_Events.Count)
		{
			m_Events.RemoveAt(i_Index);
#if UNITY_EDITOR
			m_EventsChanged = true;
			Setup();
			AnimatorPreviewer.SetAllDirty(this);
#endif
		}
	}

	public bool RemoveEvent(AnimatorEvent i_Event)
	{
#if UNITY_EDITOR
		if (m_Events.Remove(i_Event))
		{
			m_EventsChanged = true;
			Setup();
			AnimatorPreviewer.SetAllDirty(this);
			return true;
		}
		return false;
#else
		return m_Events.Remove(i_Event);
#endif
	}

#if UNITY_EDITOR
	public void CopyEvents(List<AnimatorEvent> i_Events)
	{
		m_Events = i_Events;
		Setup();
	}
#endif
	#endregion
}