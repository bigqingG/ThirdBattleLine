using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �������
    public string playerName;
    public bool isHuman = true;     //�˻�У��
    

    // ���ƹ���
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
    private  void DrawCard() //��װ�鿨�߼�
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("���Դӿտ���鿨");//�������ƣ��ϵͳ
            return;
        }

        var card = deck.Draw();//��ȡ���鶥������
        if (card == null)
        {
            Debug.LogError("�鿨ʧ�ܣ�����ʵ����Ч");
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
            Debug.Log("���������޷��ٳ鿨");
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
        // ������֤
        if (card == null || !hand.HasCard(card)) return false;
        if (!mana.CanAfford(card.sourceData.cardData.manaCost)) return false;

        // ���ķ���ֵ
        mana.SpendMana(card.sourceData.cardData.manaCost);

        // �ض��������ʹ���
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
                // ������ʹ�ú󴥷�Ч��������
                card.TriggerEffects(TriggerType.OnPlay);
                GameManager.Instance.CardGenerator.ReturnToPool(card);
                break;

            case CardType.Hex:
                // а���Ƽ������Ч��
                card.TriggerEffects(TriggerType.OnPlay);
                card.ActivatePersistentEffects();
                break;
        }

        // ���������Ƴ�
        hand.RemoveCard(card);

        // �������ƴ���¼�
        GameManager.Instance.EventSystem.Trigger(
            GameEventType.CardPlayed,
            GameEventData.FromCard(card)
        );

        return true;
    }
}
