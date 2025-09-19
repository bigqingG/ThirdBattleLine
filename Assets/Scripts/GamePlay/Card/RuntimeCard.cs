using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeCard 
{
    //获取卡牌SO数据，避免直接访问原始数据
    public readonly CardSO sourceData;  
    //卡牌所属玩家，校验卡牌权限并为多人系统提供支持
    public PlayerController owner;

    // 运行时状态，生命值，攻击力，是否激活，是否已被摧毁，仅获取数据，通过解耦提供修改接口同时不修改原始数据
    public int currentHealth;
    public int currentAttack;
    public bool isActive = false;
    public bool isDestroyed = false;

    // 效果相关，卡牌修改器以及效果事件绑定
    public List<CardModifier> activeModifiers = new List<CardModifier>();
    public List<EffectEventBinding> eventBindings = new List<EffectEventBinding>();

    // 生命和攻击变化事件，仅用于外部系统更新UI
    public event Action<int> OnHealthChanged;
    public event Action<int> OnAttackChanged;
    public event Action OnCardDestroyed;// 卡牌销毁事件，通知外部系统销毁卡牌清理相关资源并更新UI和生成特效

    public RuntimeCard(CardSO source, PlayerController owner)
    {
        this.sourceData = source;
        this.owner = owner;
        ResetState();
    }

    public void ResetState()
    {
        currentHealth = sourceData.cardData.health;
        currentAttack = sourceData.cardData.attack;
        isActive = false;
        isDestroyed = false;
        activeModifiers.Clear();
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0 && !isDestroyed)
        {
            DestroyCard();
        }
    }

    public void Heal(int amount)
    {
        int maxHealth = GetMaxHealth();
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetAttack(int value)
    {
        currentAttack = Mathf.Max(0, value);
        OnAttackChanged?.Invoke(currentAttack);
    }

    public int GetMaxHealth()
    {
        // 基础生命值 + 来自modifier的生命值加成
        int baseHealth = sourceData.cardData.health;
        int bonus = activeModifiers.Sum(m => m.healthBonus);
        return baseHealth + bonus;
    }

    // 销毁卡牌
    public void DestroyCard()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // 1. 触发亡语效果
        TriggerEffects(TriggerType.OnDeath);

        // 2. 清理事件绑定
        ClearEventBindings();

        // 3. 通知销毁
        OnCardDestroyed?.Invoke();
    }

    // 清除所有事件绑定
    public void ClearEventBindings()
    {
        foreach (var binding in eventBindings)
        {
            GameManager.Instance.EventSystem.Unregister(binding.eventType, binding.handler);
        }
        eventBindings.Clear();
    }

    // 触发特定类型的即时效果
    public void TriggerEffects(TriggerType trigger)
    {
        var applicableEffects = sourceData.effects.Where(e => e.triggerType == trigger);

        foreach (var effect in applicableEffects)
        {
            var context = new EffectContext
            {
                sourceCard = this,
                castingPlayer = owner,
                gameManager = GameManager.Instance
            };

            if (effect.CheckCondition(context))
            {
                effect.ExecuteEffect(context);
            }
        }
    }

    // 激活持久效果（注册事件监听）
    public void ActivatePersistentEffects()
    {
        foreach (var effect in sourceData.effects.Where(e => e.isPersistent))
        {
            // 创建事件处理器
            var handler = CreatePersistentHandler(effect);

            // 注册到事件系统
            var binding = new EffectEventBinding
            {
                eventType = effect.listenEvent,
                handler = handler,
                sourceEffect = effect,
                ownerCard = this
            };

            GameManager.Instance.EventSystem.Register(binding.eventType, binding.handler);
            eventBindings.Add(binding);
        }
    }

    // 为持久效果创建事件处理器
    private Action<GameEventData> CreatePersistentHandler(EffectSO effect)
    {
        return eventData => {
            // 检查触发条件是否满足
            if (effect.CheckTriggerCondition(eventData, this))
            {
                // 创建执行上下文
                var context = new EffectContext
                {
                    sourceCard = this,
                    castingPlayer = owner,
                    targetCard = eventData.card,
                    gameManager = GameManager.Instance
                };

                // 执行效果
                effect.ExecuteEffect(context);
            }
        };
    }
}

