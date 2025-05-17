using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TSD.AsmdefManagement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TSD.AsmdefManagement.UI
{
	public class FolderItem : VisualElement
	{
		public string parentPath { get; private set; }
		private readonly string fullPath;
		private string displayName;
		private int depth;

		private ScrollView hierarchySV;
		private readonly Toggle toggle;

		private HashSet<FolderItem> childItems;

		public static Action FolderItemSelectionChanged;

		private bool isSpecialFolder;
		private bool excludedByRegex;
		public FolderItem(string targetDirectory, bool isOpen = false)//int index, int room, int count, string idType)
		{
			
			var vta = UIToolkitTSD.cloneVTA(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{AutoASMDEFWindow.path}/autoasmdef_folder_item.uxml"));
			this.Add(vta);

			targetDirectory = targetDirectory.Replace('\\', '/');
			fullPath = targetDirectory.Substring(Application.dataPath.Length - 6).Replace('\\', '/'); //-7 to keep the Assets part
			displayName = targetDirectory.Contains('/') ? targetDirectory.Substring(targetDirectory.LastIndexOf('/') + 1) : targetDirectory;
			depth = fullPath.Count(f => f == '/'); //used offsetting horizontally

			/*var temp = Directory.GetFiles(targetDirectory, "*.cs", SearchOption.TopDirectoryOnly);
			var asmItem = new AsmdefTreeViewItem
			{
			//	id = _id,
				depth = d,
				displayName = str,
				fullPath = m_fullPath,
				relativePath = str,
				absoluteFilePath = targetDirectory,
				isMarked = !AsmdefManagerDatabase.Instance.isPathExcluded("/" + m_fullPath + "/"),
				isSpecialFolder = temp.Length != 0 && !AsmdefUtils.IsPartOfBaseAssembly(temp[0].Replace('\\', '/'), AsmdefUtils.CompilationFolder.Any, false),
				isDeselectedViaRegex =  true
				//	asmdefPath = AsmdefManagerEditorWindow.Instance.dCombinedPass.ContainsKey(item) ? AsmdefManagerEditorWindow.Instance.dCombinedPass[item].Substring(AsmdefManagerEditorWindow.Instance.dCombinedPass[item].LastIndexOf("Assets") + 7) : ""
			};*/

			AsmdefManagerDatabase.Instance.AddDirectory(targetDirectory);

			hierarchySV = this.Q<ScrollView>("childrenScrollView");

			var foldout = this.Q<Foldout>("foldout");
			foldout.RegisterValueChangedCallback((e) =>
			{
				hierarchySV.style.display = 
					e.newValue == false 
						? DisplayStyle.None : DisplayStyle.Flex;
			});

			// Recurse into subdirectories of this directory.
			string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory, "*", SearchOption.TopDirectoryOnly);
			this.Q<Foldout>("foldout").visible = subdirectoryEntries != null && subdirectoryEntries.Length > 0;

			toggle = this.Q<Toggle>("tgFolder");

			toggle.RegisterValueChangedCallback((e) =>
				{
					SetStateAllChildren(e.newValue, true);
				});
			
			toggle.RegisterCallback<MouseDownEvent>((e) =>
			{
				if (e.button == 1)
				{
					toggle.SetValueWithoutNotify(!toggle.value);
				//	SetStateAllChildren(toggle.value);
				}
				AsmdefManagerDatabase.Instance.SaveDatabase();
			});

			toggle.RegisterValueChangedCallback((e) =>
			{
			//	AsmdefManagerDatabase.Instance.TryAddExcludedPath(this.name);
				FolderItemSelectionChanged?.Invoke();
				AsmdefManagerDatabase.Instance.SaveDatabase();
			});

			childItems = new HashSet<FolderItem>();
			
			foreach (var subdirectory in subdirectoryEntries)
			{
				if (subdirectory.EndsWith(".git")) { continue; } //skip .git and its subfolders

				var fi = new FolderItem(subdirectory);
				childItems.Add(fi);
				hierarchySV.Add(fi);// ProcessDirectory(subdirectory);//, ref refTree, ref _id);
			}
			
			var temp = Directory.GetFiles(targetDirectory, "*.cs", SearchOption.TopDirectoryOnly);
			isSpecialFolder = temp.Length != 0 && !AsmdefUtils.IsPartOfBaseAssembly(temp[0].Replace('\\', '/'),
				CompilationFolder.Any, false);

			SetupVariables();
			if (fullPath.Length > 7)
			{
				toggle.SetValueWithoutNotify(!AsmdefManagerDatabase.Instance.isPathExcluded($"{fullPath}/"));
				UpdateExcludedRegex();
			}
			
			foldout.value = isOpen;
			hierarchySV.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;
		}

		public bool ToggleValue => toggle.value;
		
		private void SetupVariables()
		{
			this.name = fullPath;
			toggle.text = displayName;// displayName;
		}

		internal void UpdateExcludedRegex()
		{
			excludedByRegex = AsmdefManagerDatabase.Instance.isPathExcludedByCustomRegex(fullPath + "/");
		//	toggle.text = excludedByRegex ? $"{fullPath} [Disabled via RegEX]" : fullPath;
		}

		/// <summary>
		/// If firstRun set to true it's going to save the database after it finishes with the children
		/// </summary>
		/// <param name="newState"></param>
		/// <param name="firstRun"></param>
		public void SetStateAllChildren(bool newState, bool firstRun = false)
		{
			toggle.SetValueWithoutNotify(newState);

			foreach (var child in childItems)
			{
				child.SetStateAllChildren(newState);
			}
		
		//	Debug.Log(this.name);
		//	AsmdefManagerDatabase.Instance.TryAddExcludedPath(this.name);
			if(firstRun)
				AsmdefManagerDatabase.Instance.SaveDatabase();
		}

		public void UpdateBackgroundColor()
		{
			Color gray = new Color(0.2196079f, 0.2196079f, 0.2196079f, .1f);
			Color textGray = new Color(0.854902f, 0.854902f, 0.854902f);
			bool partiallyExcluded = depth != 0 && this.Query<Toggle>("tgFolder").Build().ToList().Any(x => x.value != toggle.value);

			toggle.style.backgroundColor = new StyleColor(gray);

			if (partiallyExcluded)
				toggle.style.backgroundColor = new Color(1, 0.7450981f, 0.1294118f, 1f);
			if (isSpecialFolder)
				toggle.style.backgroundColor = new Color(0, 0.4470589f, 1f, 1f);

			toggle.SetEnabled(!isSpecialFolder && !excludedByRegex);
			//update text color as well
			toggle.Q<Label>().style.color = isSpecialFolder || partiallyExcluded ? Color.black : textGray;

			var msg = "";
			msg += isSpecialFolder ? "[Custom assembly]" : "";
			msg += excludedByRegex ? "[Disabled via RegEX]" : "";
			toggle.text = excludedByRegex ? $"{displayName} {msg}" : displayName;
			//	toggle.SetEnabled(!excludedByRegex);
		}
	}

	public class AssemblyReference : VisualElement
	{

		private Toggle both;
		private Toggle editor;
		private Toggle runtime;
		
		private string assName;

		public bool EditorValue => editor.value;
		public bool RuntimeValue => runtime.value;
		
		public AssemblyReference(string assemblyName, bool runtime = true, bool editor = true)
		{
			var vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{AutoASMDEFWindow.path}autoasmdef_referenced_assembly.uxml");
			this.Add(UIToolkitTSD.cloneVTA(vta));

			this.Q<Label>("labelAssembly").text = assemblyName;
			
			SetupToggleLinks(runtime, editor);
			
			LoadValues();
			//	var hierarchy = this.Q<ScrollView>("svAlwaysReferencedAssemblies");
		}

		void SetupToggleLinks(bool rt = true, bool et = true)
		{
			both = this.Q<Toggle>("tgBoth");
			editor = this.Q<Toggle>("tgEditor");
			runtime = this.Q<Toggle>("tgRuntime");

			both.RegisterValueChangedCallback((e) =>
			{
				editor.SetValueWithoutNotify( e.newValue );
				runtime.SetValueWithoutNotify( e.newValue );
				UpdateDatabase();
			});
			
			editor.RegisterValueChangedCallback((e) =>
			{
				both.SetValueWithoutNotify( editor.value && runtime.value );
				UpdateDatabase();
			});

			runtime.RegisterValueChangedCallback((e) =>
			{
				both.SetValueWithoutNotify( editor.value && runtime.value );
				UpdateDatabase();
			});

			editor.value = et;
			runtime.value = rt;
			
			//either wait a frame or set it("both" checkbox) up manually for the first time
			both.SetValueWithoutNotify(et && rt);
		}

		internal void LoadValues()
		{
			assName = this.Q<Label>("labelAssembly").text;
			editor.value = AsmdefManagerDatabase.Instance.alwaysIncludeEditorAssemblies.Contains(assName);
			runtime.value = AsmdefManagerDatabase.Instance.alwaysIncludeRuntimeAssemblies.Contains(assName);
		}

		internal void SetValue(CompilationFolder folder, bool newValue)
		{
			switch (folder)
			{
				case CompilationFolder.Editor:
					editor.value = newValue;
					AddOrRemove(editor.value, ref AsmdefManagerDatabase.Instance.alwaysIncludeEditorAssemblies);
					break;
				case CompilationFolder.Runtime:
					runtime.value = newValue;
					AddOrRemove(runtime.value, ref AsmdefManagerDatabase.Instance.alwaysIncludeRuntimeAssemblies);
					break;
				case CompilationFolder.Any:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(folder), folder, null);
			}
			
		}
		
		internal void SetValues(bool rt, bool et)
		{
			SetValue(CompilationFolder.Editor, et);
			SetValue(CompilationFolder.Runtime, rt);
			both.SetValueWithoutNotify(rt && et);
		}

		void AddOrRemove(bool newValue, ref List<string> refList)
		{
			if (newValue)
			{
				if(refList.Contains(assName))
					return;
				else
					refList.Add(assName);
			}
			else 
				if (refList.Contains(assName))
					refList.Remove(assName);
		}

		void UpdateDatabase()
		{
			var database = AsmdefManagerDatabase.Instance;
			AddOrRemove(editor.value, ref database.alwaysIncludeEditorAssemblies);
			AddOrRemove(runtime.value, ref database.alwaysIncludeRuntimeAssemblies);
		}
	}
}
