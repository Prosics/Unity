using System;
using System.Collections;
using System.Collections.Generic;

namespace Prosics
{
	public class NetMessage
	{
		//前100预留
		public const int MsgID_Connect_Success = 1;
		public const int MsgID_Connect_Failed = 2;
		public const int MsgID_Connection_Lost = 3;
		public const int MsgID_Reconnect_Success = 4;
		public const int MsgID_Reconnect_Failed = 5;
		public const int MsgID_Reconnection_Begin = 6;
		public const int MsgID_Send_Failed = 7;


		Dictionary<string,System.Object> _params = new Dictionary<string, Object>();

		public NetMessage(int msgId)
		{
			_params.Add ("id", msgId);
		}
		public int msgId
		{
			get
			{ 
				if ( _params.ContainsKey ("id") )
				{
					return (int)_params ["id"];
				}
				else
					return -1;
			}
		}
		public void AddParams(string key, Object value)
		{
			if ( _params.ContainsKey (key) )
				_params [key] = value;
			else
				_params.Add (key,value);
		}
		public void Remove(string key)
		{
			if ( _params.ContainsKey (key) )
				_params.Remove (key);
		}
		public Object GetValue(string key)
		{
			if ( _params.ContainsKey (key) )
				return _params [key];
			else
				return null;
		}
		public bool Contains(string key)
		{
			return _params.ContainsKey (key);
		}
		public byte[] GetBytes()
		{
			return Serializer.Serialize (_params);
		}
		public static NetMessage Create(List<byte> bytes)
		{
			NetMessage msg = new NetMessage (0);
			msg._params = Serializer.Deserilize(bytes.ToArray(),0,bytes.Count) as Dictionary<string ,Object>;
			return msg;
		}
	}
}
