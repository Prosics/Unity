using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Prosics
{
	public class Logger
	{
		public static void Log(System.Object obj)
		{
			Debug.Log (obj);		
		}
		public static void LogException(System.Exception e)
		{
			Debug.Log (e.ToString ());
			Debug.Log (e.StackTrace);
		}
	}
}
