using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDeck : MonoBehaviour
{
    [SerializeField] private PlayerController testPlayer; // ��Inspector��ק���PlayerController
    [SerializeField] private CardSO[] testCardSOs; // ��Inspector��ק����CardSO���飬����ء�Ħ����
    [SerializeField] private Text debugOutput;
    private DeckSystem deck; // ����DeckSystem

    private void Start()
    {
        #region ��ʼ��DeckSystem
        if (testPlayer == null)
        {
            testPlayer = FindObjectOfType<PlayerController>(); // �����Զ���ȡPlayerController
            if (testPlayer == null)
            {
                Debug.LogError("�޷��ҵ�PlayerController��");
                return;
            }
        }
        // ��ʼ��DeckSystem
        deck = testPlayer.deck;
        if (deck == null)
        {
            Debug.LogError("�޷��ҵ�DeckSystem��");
            return;
        }
        deck.Initialize(testPlayer);
        
        #endregion

        // ��ӿ��Ƶ����飬���Ӷ�����
        if (testCardSOs == null || testCardSOs.Length == 0)
        {
            Debug.LogError("����Inspector����ק����CardSO��");
            return;
        }
        foreach (var cardSO in testCardSOs)
        {
            deck.AddCard(cardSO, 2); // ÿ�����2��
            Debug.Log($"��ӿ���: {cardSO.cardData.displayName} (ѧ�ƹ�: {cardSO.cardData.description}), �ܿ���: {deck.Count}");
        }

        // ϴ��
        deck.Shuffle();
        Debug.Log($"����ϴ����ɣ��ܿ���: {deck.Count}");
        Debug.Log($"�����ʼ����� | ��ʼ����: {deck.Count}");
        // ���Գ�ȡ����̬���ݿ����С
        int drawCount = 0
            ; // һ�γ�ȡ3��
        TestDrawCards(drawCount);
    }

    // ���Գ�ȡ����
    public void TestDrawCards(int count)
    {
        int initialCount = deck.Count; // ��¼��ʼ����
        if (initialCount == 0) { Debug.Log("����Ϊ�գ��޷���ȡ"); return; }
        Debug.Log($"��ʼ��ȡ {count} �ſ��ƣ�����ʣ��: {deck.Count}");
        for (int i = 0; i < count && deck.Count > 0; i++)
        { 
            if (testPlayer != null)
            {
                StartCoroutine(DrawCardCoroutine()); // ����PlayerController�ĳ鿨����
                Debug.Log($"(ʣ�࿨��: {deck.Count}, �ܳ�ȡ: {initialCount - deck.Count})");
            }
           
            // ȷ����������
            //var drawnCard = deck.Draw();
            //if (drawnCard != null)
            //{
            //    
            //    //testPlayer.TryPlayCard(drawnCard);
            //}
            //else
            //{
            //    Debug.Log("����Ϊ�գ��޷���ȡ");
            //    break;
            //}
        }
    }

    public void TestShuffle()
    {
        testPlayer.ShuffleDeck();
        Debug.Log("����ϴ�����");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        { // ��D����ȡһ��
            Debug.Log("����D����ȡһ��");
            TestDrawCards(1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            TestShuffle();
            Debug.Log("����J��ϴ��");
        }
    }

    IEnumerator DrawCardCoroutine()
    {
        yield return new WaitForSeconds(1);
        testPlayer.DrawCards(1);
    }
}