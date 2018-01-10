using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace Prosics
{
	public class ConnectionThread 
	{
		Socket _socket = null;
		Thread _thread = null;
		List<byte> _mainBuff = new List<byte>();
		byte[] _tmpBuff = new byte[4096];
		int _heartbeatInterval = 20;//秒

		public System.Action<ConnectionThread> eventThreadWillTerminate = null;
		public System.Action<List<byte>> eventReceiveData = null;


		public ConnectionThread(Socket socket, int heartbeat = 20)
		{
			_heartbeatInterval = heartbeat;
			_socket = socket;
			_socket.ReceiveTimeout = 2000;
			Logger.Log ("connection create :");
			Logger.Log ("receive TimeOut:" + socket.ReceiveTimeout);
			Logger.Log ("send TimeOut:" + socket.SendTimeout);
			Logger.Log ("block:" + socket.Blocking); 

		}

		public void Start()
		{
			
			_thread = new Thread (IOThread);
			_thread.Start ();
		}
		public void Close()
		{
			Logger.Log ("ConnectionThread will Terminate.");
			if ( _thread != null )
				_thread.Abort ();
			else
			{
				if ( eventThreadWillTerminate != null )
					eventThreadWillTerminate (this);
			}
				
		}

		void IOThread()
		{
			ConnectionThread instance = this;
			long lastTicks = DateTime.Now.Ticks;
			try
			{
				while (true)
				{
					try
					{
						//心跳检测

						if(DateTime.Now.Ticks - lastTicks >=_heartbeatInterval * 10000000)
						{
							throw new Exception(" Heartbeat out time.");
						}


						int count = _socket.Receive(_tmpBuff);
						if (count > 0)
						{
							Logger.Log("receive :" + count);
							NetWorkHelper.PushToBuffer(_tmpBuff,count,_mainBuff);
							int msgType;
							List<byte> data = NetBitPacker.GetPacket (_mainBuff,out msgType);
							if (data != null)
							{
								//心跳包
								if(msgType == 255)
								{
									Logger.Log("heart beat");
									lastTicks = DateTime.Now.Ticks;
									//发送心跳包
									_socket.Send(NetBitPacker.Packet(null,255));
								}
								else
								{
									if(eventReceiveData != null)
										eventReceiveData(data);
								}
							}
						}
						else if(count == 0)
						{
							throw new Exception(" client has disconnect!");
						}
					}
					catch(SocketException e)
					{
						if(e.SocketErrorCode == SocketError.WouldBlock)
						{
							continue;
						}
						else if(e.SocketErrorCode == SocketError.TimedOut)
						{
							continue;
						}
						else
						{
							Logger.Log(e.SocketErrorCode.ToString());
							Logger.Log(e);
							break;
						}
					}
					catch(Exception e)
					{
						Logger.Log(e);
						break;
					}
				}
					

			}
			finally
			{
				NetWorkHelper.SafeClose (_socket);
				if ( eventThreadWillTerminate != null )
					eventThreadWillTerminate (this);
				Logger.Log ("<ConnectionThread> has Terminated.");


			}
				


		}
	}
}
