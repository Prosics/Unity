using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetbundleBuilder : EditorWindow 
{
	[MenuItem("Custom/Assetbundle Builder")]
	public static void Create()
	{
		EditorWindow.GetWindow<AssetbundleBuilder> ();
	}

	int _plateformIdx = 0;
	SelectionMode _selectionMode = SelectionMode.DeepAssets;
	string bundleName = "new assetbundle";

	string result = "";
	string[] _plateformAry = null;
	public AssetbundleBuilder()
	{
		this.titleContent.text = "AssetBundle Builder";
		_plateformAry = new string[]{
			BuildTarget.NoTarget.ToString()
			,BuildTarget.Android.ToString()
			,BuildTarget.iOS.ToString()
			,BuildTarget.StandaloneWindows64.ToString()
			,BuildTarget.StandaloneOSXUniversal.ToString()};

	}

	void OnGUI()
	{
		GUILayout.BeginVertical ();

		SetOptions();

		GUILayout.Space (100);
		if(GUILayout.Button("Build AssetBundle",null))
			Build ();
		GUILayout.Label (result);
		GUILayout.EndVertical ();
	}

	void SetOptions()
	{

		GUILayout.Label ("Plateform:");
		_plateformIdx = GUILayout.Toolbar (_plateformIdx,_plateformAry,null);

		GUILayout.Space (20);
		GUILayout.Label ("Selection Mode:");
		GUILayout.BeginHorizontal ();
		ToggleSelectionMode (SelectionMode.Assets);
		ToggleSelectionMode (SelectionMode.Deep);
		ToggleSelectionMode (SelectionMode.DeepAssets);
		ToggleSelectionMode (SelectionMode.Editable);
		ToggleSelectionMode (SelectionMode.ExcludePrefab);
		//ToggleSelectionMode (SelectionMode.OnlyUserModifiable);
		ToggleSelectionMode (SelectionMode.TopLevel);
		GUILayout.EndHorizontal ();
	}
	void ToggleSelectionMode(SelectionMode mode)
	{
		if ( GUILayout.Toggle ((_selectionMode & mode) == SelectionMode.Unfiltered, mode.ToString (), null) )
		{
			_selectionMode = _selectionMode | mode;
		}
		else
		{
			_selectionMode = _selectionMode & (~mode );
		}
	}
	void Build()
	{
		Caching.CleanCache ();
		Object[] selectedAsset = Selection.GetFiltered (typeof(Object), _selectionMode);
		Debug.Log ("selected object count :" + selectedAsset == null? 0: selectedAsset.Length);
		if ( selectedAsset != null && selectedAsset.Length > 0 )
		{
			string path = Application.dataPath + "/StreamingAssets/" + bundleName + ".assetbundle";
			Debug.Log ("path :" + path);
			BuildTarget selMode = (BuildTarget)System.Enum.Parse (typeof(BuildTarget), _plateformAry [_plateformIdx]);
			if ( BuildPipeline.BuildAssetBundle (null, selectedAsset, path, BuildAssetBundleOptions.CollectDependencies, selMode) )
			{
				AssetDatabase.Refresh ();
				result = "success!";
			}
			else
			{
				result = "failed!";
			}
		}
	}
}
