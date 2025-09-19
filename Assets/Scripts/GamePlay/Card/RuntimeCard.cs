using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeCard 
{
    //��ȡ����SO���ݣ�����ֱ�ӷ���ԭʼ����
    public readonly CardSO sourceData;  
    //����������ң�У�鿨��Ȩ�޲�Ϊ����ϵͳ�ṩ֧��
    public PlayerController owner;

    // ����ʱ״̬������ֵ�����������Ƿ񼤻�Ƿ��ѱ��ݻ٣�����ȡ���ݣ�ͨ�������ṩ�޸Ľӿ�ͬʱ���޸�ԭʼ����
    public int currentHealth;
    public int currentAttack;
    public bool isActive = false;
    public bool isDestroyed = false;

    // Ч����أ������޸����Լ�Ч���¼���
    public List<CardModifier> activeModifiers = new List<CardModifier>();
    public List<EffectEventBinding> eventBindings = new List<EffectEventBinding>();

    // �����͹����仯�¼����������ⲿϵͳ����UI
    public event Action<int> OnHealthChanged;
    public event Action<int> OnAttackChanged;
    public event Action OnCardDestroyed;// ���������¼���֪ͨ�ⲿϵͳ���ٿ������������Դ������UI��������Ч

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
        // ��������ֵ + ����modifier������ֵ�ӳ�
        int baseHealth = sourceData.cardData.health;
        int bonus = activeModifiers.Sum(m => m.healthBonus);
        return baseHealth + bonus;
    }

    // ���ٿ���
    public void DestroyCard()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // 1. ��������Ч��
        TriggerEffects(TriggerType.OnDeath);

        // 2. �����¼���
        ClearEventBindings();

        // 3. ֪ͨ����
        OnCardDestroyed?.Invoke();
    }

    // ��������¼���
    public void ClearEventBindings()
    {
        foreach (var binding in eventBindings)
        {
            GameManager.Instance.EventSystem.Unregister(binding.eventType, binding.handler);
        }
        eventBindings.Clear();
    }

    // �����ض����͵ļ�ʱЧ��
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

    // ����־�Ч����ע���¼�������
    public void ActivatePersistentEffects()
    {
        foreach (var effect in sourceData.effects.Where(e => e.isPersistent))
        {
            // �����¼�������
            var handler = CreatePersistentHandler(effect);

            // ע�ᵽ�¼�ϵͳ
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

    // Ϊ�־�Ч�������¼�������
    private Action<GameEventData> CreatePersistentHandler(EffectSO effect)
    {
        return eventData => {
            // ��鴥�������Ƿ�����
            if (effect.CheckTriggerCondition(eventData, this))
            {
                // ����ִ��������
                var context = new EffectContext
                {
                    sourceCard = this,
                    castingPlayer = owner,
                    targetCard = eventData.card,
                    gameManager = GameManager.Instance
                };

                // ִ��Ч��
                effect.ExecuteEffect(context);
            }
        };
    }
}

