using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;

using System.Text;
using System.IO;

namespace Prosics
{
	public class ProsicsClient
	{
		public string host
		{ 
			get
			{
				if ( _socket != null )
				{
					return _socket.host;
				}
				else
					return null;
			} 
		}

		public int port
		{ 
			get
			{
				if ( _socket != null )
					return _socket.port;
				else
					return -1;
			}
		}


		ClientSocket _socket = null;
		Thread _socketThread = null;

		public System.Action eventConnectSuccess = null;
		public System.Action eventConnectFailed = null;
		public System.Action eventConnectionLost = null;
		public System.Action eventBeginReconnect = null;
		public System.Action eventReconnected = null;
		public System.Action eventReconnectFailed = null;
		public System.Action<List<byte>> eventReceiveMsg = null;
		public System.Action eventSendFailed = null;


		#region interface
		public bool connected
		{
			get
			{ 
				if ( _socket == null )
					return false;
				else
					return _socket.connected;;

			}
		}
		/// <summary>
		/// 连接到目标服务器。
		/// 连接一旦建立只能在断开后才能重新建立或更改目标
		/// </summary>
		/// <param name="host">Host.</param>
		/// <param name="port">Port.</param>
		public void Connect(string host, int port)
		{
			
			if ( _socket == null )
			{
				InitSocket (host,port);
				StartThread ();
			}
			else
			{
				if ( _socket._reconnecting )
				{
					Disconnect ();
					InitSocket (host,port);
					StartThread ();
				}
				
			}

		}
		/// <summary>
		/// 断开连接,同时释放socket;
		/// </summary>
		public void Disconnect()
		{
			
			CloseConnection ();
		}
		public void Send(byte[] data)
		{
			if (connected )
			{
				byte[] package = NetBitPacker.Packet (data);
				_socket.SendPackage_Asyn (package);
			}
		}

		#endregion

	









		/// <summary>
		/// 清理socket以及其线程。
		/// </summary>
		void CloseConnection()
		{
			if ( _socket != null )
			{
				_socket.Close ();
				_socket = null;
			}
			
			if ( _socketThread != null )
			{
				_socketThread.Abort ();
				_socketThread = null;
			}
		}
			

		void InitSocket(string host, int port)
		{
			_socket = new ClientSocket ();
			_socket.host = host;
			_socket.port = port;

			_socket.eventConnectFailed = OnConnectFailed;
			_socket.eventConnectSuccess = OnConnected;
			_socket.eventConnectionLost = OnConnectionLost;
			_socket.eventBeginReconnect = OnBeginReconnect;
			_socket.eventReconnectFailed = OnReconnectFailed;
			_socket.eventReconnected = OnReconnected;
			_socket.eventSendFailed = OnSendFailed;
			_socket.eventReceived = OnReceived;
		}
		void StartThread()
		{
			_socketThread = new Thread (new ThreadStart (IOThread));
			_socketThread.IsBackground = true;
			_socketThread.Start ();
		}
			
#region Receive/Send

		void IOThread()
		{
			ClientSocket socket = _socket;


			try
			{
				while (true)
				{
					//重连循环

					if (socket.Thread_TryConnect () )
					{
						socket.Thread_Com ();

						//连接中断 尝试重连
						socket._reconnecting = true;
					}
					Thread.Sleep(10000);

				}
			}
			catch(ThreadAbortException e)
			{
				Logger.LogException (e);
			}
			catch(Exception e)
			{
				Logger.LogException (e);
			}
			finally
			{
				if ( socket != null )
				{
					socket._reconnecting = false;
					socket.Close ();
				}
			}

				


		}
			

#endregion


#region Callback
		void OnConnected()
		{
			Logger.Log ("ProsicsClient:" + "OnConnected");
			if ( eventConnectSuccess != null )
				eventConnectSuccess ();

		}
		void OnConnectFailed()
		{
			Logger.Log ("ProsicsClient:" + "OnConnectFailed");
			if ( eventConnectFailed != null )
				eventConnectFailed ();
		}
		void OnConnectionLost()
		{
			Logger.Log ("ProsicsClient:" + "OnConnectionLost");
			if ( eventConnectionLost != null )
				eventConnectionLost ();
		}
		void OnBeginReconnect()
		{
			Logger.Log ("ProsicsClient:" + "OnBeginReconnect");
			if ( eventBeginReconnect != null )
				eventBeginReconnect ();
		}
		void OnReconnected()
		{
			Logger.Log ("ProsicsClient:" + "OnReconnected");
			if ( eventReconnected != null )
				eventReconnected ();
			
		}
		void OnReconnectFailed()
		{
			Logger.Log ("ProsicsClient:" + "OnReconnectFailed");
			if ( eventReconnectFailed != null )
				eventReconnectFailed ();
		}

		void OnSendFailed()
		{
			Logger.Log ("ProsicsClient:" + "OnSendFailed");
			if ( eventSendFailed != null )
				eventSendFailed ();
		}
		void OnReceived(List<byte> data)
		{
			if ( eventReceiveMsg != null )
				eventReceiveMsg (data);
		}
#endregion
	}


		
}
