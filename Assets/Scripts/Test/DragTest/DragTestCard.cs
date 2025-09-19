using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class DragTestCard : MonoBehaviour, IDragHandler,IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    //获取gameobject的中点坐标
    private Vector3 centerPos;
    private Vector2 centerPos2;
    [SerializeField] DragTest dragTest;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        centerPos = rectTransform.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        //鼠标拖拽UI时，UI跟随鼠标移动
        rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        centerPos= rectTransform.position;
        //将中心坐标转换为屏幕坐标
        centerPos2 = RectTransformUtility.WorldToScreenPoint(null, centerPos);
        //Debug.Log("centerPos2: " + centerPos2);
        
    }

    

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnDragEnd");
        dragTest.CheckInRange(centerPos2);
    }
}
