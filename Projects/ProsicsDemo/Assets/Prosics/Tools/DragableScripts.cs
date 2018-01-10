using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Prosics;
using Prosics.Util;

public class DragableScripts : MonoScriptBase , IPointerDownHandler,IPointerUpHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
	private Vector3 _oriLocalPos = Vector3.zero;
	private bool _isDrag = false;
	private Vector2 _offset = Vector2.zero;
	protected override void OnEnable ()
	{
		base.OnEnable ();
		_oriLocalPos = transform.localPosition;
		if (transform.parent as RectTransform == null)
		{
			Debug.LogError (gameObject.name + ">DragableScripts need a parent with RectTransform!");
			enabled = false;
		}
	}
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		//if(eventData.pointerDrag == gameObject)
			_isDrag = true;
		Vector2 pos;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (transform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out pos))
		{
			_offset = new Vector2(pos.x - transform.localPosition.x, pos.y - transform.localPosition.y);
		}
	}

	public virtual void OnDrag(PointerEventData data)
	{
		if(_isDrag)
		{
			Vector2 pos;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle (transform.parent as RectTransform, data.position, data.pressEventCamera, out pos))
			{
				transform.localPosition = pos - _offset;
			}

		}
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		_isDrag = false;
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
	}
	public virtual void OnPointerClick(PointerEventData eventData)
	{

	}

	public virtual  void OnPointerUp (PointerEventData eventData)
	{
	}
	public virtual void OnPointerEnter(PointerEventData eventData)
	{	
	} 

	public void ResetPosition()
	{
		_isDrag = false;
		transform.localPosition = _oriLocalPos; 
	}
}
