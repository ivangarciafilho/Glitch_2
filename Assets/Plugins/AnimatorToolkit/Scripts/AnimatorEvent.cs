using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace BFS
{
	public enum AnimatorEventType
	{
		EnterState = 1,
		ExitState = 2,
		Custom = 4
	}

	[System.Serializable]
	public class AnimatorEvent
	{
#if UNITY_EDITOR
		public int m_UniqueID;
		public bool m_HasChanged;
		public bool m_Foldout;
		public bool m_ShowOptions;
#endif
		public AnimatorEventType m_EventType = AnimatorEventType.EnterState;
		public int m_Layer;
		public int m_NameHash;
		public string m_StateName;
		public float m_NormalizedTime;
		public float m_StateLength;
		public GameObject m_Sender;
		public string m_Function;

		public bool m_Option1 = true;
		public bool m_Option2 = true;
		public bool m_Option3;

		public TypesEnum m_ParameterType = TypesEnum.NONE;
		public int m_IntValue;
		public float m_FloatValue;
		public string m_StringValue;
		public Object m_ObjectReferenceParameter;

		public AnimatorEvent() {}

		public AnimatorEvent(AnimatorEvent i_Copy)
		{
#if UNITY_EDITOR
			m_UniqueID = i_Copy.m_UniqueID;
#endif
			m_EventType = i_Copy.m_EventType;
			m_Layer = i_Copy.m_Layer;
			m_NameHash = i_Copy.m_NameHash;
			m_StateName = i_Copy.m_StateName;
			m_NormalizedTime = i_Copy.m_NormalizedTime;
			m_StateLength = i_Copy.m_StateLength;
			m_Sender = i_Copy.m_Sender;
			m_Function = i_Copy.m_Function;
			m_Option1 = i_Copy.m_Option1;
			m_Option2 = i_Copy.m_Option2;
			m_Option3 = i_Copy.m_Option3;
			m_ParameterType = i_Copy.m_ParameterType;
			m_IntValue = i_Copy.m_IntValue;
			m_FloatValue = i_Copy.m_FloatValue;
			m_StringValue = i_Copy.m_StringValue;
			m_ObjectReferenceParameter = i_Copy.m_ObjectReferenceParameter;
		}

		public bool IsEnterStateEvent
		{
			get { return m_EventType == AnimatorEventType.EnterState; }
		}

		public bool IsExitStateEvent
		{
			get { return m_EventType == AnimatorEventType.ExitState; }
		}

		public bool IsCustomEvent
		{
			get { return m_EventType == AnimatorEventType.Custom; }
		}

		public float FixedTime
		{
			get { return m_StateLength * m_NormalizedTime; }
		}

		public void Invoke()
		{
			if (m_Sender == null)
			{
				Debug.LogError("Can't Invoke AnimatorEvent since sender is null for state ("+m_StateName+")");
				return;
			}
			if (string.IsNullOrEmpty(m_Function))
			{
				Debug.LogError("Can't Invoke AnimatorEvent function is not set  for state ("+m_StateName+")");
				return;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				InvokeEditor();
				return;
			}
#endif

			switch (m_ParameterType)
			{
				case TypesEnum.FLOAT:
					m_Sender.SendMessage(m_Function, m_FloatValue, SendMessageOptions.RequireReceiver);
					break;
				case TypesEnum.INT:
					m_Sender.SendMessage(m_Function, m_IntValue, SendMessageOptions.RequireReceiver);
					break;
				case TypesEnum.STRING:
					m_Sender.SendMessage(m_Function, m_StringValue, SendMessageOptions.RequireReceiver);
					break;
				case TypesEnum.OBJECT:
					if (m_ObjectReferenceParameter == null)
					{
						Debug.LogError("Couldn't Invoke " + m_Function + " since the given parameter is null! Might have been destroyed");
						return;
					}
					m_Sender.SendMessage(m_Function, m_ObjectReferenceParameter, SendMessageOptions.RequireReceiver);
					break;
				default:
					m_Sender.SendMessage(m_Function, SendMessageOptions.RequireReceiver);
					break;
			}
		}

#if UNITY_EDITOR
		public bool m_ForceUpdate;
		public bool m_EditNormalizedTime;
		public bool m_EditFixedTime;
		public float m_TempNormValue;
		public float m_TempFixedValue;

		private void InvokeEditor()
		{
			string param = "";
			switch (m_ParameterType)
			{
				case TypesEnum.FLOAT:
					param = m_FloatValue.ToString();
					break;
				case TypesEnum.INT:
					param = m_IntValue.ToString();
					break;
				case TypesEnum.STRING:
					param = m_StringValue;
					break;
				case TypesEnum.OBJECT:
					param = m_ObjectReferenceParameter == null ? "N/A" : m_ObjectReferenceParameter.ToString();
					break;
			}
			string callOnlyOnce = "";
			if (m_Option3)
			{
				callOnlyOnce += " (Not working with 'Call only once' in edit mode)";
			}
			Debug.LogWarning("Invoke in edit mode for state ( "+m_StateName+") " + callOnlyOnce + " -> " +
				m_Function + " on " +
				(m_Sender == null ? "N/A" : m_Sender.name)+
				(m_ParameterType == TypesEnum.NONE ? "" : ", param: " + param));
		}

		public void Draw()
		{
			AnimatorEventDrawer.Draw(this);
		}
		public void EditorSetup()
		{
			AnimatorEventDrawer.EditorSetup(this);
		}
		public void UpdateTime(bool i_ShowWarning = true)
		{
			if (Application.isPlaying && i_ShowWarning)
			{
				Debug.LogError("Modifications won't be saved in runtime. You can copy the AnimatorHandler's component before stopping the application then paste it back after");
			}
			m_HasChanged = true;
			m_TempNormValue = m_NormalizedTime * 100f;
			m_TempFixedValue = FixedTime;
		}
#endif
	}
}