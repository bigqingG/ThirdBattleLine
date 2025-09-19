using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 玩家属性
    public string playerName;
    public bool isHuman = true;     //人机校验
    

    // 卡牌管理
    public DeckSystem deck;
    public HandSystem hand;
    public FieldSystem field;
    public ManaSystem mana;

    private void Start()
    {
        mana = new ManaSystem(this);
        deck.Initialize(this);
        //DrawInitialHand();
    }

    private void DrawInitialHand(int count = 3)
    {
        for (int i = 0; i < count; i++)
        {
            DrawCard();
        }
    }

    public void StartTurn()
    {
        mana.StartTurn();
        DrawCard();
    }
    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (CanDrawCard())
            {
                DrawCard();
            }
        }
    }
    private  void DrawCard() //封装抽卡逻辑
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("尝试从空卡组抽卡");//后面接入疲劳系统
            return;
        }

        var card = deck.Draw();//获取卡组顶部卡牌
        if (card == null)
        {
            Debug.LogError("抽卡失败：卡牌实例无效");
            return;
        }

        hand.AddCard(card);
        GameManager.Instance.EventSystem.Trigger(
            GameEventType.CardDrawn,
            GameEventData.FromCard(card)
        );
    }

    public bool CanDrawCard()
    {
        if (deck.Count == 0 || hand.Cards.Count >= hand.MaxHandSize)
        {
            Debug.Log("手牌已满无法再抽卡");
            return false;
        }
        return deck.Count > 0;
    }

    public void ShuffleDeck()
    {
       // hand.ShuffleHand();
    }


    public bool TryPlayCard(RuntimeCard card)
    {
        // 基础验证
        if (card == null || !hand.HasCard(card)) return false;
        if (!mana.CanAfford(card.sourceData.cardData.manaCost)) return false;

        // 消耗法力值
        mana.SpendMana(card.sourceData.cardData.manaCost);

        // 特定卡牌类型处理
        switch (card.sourceData.cardData.cardType)
        {
            case CardType.Hero:
                if (!field.TrySetHero(card)) return false;
                break;

            case CardType.Support:
                if (!field.TryAddSupport(card)) return false;
                card.ActivatePersistentEffects();
                break;

            case CardType.Apostle:
                if (!field.TryAddApostle(card)) return false;
                card.ActivatePersistentEffects();
                break;

            case CardType.Spell:
                // 法术牌使用后触发效果并销毁
                card.TriggerEffects(TriggerType.OnPlay);
                GameManager.Instance.CardGenerator.ReturnToPool(card);
                break;

            case CardType.Hex:
                // 邪术牌激活持续效果
                card.TriggerEffects(TriggerType.OnPlay);
                card.ActivatePersistentEffects();
                break;
        }

        // 从手牌中移除
        hand.RemoveCard(card);

        // 触发卡牌打出事件
        GameManager.Instance.EventSystem.Trigger(
            GameEventType.CardPlayed,
            GameEventData.FromCard(card)
        );

        return true;
    }
}
