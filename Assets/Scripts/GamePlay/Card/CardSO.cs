using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������Դ���ã�ScriptableObject��
[CreateAssetMenu(menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    public CardData cardData;
    public List<EffectSO> effects;
    [Header("Visuals")]  // �����Ӿ�Ч��, �翨�Ʊ���������ͼ�����RunTime
    public Sprite cardArt;
    public GameObject cardPrefab;

}
