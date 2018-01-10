using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Prosics
{
	public class ProsicsServer
	{
		string _ip;
		int _port;
		Socket _serverSocket = null;
		Thread _listenThread = null;
		List<ConnectionThread> _connections = new List<ConnectionThread>();
		bool _isRunning = false;

		public System.Action eventNewConnection = null; 
		public System.Action<List<byte>> eventReceiveMsg = null; 
		public void StartUp(string ip,int port)
		{
			//服务器IP地址  
			if ( !_isRunning )
			{
				_ip = ip;
				_port = port;
				StartUp ();
			}


		}
		public void Stop()
		{
			if ( _isRunning )
			{
				Logger.Log ("Server will Terminated.");
				_listenThread.Abort ();
			}
			lock (_connections)
			{
				if ( _connections.Count > 0 )
				{
					foreach (ConnectionThread t in _connections)
					{
						t.eventThreadWillTerminate = null;
						t.eventReceiveData = null;
					}

					while (_connections.Count > 0)
					{
						ConnectionThread ct = _connections [0];
						_connections.RemoveAt (0);
						ct.Close ();
					}
					Logger.Log ("All ConnectionThread has been Close");
				}
			}
		}
		void StartUp()
		{
			try
			{
				IPAddress ip = IPAddress.Parse(_ip);  
				_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
				_serverSocket.Bind(new IPEndPoint(ip, _port));  //绑定IP地址：端口  
				_serverSocket.Listen(10);    //设定最多10个排队连接请求  
				//通过Clientsoket发送数据  
				_listenThread = new Thread(ListenThread);  
				_listenThread.Start();
				_isRunning = true;
				Logger.Log ("Server has startup! Binded : " + _ip + ":" + _port);
			}
			catch (Exception e)
			{
				Logger.Log (e.ToString());
			}
		}

		void ListenThread()
		{
			ProsicsServer instance = this;
			try
			{
				while (true)  
				{ 
					
						Socket client = _serverSocket.Accept();  
						//client.IOControl(IOControlCode.KeepAliveValues,NetWorkHelper.GetKeppAlive(),null);
						ConnectionThread ct = new ConnectionThread (client);
						Logger.Log ("#### new connection :" + client.RemoteEndPoint.ToString());
						lock(_connections)
						{
							_connections.Add(ct);
						}
						ct.eventThreadWillTerminate = OnConnectionWillTerminate;
						ct.eventReceiveData = OnReceiveData;
						ct.Start ();
					
				}  
			}
			catch(System.Exception e)
			{
				Logger.Log (e.ToString());
				_isRunning = false;
			}
			finally
			{
				
				Stop ();
				NetWorkHelper.SafeClose (_serverSocket);
				Logger.Log ("Server is Terminated.");

			}

		}

		void OnConnectionWillTerminate(ConnectionThread connection)
		{
			if ( this == null )
				return;
			lock (_connections)
			{
				if ( _connections != null && _connections.Contains (connection) )
					_connections.Remove (connection);
				Logger.Log ("a ConnectThread has removed.");
			}
		}
		void OnReceiveData(List<byte> bytes)
		{
			Logger.Log ("receive net msg size:" + bytes.Count);
			if ( this == null )
				return;
			if ( eventReceiveMsg != null )
				eventReceiveMsg (bytes);
		}
	}
}
