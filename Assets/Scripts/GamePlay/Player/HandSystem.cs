using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using DG.Tweening; // DOTween������
using TMPro;
using System.Collections;




public class HandSystem : MonoBehaviour
{
    #region �ܴ���
    [Header("���ֲ���")]
    [SerializeField] private int maxHandSize = 10; // �����������������������¿�
    [SerializeField] private SplineContainer splineContainer; // Spline·�����������ڶ��������������ߣ���Inspector�����룬������Bezier���ߴ�����������
    [SerializeField] private RectTransform handContainer; // ���Ƹ�������Canvas�µ�RectTransform�������п���ͼ��parent
    [SerializeField] private RectTransform spawnPoint; // �������ɵ㣨RectTransform�����¿������ﻬ�����ƣ�����������������⣩

    [Header("��������")]
    [SerializeField] private float cardSpacing=0.08f; // ���Ƽ�������0-1�������������ܶȣ�Сֵ=���գ���ֵ=չ����Ĭ��0.08�ʺ�10���ƣ�
    [SerializeField] private float animationDuration = 0.3f; // ��������ʱ�䣨�룩�����ƻ���/�����ٶ�
    [SerializeField] private Ease animationEase = Ease.OutQuad; // �����������ߣ�OutQuad=ƽ������
                                                                //
                                                                // ����ǿ�ָ�
    [SerializeField] private float overlapOffset = -20f; // �����ص�Yƫ�ƣ����أ���������ʱ�ϲ㿨��΢̧�ߣ�������ȫ�ص�����ֵ=���³���

    private readonly List<RuntimeCard> cards = new List<RuntimeCard>(); // ���������б�˳��������֣������ң�
    private readonly Dictionary<RuntimeCard, CardViewController> cardViews = new(); // ����-��ͼӳ�䣬���ڿ��ٷ���

    public IReadOnlyList<RuntimeCard> Cards => cards.AsReadOnly();
    public int MaxHandSize => maxHandSize;

    // ��ӿ��ƣ��¿���spawnPoint���룬���뵽����ĩβ���Ҳࣩ��Ȼ����������
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

            // �¿���spawnPoint����Ŀ��λ�ã��޸��ͺ����⣩
            cardView.transform.position = spawnPoint.position; // �����������
            Vector3 targetPos = GetTargetPosition(cards.Count - 1); // ��ȡĩβλ��
            AnimateCardToPosition(cardView, targetPos); // ��������

            // �ӳ���������������ֹ������ͻ��
            DOVirtual.DelayedCall(animationDuration * 0.5f, () => UpdateHandLayout(true));
        }
    }
    // �Ƴ����ƣ��Ƴ�����������ʣ�࿨����λƽ�����
    public void RemoveCard(RuntimeCard card)
    {
        if (!cards.Remove(card)) return; // �������Ƴ�

        if (cardViews.TryGetValue(card, out var cardView))
        {
            // �����Ƴ���������ѡ���ɳ���Ĺ�أ���Ȼ�󷵻س�
            AnimateCardExit(cardView); // �Զ����˳�����
            CardViewManager.Instance.ReturnView(cardView); // ע�⣺ReturnView�贫RuntimeCard
            cardViews.Remove(card);
        }

        UpdateHandLayout(true); // ����ʣ�࿨
    }
    // ������Spline�ϵ�Ŀ��λ�ã���index����Yƫ�ƣ������ú����߼�
    private Vector3 GetTargetPosition(int index)
    {
        if (splineContainer == null || cards.Count == 0) return Vector3.zero;
        Spline spline = splineContainer.Spline;
        float totalSpacing = (cards.Count - 1) * cardSpacing;
        float startPos = 0.5f - totalSpacing / 2f;
        float param = startPos + index * cardSpacing;
        return GetSplinePosition(spline, param, index); // ֱ�ӵ��ú���λ�ü���
    }

    // �������Ʋ��֣����ķ��������ݵ�ǰcards˳�򣬼���Splineλ�ò������ƶ�
    public void UpdateHandLayout(bool animate = false)
    {
        if (splineContainer == null || cards.Count == 0) return;
        UpdateCardSpacing(cards.Count); // ���¿��Ƽ��
        Spline spline = splineContainer.Spline;
        float totalSpacing = (cards.Count - 1) * cardSpacing; // �ܼ��
        float startPos = 0.5f - totalSpacing / 2f; // ��ʼ������0-1����ȷ���������

        for (int i = 0; i < cards.Count; i++)
        {
            float param = startPos + i * cardSpacing; // ��ǰ���Ĳ���ֵ��0-1��
            Vector3 targetPos = GetSplinePosition(spline, param, i); // ��ȡλ��+ƫ��

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
                    view.MoveToPosition(targetPos); // �޶�����ֱ������
                }
            }
        }
    }
    //���¿��Ƽ��
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
    // ��ȡSpline�ϵ�Ŀ��λ�ã���Yƫ�ƣ������ص���
    private Vector3 GetSplinePosition(Spline spline, float param, int index)
    {
        Vector3 pos = spline.EvaluatePosition(param); // ����λ��
        Vector3 up = spline.EvaluateUpVector(param); // ������������Yƫ�ƣ�
        pos += up * (index * overlapOffset); // �������ۼ�Yƫ�ƣ��ϲ㿨̧��
        return pos;
    }

    private Quaternion GetTargetRotation(Spline spline, float param)
    {
        if (spline == null) return Quaternion.identity;

        //Vector3 tangent= spline.EvaluateTangent(param); // ���߷���
        Vector3 up = spline.EvaluateUpVector(param); // ������
        Vector3 forward = spline.EvaluateTangent(param);

        Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
        return rotation;

    }

    // �����ƶ����ſ���Ŀ��λ�ã�ʹ��DOTween��
    private void AnimateCardToPosition(CardViewController view, Vector3 targetPos)
    {
        view.gameObject.transform.DOLocalMove(targetPos, animationDuration).SetEase(animationEase);
        
    }

    // �����Ƴ�����ʾ��������ɳ���������
    private void AnimateCardExit(CardViewController view)
    {
        Vector3 exitPos = handContainer.position + Vector3.left * 200f; // �ɳ����
        view.gameObject.transform.DOLocalMove(exitPos, animationDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => view.gameObject.SetActive(false)); // ��ɺ�����
    }

    // ��ȡ���ſ���ͼ�����ⲿ���¼�ϵͳ���ã�
    public CardViewController GetCardView(RuntimeCard card)
    {
        return cardViews.TryGetValue(card, out var view) ? view : null;
    }

    // ������ƣ���Ϸ�����ã������𶯻���
    public void ClearHand()
    {
        foreach (var card in cards.ToList())
        {
            RemoveCard(card);
        }
        cards.Clear();
        cardViews.Clear();
    }

    // ================= ��չ�����������������ã� =================

    // ϴ�ƶ������������λ�ã�ģ��ϴ��Ч��
    public void ShuffleHand()
    {
        // �������cards˳�򣨲��ı����ݣ�ֻ���֣�
        var shuffled = cards.OrderBy(x => Random.Range(0f, 1f)).ToList();
        cards.Clear();
        cards.AddRange(shuffled);

        // ��������
        UpdateHandLayout(true);
        Debug.Log("������ϴ��");
    }

    // չ�����������ƴӽ��յ����Σ��غϿ�ʼʱ���ã�
    public void FanOut()
    {
        // ��ʱ��С��ൽ0��Ȼ��ָ�
        float originalSpacing = cardSpacing;
        cardSpacing = 0f;
        UpdateHandLayout(false); // ˲�����
        cardSpacing = originalSpacing;
        DOVirtual.DelayedCall(0.1f, () => UpdateHandLayout(true)); // �ӳ�չ��
    }

    // ���𶯻������ε����գ��غϽ���ʱ���ã�
    public void FanIn()
    {
        float originalSpacing = cardSpacing;
        DOTween.To(() => cardSpacing, x => cardSpacing = x, 0f, animationDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                cardSpacing = originalSpacing; // �ָ�
                UpdateHandLayout(true);
            });
    }

    // �������ſ�������+���⣨���������ͣ��ѡ��
    public void HighlightCard(RuntimeCard card, bool highlight = true)
    {
        if (cardViews.TryGetValue(card, out var view))
        {
            Vector3 targetScale = highlight ? Vector3.one * 1.1f : Vector3.one;
            view.gameObject.transform.DOScale(targetScale, 0.2f).SetEase(Ease.OutBack);
            // ����չ�����CanvasGroup���뷢��Ч��
        }
    }

    // ����������e.g., �����ض����Ϳ���
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


