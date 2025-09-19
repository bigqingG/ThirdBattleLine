using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem 
{
    private PlayerController player;
    private int current;
    private int max;

    public int Current => current;
    public int Max => max;

    public ManaSystem(PlayerController player)
    {
        this.player = player;
        current = 0;
        max = 0;
    }

    public void StartTurn()
    {
        max = Mathf.Min(10, max + 1); // 最大10点法力
        current = max;
    }

    public bool CanAfford(int cost) => cost <= current;

    public void SpendMana(int amount)
    {
        current = Mathf.Max(0, current - amount);
    }
}
