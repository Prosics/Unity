using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prosics
{
	public class NetBitPacker
	{

		static readonly byte[] packetBegin = new byte[]{ 0X00, 0Xff };
		//包头
		static readonly byte[] packetEnd = new byte[]{ 0X3e, 0Xf1 };
		//基本chekcode
		static readonly byte baseCheckCode = 0x8a;
		//包尾
		//基本数据长度，包头包尾数据长度，校验位 的和，不包括命令内容和  数据内容
		//小于这个长度无法继续进行
		const int baseBytesLength = 8;



		/// <summary>
		/// 数据格式[包头2byte][消息类型1B][内容长度2byte][内容][校验位1byte][包尾2byte]
		/// 消息类型：默认为0，255为心跳包
		/// 数据长度: 数据长度与校验位之间的长度
		/// 校验位：，数据长度 ，网络事件ID长度，内容长度  字节以此异或
		/// </summary>
		/// <returns>返回null解析失败，否则是数据包内容</returns>
		public static List<byte> GetPacket(List<byte> buff,out int msgType)
		{
			msgType = 0;
			if ( buff.Count < baseBytesLength )
			{
				//数据长度不足以支持基本解析，放弃解析。
				return null;
			}

			int idx = 0;
			for (;;)
			{
				int startIdx = -1;
				//查找头索引
				for (; idx + 1 < buff.Count; idx++)
				{

					if ( buff [idx] == packetBegin [0] && buff [idx + 1] == packetBegin [1] )
					{
						startIdx = idx;
						break;
					}
				}


				if ( startIdx >= 0 )
				{
					/******************************/
					//找到包头，继续查找包尾
					/******************************/
					if ( startIdx + baseBytesLength <= buff.Count )
					{
						//数据长度足以支持基本解析。
						//数据长度开始索引
						int dtaLIdx = startIdx +1 + 2;
						byte[] dataLBytes = buff.GetRange (dtaLIdx, 2).ToArray ();
						System.Array.Reverse (dataLBytes);
						ushort dataL = System.BitConverter.ToUInt16 (dataLBytes, 0);

						//包尾数据起始索引 
						int packetEndIdx = dtaLIdx + 2 + dataL + 1;
						if ( packetEndIdx + 2 <= buff.Count )
						{
							/////////////////////
							//数据长度足以解析,被认可的数据包
							/////////////////////
							if ( buff [packetEndIdx] == packetEnd [0] && buff [packetEndIdx + 1] == packetEnd [1] )
							{
								////////////////
								List<byte> data = buff.GetRange (dtaLIdx + 2, dataL);
								byte checkCode = buff [dtaLIdx + 2 + dataL];
								if ( checkCode == GetCheckCode (data) )
								{
									msgType = buff [startIdx + 2];
									UnityEngine.Debug.Log ("校验成功 有效的数据");
									buff.RemoveRange (0, packetEndIdx + 2);
									return data;
								}
								else
								{
									UnityEngine.Debug.Log ("无效的数据，把查询索引设置为包尾，从包尾之后继续查询，之前的将会丢弃。");
									idx = packetEndIdx + 1; 
									continue;
								}
							}
							else
							{
								////////////////////
								UnityEngine.Debug.Log ("idx:" + idx + " dataL:" + dataL + "  PacketEndIdx:" + packetEndIdx);
								UnityEngine.Debug.Log ("包尾错误，无效的包头,继续查找下一个包头,curBuffSize:" + buff.Count + "  curHeadIdx:" + startIdx);
								////////////////////
								continue;

							}
						}
						else
						{
							UnityEngine.Debug.Log ("idx:" + idx + " dataL:" + dataL + "  PacketEndIdx:" + packetEndIdx);
							UnityEngine.Debug.Log ("包尾索引超出缓存长度，放弃解析继续接受数据,curBuffSize:" + buff.Count + "  curHeadIdx:" + startIdx);

						}

					}
					else
					{

						UnityEngine.Debug.Log ("数据长度不足以支持基本解析，放弃解析。");
					}

					//移除包头(******不包含******)之前的数据
					buff.RemoveRange (0, startIdx - 0);
					return null;


				}
				else
				{
					UnityEngine.Debug.Log ("未找到包头，丢弃数据");
					buff.Clear ();
					return null;
				}
			}

		}
			
		/// <summary>
		/// 数据格式[包头2byte][消息类型1B][内容长度2byte][内容][校验位1byte][包尾2byte]
		/// 消息类型：默认为0，255为心跳包
		/// 数据长度: 数据长度与校验位之间的长度
		/// 校验位：，数据长度 ，网络事件ID长度，内容长度  字节以此异或
		/// </summary>
		public static byte[] Packet(byte[] data, int msgType = 0)
		{
			if ( data == null )
				data = new byte[0];
			int dataLength = data.Length;


			int packageLength = 2 + 1 + 2 + dataLength + 1 + 2;

			byte[] bytes = new byte[packageLength];

			byte[] dataLBytes = System.BitConverter.GetBytes((ushort)dataLength);
			System.Array.Reverse (dataLBytes); //翻转为大端字节序

			int idx = 0;

			bytes [idx++] = packetBegin [0];
			bytes [idx++] = packetBegin [1];
			bytes [idx++] = (byte)(msgType & 0x000000ff);//消息类型
			System.Array.Copy (dataLBytes,0,bytes,idx,dataLBytes.Length);
			idx += dataLBytes.Length;

			System.Array.Copy (data,0,bytes,idx,data.Length);
			idx += data.Length;

			bytes [idx++] = GetCheckCode (data);
			bytes [idx++] = packetEnd [0];
			bytes [idx++] = packetEnd [1];



			UnityEngine.Debug.Log ("packet size:" + bytes.Length);
			return bytes;



		}
		/// <summary>
		/// 异或各个字节
		/// </summary>
		/// <returns>The check code.</returns>
		/// <param name="bytes">Bytes.</param>
		static byte GetCheckCode(ICollection<byte> bytes)
		{
			byte cc = baseCheckCode;

			foreach(byte b in bytes)
			{
				cc = (byte)(cc ^ b);
			}
			return cc;
		}

	}
}
