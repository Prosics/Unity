using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllor : MonoBehaviour 
{
	public float speedX = 1;
	public float speedY = 1;
	public float speedZ = 1;
	public float speedR = 1;

	// Use this for initialization
	void Start () 
	{
		
	}
	Vector3 mousePos;
	Vector3 rotate;
	// Update is called once per frame
	void Update () 
	{
		float x = 0;
		float y = 0;
		float z = 0;

		if(Input.GetKey(KeyCode.A))
			x = -speedX * Time.deltaTime;
		if(Input.GetKey(KeyCode.D))
			x = speedX * Time.deltaTime;

		if(Input.GetKey(KeyCode.W))
			z = speedZ *Time.deltaTime;
		if(Input.GetKey(KeyCode.S))
			z = -speedZ *Time.deltaTime;

		if(Input.GetKey(KeyCode.Space))
			y = speedY * Time.deltaTime;
		transform.Translate (x,y,z);

		if ( Input.GetMouseButtonDown (1) )
		{
			mousePos = Input.mousePosition;
			rotate = transform.localEulerAngles;
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = false;;
		}
		else if ( Input.GetMouseButtonUp (1) )
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		if(Input.GetMouseButton(1))
		{
			Vector2 offset = Input.mousePosition - mousePos;
			transform.localEulerAngles = new Vector3(rotate.x - offset.y * speedR,rotate.y + offset.x * speedR,0);
		}


	}
}
