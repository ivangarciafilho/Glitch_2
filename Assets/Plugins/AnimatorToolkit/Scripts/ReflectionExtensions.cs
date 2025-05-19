using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BFS
{
	public static class ReflectionExtensions
	{
		#region GameObject
		/// <summary>
		/// <para>Gathers all methods of every MonoBehaviours attached to the GameObject</para>
		/// <para>Dictionary key represents the script name (Supports Inheritance)</para>
		/// <para></para>
		/// <para>*WARNING: Use it carefully since it is a very heavy process</para>
		/// </summary>
		public static Dictionary<string, List<MethodInfo>> GetAllMethods(this GameObject i_GameObject, System.Type i_FilterAttributeType = null, int i_MaxParamsLength = -1)
		{
			Dictionary<string, List<MethodInfo>> allMethods = new Dictionary<string, List<MethodInfo>>();
			MonoBehaviour[] monoBehaviours = i_GameObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in monoBehaviours)
			{
				if (monoBehaviour == null)
				{
					continue;
				}

				Dictionary<string, List<MethodInfo>> monoMethods = monoBehaviour.GetAllMethods(i_FilterAttributeType, i_MaxParamsLength);
				foreach (KeyValuePair<string, List<MethodInfo>> monoKvp in monoMethods)
				{
					if (!allMethods.ContainsKey(monoKvp.Key))
					{
						allMethods.Add(monoKvp.Key, monoKvp.Value);
					}
					else
					{
						foreach (MethodInfo method in monoKvp.Value)
						{
							if (!allMethods[monoKvp.Key].Contains(method))
							{
								allMethods[monoKvp.Key].Add(method);
							}
						}
					}
				}
			}

			return allMethods;
		}

		/// <summary>
		/// <para>Gathers all method names of every MonoBehaviours attached to the GameObject</para>
		/// <para></para>
		/// <para>*WARNING: Use it carefully since it is a very heavy process</para>
		/// </summary>
		public static List<string> GetAllMethodNames(this GameObject i_GameObject, System.Type i_FilterAttributeType = null, int i_MaxParamsLength = -1)
		{
			List<string> methodNames = new List<string>();
			MonoBehaviour[] monoBehaviours = i_GameObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in monoBehaviours)
			{
				Dictionary<string, List<MethodInfo>> allMethods = monoBehaviour.GetAllMethods(i_FilterAttributeType, i_MaxParamsLength);
				foreach (List<MethodInfo> methods in allMethods.Values)
				{
					foreach (MethodInfo method in methods)
					{
						if (!methodNames.Contains(method.Name))
						{
							methodNames.Add(method.Name);
						}
					}
				}
			}

			return methodNames;
		}
		#endregion

		#region Monobehaviour
		/// <summary>
		/// <para>Gathers all methods of the MonoBehaviour</para>
		/// <para>Dictionary key represents the script name (Supports Inheritance)</para>
		/// <para></para>
		/// <para>*WARNING: Use it carefully since it is a very heavy process</para>
		/// </summary>
		public static Dictionary<string, List<MethodInfo>> GetAllMethods(this MonoBehaviour i_MonoBehaviour, System.Type i_FilterAttributeType = null, int i_MaxParamsLength = -1)
		{
			Dictionary<string, List<MethodInfo>> methodInfos = new Dictionary<string, List<MethodInfo>>();

			MethodInfo[] methods = i_MonoBehaviour.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (MethodInfo method in methods)
			{
				if ((i_MaxParamsLength == -1 || method.GetParameters().Length <= i_MaxParamsLength)&&
					method.DeclaringType.IsSubclassOf(typeof(MonoBehaviour)))
				{
					if (i_FilterAttributeType == null || Attribute.GetCustomAttribute(method, i_FilterAttributeType)!= null)
					{
						string declaringType = method.DeclaringType.ToString();
						if (!methodInfos.ContainsKey(declaringType))
						{
							methodInfos.Add(declaringType, new List<MethodInfo>());
						}

						methodInfos[declaringType].Add(method);
					}
				}
			}

			return methodInfos;
		}

		/// <summary>
		/// <para>Gathers all method names of the MonoBehaviour</para>
		/// <para></para>
		/// <para>*WARNING: Use it carefully since it is a very heavy process</para>
		/// </summary>
		public static List<string> GetAllMethodNames(this MonoBehaviour i_MonoBehaviour, System.Type i_FilterAttributeType = null, int i_MaxParamsLength = -1)
		{
			List<string> methodNames = new List<string>();
			Dictionary<string, List<MethodInfo>> allMethods = i_MonoBehaviour.GetAllMethods(i_FilterAttributeType, i_MaxParamsLength);
			foreach (List<MethodInfo> methods in allMethods.Values)
			{
				foreach (MethodInfo method in methods)
				{
					if (!methodNames.Contains(method.Name))
					{
						methodNames.Add(method.Name);
					}
				}
			}

			return methodNames;
		}
		#endregion
	}
}