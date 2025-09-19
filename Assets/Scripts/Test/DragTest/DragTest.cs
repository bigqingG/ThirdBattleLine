using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTest : MonoBehaviour
{
    // 可配置的视图边界（世界坐标）
    [Header("视图坐标范围限制")]
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


    public void CheckInRange(Vector2 cardPos)
    {
        //检查卡牌是否在视图范围内
        if (cardPos.x < xMin || cardPos.x > xMax || cardPos.y < yMin || cardPos.y > yMax)
        {
            //卡牌不在出牌区域，禁止出牌
            Debug.Log("卡牌不在出牌区域，禁止出牌");
            return;
        
        }
        else
        {
            //卡牌在出牌区域，允许出牌
            Debug.Log("卡牌在出牌区域，允许出牌");
            return;
        }
    }

    // 编辑器可视化辅助（运行时自动隐藏）
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
