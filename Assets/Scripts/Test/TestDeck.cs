using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDeck : MonoBehaviour
{
    [SerializeField] private PlayerController testPlayer; // 在Inspector拖拽你的PlayerController
    [SerializeField] private CardSO[] testCardSOs; // 在Inspector拖拽测试CardSO数组，如亨特·摩尔根
    [SerializeField] private Text debugOutput;
    private DeckSystem deck; // 引用DeckSystem

    private void Start()
    {
        #region 初始化DeckSystem
        if (testPlayer == null)
        {
            testPlayer = FindObjectOfType<PlayerController>(); // 尝试自动获取PlayerController
            if (testPlayer == null)
            {
                Debug.LogError("无法找到PlayerController！");
                return;
            }
        }
        // 初始化DeckSystem
        deck = testPlayer.deck;
        if (deck == null)
        {
            Debug.LogError("无法找到DeckSystem！");
            return;
        }
        deck.Initialize(testPlayer);
        
        #endregion

        // 添加卡牌到卡组，增加多样性
        if (testCardSOs == null || testCardSOs.Length == 0)
        {
            Debug.LogError("请在Inspector中拖拽测试CardSO！");
            return;
        }
        foreach (var cardSO in testCardSOs)
        {
            deck.AddCard(cardSO, 2); // 每张添加2张
            Debug.Log($"添加卡牌: {cardSO.cardData.displayName} (学科梗: {cardSO.cardData.description}), 总卡数: {deck.Count}");
        }

        // 洗牌
        deck.Shuffle();
        Debug.Log($"卡组洗牌完成，总卡数: {deck.Count}");
        Debug.Log($"卡组初始化完成 | 初始卡数: {deck.Count}");
        // 测试抽取，动态根据卡组大小
        int drawCount = 0
            ; // 一次抽取3张
        TestDrawCards(drawCount);
    }

    // 测试抽取方法
    public void TestDrawCards(int count)
    {
        int initialCount = deck.Count; // 记录初始数量
        if (initialCount == 0) { Debug.Log("卡组为空，无法抽取"); return; }
        Debug.Log($"开始抽取 {count} 张卡牌，卡组剩余: {deck.Count}");
        for (int i = 0; i < count && deck.Count > 0; i++)
        { 
            if (testPlayer != null)
            {
                StartCoroutine(DrawCardCoroutine()); // 调用PlayerController的抽卡方法
                Debug.Log($"(剩余卡组: {deck.Count}, 总抽取: {initialCount - deck.Count})");
            }
           
            // 确保不超卡组
            //var drawnCard = deck.Draw();
            //if (drawnCard != null)
            //{
            //    
            //    //testPlayer.TryPlayCard(drawnCard);
            //}
            //else
            //{
            //    Debug.Log("卡组为空，无法抽取");
            //    break;
            //}
        }
    }

    public void TestShuffle()
    {
        testPlayer.ShuffleDeck();
        Debug.Log("卡组洗牌完成");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        { // 按D键抽取一张
            Debug.Log("按下D键抽取一张");
            TestDrawCards(1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            TestShuffle();
            Debug.Log("按下J键洗牌");
        }
    }

    IEnumerator DrawCardCoroutine()
    {
        yield return new WaitForSeconds(1);
        testPlayer.DrawCards(1);
    }
}