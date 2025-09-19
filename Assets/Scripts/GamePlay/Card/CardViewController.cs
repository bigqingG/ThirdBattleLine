using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
//using UnityEngine.U2D;
using UnityEngine.UI;

public class CardViewController : MonoBehaviour
{
    [Header("UI���")]
    [SerializeField] private Image cardArt;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text manaCost;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;

    private RuntimeCard boundCard;
    public RuntimeCard BoundCard => boundCard;
    private RectTransform rectTransform;

    public RectTransform RectTransform => rectTransform;

    private bool isInHand;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //��ʼ��
    public void Initialize(RuntimeCard card)
    {



        boundCard = card;

        // �󶨻������ݣ����࿨��ͨ�ã�
        cardArt.sprite = card.sourceData.cardArt;
        cardName.text = card.sourceData.cardData.displayName;
        descriptionText.text = card.sourceData.cardData.description;
        // ���������жϣ��ؼ��޸ĵ㣩
        CardType cardType = card.sourceData.cardData.cardType;

        // 1. Ӣ�������޴��� (�޹����ͻ���)
        if (cardType == CardType.Hero)
        {

        }
        // 2. ����/а������ (�޹���)
        else if (cardType == CardType.Spell || cardType == CardType.Hex)
        {
            manaCost.text = card.sourceData.cardData.manaCost.ToString();

        }
        // 3. ����/ʹͽ���� (��������)
        else if (cardType == CardType.Support || cardType == CardType.Apostle)
        {
            Debug.Log($"��ǰ������ {card.currentAttack}����ǰ����ֵ {card.currentHealth}");
            manaCost.text = card.sourceData.cardData.manaCost.ToString();
            attackText.text = card.currentAttack.ToString();
            healthText.text = card.currentHealth.ToString();


        }
        // �󶨶����¼�
        switch (cardType)
        {
            case CardType.Support:
            case CardType.Apostle:
            case CardType.Hero: // Ӣ�ۼ���Ѫ���仯
                card.OnHealthChanged += UpdateHealthUI;
                card.OnCardDestroyed += HandleCardDestroyed;
                break;
        }

        if (cardType == CardType.Support || cardType == CardType.Apostle)
        {
            card.OnAttackChanged += UpdateAttackUI;
        }

    }


    public void Cleanup()
    {
        if (boundCard != null)
        {
            boundCard.OnHealthChanged -= UpdateHealthUI;
            boundCard.OnAttackChanged -= UpdateAttackUI;
            boundCard.OnCardDestroyed -= HandleCardDestroyed;
        }
        boundCard = null;
    }
    #region UI���¼�
    private void UpdateHealthUI(int health)
    {
        healthText.text = health.ToString();
    }

    private void UpdateAttackUI(int attack)
    {
        attackText.text = attack.ToString();
    }
    #endregion
    private void HandleCardDestroyed()
    {
        // ������ͼ�������
        //CardViewManager.Instance.ReturnView(boundCard);
        var handSystem = GameObject.FindObjectOfType<HandSystem>();
        if (handSystem != null)
        {
            var view = handSystem.GetCardView(boundCard);
            if (view != null)
            {
                CardViewManager.Instance.ReturnView(view);
                //handSystem.RemoveCard(boundCard);     //δ���������ٵз����п��ƻ��ƿ�����Ҫ
            }
        }
    }

    public void SetPosition(Transform parent)
    {







        transform.SetParent(parent);
        //transform.localPosition = Vector3.zero;
        //transform.localScale = Vector3.one;
    }
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void MoveToPosition(Vector3 position)
    {
        StartCoroutine(AnimateMove(position));
    }

    private IEnumerator AnimateMove(Vector3 targetPos, float duration = 0.3f)
    {
        Vector3 startPos = rectTransform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            rectTransform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.position = targetPos;
    }

}
