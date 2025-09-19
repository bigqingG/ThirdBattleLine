using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using DG.Tweening; // DOTween动画库
using TMPro;
using System.Collections;




public class HandSystem : MonoBehaviour
{
    #region 总代码
    [Header("布局参数")]
    [SerializeField] private int maxHandSize = 10; // 最大手牌数量，超出不添加新卡
    [SerializeField] private SplineContainer splineContainer; // Spline路径容器，用于定义手牌扇形曲线（在Inspector中拖入，建议用Bezier曲线从左到右弯曲）
    [SerializeField] private RectTransform handContainer; // 手牌父容器（Canvas下的RectTransform），所有卡视图的parent
    [SerializeField] private RectTransform spawnPoint; // 抽牌生成点（RectTransform），新卡从这里滑入手牌（建议置于手牌左侧外）

    [Header("动画参数")]
    [SerializeField] private float cardSpacing=0.08f; // 卡牌间距比例（0-1），控制扇形密度，小值=紧凑，大值=展开（默认0.08适合10张牌）
    [SerializeField] private float animationDuration = 0.3f; // 动画持续时间（秒），控制滑入/重排速度
    [SerializeField] private Ease animationEase = Ease.OutQuad; // 动画缓动曲线，OutQuad=平滑减速
                                                                //
                                                                // ，增强手感
    [SerializeField] private float overlapOffset = -20f; // 卡牌重叠Y偏移（像素），多张牌时上层卡略微抬高，避免完全重叠（负值=向下沉）

    private readonly List<RuntimeCard> cards = new List<RuntimeCard>(); // 手牌数据列表（顺序决定布局，从左到右）
    private readonly Dictionary<RuntimeCard, CardViewController> cardViews = new(); // 数据-视图映射，便于快速访问

    public IReadOnlyList<RuntimeCard> Cards => cards.AsReadOnly();
    public int MaxHandSize => maxHandSize;

    // 添加卡牌：新卡从spawnPoint滑入，插入到手牌末尾（右侧），然后整体重排
    public void AddCard(RuntimeCard card)
    {
        if (cards.Contains(card) || cards.Count >= maxHandSize) return;

        cards.Add(card);

        var cardView = CardViewManager.Instance.GetView(card);
        if (cardView != null)
        {
            cardView.transform.SetParent(handContainer);
            cardView.SetVisible(true);
            cardViews[card] = cardView;

            // 新卡从spawnPoint滑入目标位置（修复滞后问题）
            cardView.transform.position = spawnPoint.position; // 立即置于起点
            Vector3 targetPos = GetTargetPosition(cards.Count - 1); // 获取末尾位置
            AnimateCardToPosition(cardView, targetPos); // 启动动画

            // 延迟重排其他卡（防止动画冲突）
            DOVirtual.DelayedCall(animationDuration * 0.5f, () => UpdateHandLayout(true));
        }
    }
    // 移除卡牌：移除后立即重排剩余卡，空位平滑填充
    public void RemoveCard(RuntimeCard card)
    {
        if (!cards.Remove(card)) return; // 从数据移除

        if (cardViews.TryGetValue(card, out var cardView))
        {
            // 播放移除动画（可选：飞出到墓地），然后返回池
            AnimateCardExit(cardView); // 自定义退出动画
            CardViewManager.Instance.ReturnView(cardView); // 注意：ReturnView需传RuntimeCard
            cardViews.Remove(card);
        }

        UpdateHandLayout(true); // 重排剩余卡
    }
    // 它计算Spline上的目标位置（带index用于Y偏移），复用核心逻辑
    private Vector3 GetTargetPosition(int index)
    {
        if (splineContainer == null || cards.Count == 0) return Vector3.zero;
        Spline spline = splineContainer.Spline;
        float totalSpacing = (cards.Count - 1) * cardSpacing;
        float startPos = 0.5f - totalSpacing / 2f;
        float param = startPos + index * cardSpacing;
        return GetSplinePosition(spline, param, index); // 直接调用核心位置计算
    }

    // 更新手牌布局：核心方法，根据当前cards顺序，计算Spline位置并动画移动
    public void UpdateHandLayout(bool animate = false)
    {
        if (splineContainer == null || cards.Count == 0) return;
        UpdateCardSpacing(cards.Count); // 更新卡牌间距
        Spline spline = splineContainer.Spline;
        float totalSpacing = (cards.Count - 1) * cardSpacing; // 总间距
        float startPos = 0.5f - totalSpacing / 2f; // 起始参数（0-1），确保整体居中

        for (int i = 0; i < cards.Count; i++)
        {
            float param = startPos + i * cardSpacing; // 当前卡的参数值（0-1）
            Vector3 targetPos = GetSplinePosition(spline, param, i); // 获取位置+偏移

            if (cardViews.TryGetValue(cards[i], out var view))
            {
                if (animate)
                {
                    AnimateCardToPosition(view, targetPos);
                    view.gameObject.transform.DORotateQuaternion(GetTargetRotation(spline, param), animationDuration);
                    //view.gameObject.transform.localScale = Vector3.one;
                }
                else
                {
                    view.MoveToPosition(targetPos); // 无动画，直接设置
                }
            }
        }
    }
    //更新卡牌间距
    public void UpdateCardSpacing(float cardCount)
    {
        if (cardCount <= 2)
        {
            cardSpacing = 0.5f;
        }

        if (cardCount>=3&& cardCount <= 4)
        {
            cardSpacing = 0.3f;
        }
        if (cardCount == 5)
        {
            cardSpacing = 0.2f;
        }
        if (cardCount >= 6&&cardCount <=7)
        {
            cardSpacing = 0.15f;
        }
        if (cardCount >= 8 && cardCount <= 9)
        {
            cardSpacing = 0.12f;
        }
        if (cardCount == maxHandSize)
        {
            //Debug.Log("1111111");
            cardSpacing = 0.1f;
        }
    }
    // 获取Spline上的目标位置（带Y偏移，避免重叠）
    private Vector3 GetSplinePosition(Spline spline, float param, int index)
    {
        Vector3 pos = spline.EvaluatePosition(param); // 基础位置
        Vector3 up = spline.EvaluateUpVector(param); // 上向量（用于Y偏移）
        pos += up * (index * overlapOffset); // 按索引累加Y偏移，上层卡抬高
        return pos;
    }

    private Quaternion GetTargetRotation(Spline spline, float param)
    {
        if (spline == null) return Quaternion.identity;

        //Vector3 tangent= spline.EvaluateTangent(param); // 切线方向
        Vector3 up = spline.EvaluateUpVector(param); // 上向量
        Vector3 forward = spline.EvaluateTangent(param);

        Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
        return rotation;

    }

    // 动画移动单张卡到目标位置（使用DOTween）
    private void AnimateCardToPosition(CardViewController view, Vector3 targetPos)
    {
        view.gameObject.transform.DOLocalMove(targetPos, animationDuration).SetEase(animationEase);
        
    }

    // 动画移除卡（示例：向左飞出，淡出）
    private void AnimateCardExit(CardViewController view)
    {
        Vector3 exitPos = handContainer.position + Vector3.left * 200f; // 飞出左侧
        view.gameObject.transform.DOLocalMove(exitPos, animationDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => view.gameObject.SetActive(false)); // 完成后隐藏
    }

    // 获取单张卡视图（供外部如事件系统调用）
    public CardViewController GetCardView(RuntimeCard card)
    {
        return cardViews.TryGetValue(card, out var view) ? view : null;
    }

    // 清空手牌（游戏结束用，带收起动画）
    public void ClearHand()
    {
        foreach (var card in cards.ToList())
        {
            RemoveCard(card);
        }
        cards.Clear();
        cardViews.Clear();
    }

    // ================= 扩展动画方法（供外界调用） =================

    // 洗牌动画：随机重排位置，模拟洗牌效果
    public void ShuffleHand()
    {
        // 随机打乱cards顺序（不改变数据，只布局）
        var shuffled = cards.OrderBy(x => Random.Range(0f, 1f)).ToList();
        cards.Clear();
        cards.AddRange(shuffled);

        // 动画重排
        UpdateHandLayout(true);
        Debug.Log("手牌已洗牌");
    }

    // 展开动画：手牌从紧凑到扇形（回合开始时调用）
    public void FanOut()
    {
        // 临时缩小间距到0，然后恢复
        float originalSpacing = cardSpacing;
        cardSpacing = 0f;
        UpdateHandLayout(false); // 瞬间紧凑
        cardSpacing = originalSpacing;
        DOVirtual.DelayedCall(0.1f, () => UpdateHandLayout(true)); // 延迟展开
    }

    // 收起动画：扇形到紧凑（回合结束时调用）
    public void FanIn()
    {
        float originalSpacing = cardSpacing;
        DOTween.To(() => cardSpacing, x => cardSpacing = x, 0f, animationDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                cardSpacing = originalSpacing; // 恢复
                UpdateHandLayout(true);
            });
    }

    // 高亮单张卡：缩放+发光（用于鼠标悬停或选择）
    public void HighlightCard(RuntimeCard card, bool highlight = true)
    {
        if (cardViews.TryGetValue(card, out var view))
        {
            Vector3 targetScale = highlight ? Vector3.one * 1.1f : Vector3.one;
            view.gameObject.transform.DOScale(targetScale, 0.2f).SetEase(Ease.OutBack);
            // 可扩展：添加CanvasGroup淡入发光效果
        }
    }

    // 批量高亮（e.g., 过滤特定类型卡）
    public void HighlightCardsByType(CardType type, bool highlight = true)
    {
        foreach (var card in cards.Where(c => c.sourceData.cardData.cardType == type))
        {
            HighlightCard(card, highlight);
        }
    }

    //=======================HasCard===================
    public bool HasCard(RuntimeCard card) => cards.Contains(card);
    #endregion


}


