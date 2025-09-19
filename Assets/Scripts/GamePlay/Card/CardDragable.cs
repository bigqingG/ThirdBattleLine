using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragable : MonoBehaviour,IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("拖拽设置")]
    [SerializeField] private RectTransform playZone; // 出牌区域（Inspector赋值，或自动Find）
    [SerializeField] private float dragScale = 1f; // 拖拽时放大倍数
    [SerializeField] private float returnDuration = 0.3f; // 返回动画时长
    [SerializeField] private float destroyDelay = 2f; // 出牌后销毁延迟

    private RectTransform rectTransform;
    private Canvas canvas;
    private CardViewController cardView; // 代理访问boundCard和Cleanup
    private RuntimeCard boundCard; // 运行时卡牌数据
    private Vector2 originalPosition; // 拖拽前原位置（anchoredPosition）
    private bool isDragging = false;

    private void Awake()
    {
        //playZone = GameObject.Find("Canvas/PlayZone")?.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        cardView = GetComponent<CardViewController>(); // 获取视图控制器

        //if (playZone == null)
        //{
        //    playZone = GameObject.Find("PlayZone")?.GetComponent<RectTransform>();
        //    if (playZone == null) Debug.LogError("PlayZone not found! 请在Canvas下创建PlayZone GameObject。");
        //}
    }

    // 拖拽开始：检查权限，记录位置，放大反馈
    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardView == null || cardView.BoundCard == null) return; // 视图未初始化
        boundCard = cardView.BoundCard;
        if (boundCard.owner == null || !boundCard.owner.isHuman) return; // 只允许真人玩家拖拽

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.localScale = Vector3.one * dragScale;
        Debug.Log($"开始拖拽卡牌: {boundCard.sourceData.cardData.displayName}");
    }

    // 拖拽中：跟随鼠标（屏幕点转本地点）
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        rectTransform.anchoredPosition = localPoint;
    }

    // 拖拽结束：检测区域，处理出牌或返回
    public void OnEndDrag(PointerEventData eventData)
    {
        //if (!isDragging) return;
        //isDragging = false;
        ////rectTransform.localScale = Vector3.one; // 恢复大小

        //Vector2 endScreenPos = eventData.position;
        //bool inPlayZone = RectTransformUtility.RectangleContainsScreenPoint(playZone, endScreenPos, canvas.worldCamera);

        //if (inPlayZone)
        //{
        //    // 在出牌区：固定到松开位置，尝试出牌，两秒后销毁
        //    Vector2 finalLocalPos;
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //        playZone,
        //        endScreenPos,
        //        canvas.worldCamera,
        //        out finalLocalPos
        //    );
        //    rectTransform.SetParent(playZone, false); // 移到出牌区（worldPositionStays=false保持本地坐标）
        //    rectTransform.anchoredPosition = finalLocalPos;

        //    // 模拟出牌（调用PlayerController）
        //    if (boundCard.owner.TryPlayCard(boundCard))
        //    {
        //        Debug.Log($"出牌成功: {boundCard.sourceData.cardData.displayName}");
        //        StartCoroutine(DestroyAfterDelay(destroyDelay));
        //    }
        //    else
        //    {
        //        // 出牌失败，返回原位
        //        ReturnToOriginal();
        //    }
        //}
        //else
        //{
        //    // 不在出牌区：返回原位
        //    ReturnToOriginal();
        //}
    }

    // 返回原位置动画协程
    private void ReturnToOriginal()
    {
        // 先设置回原父级（手牌容器），否则anchoredPosition无效
        if (cardView != null && cardView.transform.parent != null)
        {
            rectTransform.SetParent(cardView.transform.parent, false);
        }
        StartCoroutine(AnimateReturn(originalPosition, returnDuration));
    }

    private IEnumerator AnimateReturn(Vector2 targetPos, float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
    }

    // 延迟销毁协程：触发卡牌销毁，清理视图，返回池
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (boundCard != null)
        {
            boundCard.DestroyCard(); // 触发亡语、事件清理
        }
        if (cardView != null)
        {
            cardView.Cleanup(); // 解绑事件
            CardViewManager.Instance.ReturnView(cardView); // 返回视图池
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cardView == null || cardView.BoundCard == null) return; // 视图未初始化
        boundCard = cardView.BoundCard;
        if (boundCard.owner == null || !boundCard.owner.isHuman) return; // 只允许真人玩家拖拽

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        //rectTransform.localScale = Vector3.one * dragScale;
        Debug.Log($"开始拖拽卡牌: {boundCard.sourceData.cardData.displayName}");
    }


    ///Gizmo绘制出牌区域
    private void OnDrawGizmos()
    {
        if (playZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(playZone.position, playZone.sizeDelta);
        }
    }
}
