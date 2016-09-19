﻿using UnityEngine;
using System.Collections;

public class CharacterAnimationComponent : CharacterComponent
{
    public static readonly string RunKey = "run";

    public CharacterAnimationComponent(CharacterAgent agent) : base (agent) { }

    #region implemented abstract members of AgentComponent

    public override void FrameFeed()
    {
        float runningSpeed = InputConfig.Run() ? GameConfig.maxCharacterSpeed : GameConfig.minCharacterSpeed;
        SetRun(runningSpeed);
    }

    #endregion

    protected virtual void SetRun(float speed)
    {
        agent.characterAnimator.SetFloat(RunKey, speed);
    }
}