using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using DG.Tweening;
using System.Collections.Generic;
using System;

public class ItemDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler//, IDropHandler
{
    /// <summary>
    /// Scroll View上的Scroll Rect组件
    /// </summary>
    private ScrollRect _scrollRect;
    /// <summary>
    /// 拖拽的是否是子物体
    /// </summary>
    private bool _isDragItem;
    public Action MoveRight;
    public Action MoveLeft;

    void Awake()
    {
        Input.multiTouchEnabled = false;    //限制多指拖拽
        //注意面板中默认创建的ScrollView中间有空格
        _scrollRect = transform.parent.parent.parent.GetComponent<ScrollRect>();
        _isDragItem = false;
    }


    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 touchDeltaPosition = Vector2.zero;
#if UNITY_EDITOR
        float delta_x = Input.GetAxis("Mouse X");
        float delta_y = Input.GetAxis("Mouse Y");
        touchDeltaPosition = new Vector2(delta_x, delta_y);

#elif UNITY_ANDROID || UNITY_IPHONE
        touchDeltaPosition = Input.GetTouch(0).deltaPosition;  
#endif
        //通过touchDeltaPosition去判断你的手指（鼠标）的移动方向，是和Scroll View同方向还是拖拽的方向
        if (Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
        {
            //在这里区分是拖拽Item还是ScrollRect
            _isDragItem = true;
            if (touchDeltaPosition.x > 0)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
        else
        {
            _isDragItem = false;
            if (_scrollRect != null)
                //调用Scroll的OnBeginDrag方法，有了区分，就不会被item的拖拽事件屏蔽
                _scrollRect.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragItem)
        {

            if (_scrollRect != null)
                _scrollRect.OnDrag(eventData);
        }
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {

        if (_isDragItem)
        {
            //处理item拖拽结束的逻辑处理
        }
        else
        {
            //Scroll Rect 拖拽结束
            if (_scrollRect != null)
                _scrollRect.OnEndDrag(eventData);
        }

    }
}