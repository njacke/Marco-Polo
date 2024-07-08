using System;
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
        SetOnCDSprites();   

        if (TutorialManager.Instance != null) {
            _catchImage.enabled = false;
            _scanImage.enabled = false;
            _cheatImage.enabled = false;
        }   
    }

    private void OnEnable() {
        GameManager.OnLevelCompleted += GameManager_OnLevelCompleted;
        GameManager.OnGamePaused += GameManager_OnGamePaused;
        GlobalCooldown.OnCooldownStart += GlobalCooldown_OnCooldownStart;
        GlobalCooldown.OnCooldownEnd += GlobalCooldown_OnCooldownEnd;
        TutorialPopUpsUI.OnEnableScan += TutorialPopUpsUI_OnEnableScan;
        TutorialPopUpsUI.OnEnableCheat += TutorialPopUpsUI_OnEnableCheat;
        TutorialPopUpsUI.OnEnableCatch += TutorialPopUpsUI_OnEnableCatch;
    }


    private void OnDisable() {
        GameManager.OnLevelCompleted -= GameManager_OnLevelCompleted;
        GameManager.OnGamePaused -= GameManager_OnGamePaused;
        GlobalCooldown.OnCooldownStart -= GlobalCooldown_OnCooldownStart;        
        GlobalCooldown.OnCooldownEnd -= GlobalCooldown_OnCooldownEnd;
        TutorialPopUpsUI.OnEnableScan -= TutorialPopUpsUI_OnEnableScan;
        TutorialPopUpsUI.OnEnableCheat -= TutorialPopUpsUI_OnEnableCheat;
        TutorialPopUpsUI.OnEnableCatch -= TutorialPopUpsUI_OnEnableCatch;
    }

    private void TutorialPopUpsUI_OnEnableScan() {
        _scanImage.sprite = _scanImages[0];
        _scanImage.enabled = true;
    }

    private void TutorialPopUpsUI_OnEnableCheat() {
        _cheatImage.sprite = _cheatImages[0];
        _cheatImage.enabled = true;
    }

    private void TutorialPopUpsUI_OnEnableCatch() {
        _catchImage.sprite = _catchImages[0];
        _catchImage.enabled = true;
    }

    private void GameManager_OnLevelCompleted() {
        SetOnCDSprites();
    }

    private void GameManager_OnGamePaused() {
        SetOnCDSprites();
    }


    private void GlobalCooldown_OnCooldownStart(PlayerController.SkillType skillType, float cdDuration) {
        SetOnCDSprites();
        SetActiveSkillSprite(skillType);
    }

    private void GlobalCooldown_OnCooldownEnd() {
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
    
