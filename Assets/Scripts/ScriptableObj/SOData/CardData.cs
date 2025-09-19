using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������������
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
// ���ƻ������ݲ�
[System.Serializable]
public struct CardData
{
    public string cardID;
    public string displayName;
    [TextArea] public string description;
    public CardType cardType;
    public TriggerType triggerType;
    public int manaCost;      //���ƻ��ѵķ���ֵ
    public int health;        // Ӣ��/��λ����ֵ
    public int attack;        // ��λ������
    public bool isPersistent; // а�������Լ������ڳ���Ч
    public int maxHealth;     // Ӣ������ֵ
}


