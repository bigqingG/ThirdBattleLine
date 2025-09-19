using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    private readonly Dictionary<GameEventType, List<Action<GameEventData>>> eventHandlers =
         new Dictionary<GameEventType, List<Action<GameEventData>>>();

    public void Register(GameEventType eventType, Action<GameEventData> handler)
    {
        if (!eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = new List<Action<GameEventData>>();
        }

        if (!eventHandlers[eventType].Contains(handler))
        {
            eventHandlers[eventType].Add(handler);
        }
    }

    public void Unregister(GameEventType eventType, Action<GameEventData> handler)
    {
        if (eventHandlers.TryGetValue(eventType, out var handlers))
        {
            handlers.Remove(handler);
        }
    }

    public void Trigger(GameEventType eventType, GameEventData data)
    {
        if (eventHandlers.TryGetValue(eventType, out var handlers))
        {
            // 使用副本遍历防止迭代时修改
            foreach (var handler in handlers.ToList())
            {
                handler?.Invoke(data);
            }
        }
    }
}
