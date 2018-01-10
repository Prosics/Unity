using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HandScript : MonoBehaviour 
{
	public Transform _handFrom;
	public RectTransform _handLine;

	void OnEnable()
	{
		if ( _handFrom == null )
			enabled = false;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = (_handFrom.position + transform.position) / 2f;
		_handLine.position = pos;
		float width = Vector3.Distance (_handFrom.localPosition,transform.localPosition);
		_handLine.sizeDelta = new Vector2 (30,width);

		float angle = Prosics.Vector3Helper.Angle_180(Vector3.up,(transform.transform.localPosition - _handFrom.localPosition).normalized);
		_handLine.localEulerAngles = new Vector3 (0,0,angle);
	}
}
