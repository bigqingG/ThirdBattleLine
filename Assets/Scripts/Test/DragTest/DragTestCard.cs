using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class DragTestCard : MonoBehaviour, IDragHandler,IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    //��ȡgameobject���е�����
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
        //�����קUIʱ��UI��������ƶ�
        rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        centerPos= rectTransform.position;
        //����������ת��Ϊ��Ļ����
        centerPos2 = RectTransformUtility.WorldToScreenPoint(null, centerPos);
        //Debug.Log("centerPos2: " + centerPos2);
        
    }

    

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnDragEnd");
        dragTest.CheckInRange(centerPos2);
    }
}
