using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace TSD.AsmdefManagement
{
	public class AsmdefManagerDatabase : ScriptableObject
	{
		public enum SessionProgress
		{
			WaitingForInput,
			LookForPasses,
			SelectAsmdefFolder,
			CreateAutoPassAssembliesAndReferences,
			Ready,
			Finished
		}

		[System.Serializable]
		public struct SessionData
		{

			public bool working;
			public SessionProgress sessionProgress;

			public List<string> scriptsInAssets;

			public List<string> firstPass;
			public List<string> firstPassEditor;
			public List<string> secondPass;
			public List<string> secondPassEditor;

			public SessionData(List<string> sA, List<string> fp, List<string> fpe, List<string> sp, List<string> spe, SessionProgress prog, bool isWorking = false)
			{
				scriptsInAssets = sA;
				firstPass = fp;
				firstPassEditor = fpe;
				secondPass = sp;
				secondPassEditor = spe;

				sessionProgress = prog;
				working = isWorking;
			}
		}

		[SerializeField]
		public SessionData sessionData;
		
		public List<string> assembliesInProject { get; private set; }

	//	public List<string> alwaysIncludeAssemblies = new List<string>();
		public List<string> alwaysIncludeEditorAssemblies = new List<string>();
		public List<string> alwaysIncludeRuntimeAssemblies = new List<string>();
		
	//	public TreeViewState alwaysIncludeTreeViewState;
	//	public TreeViewState alwaysIncludeAssembliesTreeViewState;

		public bool initialSetupDone = false; //if false(~running for the first time) we will include every asmdef file found in the project
		
	//	public AsmdefAssembliesTreeViewBase alwaysIncAssTreeView;
		
		public string baseAsmdefFolder = "";
		public AutoASMDEFAssembliesGUID autoASMDEFAssembliesGUID;

		//settings
		public bool showDebugMessages { get; set; } = false;
		public bool showClassDebugMessages { get; set; } = false;
		
		public string manualAsmdefFile = ""; //this contains a reference to every auto-asmdef file, should be placed in a "myGame" directory or similar
		/// <summary>
		/// Contains every assembly
		/// </summary>
		public List<string> createdAsmdefFiles = new List<string>();
		/// <summary>
		/// Contains every created assembly reference
		/// </summary>
		public List<string> createdAsmrefFiles = new List<string>();
		/// <summary>
		/// Contains both First and Second pass Runtime assemblies
		/// </summary>
		public List<string> createdAsmdefFilesRuntime =  new List<string>();
		/// <summary>
		/// Contains both First and Second pass Editor assemblies
		/// </summary>
		public List<string> createdAsmdefFilesEditor = new List<string>();
		
		//1.1 and up
		public List<PathGUID> createdAsmrefFilesGUID = new List<PathGUID>();
		//public List<PathGUID> createdAsmdefFilesGUID = new List<PathGUID>();


		private string[] regexLines;
		private string[] regexLinesCustom;

		/// <summary>
		/// Used to store manually typed (possibly regex) paths
		/// </summary>
		internal string excludedDirectoriesRegexCustom
		{
			get => excludedDirectoriesRegexCustomActualData;
			set => excludedDirectoriesRegexCustomActualData = value;
		}
		//There are serialziation issues if we try to seralize an internal variable, however a private works just fine
		//So we access this by the internal <excludedDirectoriesRegexCustom>,
		//but store it as private <excludedDirectoriesRegexCustomActualData>
		//variable names could appreciate some love though
		[SerializeField, Multiline] 
		private string excludedDirectoriesRegexCustomActualData = "";

		private string excludedDirectoriesRegex
		{
			get => excludedDirectoriesRegexActualData; 
			set => excludedDirectoriesRegexActualData = value;
		}
		
		[SerializeField, Multiline] 
		private string excludedDirectoriesRegexActualData;

		internal List<string> excludedDirectoryList { get; set; } = new List<string>();

		public HashSet<string> directories { get; private set; }

		/// <summary>
		/// For safety reasons if the version doesn't match the current version we should always revert the changes first
		/// </summary>
		public string savedWithVersion;
		
		internal void AddDirectory(string path)
		{
			if(directories == null)
				directories = new HashSet<string>();
			directories.Add(path);
		}

		internal bool CanUseRegex = true;
		internal HashSet<string> DirectoriesThatPreventUsingRegex;
		public void CheckIfCanUseRegex()
		{
			char[] forbiddenRegexCharacters = {'^', '$', '|', '*', '+', '(', ')', '[', ']', '{', '}' };
		//	if(directories == null)
		//		directories = new HashSet<string>();
			CanUseRegex = Application.dataPath.IndexOfAny(forbiddenRegexCharacters) == -1 
			              || directories.All(i => i.IndexOfAny(forbiddenRegexCharacters) == -1);

			if (Application.dataPath.IndexOfAny(forbiddenRegexCharacters) != -1)
			{
				DirectoriesThatPreventUsingRegex = new HashSet<string>(){$"Project path contains (a) forbidden character(s), can't use regex./n{Application.dataPath}"};
			}
			else
			{
				DirectoriesThatPreventUsingRegex =
					new HashSet<string>(directories.Where(s => s.IndexOfAny(forbiddenRegexCharacters) != -1));
			}
		}
		/// <summary>
		/// Creates a list from the excluded string
		/// </summary>
		internal void prepareExcluded()
		{
		//	var custom = Regex.Split(@excludedDirectoriesRegexCustom, "\n").Where(s => s != "").ToArray();
			//TODO take custom regex into consideration
			excludedDirectoryList = 
				Regex.Split(@excludedDirectoriesRegex, "\n")
					.Concat( Regex.Split(@excludedDirectoriesRegexCustom, "\n")
					.Where(s => s != "")
					.ToArray())
					.ToList();
		}

		public List<string> lDebug;

		internal void UpdateRegEXLines()
		{
			regexLines = Regex.Split(excludedDirectoriesRegex, "\n")
				.Concat(Regex.Split(excludedDirectoriesRegexCustom, "\n"))
				.Where(s => s != "")
				.ToArray();
			
			regexLinesCustom = Regex.Split(excludedDirectoriesRegexCustom, "\n");
		}
		
		//Refactor
		public bool isPathExcluded(string pathToCheck)
		{
			if (string.Equals(pathToCheck, "")) 
				return false;
			/*string[] lines = 
				Regex.Split(excludedDirectoriesRegex, "\n")
				.Concat(Regex.Split(excludedDirectoriesRegexCustom, "\n"))
				.Where(s => s != "")
				.ToArray(); //todo optimize, move this outside of the function
			*/
			pathToCheck = pathToCheck.Replace('\\', '/');

			return CanUseRegex ? 
				regexLines.Any(s => Regex.IsMatch(pathToCheck, $"^Assets/{s}$"))
				: regexLines.Any(s => string.Equals(pathToCheck, $"Assets/{s}"));
		}

		/// <summary>
		/// If a path is excluded by a custom regex it shouldn't be interactible
		/// </summary>
		/// <param name="pathToCheck"></param>
		/// <returns></returns>
		public bool isPathExcludedByCustomRegex(string pathToCheck)
		{
			if (pathToCheck.Length < 7) //"Assets/"
				return false;
			
			return regexLinesCustom.Any(s => s != "" && Regex.IsMatch(pathToCheck, $"^Assets/{s}$"));//we don't need the "Assets/" part
		}

		[ContextMenu("Find guids")]
		public void FindGUIDs()
		{
			AssetDatabase.Refresh();
			SetGUID(ref createdAsmrefFilesGUID, ref createdAsmrefFiles);
		}

		static void SetGUID(ref List<PathGUID> combined, ref List<string> absolutePathOnly)
		{
			combined = new List<PathGUID>();
			foreach (var item in absolutePathOnly)
			{
				combined.Add(new PathGUID(item,AssetDatabase.AssetPathToGUID(item)));
			}
		}

		/// <summary>
		/// Call after the changes are done
		/// </summary>
		public void SaveDatabase()
		{
			savedWithVersion = AsmdefManager.CurrentVersion;
			
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}
		
		//Only add if it isn't there already
		public void TryAddExcludedPath(string fullPath, bool add = true)
		{
			string pathToAdd = fullPath + "/";
			excludedDirectoryList = 
				Regex.Split(@excludedDirectoriesRegex, "\n")
					.Concat( excludedDirectoriesRegexCustom == "" ? Array.Empty<string>() : Regex.Split(@excludedDirectoriesRegexCustom, "\n"))
					.ToList();
			pathToAdd = pathToAdd.Replace('\\', '/');
			pathToAdd = pathToAdd.Substring(7);

			switch (!add)
			{
				case true when !excludedDirectoryList.Contains(pathToAdd):
					excludedDirectoryList.Add(pathToAdd);
					//		Debug.Log("add " + pathToAdd);
					break;
				case false when excludedDirectoryList.Contains(pathToAdd):
					excludedDirectoryList.Remove(pathToAdd);
			//		Debug.Log("remove " + pathToAdd);
					break;
			}
			
			excludedDirectoriesRegex = "";
			foreach (var item in excludedDirectoryList)
			{
				if( !Regex.IsMatch(@item, @"^\s+$[\r\n]*") && !Regex.IsMatch(excludedDirectoriesRegexCustom, @item))
				{
					excludedDirectoriesRegex += @item + "\n";
				}
			}
			
			excludedDirectoriesRegex = RemoveEmptyLines(excludedDirectoriesRegex);
			
			//	SaveDatabase();
		}
		
		internal void SetAssembliesInProject(List<string> assembliesInProject)
		{
			this.assembliesInProject = assembliesInProject;
		}

		private string RemoveEmptyLines(string lines)
		{
			return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
		}

		[System.Serializable]
		public struct PathGUID
		{
			public string absolutePath;
			public string GUID;

			public PathGUID(string absolutePath, string GUID)
			{
				this.absolutePath = absolutePath;
				this.GUID = GUID;
			}
		}
		
		[System.Serializable]
		public class AutoASMDEFAssembliesGUID
		{
			public string firstPass;
			public string firstPassEditor;
			public string secondPass;
			public string secondPassEditor;

			public bool containsGUID(string GUID)
			{
				string[] temp = {firstPass, firstPassEditor, secondPass, secondPassEditor};
				return temp.Contains(GUID);
			}

			/// <summary>
			/// Returns true if any of the created ASMDEF files are in the set folder(or child of the folder)
			/// </summary>
			/// <param name="localBasePath"></param>
			/// <returns></returns>
			public bool isAnyInFolder(string localBasePath)
			{
				var paths = new List<string>();
				paths.Add(AssetDatabase.GUIDToAssetPath(firstPass));
				paths.Add(AssetDatabase.GUIDToAssetPath(firstPassEditor));
				paths.Add(AssetDatabase.GUIDToAssetPath(secondPass));
				paths.Add(AssetDatabase.GUIDToAssetPath(secondPassEditor));
				
				//Debug.Log(paths[0]);
				//Debug.Log(localBasePath);
				
				return paths.Any(item => item.StartsWith(localBasePath));
			}
		}

		static AsmdefManagerDatabase instance;
		public static AsmdefManagerDatabase Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (AsmdefManagerDatabase)Resources.LoadAll("", typeof(AsmdefManagerDatabase)).FirstOrDefault();
					if (instance == null)
					{
						instance = (AsmdefManagerDatabase)CreateInstance(typeof(AsmdefManagerDatabase));

						instance.directories = new HashSet<string>();
						instance.excludedDirectoriesRegex = "";
						instance.excludedDirectoriesRegexCustom = "";

					//	Debug.Log(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(AsmdefManagerEditorWindow.Instance)));
					//	string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(AsmdefManagerEditorWindow.Instance));
						string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(instance));
						path = path.Replace('\\', '/');
						path = path.Substring(0, path.LastIndexOf("/"));
						path += "/Resources/";
						if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
						AssetDatabase.Refresh();
						AssetDatabase.CreateAsset(instance, path + instance.GetType().ToString() + ".asset");
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						
					//	AsmdefManagerEditorWindow.Instance.RunCleanup();
						AsmdefManager.Instance.RunCleanup();
					}
				}
				return instance;
			}
		}
	}
}