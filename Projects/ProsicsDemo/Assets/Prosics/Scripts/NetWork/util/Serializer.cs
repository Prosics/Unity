// Date: 	    2017/8/11
// Author: 	    li kunlun <>
// Description:
using System;
using System.Collections;
using System.Collections.Generic;

namespace Prosics
{
	public class Serializer
	{
		public const int  ValueTypeID_Bytes = 0;
		public const int  ValueTypeID_Char = 1;
		public const int  ValueTypeID_Bool = 2;
		public const int  ValueTypeID_Int16 = 3;
		public const int  ValueTypeID_UInt16 = 4;
		public const int ValueTypeID_Int32 = 5;
		public const int  ValueTypeID_UInt32 = 6;
		public const int ValueTypeID_Int64 = 7;
		public const int  ValueTypeID_UInt64 = 8;
		public const int ValueTypeID_Float = 9;
		public const int  ValueTypeID_Double = 10;

		public const int  ValueTypeID_String = 20;
		public const int  ValueTypeID_Collection = 30;
		public const int  ValueTypeID_Dictionary = 40;

		//[类型1][长度2][value]
		public static byte[] Serialize(Object obj)
		{
			if ( obj == null )
				return null;
			if(obj is byte[])
				return SerializeField (obj);
			else if ( obj is ICollection)
				return SerializeCollection (obj as ICollection);
			else
				return SerializeField (obj);
		}
		public static byte[] SerializeField(Object obj)
		{
			int type = GetValueTypeId(obj);
			byte[] valuB = GetValueBytes (obj);
			List<byte> bytes = new List<byte> ();
			bytes.Add ((byte)(type & 0x000000ff));
			bytes.AddRange (System.BitConverter.GetBytes (valuB.Length));
			bytes.AddRange (valuB);
			return bytes.ToArray();
		}

		public static byte[] SerializeCollection(ICollection collection)
		{
			List<byte> bytes = new List<byte> ();
			int type = 0;
			List<byte> valueBytes = new List<byte> ();
			if ( collection is IDictionary )
			{
				type = ValueTypeID_Dictionary;
				IDictionary dic = collection as IDictionary;
				foreach (Object obj in dic.Keys)
				{
					if ( dic [obj] != null )
					{
						valueBytes.AddRange (Serialize (obj));
						valueBytes.AddRange (Serialize (dic [obj]));
					}
				}
			}
			else
			{
				type = ValueTypeID_Collection;
				foreach (Object obj in collection)
				{
					
					valueBytes.AddRange (Serialize(obj));
				}
			}
			bytes.Add ((byte)(type & 0x000000ff));
			bytes.AddRange (System.BitConverter.GetBytes (valueBytes.Count));
			bytes.AddRange (valueBytes);
			return bytes.ToArray ();
		}
		public static Object Deserilize(byte[] bytes , int idx, int length)
		{
			if ( bytes == null || length == 0)
				return null;
			if ( idx >= bytes.Length )
				return null;
			int type = bytes[idx];
			if(type == ValueTypeID_Collection)
				return DeserilizeCollection (bytes, idx, length);
			else if(type == ValueTypeID_Dictionary )
				return DeserilizeDictionary (bytes, idx, length);
			else
				return DeserilizeField (bytes, idx, length);
		}
		public static Dictionary<string, Object> DeserilizeDictionary(byte[] bytes , int idx, int length)
		{
			Dictionary<string, Object> dic = new Dictionary<string, object> ();
			List<Object> fields = DeserilizeCollection (bytes,idx,length);
			for (int i = 1; i < fields.Count; i += 2)
			{
				string key = fields [i - 1] as string;
				Object value = fields [i];
				dic.Add (key,value);
			}
			return dic;
		}
		public static List<Object> DeserilizeCollection(byte[] bytes , int idx, int length)
		{
			List<Object> list = new List<Object> ();  
			if ( bytes == null || length == 0)
				return list;

			//参数索引
			int pIdx = idx +5;
			//长度索引
			int lIdx = pIdx + 1;
			//值索引
			int vIdx = lIdx + 4;


			//获取值长度
			//无法获取值长度，取消
			if ( vIdx - idx > length )
				return list;
			

			int vL = System.BitConverter.ToInt32 (bytes,lIdx);
			if ( vIdx + vL - idx > length)
				return list;
		

			while (vIdx + vL - idx <= length)
			{
				list.Add (Deserilize (bytes, pIdx, vL + 5));



				pIdx = vIdx + vL;
				lIdx = pIdx + 1;
				vIdx = lIdx + 4;

				//获取值长度
				//无法获取值长度，取消
				if ( vIdx - idx > length )
					break;
				vL = System.BitConverter.ToInt32 (bytes,lIdx);

			}

			return list;
		}
		public static Object DeserilizeField(byte[] bytes , int idx, int length)
		{
			if ( bytes == null || length == 0)
				return null;

			//参数索引
			int pIdx = idx;
			//类型索引
			int tIdx = pIdx;
			//长度索引
			int lIdx = pIdx + 1;
			//值索引
			int vIdx = lIdx + 4;


			//获取值长度
			//无法获取值长度，取消
			if ( vIdx - idx > length )
				return null;


			int vL = System.BitConverter.ToInt32 (bytes,lIdx);
			if ( vIdx + vL - idx > length)
				return null;
			byte[] value = new byte[vL];
			System.Array.Copy (bytes,vIdx,value,0,vL);
			int type = (int)(bytes [tIdx] & 0x000000ff);
			return GetObject (value,type);
		}



		static int GetValueTypeId(Object obj)
		{
			if ( obj is byte[] )
				return ValueTypeID_Bytes;
			else if ( obj is char )
				return ValueTypeID_Char;
			else if ( obj is bool )
				return ValueTypeID_Bool;
			else if ( obj is Int16 )
				return ValueTypeID_Int16;
			else if ( obj is UInt16 )
				return ValueTypeID_UInt16;
			else if ( obj is Int32 )
				return ValueTypeID_Int32;
			else if ( obj is UInt32 )
				return ValueTypeID_UInt32;
			else if ( obj is Int64 )
				return ValueTypeID_Int64;
			else if ( obj is UInt64 )
				return ValueTypeID_UInt64;
			else if ( obj is float )
				return ValueTypeID_Float;
			else if ( obj is double )
				return ValueTypeID_Double;
			else if(obj is string)
				return ValueTypeID_String;
			else
				return 255;
		}

		static byte[] GetValueBytes(Object obj)
		{
			if ( obj is byte[] )
				return obj as byte[];
			else if ( obj is char )
				return System.BitConverter.GetBytes ((char)obj);
			else if ( obj is bool )
				return System.BitConverter.GetBytes ((bool)obj);
			else if ( obj is Int16 )
				return System.BitConverter.GetBytes ((Int16)obj);
			else if ( obj is UInt16 )
				return System.BitConverter.GetBytes ((UInt16)obj);
			else if ( obj is Int32 )
				return System.BitConverter.GetBytes ((Int32)obj);
			else if ( obj is UInt32 )
				return System.BitConverter.GetBytes ((UInt32)obj);
			else if ( obj is Int64 )
				return System.BitConverter.GetBytes ((Int64)obj);
			else if ( obj is UInt64 )
				return System.BitConverter.GetBytes ((UInt64)obj);
			else if ( obj is float )
				return System.BitConverter.GetBytes ((float)obj);
			else if ( obj is double )
				return System.BitConverter.GetBytes ((double)obj);
			else if(obj is string)
				return System.Text.Encoding.UTF8.GetBytes(obj as string);
			else
				return null;
		}
		public static Object GetObject(byte[] bytes, int type)
		{
			if ( bytes == null || bytes.Length == 0 )
				return null;

			switch (type)
			{
			case ValueTypeID_Bytes:
				return bytes;

			case ValueTypeID_Char:
				return System.BitConverter.ToChar (bytes,0);

			case ValueTypeID_Bool:
				return System.BitConverter.ToBoolean (bytes,0);
			case ValueTypeID_Int16:
				return System.BitConverter.ToInt16 (bytes,0);
			case ValueTypeID_UInt16:
				return System.BitConverter.ToUInt16 (bytes,0);
			case ValueTypeID_Int32:
				return System.BitConverter.ToInt32 (bytes,0);
			case ValueTypeID_UInt32:
				return System.BitConverter.ToUInt32 (bytes,0);
			case ValueTypeID_Int64:
				return System.BitConverter.ToInt64 (bytes,0);
			case ValueTypeID_UInt64:
				return System.BitConverter.ToUInt64 (bytes,0);
			case ValueTypeID_Float:
				return System.BitConverter.ToSingle (bytes,0);
			case ValueTypeID_Double:
				return System.BitConverter.ToDouble (bytes,0);
			case ValueTypeID_String:
				return System.Text.Encoding.UTF8.GetString(bytes);
			default:
				return null;
			}
		}





	}
}

