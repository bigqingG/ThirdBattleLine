using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
//using UnityEngine.U2D;
using UnityEngine.UI;

public class CardViewController : MonoBehaviour
{
    [Header("UI组件")]
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

    //初始化
    public void Initialize(RuntimeCard card)
    {



        boundCard = card;

        // 绑定基础数据（各类卡牌通用）
        cardArt.sprite = card.sourceData.cardArt;
        cardName.text = card.sourceData.cardData.displayName;
        descriptionText.text = card.sourceData.cardData.description;
        // 卡牌类型判断（关键修改点）
        CardType cardType = card.sourceData.cardData.cardType;

        // 1. 英雄牌暂无处理 (无攻防和花费)
        if (cardType == CardType.Hero)
        {

        }
        // 2. 法术/邪术处理 (无攻防)
        else if (cardType == CardType.Spell || cardType == CardType.Hex)
        {
            manaCost.text = card.sourceData.cardData.manaCost.ToString();

        }
        // 3. 辅助/使徒处理 (完整属性)
        else if (cardType == CardType.Support || cardType == CardType.Apostle)
        {
            Debug.Log($"当前攻击力 {card.currentAttack}，当前生命值 {card.currentHealth}");
            manaCost.text = card.sourceData.cardData.manaCost.ToString();
            attackText.text = card.currentAttack.ToString();
            healthText.text = card.currentHealth.ToString();


        }
        // 绑定额外事件
        switch (cardType)
        {
            case CardType.Support:
            case CardType.Apostle:
            case CardType.Hero: // 英雄监听血量变化
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
    #region UI绑定事件
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
        // 返回视图到对象池
        //CardViewManager.Instance.ReturnView(boundCard);
        var handSystem = GameObject.FindObjectOfType<HandSystem>();
        if (handSystem != null)
        {
            var view = handSystem.GetCardView(boundCard);
            if (view != null)
            {
                CardViewManager.Instance.ReturnView(view);
                //handSystem.RemoveCard(boundCard);     //未来增加销毁敌方手中卡牌机制可能需要
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
