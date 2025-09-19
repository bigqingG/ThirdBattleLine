using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 卡牌数据类型
public enum CardType { Hero, Support, Apostle, Spell, Hex }
public enum TriggerType { OnPlay, OnDeath, OnSpellCast, OnSupportPlaced, OnTurnStart, OnTurnEnd }
public enum EffectTargetType
{
    Self, TargetUnit, RandomEnemyUnit, AllEnemyUnits,
    OpponentHero, FriendlyHero, ChooseOne, MultipleTargets
}
public enum GameEventType
{
    CardDrawn, CardPlayed, CardDestroyed,
    SpellCast, SupportPlaced,
    TurnStarted, TurnEnded,
    DamageDealed, HealApplied
}
public enum CardRarity { Common, Rare, Epic, Legendary }
public enum CardCamp { Lang,Math,Bio,Phy,Che,His}
// 卡牌基础数据层
[System.Serializable]
public struct CardData
{
    public string cardID;
    public string displayName;
    [TextArea] public string description;
    public CardType cardType;
    public TriggerType triggerType;
    public int manaCost;      //卡牌花费的法力值
    public int health;        // 英雄/单位生命值
    public int attack;        // 单位攻击力
    public bool isPersistent; // 邪术触发以及辅助在场特效
    public int maxHealth;     // 英雄生命值
}


