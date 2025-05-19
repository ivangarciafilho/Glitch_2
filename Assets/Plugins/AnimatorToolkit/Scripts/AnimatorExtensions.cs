using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public static class AnimatorExtensions
{
	public static float GetCurrentFixedTime(this Animator i_Animator, int i_Layer = 0)
	{
		AnimatorStateInfo stateInfo = i_Animator.GetCurrentAnimatorStateInfo(i_Layer);
		AnimatorClipInfo[] clipInfos = i_Animator.GetCurrentAnimatorClipInfo(i_Layer);
		if (clipInfos.Length == 0 || clipInfos[0].clip == null)
		{
			Debug.LogWarning("State has no motion/clip assigned or animator (UnityEditor) is broken. Try to toggle the animator's gameobject!");
			return 0f;
		}

		return clipInfos[0].clip.length * stateInfo.normalizedTime;
	}

	public static string TryGetLayerName(this Animator i_Animator, int i_Layer)
	{
#if UNITY_EDITOR
		if (i_Animator.runtimeAnimatorController != null)
		{
			AnimatorControllerLayer[] layers = (i_Animator.runtimeAnimatorController as AnimatorController).layers;
			if (i_Layer >= 0 && i_Layer < layers.Length)
			{
				return layers[i_Layer].name;
			}
			else
			{
				Debug.LogError("TryGetLayerName -> Layer index out of range (" + i_Layer + ")");
				return "N/A";
			}
		}

#endif
		if (!i_Animator.isActiveAndEnabled)
		{
			return "N/A";
		}
		return i_Animator.GetLayerName(i_Layer);
	}

#if UNITY_EDITOR

	/// <summary>
	/// (Editor-Only) Get the animation clip with a given state name 
	/// </summary>
	public static AnimationClip GetClipByState(this Animator i_Animator, string i_StateName, int i_Layer = 0)
	{
		AnimatorState animatorState = GetStateByName(i_Animator, i_StateName, i_Layer);
		if (animatorState == null)
		{
			Debug.LogError("Couldn't GetClipByState since the state doesn't exist or it's not the good given layer ( " + i_Layer + ")! Animator = " + i_Animator);
			return null;
		}

		return animatorState.motion as AnimationClip;
	}

	/// <summary>
	/// (Editor-Only) Get the animation clip length with a given trigger parameter
	/// </summary>
	/// <returns></returns>
	public static float GetClipLengthByTrigger(this Animator i_Animator, string i_Trigger, float i_BlendValue = 0f)
	{
		AnimatorController animator = (AnimatorController)i_Animator.runtimeAnimatorController;

		AnimatorStateMachine stateMachine = animator.layers[0].stateMachine;
		AnimatorStateTransition[] transitions = stateMachine.anyStateTransitions;
		for (int i = 0; i < transitions.Length; i++)
		{
			AnimatorCondition[] conditions = transitions[i].conditions;
			if (conditions != null && conditions[0].parameter == i_Trigger)
			{
				if (transitions[i].destinationState.motion.GetType()== typeof(BlendTree))
				{
					BlendTree blendtree = (BlendTree)transitions[i].destinationState.motion;
					return blendtree.children[(int)i_BlendValue].motion.averageDuration;
				}
				else
				{
					return transitions[i].destinationState.motion.averageDuration;
				}
			}
		}

		return 0.5f;
	}

	/// <summary>
	/// (Editor-Only) Get the given animator's state length
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static float GetLengthByState(this Animator i_Animator, string i_StateName, int i_Layer = 0)
	{
		AnimatorState animState = GetStateByName(i_Animator, i_StateName, i_Layer);
		if (animState == null)
		{
			Debug.LogError("Couldn't find state (" + i_StateName + ") with the given layer (" + i_Layer + ")! Animator = " + i_Animator);
			return 0f;
		}
		if (animState.motion == null)
		{
			Debug.LogError("Couldn't get length for state (" + i_StateName + ") since the motion/clip is null! Animator = " + i_Animator);
			return 0f;
		}
		return animState.motion.averageDuration;
	}

	/// <summary>
	/// (Editor-Only) Get the animator state given the state name
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static AnimatorState GetState(this Animator i_Animator, string i_StateName)
	{
		return GetStateByName(i_Animator, i_StateName);
	}

	/// <summary>
	/// (Editor-Only) Get the animator state given the state short hash name
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static AnimatorState GetState(this Animator i_Animator, int i_ShortNameHash)
	{
		return GetStateByShortNameHash(i_Animator, i_ShortNameHash);
	}

	/// <summary>
	/// (Editor-Only) Get the name of the current state
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static string GetCurrentStateName(this Animator i_Animator, int i_Layer = 0)
	{
		return GetStateNameByShortNameHash(i_Animator, i_Animator.GetCurrentAnimatorStateInfo(i_Layer).shortNameHash, i_Layer);
	}

	/// <summary>
	/// (Editor-Only) This method retrieves all states name (All layers combined)
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static List<string> GetAllStateNames(this Animator i_Animator)
	{
		List<List<AnimatorState>> animatorStates = GetAllStates(i_Animator);

		List<string> stateNames = new List<string>();
		for (int layer = 0; layer < animatorStates.Count; layer++)
		{
			for (int i = 0; i < animatorStates[layer].Count; i++)
			{
				if (!stateNames.Contains(animatorStates[layer][i].name))
				{
					stateNames.Add(animatorStates[layer][i].name);
				}
			}
		}

		return stateNames;
	}

	/// <summary>
	/// (Editor-Only) This method retrieves all states name by layers
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static List<List<string>> GetAllStateNamesByLayers(this Animator i_Animator)
	{
		List<List<AnimatorState>> animatorStates = GetAllStates(i_Animator);

		List<List<string>> stateNames = new List<List<string>>();
		for (int layer = 0; layer < animatorStates.Count; layer++)
		{
			stateNames.Add(new List<string>());
			for (int i = 0; i < animatorStates[layer].Count; i++)
			{
				if (!stateNames[layer].Contains(animatorStates[layer][i].name))
				{
					stateNames[layer].Add(animatorStates[layer][i].name);
				}
			}
		}

		return stateNames;
	}

	/// <summary>
	/// (Editor-Only) Returns the 'shortNameHash' from the i_StateName
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <param name="i_StateName"></param>
	/// <param name="i_Layer"></param>
	/// <returns></returns>
	public static int GetNameHashByState(this Animator i_Animator, string i_StateName, int i_Layer = 0)
	{
		AnimatorState animatorState = GetStateByName(i_Animator, i_StateName, i_Layer);
		if (animatorState == null)
		{
			Debug.LogError("Couldn't GetNameHashByState since the state doesn't exist or it's not the good given layer ( " + i_Layer + ")! Animator = " + i_Animator);
			return -1;
		}

		return animatorState.nameHash;
	}

	/// <summary>
	/// (Editor-Only) Gets all AnimatorStates by layers
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static List<List<AnimatorState>> GetAllStates(this Animator i_Animator)
	{
		List<List<AnimatorState>> animatorStates = new List<List<AnimatorState>>();
		if (i_Animator == null)
		{
			Debug.LogError("GetAllStates -> Animator is null!");
			return animatorStates;
		}
		if (i_Animator.runtimeAnimatorController == null)
		{
			Debug.LogError("GetAllStates -> Animator's controller is null!");
			return animatorStates;
		}
		return GetAllStates(i_Animator.runtimeAnimatorController);
	}

	/// <summary>
	/// (Editor-Only) Gets all AnimatorStates by layers
	/// </summary>
	/// <param name="i_Animator"></param>
	/// <returns></returns>
	public static List<List<AnimatorState>> GetAllStates(this RuntimeAnimatorController i_Controller)
	{
		List<List<AnimatorState>> animatorStates = new List<List<AnimatorState>>();
		AnimatorController editorAnimator = i_Controller as AnimatorController;
		if (editorAnimator != null)
		{
			AnimatorControllerLayer[] allLayer = editorAnimator.layers;
			for (int i = 0; i < allLayer.Length; i++)
			{
				animatorStates.Add(new List<AnimatorState>(GetRecursiveStates(allLayer[i].stateMachine)));
			}
		}

		return animatorStates;
	}

	private static AnimatorState GetStateByName(this Animator i_Animator, string i_Name, int i_Layer = 0)
	{
		List<List<AnimatorState>> animatorStates = GetAllStates(i_Animator);
		if (i_Layer < 0 || i_Layer >= animatorStates.Count)
		{
			Debug.LogError("Couldn't GetStateByName since the layer is out of reach! Animator = " + i_Animator);
			return null;
		}
		return animatorStates[i_Layer].FirstOrDefault(x => x.name == i_Name);
	}

	private static AnimatorState GetStateByShortNameHash(this Animator i_Animator, int i_ShortNameHash, int i_Layer = 0)
	{
		List<List<AnimatorState>> animatorStates = GetAllStates(i_Animator);
		if (i_Layer < 0 || i_Layer >= animatorStates.Count)
		{
			Debug.LogError("Couldn't GetStateByShortNameHash since the layer is out of reach! Animator = " + i_Animator);
			return null;
		}
		return animatorStates[i_Layer].FirstOrDefault(x => x.nameHash == i_ShortNameHash);
	}

	private static string GetStateNameByShortNameHash(this Animator i_Animator, int i_ShortNameHash, int i_Layer = 0)
	{
		AnimatorState state = GetStateByShortNameHash(i_Animator, i_ShortNameHash, i_Layer);
		if (state == null)
		{
			Debug.LogError("Couldn't find state name with given shortNameHash or it's not the good given layer ( " + i_Layer + ")! Animator = " + i_Animator);
			return null;
		}
		return state.name;
	}

	private static List<AnimatorState> GetRecursiveStates(AnimatorStateMachine i_StateMachine)
	{
		List<AnimatorState> animatorStates = new List<AnimatorState>();

		if (i_StateMachine.states.Length == 0 && i_StateMachine.stateMachines.Length == 0)
		{
			return animatorStates;
		}

		for (int i = 0; i < i_StateMachine.states.Length; i++)
		{
			animatorStates.Add(i_StateMachine.states[i].state);
		}

		for (int i = 0; i < i_StateMachine.stateMachines.Length; i++)
		{
			animatorStates.AddRange(GetRecursiveStates(i_StateMachine.stateMachines[i].stateMachine));
		}

		return animatorStates;
	}
#endif
}