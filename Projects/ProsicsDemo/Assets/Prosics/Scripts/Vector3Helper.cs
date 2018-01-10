//
// Vector3Helper.cs
//
// Author:
//       Prosics <Prosics@163.com>
//
// Copyright (c) 2017 Prosics
//
//
using UnityEngine;

namespace Prosics
{
	public class Vector3Helper
	{
		
		/// <summary>
		/// 从from到to逆时针旋转的角度[0-360)
		/// </summary>
		/// <returns>The 360.</returns>
		/// <param name="from">起始向量.</param>
		/// <param name="to">终止向量</param>
		/// <param name="forward">观察方向 默认为z轴方向</param>
		public static float Angle_360(Vector3 from, Vector3 to ,Vector3 forward)
		{
			Vector3 v3 = Vector3.Cross (from,to).normalized;
			if ( Vector3.Dot (v3, forward) >= 0 )
			{
				return Vector3.Angle (from, to);
			}
			else
			{
				return 360 - Vector3.Angle (from, to);
			}
		}
		public static float Angle_360(Vector3 from, Vector3 to)
		{
			return Angle_360 (from,to,Vector3.forward);
		}

		/// <summary>
		/// 从from到to的角度(-180 至 180],逆时针为正，顺时针为负
		/// 
		/// </summary>
		/// <returns>The 180.</returns>
		/// <param name="from">起始向量.</param>
		/// <param name="to">终止向量</param>
		/// <param name="forward">观察方向 默认为z轴方向</param>
		public static float Angle_180(Vector3 from, Vector3 to, Vector3 forward)
		{
			Vector3 v3 = Vector3.Cross (from,to).normalized;
			if ( Vector3.Dot (v3, forward) >= 0 )
			{
				return Vector3.Angle (from, to);
			}
			else
			{
				return 0 - Vector3.Angle (from, to);
			}

		}
		public static float Angle_180(Vector3 from, Vector3 to)
		{
			return Angle_180 (from,to,Vector3.forward);
		}
	}
}

