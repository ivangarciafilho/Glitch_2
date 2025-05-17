using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TSD.AsmdefManagement.UI
{
	public class AutoASMDEFWindow : EditorWindow
	{
		private AsmdefManager aMgr;

		#if UNITY_2022_1_OR_NEWER
		public const string path = "Assets/Plugins/2SD/2SD/AsmdefManager/UIToolkit2022/";
		#else
		public const string path = "Assets/Plugins/2SD/2SD/AsmdefManager/UIToolkit/";
		#endif
		
		private VisualElement container;

		private VisualElement containerControlElements;
		private VisualElement containerCompiling;

		private VisualElement progressbar;
		private Label progressLabel;

		private AsmdefManagerDatabase database;

		private Button toggleAllRuntime;
		private Button toggleAllEditor;
		
		private TextField excludeTextbox;
		private Button actionExcludeUpdate;
		
		private Button actionStartProcess;
		private Button actionRevert;
		private Button actionUpdate;

		private bool firstTimeRunning;

		[MenuItem("Window/2SD/Auto Assembly Definition Manager &q")]
		public static void ShowWindow()
		{
			AutoASMDEFWindow window = GetWindow<AutoASMDEFWindow>();
			window.titleContent = new GUIContent("AutoASMDEF Manager");
		}
		
		private void OnEnable()
		{
			initAMGR();
			aMgr.OnEnable();

			if (containerControlElements != null)
			{
				containerControlElements.style.display = DisplayStyle.Flex;
				containerCompiling.style.display = DisplayStyle.None;
			}
		}

		private void OnDestroy()
		{
			aMgr.OnDestroy();
		}

		private void Update()
		{
			aMgr.Update();
		}

		private void initAMGR()
		{
			if(aMgr != null)
				return;
			aMgr = AsmdefManager.Instance;
			aMgr.OnFirstTimeRunning += () =>
			{
				firstTimeRunning = true;
			};
		}
		
		private void CreateGUI()
		{
			initAMGR();
			database = AsmdefManagerDatabase.Instance;

			container = rootVisualElement;
			VisualTreeAsset vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{path}autoasmdef_main_window.uxml");

			container.Add( UIToolkitTSD.cloneVTA(vta));

			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{path}contUSS.uss");
			container.styleSheets.Add(styleSheet);

			containerControlElements = container.Q("containerControls");
			containerCompiling = container.Q("containerCompiling");
			
			containerControlElements.SetEnabled(true);
			containerCompiling.SetEnabled(false);
			containerCompiling.style.display = DisplayStyle.None;

			var versionLabel = container.Q<Label>("lbVersion");
			versionLabel.text = $"Version {AsmdefManager.CurrentVersion}";
			versionLabel.tooltip = ((TextAsset)Resources.Load("auto_asmdef_changelog")).text;
			
			//buttons
			//debug

			var btnSetupGuide = container.Q<ToolbarButton>("btnGuide");
			btnSetupGuide.clicked += () => { Application.OpenURL("https://youtu.be/JhMjYnt_Yn8"); };
			
			var showDebugMsgs = container.Q<ToolbarToggle>("showDebugMsgs");
			showDebugMsgs.value = database.showDebugMessages;
			showDebugMsgs.RegisterValueChangedCallback((e) =>
			{
				database.showDebugMessages = e.newValue;
			});
			var showClassDebugMsgs = container.Q<ToolbarToggle>("showClassDebugMsgs");
			showClassDebugMsgs.value = database.showClassDebugMessages;
			showClassDebugMsgs.RegisterValueChangedCallback((e) =>
			{
				database.showClassDebugMessages = e.newValue;
			});

			var btnRescanFolders = container.Q<ToolbarButton>("debugRescan");
			btnRescanFolders.clicked += aMgr.ScanProject; 
			
			var btnCleanup = container.Q<ToolbarButton>("debugRunCleanup");
			btnCleanup.clicked += aMgr.RunCleanup;

			//workaround for different behaviour in UIBuilder than in the actual EditorWindow
			var template = container.Q<TemplateContainer>();
			if (template != null)
				template.style.flexGrow = 1;

			//	inpField.value = "set from code";

			//todo: move to a seperate function to avoid clutter
			container.Query<Toggle>().ForEach((x) =>
			{
				//	Debug.Log(x.viewDataKey);
				if (x.viewDataKey != "toolbar")
					return;

				var parent = x.parent;

				var t = x;
				var ve = parent.Q<ScrollView>();
				if (ve != null)
				{

					t.RegisterValueChangedCallback((e) =>
					{
						ve.style.display =
							e.newValue ? DisplayStyle.Flex : DisplayStyle.None;
					});

					ve.style.display =
						t.value ? DisplayStyle.Flex : DisplayStyle.None;
				}

				t.value = false;
			});

			toggleAllRuntime = container.Q<Button>("toggleAllRuntime");
			toggleAllRuntime.clicked += () => { ToggleAllAssemblies(CompilationFolder.Runtime); };
			toggleAllEditor = container.Q<Button>("toggleAllEditor");
			toggleAllEditor.clicked += () => { ToggleAllAssemblies(CompilationFolder.Editor); };
			
			excludeTextbox = container.Q<TextField>("tfExclude");
			actionExcludeUpdate = container.Q<Button>("btnExcludeTextboxApply");
			
			progressbar = container.Q("progressbarImage");
			progressLabel = container.Q<Label>("progressbarLabel");

			actionStartProcess = container.Q<Button>("actionStartProcess");
			actionRevert = container.Q<Button>("actionRevert");
			actionUpdate = container.Q<Button>("actionUpdate");

			actionStartProcess.clicked += () => { aMgr.ControlClicked(ActionButton.StartProcess); };
			actionUpdate.clicked += () => { aMgr.ControlClicked(ActionButton.Update); };
			actionRevert.clicked += () => { aMgr.ControlClicked(ActionButton.Revert); };
			
			CreateAlwaysReferencedAssembliesList(database.assembliesInProject);
			
			SetFolderHierarchy();
			SetButtonStyles();
			UpdateProgressbar();
			SetControlButtons();

			excludeTextbox.SetValueWithoutNotify(database.excludedDirectoriesRegexCustom);
			excludeTextbox.RegisterValueChangedCallback((e) => updateExcludedFoldersListFromCustomInput());

			actionExcludeUpdate.clicked += updateHierarchyTab;
			InitializeExcludedFolderList();
			//	UpdateExcludedFolderList();
			
			AsmdefManagerDatabase.Instance.CheckIfCanUseRegex();
		}

		private void OnGUI()
		{
			if (EditorApplication.isCompiling && containerControlElements.enabledSelf)
			{
				containerControlElements.SetEnabled(false);
				containerCompiling.SetEnabled(true);
				
				containerCompiling.style.display = DisplayStyle.Flex;
			}
		}

		void CreateAlwaysReferencedAssembliesList(List<string> loadedAssemblies)
		{
			foreach (var VARIABLE in loadedAssemblies)
			{

				var ass = new AssemblyReference(VARIABLE,
					database.alwaysIncludeRuntimeAssemblies.Contains(VARIABLE),
					database.alwaysIncludeEditorAssemblies.Contains(VARIABLE));
				if(firstTimeRunning)
					ass.SetValues(true, true);
				container.Q<ScrollView>("svAlwaysReferencedAssemblies").Add(ass);
			}
		}
		void SetButtonStyles()
		{
			var toggleButtons = container.Query<ToolbarToggle>().Build().ToList();
			var buttons = container.Query<Button>().Build().ToList();
			SetBtnClrs(toggleButtons);
			SetBtnClrs(buttons);
		}

		void SetBtnClrs<T>(List<T> l) where T : VisualElement
		{
			var ve = l.ConvertAll(x => (VisualElement)x);

			var cBlack = new Color(0, 0, 0);
			var cWhite = new Color(255, 255, 255);
			foreach (var item in ve)
			{
				var label = item.Q<Label>();
				var icon = item.Q("icon");
				
				item.RegisterCallback((MouseEnterEvent e) =>
				{
					SetStyleIfNotNull(label, cBlack);
					SetStyleIfNotNull(icon, cBlack);
					item.style.color = cBlack;
				});
					
				item.RegisterCallback((MouseLeaveEvent e) =>
				{
					SetStyleIfNotNull(label, cWhite);
					SetStyleIfNotNull(icon, cWhite);
					item.style.color = cWhite;
				});
			}
		}

		void SetStyleIfNotNull(VisualElement ve, Color color)
		{
			if (ve == null) return;
			if (ve.style.backgroundImage != null)
				ve.style.unityBackgroundImageTintColor = color;
			ve.style.color = color;
		}

		void SetFolderHierarchy()
		{
			var svFolderHierarchy = container.Q<ScrollView>("svFolderHierarchy");
			database.UpdateRegEXLines();
			var root = new FolderItem(Application.dataPath, true);
			FolderItem.FolderItemSelectionChanged += UpdateExcludedFolderList;
			
			svFolderHierarchy.Add(root);
		}

		void RemoveFolders()
		{
			var root = container.Q<ScrollView>("svFolderHierarchy");
			for (int i = 0; i < root.childCount; i++)
			{
				root.RemoveAt(i);
			}
		}

		void InitializeExcludedFolderList()
		{
			database.UpdateRegEXLines();
			container.Query<FolderItem>().Build().ForEach((fi) =>
				{
					fi.UpdateExcludedRegex();
					fi.UpdateBackgroundColor();	
				});
		}
		
		void UpdateExcludedFolderList()
		{
			database.UpdateRegEXLines();
			container.Query<FolderItem>().Build().ForEach((fi) =>
			{
				database.TryAddExcludedPath(fi.name, fi.ToggleValue);
				fi.UpdateBackgroundColor();
			});
		}
		
		void updateExcludedFoldersListFromCustomInput()
		{
			database.excludedDirectoriesRegexCustom = @excludeTextbox.value;
		}

		void updateHierarchyTab()
		{
			RemoveFolders();
			SetFolderHierarchy();
			
			InitializeExcludedFolderList();

			database.prepareExcluded();
			database.SaveDatabase();
		}

		void UpdateProgressbar()
		{
			var progress = Mathf.Lerp(100, 0, (float)database.sessionData.sessionProgress / 4f);
			progressbar.style.right =  new StyleLength(new Length(progress, LengthUnit.Percent));

			progressLabel.text = database.sessionData.sessionProgress.ToString();
		}

		void ToggleAllAssemblies(CompilationFolder cf)
		{
			var assemblies = container.Query<AssemblyReference>().Build().ToList();
			if (assemblies.Count > 0)
			{
				var initAssemblyReference = assemblies[0];
				bool ed = initAssemblyReference.EditorValue;
				bool ru = initAssemblyReference.RuntimeValue;
				switch (cf)
				{
					case CompilationFolder.Editor:
						assemblies.ForEach(s => s.SetValue( cf, !ed));
						break;
					case CompilationFolder.Runtime:
						assemblies.ForEach(s => s.SetValue( cf, !ru));
						break;
					case CompilationFolder.Any:
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(cf), cf, null);
				}
				
			}
		}

		void SetControlButtons()
		{
			ShowHideElement(actionStartProcess, false);
			ShowHideElement(actionUpdate, false);
			ShowHideElement(actionRevert, false);
			
			switch (database.sessionData.sessionProgress)
			{
				case AsmdefManagerDatabase.SessionProgress.Finished:
					ShowHideElement(actionRevert, true);
					ShowHideElement(actionUpdate, true);
					break;
				default:
					ShowHideElement(actionStartProcess, true);
					break;
			}
		}

		void ShowHideElement(VisualElement ve, bool state)
		{
			ve.style.display = new StyleEnum<DisplayStyle>(state ? DisplayStyle.Flex : DisplayStyle.None);
		}
	}
}