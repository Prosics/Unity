using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AngleTest : MonoBehaviour 
{
	public Transform _fromBlue;
	public Transform _toGreen;
	public Text _text;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		Vector3 from = _fromBlue.transform.position - transform.position;
		Vector3 to = _toGreen.transform.position - transform.position;
		float angle = Prosics.Vector3Helper.Angle_180 (from,to,transform.forward);
		_text.text = angle.ToString ();
	}
}
