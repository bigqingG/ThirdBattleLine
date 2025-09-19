using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTest : MonoBehaviour
{
    // �����õ���ͼ�߽磨�������꣩
    [Header("��ͼ���귶Χ����")]
    [SerializeField] private float xMin = -400f;
    [SerializeField] private float xMax = 200f;
    [SerializeField] private float yMin = -60f;
    [SerializeField] private float yMax = 200f;

    private Vector3 dragOffset;
    private Camera mainCamera;
    private float mouseZCoord;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        mouseZCoord = mainCamera.WorldToScreenPoint(transform.position).z;
        dragOffset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        Vector3 newPos = GetMouseWorldPos() + dragOffset;
        newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
        newPos.y = Mathf.Clamp(newPos.y, yMin, yMax);
        transform.position = newPos;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // �༭�����ӻ�����������ʱ�Զ����أ�
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.3f);
        Vector3 center = new Vector3(
            (xMin + xMax) / 2,
            (yMin + yMax) / 2,
            transform.position.z
        );
        Vector3 size = new Vector3(xMax - xMin, yMax - yMin, 0.1f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
