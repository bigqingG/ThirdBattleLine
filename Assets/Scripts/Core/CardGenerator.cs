using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    private readonly Dictionary<string, Stack<RuntimeCard>> cardPools =
        new Dictionary<string, Stack<RuntimeCard>>();

    public RuntimeCard CreateCard(CardSO cardSO, PlayerController owner)
    {
        // �ӳ��л�ȡ�򴴽��¿���
        if (TryGetFromPool(cardSO.cardData.cardID, out var card))
        {
            card.owner = owner;
            card.ResetState();
            return card;
        }
        return new RuntimeCard(cardSO, owner);
    }

    public void ReturnToPool(RuntimeCard card)
    {
        if (card == null || card.isDestroyed) return;

        // ������״̬
        card.ClearEventBindings();
        card.isActive = false;

        // ��ӵ������
        string id = card.sourceData.cardData.cardID;
        if (!cardPools.ContainsKey(id))
        {
            cardPools[id] = new Stack<RuntimeCard>();
        }

        cardPools[id].Push(card);
    }

    private bool TryGetFromPool(string cardId, out RuntimeCard card)
    {
        if (cardPools.TryGetValue(cardId, out var pool) && pool.Count > 0)
        {
            card = pool.Pop();
            return true;
        }

        card = null;
        return false;
    }
}
