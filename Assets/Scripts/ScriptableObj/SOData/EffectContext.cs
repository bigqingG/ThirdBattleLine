using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectContext
{
    public RuntimeCard sourceCard;
    public PlayerController castingPlayer;
    public PlayerController targetPlayer;
    public RuntimeCard targetCard;
    public List<RuntimeCard> selectedTargets;
    public GameManager gameManager;
}
