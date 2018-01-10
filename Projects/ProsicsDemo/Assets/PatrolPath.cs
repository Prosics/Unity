using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prosics;



public class PatrolPath : MonoScriptBase
{
	internal  enum MoveType
	{
		Directly,
		SmoothByVector
	}
	internal  struct PatrolPoint
	{
		public Vector3 position;
		public Vector3 direction;
		public MoveType moveType;
	}
	List<PatrolPoint> _patrolPoints = new List<PatrolPoint>();
	public Transform[] targets = new Transform[0];
	public float _speed = 1f;
	public float _rotateSpeed = 2f;
	public int _nextPosIdx = 1;

	public delegate void testDelegate();
	public testDelegate doDelegate;

	protected override void OnEnable()
	{
		base.OnEnable ();
		foreach (Transform trs in targets)
		{
			PatrolPoint pp = new PatrolPoint ();
			pp.position = trs.position;
			pp.direction = new Vector3(trs.forward.x,0,trs.forward.z).normalized;
			pp.moveType = MoveType.SmoothByVector;
			_patrolPoints.Add (pp);
		}
		if ( _patrolPoints.Count > 0 )
		{
			transform.position = _patrolPoints [0].position;
			transform.forward = _patrolPoints [0].direction;
			_nextPosIdx = 1;

		}


	}
	protected override void Update()
	{
		base.Update ();
		if ( MoveTo (_nextPosIdx) )
		{
			_nextPosIdx += 1;
			if ( _nextPosIdx >= _patrolPoints.Count )
				_nextPosIdx = 0;
		}

	}
	bool MoveTo( int pointIdx)
	{
		
		if ( pointIdx >= _patrolPoints.Count )
			return true;
		PatrolPoint point = _patrolPoints[pointIdx];
		Vector3 nowPos = transform.position;
		Vector3 nowDir = new Vector3(transform.forward.x,0,transform.forward.z).normalized;
		Vector3 direction = ((point.position - nowPos).normalized).normalized;

		transform.forward = Vector3.RotateTowards (nowDir, direction, _rotateSpeed * Time.deltaTime, 0.0f);
		transform.position +=  _speed* Time.deltaTime * transform.forward;
		//if ( Vector3Helper.Angle_360 (nowPos - transform.position, point.position - transform.position) >= 90 )
		if(Vector3.Distance(transform.position,point.position) <= _speed * 0.1f)
			return true;
		else
			return false;


		/*float angle = Vector3Helper.Angle_360 (nowDir,point.direction);
		float dAngle = 0;
		if ( angle <= 180 )
		{
			dAngle -= _rotateSpeed * Time.deltaTime;
			if ( dAngle + angle < 0 )
				dAngle = -angle;
		}
		else
		{
			dAngle += _rotateSpeed * Time.deltaTime;
			if ( dAngle + angle > 360 )
				dAngle = 360 - angle;
		}
		Vector3.RotateTowards*/
	
	}
}
