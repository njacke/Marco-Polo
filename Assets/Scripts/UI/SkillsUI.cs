using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour
{
    [SerializeField] private Image _catchImage;
    [SerializeField] private Image _scanImage;
    [SerializeField] private Image _cheatImage;

    [SerializeField] private Sprite[] _catchImages;
    [SerializeField] private Sprite[] _scanImages;
    [SerializeField] private Sprite[] _cheatImages;

    private void Start() {
        ResetCDSprites();        
    }

    private void OnEnable() {
        GlobalCooldown.OnCooldownStart += UpdateCDStartSprites;
        GlobalCooldown.OnCooldownEnd += ResetCDSprites;
    }

    private void OnDisable() {
        GlobalCooldown.OnCooldownStart -= UpdateCDStartSprites;        
        GlobalCooldown.OnCooldownEnd -= ResetCDSprites;
    }


    private void UpdateCDStartSprites(PlayerController.SkillType skillType, float cdDuration) {
        SetOnCDSprites();
        SetActiveSkillSprite(skillType);
    }

    private void ResetCDSprites() {
        _catchImage.sprite = _catchImages[0];
        _scanImage.sprite = _scanImages[0];
        _cheatImage.sprite = _cheatImages[0];
    }

    private void SetOnCDSprites() {
        _catchImage.sprite = _catchImages[1];
        _scanImage.sprite = _scanImages[1];
        _cheatImage.sprite = _cheatImages[1];
    }

    private void SetActiveSkillSprite(PlayerController.SkillType skillType) {
        switch (skillType) {
            case PlayerController.SkillType.Catch:
                _catchImage.sprite = _catchImages[2];
                break;
            case PlayerController.SkillType.Scan:
                _scanImage.sprite = _scanImages[2];
                break;
            case PlayerController.SkillType.Cheat:
                _cheatImage.sprite = _cheatImages[2];
                break;
            default:
                break;
        }
    }
}
    
