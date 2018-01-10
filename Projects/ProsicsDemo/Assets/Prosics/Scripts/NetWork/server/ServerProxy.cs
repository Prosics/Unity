using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Prosics
{
	public class ServerProxy
	{
		ProsicsServer _server = null;

		Queue<NetMessage> _msgQueue = new Queue<NetMessage>();


		public event System.Action<NetMessage> eventNetMessage = null;

		public ServerProxy()
		{
			_server = new ProsicsServer ();
			_server.eventReceiveMsg = OnReceiveNetMessage;
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
		/// 启动服务器
		/// </summary>
		/// <param name="ip">要绑定的ip.</param>
		/// <param name="port">要监听的端口.</param>
		public void StartUp(string ip,int port)
		{
			_server.StartUp (ip,8888);
		}

		/// <summary>
		/// 停止服务器
		/// </summary>
		public void Stop()
		{
			_server.Stop ();
		}

		/// <summary>
		/// 由于每个连接都有一个线程，目前无法发送消息。
		/// </summary>
		/// <param name="msg">Message.</param>
		public void SendMessage(NetMessage msg)
		{
		}




		void OnReceiveNetMessage(List<byte>  data)
		{
			NetMessage msg = NetMessage.Create (data);
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
			if ( _server != null )
				_server.Stop ();
		}
	}
}
