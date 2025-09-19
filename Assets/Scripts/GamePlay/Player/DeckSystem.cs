using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    private readonly List<RuntimeCard> cards = new List<RuntimeCard>();
    private PlayerController owner;
    
    public int Count => cards.Count;

    public void Initialize(PlayerController owner)
    {
        this.owner = owner;
        
    }

    public void AddCard(CardSO cardSO, int count = 1)
    {
        if (cardSO == null)
        {
            Debug.LogError("卡牌资源为空！");
            return;
        }
        for (int i = 0; i < count; i++)
        {
            

            var card = GameManager.Instance.CardGenerator.CreateCard(cardSO, owner);
            if (card != null) cards.Add(card);
            else Debug.LogError("CardGenerator创建失败！");



        }
    }

    public RuntimeCard Draw()
    {
        if (cards.Count == 0) return null;

        RuntimeCard card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public void Shuffle()
    {
        // Fisher-Yates 洗牌算法
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }
    }
}
