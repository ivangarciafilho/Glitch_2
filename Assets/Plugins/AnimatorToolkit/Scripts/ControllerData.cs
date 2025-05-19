using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
#endif

public class ControllerData : ScriptableObject
{
	[System.Serializable]
	public class LayerData
	{
		[System.Serializable]
		public class StateData
		{
			public int m_ShortNameHash;
			public string m_StateName;

			public StateData(int i_ShortNameHash, string i_StateName)
			{
				m_ShortNameHash = i_ShortNameHash;
				m_StateName = i_StateName;
			}
		}

		[SerializeField] private List<StateData> m_StateData = new List<StateData>();

		public List<StateData> Data
		{
			get { return m_StateData; }
		}

		public string GetStateName(int i_ShortNameHash)
		{
			for (int i = 0; i < m_StateData.Count; i++)
			{
				if (m_StateData[i].m_ShortNameHash == i_ShortNameHash)
				{
					return m_StateData[i].m_StateName;
				}
			}
			Debug.LogError("ControllerData - shortNameHash (" + i_ShortNameHash + ") not found!");
			return "";
		}

		public void Add(int i_ShortNameHash, string i_StateName)
		{
			m_StateData.Add(new StateData(i_ShortNameHash, i_StateName));
		}
	}

	[SerializeField] private string m_ControllerGUID;
	[SerializeField] private List<LayerData> m_LayerData;

	#region Getters
	public string ControllerGUID
	{
		get { return m_ControllerGUID; }
	}

	public List<LayerData> AllData
	{
		get { return m_LayerData; }
	}

	public string GetStateName(int i_Layer, int i_ShortNameHash)
	{
		if (m_LayerData != null)
		{
			if (i_Layer < 0 || i_Layer >= m_LayerData.Count)
			{
				Debug.LogError("ControllerData - given layer (" + i_Layer + ") is out of range!");
				return "";
			}
			return m_LayerData[i_Layer].GetStateName(i_ShortNameHash);
		}
		Debug.LogError("ControllerData - Layer Data is null!");
		return "";
	}
	#endregion

#if UNITY_EDITOR
	public static ControllerData AddOrGet(RuntimeAnimatorController i_Controller)
	{
		ControllerData controllerData = null;
		Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(i_Controller));
		for (int i = 0; i < allAssets.Length; i++)
		{
			if (allAssets[i].GetType()== typeof(ControllerData))
			{
				controllerData = allAssets[i] as ControllerData;
				break;
			}
		}
		if (controllerData == null)
		{
			controllerData = ScriptableObject.CreateInstance<ControllerData>();
			controllerData.name = "ControllerData";
			controllerData.AddAllStates(i_Controller);
			AssetDatabase.AddObjectToAsset(controllerData, i_Controller);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		return controllerData;
	}

	public void AddAllStates(RuntimeAnimatorController i_Controller)
	{
		m_ControllerGUID = AssetDatabase.GetAssetPath(i_Controller);
		m_LayerData = new List<LayerData>();
		List<List<AnimatorState>> allStates = i_Controller.GetAllStates();
		for (int i = 0; i < allStates.Count; i++)
		{
			LayerData layerData = new LayerData();
			m_LayerData.Add(layerData);
			for (int j = 0; j < allStates[i].Count; j++)
			{
				layerData.Add(allStates[i][j].nameHash, allStates[i][j].name);
			}
		}
	}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ControllerData))]
public class ControllerDataEditor : Editor
{
	private string m_Filter;
	private bool[] m_Foldouts;

	private ControllerData m_ControllerData;

	public override void OnInspectorGUI()
	{
		if (m_ControllerData == null)
		{
			m_ControllerData = target as ControllerData;
		}

		GUI.color = Color.red;
		if (GUILayout.Button("Delete", EditorStyles.toolbarButton))
		{
			DestroyImmediate(target, true);
			Debug.Log("ControllerData deleted. To make it disappear, just Save");
			return;
		}
		GUI.color = Color.white;

		EditorGUILayout.BeginHorizontal(GUI.skin.box);
		m_Filter = EditorGUILayout.TextField(new GUIContent("Filter", "You can filter by shortHashName or by state name"), m_Filter);
		GUI.color = Color.red;
		if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20f)))
		{
			GUI.FocusControl("");
			m_Filter = "";
		}
		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();

		List<ControllerData.LayerData> filteredData = new List<ControllerData.LayerData>(m_ControllerData.AllData);
		if (m_Foldouts == null || m_Foldouts.Length != filteredData.Count)
		{
			m_Foldouts = new bool[filteredData.Count];
		}

		RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(m_ControllerData.ControllerGUID);

		EditorGUI.indentLevel++;
		for (int i = 0; i < filteredData.Count; i++)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			string layerName = controller == null ? ("Layer " + i.ToString()): ((controller as AnimatorController).layers[i].name);
			m_Foldouts[i] = EditorGUILayout.Foldout(m_Foldouts[i], layerName, true);
			if (m_Foldouts[i])
			{
				ShowLayerData(filteredData[i]);
			}

			EditorGUILayout.EndVertical();
		}
		EditorGUI.indentLevel--;
	}

	private void ShowLayerData(ControllerData.LayerData i_LayerData)
	{
		bool isEmpty = true;
		for (int i = 0; i < i_LayerData.Data.Count; i++)
		{
			if (string.IsNullOrEmpty(m_Filter)||
				i_LayerData.Data[i].m_ShortNameHash.ToString().Contains(m_Filter)||
				i_LayerData.Data[i].m_StateName.ToString().Contains(m_Filter))
			{
				isEmpty = false;
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField(new GUIContent("Hash", "Represents the 'shortNameHash' from the animator"), new GUIContent(i_LayerData.Data[i].m_ShortNameHash.ToString()));
				EditorGUILayout.LabelField(new GUIContent("Name", "Represents the state name"), new GUIContent(i_LayerData.Data[i].m_StateName));
				EditorGUILayout.EndHorizontal();
			}
		}
		if (isEmpty)
		{
			GUI.color = Color.yellow;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.LabelField("No data was found for this layer");
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
		}
	}
}
#endif