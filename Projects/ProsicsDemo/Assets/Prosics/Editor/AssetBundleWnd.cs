using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleWnd :  EditorWindow
{
	
	public static void Create()
	{
		//EditorWindow.GetWindow<AssetBundleWnd> ();
	}


	//BuildTarget _plateform = BuildTarget.NoTarget;
	//SelectionMode _selectionMode = SelectionMode.DeepAssets;
	//string bundleName = "new assetbundle";

	//string result = "";
	public AssetBundleWnd()
	{
		//this.titleContent.text = "AssetBundle Builder";
	}
	void OnGUI()
	{
		//GUILayout.BeginVertical ();
		//SetOptions();

		//GUILayout.Space (100);
		//if(GUILayout.Button("Build AssetBundle",null))
			//Build ();
		//GUILayout.Label (result);
		//GUILayout.EndVertical ();
	}

	void SetOptions()
	{
		//_plateform = (BuildTarget)GUILayout.Toolbar ((int)_plateform,new string[]{BuildTarget.NoTarget.ToString(),BuildTarget.Android.ToString(),BuildTarget.iOS.ToString(),BuildTarget.StandaloneWindows64.ToString(),BuildTarget.StandaloneOSXUniversal.ToString()});
		//_selectionMode = (SelectionMode)GUILayout.Toolbar ((int)_selectionMode,new string[]{
		/*	SelectionMode.Assets.ToString()
			,SelectionMode.Deep.ToString()
			,SelectionMode.DeepAssets.ToString()
			,SelectionMode.Editable.ToString()
			,SelectionMode.ExcludePrefab.ToString()
			,SelectionMode.OnlyUserModifiable.ToString()
			,SelectionMode.TopLevel.ToString()
			,SelectionMode.Unfiltered.ToString()});*/
		//bundleName = GUILayout.TextField (bundleName,null);

	}
	/*
	void Build()
	{
		
		Object[] selectedAsset = Selection.GetFiltered (typeof(Object), _selectionMode);
		string path = Application.dataPath + "/StreamingAssets/" + bundleName + ".assetbundle";
		if ( BuildPipeline.BuildAssetBundle (null, selectedAsset, path, BuildAssetBundleOptions.CollectDependencies,_plateform) )
		{
			result = "success!";
		}
		else
		{
			result = "failed!";
		}
	}*/


}
