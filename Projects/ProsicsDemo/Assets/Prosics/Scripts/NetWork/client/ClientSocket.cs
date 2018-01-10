// Date: 	    2017/9/6
// Author: 	    li kunlun <>
// Description:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace Prosics
{
	internal class ClientSocket
	{
		Socket _socket = null;
		public string host{ get; set; }
		public int port{ get;  set; }
		int _sendTimeout = 5000;
		int _receiveTimeout = 2000;

		int _heartbeatInterval = 10;//秒
		int _heartbeatOutTime = 20;//秒

		List<byte> _mainBuff = new List<byte> ();
		byte[] _tmpBuff = new byte[4096];


		//断线重连中的标识
		public bool _reconnecting = false;


		public System.Action eventConnectSuccess = null;
		public System.Action eventConnectFailed = null;
		public System.Action eventConnectionLost = null;
		public System.Action eventBeginReconnect = null;
		public System.Action eventReconnected = null;
		public System.Action eventReconnectFailed = null;
		public System.Action<List<byte>> eventReceived = null;
		public System.Action eventSendFailed = null;

		public ClientSocket()
		{
			_socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
			_socket.ReceiveTimeout = _receiveTimeout;
			_socket.SendTimeout = _sendTimeout;
			_socket.Blocking = true;
		}
		public bool connected
		{
			get
			{ 
				if ( _socket != null && _socket.Connected )
					return true;
				else
					return false;

			}
		}
		public void Close()
		{
			NetWorkHelper.SafeClose (_socket);
		}

		public bool Thread_TryConnect()
		{
			try
			{
				Logger.Log ("Begin Connect to " + host + ":" + port);
				if (_socket.Connected )
					_socket.Disconnect (true);
				if(_reconnecting == true)
					OnBeginReconnect();
				IPAddress ipAddress = IPAddress.Parse (host);//解析IP地址
				IPEndPoint ipEndpoint = new IPEndPoint (ipAddress, port);
				_socket.Connect (ipEndpoint);
				Logger.Log ("Connected!");
				if(_reconnecting == true)
					OnReconnected();
				else
					OnConnected();
				_reconnecting = true;
				return true;
			}

			catch (SocketException e)
			{
				Logger.Log ("Connect failed!");
				Logger.LogException (e);
				if ( _reconnecting == true )
					OnReconnectFailed ();
				else
					OnConnectFailed ();

				return false;	


			}
			catch(ThreadAbortException e)
			{
				throw e;
			}
			catch(Exception e)
			{
				return false;
			}

		}

		public void Thread_Com()
		{
			long lastTicks = DateTime.Now.Ticks;
			long lasstServerTicks = DateTime.Now.Ticks;

			try
			{

				while (true)
				{
					try
					{
						if ( !_socket.Connected )
						{
							Logger.Log ("connection is closed");
							break;
						}
						if ( DateTime.Now.Ticks - lasstServerTicks >= _heartbeatOutTime * 10000000 )
						{
							Logger.Log ("heartbeat outtime.");
							break;
						}

						if ( DateTime.Now.Ticks - lastTicks >= _heartbeatInterval * 10000000 )
						{
							lastTicks = DateTime.Now.Ticks;
							SendPackage (NetBitPacker.Packet (null, 255));
						}


						int count = _socket.Receive (_tmpBuff);


						if ( count > 0 )
						{
							NetWorkHelper.PushToBuffer (_tmpBuff, count, _mainBuff);
							int msgType;
							List<byte> data = NetBitPacker.GetPacket (_mainBuff, out msgType);
							if ( data != null )
							{
								//心跳包
								if ( msgType == 255 )
								{
									Logger.Log ("heart beat");
									lasstServerTicks = DateTime.Now.Ticks;
								}
								else if ( data != null )
								{
									if ( eventReceived != null )
										eventReceived (data);
								}
							}

						}
						else if ( count == 0 )
						{
							throw new Exception (" Server is closed.");
						}
					}
					catch (SocketException e)
					{
						if ( e.SocketErrorCode == SocketError.WouldBlock )
						{

							continue;
						}
						else
						{
							Logger.Log (e.SocketErrorCode.ToString ());
							Logger.LogException (e);
							break;
						}
					}
					catch(ThreadAbortException e)
					{
						throw e;
					}
					catch(Exception e)
					{
						break;
					}

				}
			}
			finally
			{
				Logger.Log ("Connection lost");
				OnConnectionLost ();
			}
		}

		public void Send(byte[] data)
		{
			if ( connected )
			{
				byte[] package = NetBitPacker.Packet (data);
				SendPackage_Asyn (package);
			}
		}

		public void SendPackage(byte[] package)
		{
			try
			{
				if ( connected)
				{
					_socket.Send (package);
				}
			}
			catch (System.Exception e)
			{
				Logger.LogException (e);
				OnSendFailed ();
			}

		}

		public void SendPackage_Asyn(byte[] package)
		{
			try
			{
				if ( connected)
				{
					_socket.BeginSend(package,0,package.Length,SocketFlags.None,OnSendCallback,_socket);
				}
			}
			catch (System.Exception e)
			{
				Logger.LogException (e);
				OnSendFailed ();
			}

		}









		void OnConnected()
		{
			if ( eventConnectSuccess != null )
				eventConnectSuccess ();

		}
		void OnConnectFailed()
		{
			if ( eventConnectFailed != null )
				eventConnectFailed ();
		}
		void OnConnectionLost()
		{
			if ( eventConnectionLost != null )
				eventConnectionLost ();
		}
		void OnBeginReconnect()
		{
			if ( eventBeginReconnect != null )
				eventBeginReconnect ();
		}
		void OnReconnected()
		{
			if ( eventReconnected != null )
				eventReconnected ();
		}
		void OnReconnectFailed()
		{
			if ( eventReconnectFailed != null )
				eventReconnectFailed ();
		}

		void OnSendFailed()
		{
			if ( eventSendFailed != null )
				eventSendFailed ();
		}

		void OnSendCallback(IAsyncResult asyncSend)
		{
			try
			{
				Socket client = (Socket)asyncSend.AsyncState;
				client.EndSend (asyncSend);
			}
			catch (Exception e)
			{
				if ( eventSendFailed != null )
					eventSendFailed ();
				Logger.LogException (e);
			}
		}


	}
}

