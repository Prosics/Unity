using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
namespace Prosics
{
	public class NetWorkHelper
	{
		public static void PushToBuffer(byte[] bytes, int length, List<byte> buffer)
		{
			for (int i = 0; i < length; i++)
			{
				buffer.Add (bytes [i]);
			}
		}
		public static byte[] GetKeppAlive()
		{
			byte[] bytes = new byte[sizeof(uint) * 3];
			System.BitConverter.GetBytes ((uint)1).CopyTo (bytes,0);//是否启用keep-Alive
			System.BitConverter.GetBytes ((uint)20000).CopyTo (bytes,sizeof(uint));//多长时间后开始第一次探测
			System.BitConverter.GetBytes ((uint)5000).CopyTo (bytes,sizeof(uint)*2);//探测时间间隔
			return bytes;
		}
		public static void SafeClose(Socket socket)  
		{  
			if (socket == null)  
				return;  

			if (!socket.Connected)  
				return;  

			try  
			{  
				Debug.Log("try shutdown socket");
				socket.Shutdown(SocketShutdown.Both);

			}  
			catch  
			{  
			}  

			try  
			{  
				Debug.Log("try close socket");
				socket.Close();  
			}  
			catch  
			{  
			}  
		}
	}
}
