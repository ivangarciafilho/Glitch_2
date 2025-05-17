using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace TSD.AsmdefManagement
{
	/// <summary>
	/// Used for formatting input string to asmdef like syntax
	/// </summary>
	public class AsmdefUtils
	{
		private const string assemblyFirstPassRuntime = "Assembly-CSharp-firstpass";
		private const string assemblyFirstPassEditor = "Assembly-CSharp-Editor-firstpass";
		private const string assemblySecondEditorAssembly = "Assembly-CSharp-Editor";
		private const string assemblySecondRuntimeAssembly = "Assembly-CSharp";

		

		/// <summary>
		/// input as regex
		/// </summary>
		/// <param name="inputToSearchFor"></param>
		/// <param name="patternsToCheckAgainst"></param>
		/// <returns></returns>
		public static bool IsMatch(string inputToSearchFor, List<string> patternsToCheckAgainst)
		{
			foreach (var pattern in patternsToCheckAgainst)
			{
				if(pattern == "") { continue; }
				if (AsmdefManagerDatabase.Instance.CanUseRegex ? 
					Regex.IsMatch(inputToSearchFor, string.Format(@"{0}/{1}",Application.dataPath, pattern)) 
					: string.Equals(inputToSearchFor,string.Format(@"{0}/{1}",Application.dataPath, pattern) )) 
				{ return true; }
			}
			return false;
		}

		public static string FormatGUID(string GUID)
		{
			if (GUID == null)
			{
				throw new ArgumentNullException(nameof(GUID));
			}

			return "GUID:" + GUID;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <param name="compilationFolder">Base assembly we are looking for</param>
		/// <param name="isLocalPath">Local means the <paramref name="filePath"/> is Assets/myFolder</param> instead of C:\Whatever\Assets\myFolder</param>
		/// <returns></returns>
		public static bool IsPartOfBaseAssembly(string filePath, CompilationFolder compilationFolder, bool isLocalPath = true)
		{
			if (!isLocalPath)
			{
				filePath = filePath.Substring(Application.dataPath.Length - 6).Replace('\\', '/');
			}
			var myAssName = GetAssembly(filePath);
			switch (compilationFolder)
			{
				case CompilationFolder.Editor:
					return string.Equals(assemblyFirstPassEditor, myAssName) || string.Equals(assemblySecondEditorAssembly, myAssName);
				case CompilationFolder.Runtime:
					return string.Equals(assemblyFirstPassRuntime, myAssName) || string.Equals(assemblySecondRuntimeAssembly, myAssName);
				case CompilationFolder.Any:
					return string.Equals(assemblyFirstPassEditor, myAssName) || string.Equals(assemblySecondEditorAssembly, myAssName) || string.Equals(assemblyFirstPassRuntime, myAssName) || string.Equals(assemblySecondRuntimeAssembly, myAssName);
				default:
					Debug.LogError("Did the CompilationFolder enum change? This message shouldn't ever pop up.");
					return false;
			}
		}

		/// <summary>
		/// Returns the assembly Unity put the provided file in. Path must be local, with extension(Assets/myFile.cs)
		/// </summary>
		/// <param name="filePath">Local path with extension</param>
		/// <returns></returns>
		public static string GetAssembly(string filePath)
		{
			foreach (var item in LoadedAssemblies)
			{
				foreach (var item2 in item.sourceFiles)
				{
					if (string.Equals(item2, filePath))
					{
						return item.name;
					}
				}
			}
			return null;
		}

		static List<UnityEditor.Compilation.Assembly> _loadedAssemblies;
		static List<UnityEditor.Compilation.Assembly> LoadedAssemblies
		{
			get
			{
				if (_loadedAssemblies == null)
				{
					_loadedAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies().ToList();
				}
				return _loadedAssemblies;
			}
		}
	}
	
	public enum CompilationFolder
	{
		Editor,
		Runtime,
		Any
	}

	internal class AsmdefObject : IAutoAssemblyData
	{
		public string name;
		public string[] references;
		public string[] includePlatforms;
		public string[] excludePlatforms;
		public bool allowUnsafeCode;
		public bool autoReferenced;
		public bool overrideReferences;
		public string[] precompiledReferences;
		public string[] defineConstraints;
		public string[] optionalUnityReferences;
		public string additionalData;

		/// <summary>
		/// Call before comparing two different sets
		/// </summary>
		internal void Sort()
		{
			Array.Sort(references, StringComparer.InvariantCulture);
			Array.Sort(includePlatforms, StringComparer.InvariantCulture);
			Array.Sort(excludePlatforms, StringComparer.InvariantCulture);
			Array.Sort(precompiledReferences, StringComparer.InvariantCulture);
			Array.Sort(defineConstraints, StringComparer.InvariantCulture);
			Array.Sort(optionalUnityReferences, StringComparer.InvariantCulture);
		}
		
		internal bool Equals(AsmdefObject other)
		{
			/*
			//Elements must be sorted to avoid any potential conflicts
			//sort *this* asmdef
			Array.Sort(references, StringComparer.InvariantCulture);
			Array.Sort(includePlatforms, StringComparer.InvariantCulture);
			Array.Sort(excludePlatforms, StringComparer.InvariantCulture);
			Array.Sort(precompiledReferences, StringComparer.InvariantCulture);
			Array.Sort(defineConstraints, StringComparer.InvariantCulture);
			Array.Sort(optionalUnityReferences, StringComparer.InvariantCulture);
			//sort the *other* asmdef
			Array.Sort(other.references, StringComparer.InvariantCulture);
			Array.Sort(other.includePlatforms, StringComparer.InvariantCulture);
			Array.Sort(other.excludePlatforms, StringComparer.InvariantCulture);
			Array.Sort(other.precompiledReferences, StringComparer.InvariantCulture);
			Array.Sort(other.defineConstraints, StringComparer.InvariantCulture);
			Array.Sort(other.optionalUnityReferences, StringComparer.InvariantCulture);
			*/
			//check if the elements are equal
			return name == other.name 
			       && references.SequenceEqual(other.references) 
			       && includePlatforms.SequenceEqual(other.includePlatforms) 
			       && excludePlatforms.SequenceEqual(other.excludePlatforms) 
			       && allowUnsafeCode == other.allowUnsafeCode 
			       && autoReferenced == other.autoReferenced 
			       && overrideReferences == other.overrideReferences 
			       && precompiledReferences.SequenceEqual(other.precompiledReferences) 
			       && defineConstraints.SequenceEqual(other.defineConstraints) 
			       && optionalUnityReferences.SequenceEqual(other.optionalUnityReferences) 
			       && additionalData == other.additionalData;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (name != null ? name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (references != null ? references.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (includePlatforms != null ? includePlatforms.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (excludePlatforms != null ? excludePlatforms.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ allowUnsafeCode.GetHashCode();
				hashCode = (hashCode * 397) ^ autoReferenced.GetHashCode();
				hashCode = (hashCode * 397) ^ overrideReferences.GetHashCode();
				hashCode = (hashCode * 397) ^ (precompiledReferences != null ? precompiledReferences.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (defineConstraints != null ? defineConstraints.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (optionalUnityReferences != null ? optionalUnityReferences.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (additionalData != null ? additionalData.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return string.Format("Name: {0} \nReferences: {1} \nInclude Platforms: {2}", 
				name,
				string.Join(",", references), 
				string.Join(",", includePlatforms));
		}

		public AsmdefObject(
			string _name,
			string[] _references = null,
			string[] _includePlatforms = null,
			string[] _excludePlatforms = null,
			bool _allowUnsafeCode = true,
			bool _autoReferenced = true,
			bool _overrideReferences = false,
			string[] _precompiledReferences = null,
			string[] _defineConstraints = null,
			string[] _optionalUnityReferences = null,
			string _additionalData = null)
		{
			name = _name;
			references = _references;
			includePlatforms = _includePlatforms ?? new string[0];
			excludePlatforms = _excludePlatforms ?? new string[0];
			allowUnsafeCode = _allowUnsafeCode;
			autoReferenced = _autoReferenced;
			overrideReferences = _overrideReferences;
			precompiledReferences = _precompiledReferences ?? new string[0];
			defineConstraints = _defineConstraints ?? new string[0];
			optionalUnityReferences = _optionalUnityReferences ?? new string[0];
			additionalData = _additionalData;
		}
		
		public string AdditionalData { get=>additionalData; set=>additionalData = value; }
	}

	internal class AsmRefObject : IAutoAssemblyData
	{
		public string reference;
		public string _additionalData;

		public AsmRefObject(string reference, string additionalData)
		{
			this.reference = reference;
			this.AdditionalData = additionalData;
		}

		public string AdditionalData { get=>_additionalData; set=>_additionalData = value; }
	}

	public interface IAutoAssemblyData
	{
		string AdditionalData { get; set; }
	}
}