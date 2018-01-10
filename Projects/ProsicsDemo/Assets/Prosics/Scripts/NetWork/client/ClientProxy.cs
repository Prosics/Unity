using System;
using System.Collections;
using System.Collections.Generic;
namespace Prosics
{
	public class ClientProxy
	{
		ProsicsClient _client = null;

		Queue<NetMessage> _msgQueue = new Queue<NetMessage>();


		public event System.Action<NetMessage> eventNetMessage = null;
		public string host
		{ 
			get
			{
				if ( _client != null )
				{
					return _client.host;
				}
				else
					return null;
			} 
		}

		public int port
		{ 
			get
			{
				if ( _client != null )
					return _client.port;
				else
					return -1;
			}
		}

		public bool isConnected
		{
			get
			{
				if ( _client == null )
					return false;
				else
					return _client.connected;
			}
		}


		public ClientProxy()
		{
			_client = new ProsicsClient ();
			_client.eventConnectFailed = OnConnectFailed;
			_client.eventConnectSuccess = OnConnectSuccess;
			_client.eventConnectionLost = OnConnectionLost;
			_client.eventBeginReconnect = OnBeginReconnect;
			_client.eventReconnectFailed = OnReconnectFailed;
			_client.eventReconnected = OnReconnected;
			_client.eventReceiveMsg = OnReceiveNetMessage;
			_client.eventSendFailed = OnSendFailed;
		}

		/// <summary>
		/// 分发网络消息，在进程的主线程中调用此方法进行分发以避免多线程冲突。
		/// </summary>
		public void DispatchMsg()
		{
			lock (_msgQueue)
			{
				if ( _msgQueue.Count > 0 )
				{
					NetMessage msg = _msgQueue.Dequeue ();
					if ( eventNetMessage != null )
						eventNetMessage (msg);
				}
			}
		}
		/// <summary>
		/// 调用Client socket连接服务器
		/// </summary>
		/// <param name="host">Host.</param>
		/// <param name="port">Port.</param>
		public void Connect(string host, int port)
		{
			_client.Connect (host, port);
		}
		public void Disconnect()
		{
			_client.Disconnect ();
		}
		public void SendMessage(NetMessage msg)
		{
			_client.Send (msg.GetBytes());
		}




		void OnReceiveNetMessage(List<byte>  data)
		{
			NetMessage msg = NetMessage.Create (data);
			EnqueueMsg (msg);
		}
		void OnConnectSuccess()
		{
			NetMessage msg = new NetMessage (NetMessage.MsgID_Connect_Success);
			EnqueueMsg (msg);
		}
		void OnConnectFailed()
		{
			NetMessage msg = new NetMessage (NetMessage.MsgID_Connect_Failed);
			EnqueueMsg (msg);
		}
		void OnConnectionLost()
		{
			NetMessage msg = new NetMessage (NetMessage.MsgID_Connection_Lost);
			EnqueueMsg (msg);
		}
		void OnBeginReconnect()
		{
			NetMessage msg = new NetMessage (NetMessage.MsgID_Reconnection_Begin);
			EnqueueMsg (msg);
		}
		void OnReconnected()
		{
			NetMessage msg = new NetMessage (NetMessage.MsgID_Reconnect_Success);
			EnqueueMsg (msg);
		}
		void OnReconnectFailed()
		{
			NetMessage msg = new NetMessage (NetMessage.MsgID_Reconnect_Failed);
			EnqueueMsg (msg);
		}
		void OnSendFailed()
		{
			NetMessage msg = new NetMessage (NetMessage.MsgID_Send_Failed);
			EnqueueMsg (msg);
		}
		void EnqueueMsg(NetMessage msg)
		{
			if ( msg != null )
			{
				lock (_msgQueue)
				{
					_msgQueue.Enqueue (msg);
				}
			}
		}
		void OnApplicationQuit()
		{
			if ( _client != null )
				_client.Disconnect ();
		}
	}
}
