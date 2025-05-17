using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.IMGUI.Controls;

namespace TSD.AsmdefManagement
{
	public class AsmdefManager
	{
		internal static readonly string CurrentVersion = "1.4.0b";
		
		/// <summary>
		/// Default Unity assemblies
		/// </summary>
		readonly List<string> baseAssemblies = new List<string>() { "Assembly-CSharp", "Assembly-CSharp-Editor" };

		private const string assemblyFirstPassRuntime = "Assembly-CSharp-firstpass";
		private const string assemblyFirstPassEditor = "Assembly-CSharp-Editor-firstpass";
		private const string assemblySecondEditorAssembly = "Assembly-CSharp-Editor";
		private const string assemblySecondRuntimeAssembly = "Assembly-CSharp";

		/// <summary>
		/// Added before the saved asmdef
		/// </summary>
		string customAsmdefPrefix = "Auto-asmdef";

		private string asmdefFirstPass = "FirstPass";
		private string asmdefFirstPassEditor = "FirstPassEditor";
		private string asmdefSecondPass = "SecondPass";
		private string asmdefSecondPassEditor = "SecondPassEditor";
		

		internal Action OnFolderLookupFinished;
		internal Action OnFirstTimeRunning;

		List<string> scriptsInAssets	= new List<string>();

		List<string> firstPassRoots		= new List<string>();
		List<string> firstPassEditors	= new List<string>();
		List<string> secondPassRuntime	= new List<string>();
		List<string> secondPassEditor	= new List<string>();

		public Dictionary<string, string> dCombinedPass = new Dictionary<string, string>();
		
		AsmdefManagerDatabase database => AsmdefManagerDatabase.Instance;

		//Toggles
		bool toggleShowSettings;
		bool toggleShowAlwaysReferenced;
		bool toggleShowFolderHierarchy;
		bool toggleShowExcludeTextField;

		List<UnityEditor.Compilation.Assembly> loadedAssemblies;
		string fileSearchPattern = "*.cs";
		
		private static AsmdefManager _instance;
		public static AsmdefManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AsmdefManager();
				}

				return _instance;
			}
		}

		/// <summary>
		/// Hidden constructor
		/// </summary>
		private AsmdefManager()
		{
		}

		private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
		{
			return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
		}

		internal void OnEnable()
		{
			RunCleanup();
			ScanProject();
		}

		internal void OnDestroy()
		{
			database.SaveDatabase();
		}

		/// <summary>
		/// Looks for folders, scripts and assemblies. Takes a while, it's normal.
		/// </summary>
		internal void ScanProject()
		{
			lookForScripts();
			
			loadedAssemblies = CompilationPipeline.GetAssemblies().ToList();
			loadedAssemblies.Sort((item1, item2) => string.Compare(item1.name, item2.name, StringComparison.Ordinal));
			loadedAssemblies = loadedAssemblies.Where(ass => !baseAssemblies.Contains(ass.name) && !ass.name.StartsWith("UnityEditor")).ToList();
			
			//get loaded asmdef files
			List<string> loadedSrt = new List<string>();
			foreach (var item in loadedAssemblies)
			{
				var path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(item.name);
				if(path != null)
				{
					bool isInAssets = false;
					AsmdefObject temp = null;
					path = path.Replace("\\", "/");
					var fullpath = Application.dataPath + "/" + path.Substring(7);
					//			Debug.Log(fullpath);
					if (File.Exists(fullpath))
					{
						var sr = new StreamReader(fullpath);
						var fileContents = sr.ReadToEnd();
						sr.Close();
						//		Debug.Log(fullpath);
						//		Debug.Log(path);
						
						temp = JsonUtility.FromJson<AsmdefObject>(fileContents);
						isInAssets = true;
					}
					
					if(!isInAssets)
						loadedSrt.Add(item.name );
					else
					{
						if(temp.additionalData != "AutoASMDEF")
							loadedSrt.Add(item.name );
						else
							if(database.showDebugMessages)
								Debug.LogWarning("Skipping already existing assembly " + item.name);
					}
					if(database.showDebugMessages)
						Debug.Log(CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(item.name));
				}
			}
			
			database.SetAssembliesInProject(loadedSrt);
			
			if (!database.initialSetupDone)
			{
				database.initialSetupDone = true;
				OnFirstTimeRunning?.Invoke();
			}
		}

		#region Update Database

		internal void UpdateDatabase()
		{
			//looks for scripts that are not included
			lookForScripts();
			//puts them into passes
			lookForPasses();
			//gets the local folders for the existing asmref holder folders
			var existing = GetCreatedASMREFPathFolders();
			
			//loop through each new script and see in which asmdef do they belong
			//create the new asmref and asmdef files if necessary
			//then merge the new file list with the old
			foreach (var VARIABLE in secondPassRuntime)
			{
				if(!existing.Contains(VARIABLE.Substring(0, VARIABLE.LastIndexOf('/'))))
					Debug.Log("[Add]" + VARIABLE);
			}
		}

		List<string> GetCreatedASMREFPathFolders()
		{
			List<string> _paths = new List<string>();
			foreach (var VARIABLE in database.createdAsmrefFilesGUID)
			{
				var p = AssetDatabase.GUIDToAssetPath(VARIABLE.GUID);
				if(File.Exists(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + p))
					_paths.Add(p.Substring(0, p.LastIndexOf('/'))); //-".asmref"
			}
			return _paths;
		}
		
		#endregion

		#region GUI

		internal void Update()
		{
			if (doNextStep)
			{
				createAsmdef();
			}
		}

		internal void ControlClicked(ActionButton ab)
		{
			if (database.sessionData.sessionProgress == AsmdefManagerDatabase.SessionProgress.Finished)
			{
				//UpdateDatabase
				if (ab == ActionButton.Update)//GUILayout.Button( new GUIContent( "Update Assembly Definitions", BtnRescanTexture, "Looks for new files and adds them to their desired assembly definition without the need to revert first"), styleBoxButton, GUILayout.Width(maxWidth), GUILayout.Height(BtnSizeBig)))
				{
					UpdateDatabase();
					database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.WaitingForInput;
					createAsmdef();
				}
				
				if (ab == ActionButton.Revert)//if (GUILayout.Button( new GUIContent("Revert created Assembly Definition files", BtnBackTexture), styleBoxButton, GUILayout.Width(maxWidth), GUILayout.Height(BtnSizeBig)))
				{
					revertCreatedFiles();
				}
			}
			else
			{
				if (ab == ActionButton.StartProcess)//if (GUILayout.Button( new GUIContent( "Start Process", BtnForwardTexture, "Creates asmdef and asmref files"), styleBoxButton, GUILayout.Width(maxWidth), GUILayout.Height(BtnSizeBig)))
				{
					database.SaveDatabase();
					createAsmdef();
				}
			}
		}

		//List<string> conflictingFolders = new List<string>(); //not used anywhere??

		#endregion

		List<string> getDistinctFileListInAssembly(List<string> everyScript , string assemblyToLookFor)
		{
			var tempList = everyScript
				.Where(s => AsmdefUtils.GetAssembly(s) == assemblyToLookFor)
				.Select(s =>s.Remove(s.LastIndexOf('/')))
				.Distinct()
				.ToList();
			return tempList.Select(s => s + formatString(s) + ".asmref").ToList();
		}

		string formatString(string input)
		{
			//if it's in the root Assets folder we have to format it a bit differently 
			if( input.Count(c => c == '/') == 0)
				return "/" + input;
			else
				return input.Substring(input.LastIndexOf('/'));
		}

		string convertPathToFolder(string path)
		{
			var t = path.Substring(7);				//remove the "Assets/" from the beginning
			t = t.Remove(t.LastIndexOf('/') + 1);	//remove the file itself
			return t;
		}
/// <summary>
/// /////////////////////////////////////////////////////////////////////////////////////
/// </summary>
		void lookForScripts()
		{
			database.prepareExcluded();
			scriptsInAssets.Clear();
			scriptsInAssets = Directory.GetFiles(Application.dataPath, fileSearchPattern, SearchOption.AllDirectories)
				.Select(s => s.Substring(s.LastIndexOf( Application.dataPath) + Application.dataPath.Length - ("Assets".Length))) //.Select(s => s.Substring(s.LastIndexOf("Assets")))
				.Select(s=> s.Replace(@"\", @"/"))
				.Where(s=> !AsmdefUtils.IsMatch(Application.dataPath + "/" + convertPathToFolder(s), database.excludedDirectoryList))
				.ToList();
		}

		void lookForPasses()
		{
			firstPassRoots = getDistinctFileListInAssembly(scriptsInAssets, assemblyFirstPassRuntime);
			firstPassEditors = getDistinctFileListInAssembly(scriptsInAssets, assemblyFirstPassEditor);
			secondPassRuntime = getDistinctFileListInAssembly(scriptsInAssets, assemblySecondRuntimeAssembly);
			secondPassEditor = getDistinctFileListInAssembly(scriptsInAssets, assemblySecondEditorAssembly);

			var updateFirstPass =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefFirstPass);
			var updateFirstPassEditor =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefFirstPassEditor);
			var updateSecondPass =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefSecondPass);
			var updateSecondPassEditor =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefSecondPassEditor);
			
			firstPassRoots.AddRange(updateFirstPass);
			firstPassEditors.AddRange(updateFirstPassEditor);
			secondPassRuntime.AddRange(updateSecondPass);
			secondPassEditor.AddRange(updateSecondPassEditor);
		}
		
		void setAutoAssemblyDefinitionsFolder()
		{
			if(database.autoASMDEFAssembliesGUID == null) { database.autoASMDEFAssembliesGUID = new AsmdefManagerDatabase.AutoASMDEFAssembliesGUID(); }

			var platformList = new List<string>(){ "Editor" };

			if(firstPassRoots.Count > 0)
			{
				var temp = new List<string>();//database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeRuntimeAssemblies);
				temp = temp.Distinct().ToList();
				database.autoASMDEFAssembliesGUID.firstPass = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefFirstPass), ref database.createdAsmdefFilesRuntime, temp);
			}
			if(firstPassEditors.Count > 0)
			{
				var temp = new List<string>();//database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeEditorAssemblies);
				temp = temp.Distinct().ToList();
				if (firstPassRoots.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.firstPass)); }
				database.autoASMDEFAssembliesGUID.firstPassEditor = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefFirstPassEditor), ref database.createdAsmdefFilesRuntime, temp, includePlatforms: platformList);
			}
			if(secondPassRuntime.Count > 0)
			{
				var temp = new List<string>();//database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeRuntimeAssemblies);
				temp = temp.Distinct().ToList();
				if (firstPassRoots.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.firstPass)); }
				database.autoASMDEFAssembliesGUID.secondPass = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefSecondPass), ref database.createdAsmdefFilesRuntime, temp);
			}
			if(secondPassEditor.Count > 0)
			{
				var temp = new List<string>();// database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeEditorAssemblies);
				temp = temp.Distinct().ToList();
				if (firstPassRoots.Count > 0) { temp.Add(AsmdefUtils.FormatGUID( database.autoASMDEFAssembliesGUID.firstPass)); }
				if (firstPassEditors.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.firstPassEditor)); }
				if (secondPassRuntime.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.secondPass)); }
				database.autoASMDEFAssembliesGUID.secondPassEditor = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefSecondPassEditor), ref database.createdAsmdefFilesRuntime, temp, includePlatforms: platformList);
			}
			//?????
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "FirstPass"));
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "FirstPassEditor"));
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "SecondPass"));
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "SecondPassEditor"));
		}
		
		bool doNextStep = false;
		
		void createAsmdef()
		{
			doNextStep = false;
			switch (database.sessionData.sessionProgress)
			{
				case AsmdefManagerDatabase.SessionProgress.WaitingForInput:
					lookForScripts();
					database.sessionData = new AsmdefManagerDatabase.SessionData(scriptsInAssets, firstPassRoots, firstPassEditors, secondPassRuntime, secondPassEditor, AsmdefManagerDatabase.SessionProgress.LookForPasses, true);
					doNextStep = true;
					break;
				case AsmdefManagerDatabase.SessionProgress.LookForPasses:
					lookForPasses();
					database.sessionData = new AsmdefManagerDatabase.SessionData(scriptsInAssets, firstPassRoots, firstPassEditors, secondPassRuntime, secondPassEditor, AsmdefManagerDatabase.SessionProgress.SelectAsmdefFolder, true);
					doNextStep = true;
					break;
				case AsmdefManagerDatabase.SessionProgress.SelectAsmdefFolder:
					database.baseAsmdefFolder = GetFirstValidBaseFolder(); //todo option to UPDATE
					database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.CreateAutoPassAssembliesAndReferences;
					doNextStep = true;
					break;
				case AsmdefManagerDatabase.SessionProgress.CreateAutoPassAssembliesAndReferences:
					database.createdAsmdefFiles.Clear();
					setAutoAssemblyDefinitionsFolder(); //create asmdef files and store a reference to them
				
					createAsmrefPass(firstPassRoots, ref database.createdAsmdefFilesRuntime, database.autoASMDEFAssembliesGUID.firstPass);
					createAsmrefPass(firstPassEditors, ref database.createdAsmdefFilesEditor, database.autoASMDEFAssembliesGUID.firstPassEditor);
					createAsmrefPass(secondPassRuntime, ref database.createdAsmdefFilesRuntime, database.autoASMDEFAssembliesGUID.secondPass);
					createAsmrefPass(secondPassEditor, ref database.createdAsmdefFilesEditor, database.autoASMDEFAssembliesGUID.secondPassEditor);
				
					database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.Finished;
					EditorUtility.SetDirty(database);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
					
					database.FindGUIDs();
					break;
				case AsmdefManagerDatabase.SessionProgress.Finished:
					database.sessionData.working = false;
					break;
				case AsmdefManagerDatabase.SessionProgress.Ready:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private string GetFirstValidBaseFolder()
		{
			var baseFolderPath = Application.dataPath + "/AutoASMDEF";
			
			var exists = Directory.Exists(baseFolderPath);
			var index = 0;
			
			if (exists)
				if(IsDirectoryEmpty(baseFolderPath) || database.autoASMDEFAssembliesGUID.isAnyInFolder(baseFolderPath.Substring(Application.dataPath.Length - 6)))
					return baseFolderPath;
			while (true)
			{
				var tempName = baseFolderPath + index;
				if (Directory.Exists(tempName))
				{
					if (IsDirectoryEmpty(tempName) || database.autoASMDEFAssembliesGUID.isAnyInFolder(baseFolderPath.Substring(Application.dataPath.Length - 6)))
						return tempName;
				}
				else
					return tempName;
				index++;
			}
		}
		
		/// <summary>
		/// look up asmref files in the project, then remove the ones that were created with the tool
		/// </summary>
		public void RunCleanup()
		{
			//in case we updated from an earlier version revert everything to avoid the compilation loop
			if (database.sessionData.sessionProgress == AsmdefManagerDatabase.SessionProgress.Finished 
			    && !database.savedWithVersion.Equals(CurrentVersion))
			{
				Debug.Log($"[AutoASMDEF] The local database version doesn't match the current asset version. Reverting changes to avoid any sort of compilation loop. If you just updated the tool this is perfectly normal.");
				revertCreatedFiles();
			}
			
			LookUpAndRemoveLeftoverFiles<AsmdefObject>("*.asmdef");
			LookUpAndRemoveLeftoverFiles<AsmRefObject>("*.asmref");
			
			AssetDatabase.Refresh();
		}
		
		/// <summary>
		/// This will clean up the asmref\asmdef files if we accidentally remove the database(~lose the references)
		/// </summary>
		/// <param name="fileExtension"></param>
		/// <typeparam name="T"></typeparam>
		void LookUpAndRemoveLeftoverFiles<T>(string fileExtension) where T : IAutoAssemblyData
		{
			var files = Directory.GetFiles(Application.dataPath, fileExtension, SearchOption.AllDirectories);
			bool foundLeftoverFiles = false;
			foreach (var VARIABLE in files)
			{
				var path = VARIABLE.Replace("\\", "/");
				var sr = new StreamReader(path);
				var fileContents = sr.ReadToEnd();
				sr.Close();
				
				var temp = JsonUtility.FromJson<T>(fileContents);
				var GUID = AssetDatabase.AssetPathToGUID("Assets" + path.Substring(Application.dataPath.Length, path.Length - Application.dataPath.Length));
				if (temp.AdditionalData == "AutoASMDEF" && (database.createdAsmrefFilesGUID.All(s => s.GUID != GUID) && !database.autoASMDEFAssembliesGUID.containsGUID(GUID)))
				{
					foundLeftoverFiles = true;
					if(database.showDebugMessages)
						Debug.Log("[AutoASMDEF cleaning up leftover data] " + path);
					removeFileWithMeta(path);
				}

				var dirPath = path.Substring(0, path.LastIndexOf('/'));
				removeFileWithMeta(dirPath);
			}
			
			if(!foundLeftoverFiles && database.showDebugMessages)
				Debug.Log("[AutoASMDEF] Nothing to clean up.");
		}

		private bool IsDirectoryEmpty(string path)
		{
			return !Directory.EnumerateFileSystemEntries(path).Any();
		}
		
		void createAsmrefPass(List<string> passPaths, ref List<string> databasePassReference, string referencedAssemblyGUID, bool lookForBaseAssembly = true)
		{
			foreach (var item in passPaths)
			{
				var path = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets")) + item;
				if (File.Exists(path))
				{
					database.createdAsmrefFiles.Add(item);
					continue;
				}
				var asref = new AsmRefObject("GUID:" + referencedAssemblyGUID, "AutoASMDEF");
				if (database.showDebugMessages)
				{
					Debug.Log(path);
				}
				
				File.WriteAllText( path, JsonUtility.ToJson(asref, true));

				//database.createdAsmrefFiles.Add(fullPath); //todo change this to GUID as well 
				database.createdAsmrefFiles.Add(item); //todo change this to GUID as well 
			}
		}

		string createAsmdefPass(string passName, ref List<string> databasePassReference, List<string> referencedPasses, List<string> includePlatforms = null, List<string> excludePlatforms = null)
		{
			//creates a subfolder named passName
			//creates an asmdef file named passName in that subfolder
			//references passes that will exist before this pass 
			var fullPath = database.baseAsmdefFolder + "/" + passName;
			if(!Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}

			/*var referencedAssemblies = new List<string>();
			referencedAssemblies.AddRange(database.alwaysIncludeAssemblies);
			if(referencedPasses != null)
			{
				referencedAssemblies.AddRange(referencedPasses);
			}*/
			string pathWithExtension = $"{fullPath}/{passName}.asmdef";
			
			//referenced passes, platforsm, excluded, what not. Add here, compare to the original if there is an original, change if required
			//1. look for existing asmdef file
			//2.a if there is one, compare the data.
			//	2.a.a We might need to update it
			//2.b if there isn't any, create one!

			AsmdefObject tempASMDEF = new AsmdefObject(passName, 
				referencedPasses.ToArray(), 
				includePlatforms == null ? null : includePlatforms.ToArray(), 
				excludePlatforms == null? null : excludePlatforms.ToArray(),
				_additionalData: "AutoASMDEF"
			);
			
			//TODO update the asmdef file if needed!
			if (File.Exists(pathWithExtension))
			{
				var _p = pathWithExtension.Substring(pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) , pathWithExtension.Length - (pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) ));

				var path = pathWithExtension.Replace("\\", "/");
				var sr = new StreamReader(path);
				var fileContents = sr.ReadToEnd();
				sr.Close();
				
				var loadedAsmdefFromFile = JsonUtility.FromJson<AsmdefObject>(fileContents);
				
				tempASMDEF.Sort();
				loadedAsmdefFromFile.Sort();
				
				//check if the loaded asmdef file differs from what we want to store now
				var isTheSame = loadedAsmdefFromFile.Equals(tempASMDEF);
				if(isTheSame)
					return AssetDatabase.AssetPathToGUID(_p);
				else
					Debug.Log("Update required! " + passName);
			}
			
			string data = JsonUtility.ToJson(tempASMDEF, true);
			
			File.WriteAllText(pathWithExtension, data);
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

			var p = pathWithExtension.Substring(pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) , pathWithExtension.Length - (pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) ));
			var nameWithoutExtension = p.Remove(p.LastIndexOf('.'));
			
			//todo save the folder GUID as well
			return AssetDatabase.AssetPathToGUID(p);
		}
		
		void revertCreatedFiles()
		{
			database.sessionData.working = false;
			
			for (int i = 0; i < database.createdAsmrefFilesGUID.Count; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(database.createdAsmrefFilesGUID[i].GUID);// database.createdAsmrefFiles[i];
				removeFileWithMeta(path);
			}
			
			database.createdAsmrefFiles.Clear();
			database.createdAsmrefFilesGUID.Clear();
			
			var p =Application.dataPath.Remove(Application.dataPath.LastIndexOf( Application.dataPath, StringComparison.Ordinal) + Application.dataPath.Length - ("Assets".Length));// Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));

			var asmdefHolderFolder = new List<string>();
			var fp = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.firstPass);
			var fpe = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.firstPassEditor);
			var sp = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.secondPass);
			var spe = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.secondPassEditor);

			var datapath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/";
			if (!string.Equals(fp, ""))
			{
				asmdefHolderFolder.Add(datapath + fp.Substring(0, fp.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + fp); //AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.firstPass));
			}
			if (!string.Equals(fpe, ""))
			{
				asmdefHolderFolder.Add(datapath + fpe.Substring(0, fpe.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + fpe);
			}
			if (!string.Equals(sp, ""))
			{
				asmdefHolderFolder.Add(datapath + sp.Substring(0, sp.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + sp);
			}
			if (!string.Equals(spe, ""))
			{
				asmdefHolderFolder.Add(datapath + spe.Substring(0, spe.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + spe);
			}

			foreach (var VARIABLE in asmdefHolderFolder)
			{
				var dirinfo = new DirectoryInfo(@VARIABLE);
				if(dirinfo.Exists && IsDirectoryEmpty(@VARIABLE))
					dirinfo.Delete(true);
			}

			database.autoASMDEFAssembliesGUID.firstPass = "";
			database.autoASMDEFAssembliesGUID.firstPassEditor = "";
			database.autoASMDEFAssembliesGUID.secondPass =  "";
			database.autoASMDEFAssembliesGUID.secondPassEditor = "";

			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

			database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.WaitingForInput;
		}

		/// <summary>
		/// Removes files or folders with metadata. It only removes empty folders!
		/// </summary>
		/// <param name="fullPath"></param>
		void removeFileWithMeta(string fullPath)
		{
			if(database.showDebugMessages)
				Debug.Log(fullPath);

			if (fullPath != null)
			{
				if(File.Exists(fullPath))
					File.Delete(fullPath);
				if (Directory.Exists(fullPath) && IsDirectoryEmpty(fullPath))
					Directory.Delete(fullPath);
				if(File.Exists(fullPath + ".meta"))
					File.Delete(fullPath + ".meta");
			}
		}
	}

	public enum ActionButton
	{
		StartProcess,
		Revert,
		Update
	}
}