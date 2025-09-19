using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ¿¨ÅÆ×ÊÔ´ÅäÖÃ£¨ScriptableObject£©
[CreateAssetMenu(menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    public CardData cardData;
    public List<EffectSO> effects;
    [Header("Visuals")]  // ¿¨ÅÆÊÓ¾õĞ§¹û, Èç¿¨ÅÆ±³¾°¡¢¿¨ÅÆÍ¼±ê´îÅäRunTime
    public Sprite cardArt;
    public GameObject cardPrefab;

}
