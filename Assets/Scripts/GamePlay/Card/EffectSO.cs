using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EffectSO : ScriptableObject
{
    [Header("Ŀ������")]
    public EffectTargetType targetType = EffectTargetType.Self;

    [Header("��������")]
    public TriggerType triggerType;
    public bool isPersistent = false;
    public bool requiresExplicitTarget = false;
    public GameEventType listenEvent;

    // ִ��Ч���ĺ��ķ�����������ʵ�֣�
    public abstract void ExecuteEffect(EffectContext context);

    // ������飨�������า�ǣ����Ǳ�Ҫ����д
    public virtual bool CheckCondition(EffectContext context) => true;
    public virtual bool CheckTriggerCondition(GameEventData eventData, RuntimeCard owner) => true;

    // ��ȡ����Ԥ�����༭���ã���ͬ��
    public virtual string GetDescriptionPreview() => $"{name}Ч��";
}

public struct GameEventData
{
    public RuntimeCard card;
    public int intValue;
    public PlayerController player;

    public static GameEventData FromCard(RuntimeCard card) => new GameEventData { card = card, player = card?.owner };
}

public struct EffectEventBinding
{
    public GameEventType eventType;
    public System.Action<GameEventData> handler;
    public RuntimeCard ownerCard;
    public EffectSO sourceEffect;
}
