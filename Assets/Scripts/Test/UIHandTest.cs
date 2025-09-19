using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandTest : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHandSize = 10;
    [SerializeField] private float curveHeight = 100f;
    [SerializeField] private float animationDuration = 0.25f;
    [Header("References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform cardSpawnPoint;
    [SerializeField] private HorizontalLayoutGroup cardLayout;
    private List<RectTransform> handCards = new List<RectTransform>();
    private float cardWidth;

    private void Start()
    {
        cardLayout.spacing = CalculateSpacing();
        cardWidth = cardPrefab.GetComponent<RectTransform>().rect.width;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Draw();
        }
    }

    public void Draw()
    {
        if (handCards.Count >= maxHandSize) return;
        // 创建新卡牌
        GameObject newCard = Instantiate(cardPrefab, cardSpawnPoint.position,
                                       Quaternion.identity, cardLayout.transform);
        RectTransform cardRect = newCard.GetComponent<RectTransform>();

        // 设置初始状态
        cardRect.localScale = Vector3.zero;
        cardRect.SetAsLastSibling(); // 确保新卡牌在最后

        handCards.Add(cardRect);

        // 更新布局并应用曲线
        UpdateHandLayout();
    }

    private void UpdateHandLayout()
    {
        // 重新计算卡片间距
        cardLayout.spacing = CalculateSpacing();

        // 强制更新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardLayout.GetComponent<RectTransform>());

        // 应用曲线效果
        ApplyCurveToCards();
    }

    private float CalculateSpacing()
    {
        // 根据手牌数量动态调整间距，使卡牌保持在屏幕内
        return handCards.Count < 5 ?
            40f :
            Mathf.Max(-cardWidth, 800f / handCards.Count - cardWidth);
    }

    private void ApplyCurveToCards()
    {
        if (handCards.Count == 0) return;

        // 计算曲线上点的位置
        for (int i = 0; i < handCards.Count; i++)
        {
            // 归一化位置 (0-1)
            float normalizedPos = ((float)i) / (handCards.Count - 1);

            // 抛物线计算曲线的高度偏移
            float heightOffset = curveHeight * Mathf.Sin(normalizedPos * Mathf.PI);

            // 计算旋转角度
            float angle = Mathf.Lerp(15f, -15f, normalizedPos);

            // 获取当前位置
            Vector3 currentPos = handCards[i].localPosition;

            // 创建新位置 (保持x不变，只修改y)
            Vector3 newPos = new Vector3(
                currentPos.x,
                currentPos.y + heightOffset,
                currentPos.z);

            // 使用DOTween平滑移动和旋转
            handCards[i].DOLocalMove(newPos, animationDuration).SetEase(Ease.OutQuad);
            handCards[i].DOLocalRotate(new Vector3(0, 0, angle), animationDuration);
        }
    }
}
