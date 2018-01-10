using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Transform))]
public class TransformEditEx : DecoratorEditor 
{
	
	SerializedProperty mPos;
	SerializedProperty mRot;
	SerializedProperty mScale;

	public TransformEditEx(): base("TransformInspector"){}

	void OnEnable()
	{
		var so = serializedObject;
		mPos = so.FindProperty("m_LocalPosition");
		mRot = so.FindProperty("m_LocalRotation");
		mScale = so.FindProperty("m_LocalScale");
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		Transform targ = (Transform)target;
		EditorGUIUtility.labelWidth = 15f;
		GUILayout.BeginHorizontal();
		if ( GUILayout.Button ("P") )
		{
			targ.localPosition = Vector3.zero;
		}
		if ( GUILayout.Button ("R") )
		{
			targ.localEulerAngles = Vector3.zero;
		}
		if ( GUILayout.Button ("S") )
		{
			targ.localScale = Vector3.one;
		}
		GUILayout.EndHorizontal();

	}
}
