using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSystem : MonoBehaviour
{
    // 英雄区
    public RuntimeCard Hero;

    // 辅助区（最多1个）
    public RuntimeCard Support;

    // 使徒区（最多3个）
    public List<RuntimeCard> Apostles = new List<RuntimeCard>(3);
    // 英雄区位置
    [SerializeField] public Transform HeroZone;
    // 辅助区位置
    public Transform SupportZone;
    // 使徒区位置
    public Transform ApostleZone;

    public void Start()
    {
        
    }
    public bool TrySetHero(RuntimeCard heroCard)
    {
        if (Hero != null) return false;

        Hero = heroCard;
        heroCard.isActive = true;
        PositionCard(heroCard, HeroZone);
        return true;
    }

    public bool TryAddSupport(RuntimeCard supportCard)
    {
        if (Support != null) return false;

        Support = supportCard;
        supportCard.isActive = true;
        PositionCard(supportCard, SupportZone);
        return true;
    }

    public bool TryAddApostle(RuntimeCard apostleCard)
    {
        if (Apostles.Count >= 3) return false;

        Apostles.Add(apostleCard);
        apostleCard.isActive = true;
        PositionCard(apostleCard, ApostleZone);
        return true;
    }

    private void PositionCard(RuntimeCard card, Transform zone)
    {
        var view = CardViewManager.Instance.GetView(card);
        //view.MoveToPosition(zone.position);
    }

    public List<RuntimeCard> GetAllUnits()
    {
        var units = new List<RuntimeCard>();
        if (Hero != null) units.Add(Hero);
        if (Support != null) units.Add(Support);
        units.AddRange(Apostles);
        return units;
    }
}
