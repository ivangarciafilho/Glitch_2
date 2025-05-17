using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public static class UIToolkitTSD 
{
	
	public static TemplateContainer cloneVTA(VisualTreeAsset vta)
	{
#if UNITY_2020_OR_NEWER
		return vta.Instantiate(); //make sure this is compatible with later unity versions!
#else //UNITY 2019
		return vta.CloneTree();
#endif
	}
}
