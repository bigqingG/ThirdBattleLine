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
        // �����¿���
        GameObject newCard = Instantiate(cardPrefab, cardSpawnPoint.position,
                                       Quaternion.identity, cardLayout.transform);
        RectTransform cardRect = newCard.GetComponent<RectTransform>();

        // ���ó�ʼ״̬
        cardRect.localScale = Vector3.zero;
        cardRect.SetAsLastSibling(); // ȷ���¿��������

        handCards.Add(cardRect);

        // ���²��ֲ�Ӧ������
        UpdateHandLayout();
    }

    private void UpdateHandLayout()
    {
        // ���¼��㿨Ƭ���
        cardLayout.spacing = CalculateSpacing();

        // ǿ�Ƹ��²���
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardLayout.GetComponent<RectTransform>());

        // Ӧ������Ч��
        ApplyCurveToCards();
    }

    private float CalculateSpacing()
    {
        // ��������������̬������࣬ʹ���Ʊ�������Ļ��
        return handCards.Count < 5 ?
            40f :
            Mathf.Max(-cardWidth, 800f / handCards.Count - cardWidth);
    }

    private void ApplyCurveToCards()
    {
        if (handCards.Count == 0) return;

        // ���������ϵ��λ��
        for (int i = 0; i < handCards.Count; i++)
        {
            // ��һ��λ�� (0-1)
            float normalizedPos = ((float)i) / (handCards.Count - 1);

            // �����߼������ߵĸ߶�ƫ��
            float heightOffset = curveHeight * Mathf.Sin(normalizedPos * Mathf.PI);

            // ������ת�Ƕ�
            float angle = Mathf.Lerp(15f, -15f, normalizedPos);

            // ��ȡ��ǰλ��
            Vector3 currentPos = handCards[i].localPosition;

            // ������λ�� (����x���䣬ֻ�޸�y)
            Vector3 newPos = new Vector3(
                currentPos.x,
                currentPos.y + heightOffset,
                currentPos.z);

            // ʹ��DOTweenƽ���ƶ�����ת
            handCards[i].DOLocalMove(newPos, animationDuration).SetEase(Ease.OutQuad);
            handCards[i].DOLocalRotate(new Vector3(0, 0, angle), animationDuration);
        }
    }
}
