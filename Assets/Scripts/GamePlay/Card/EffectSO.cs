using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EffectSO : ScriptableObject
{
    [Header("目标配置")]
    public EffectTargetType targetType = EffectTargetType.Self;

    [Header("触发配置")]
    public TriggerType triggerType;
    public bool isPersistent = false;
    public bool requiresExplicitTarget = false;
    public GameEventType listenEvent;

    // 执行效果的核心方法（由子类实现）
    public abstract void ExecuteEffect(EffectContext context);

    // 条件检查（可在子类覆盖），非必要不重写
    public virtual bool CheckCondition(EffectContext context) => true;
    public virtual bool CheckTriggerCondition(GameEventData eventData, RuntimeCard owner) => true;

    // 获取描述预览（编辑器用），同上
    public virtual string GetDescriptionPreview() => $"{name}效果";
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
