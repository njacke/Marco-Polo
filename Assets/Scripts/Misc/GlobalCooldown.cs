using System;
using UnityEngine;

public class GlobalCooldown
{
    public static Action<PlayerController.SkillType, float> OnCooldownStart;
    public static Action OnCooldownEnd;

    public float StartingCooldown { get; private set; } = 0f;
    public float RemainingCooldown { get; private set; } = 0f;
    public bool IsOnCooldown { get; private set; } = false;

    public GlobalCooldown(float cooldownDuration) {
        StartingCooldown = cooldownDuration;
    }

    public void StartCooldown(PlayerController.SkillType skillType) {
        if (StartingCooldown > 0f) {
            RemainingCooldown = StartingCooldown;
            IsOnCooldown = true;
            OnCooldownStart?.Invoke(skillType, StartingCooldown);
        }
    }

    public void ResetCooldown() {
        RemainingCooldown = 0f;
        IsOnCooldown = false;
        OnCooldownEnd?.Invoke();
    }

    public void TrackCooldown() {
        if (RemainingCooldown > 0f) {
            RemainingCooldown -= Time.deltaTime;
        } else if (IsOnCooldown) {
            ResetCooldown();
        }        
    }
}
