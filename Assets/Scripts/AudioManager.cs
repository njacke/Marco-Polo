using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Range(0f, 2f)]
    [SerializeField] private float _masterVolume = 1f;
    [Range(0f, 2f)]
    [SerializeField] private float _winVoiceMaxDelay = .5f;
    [SerializeField] private SoundsCollectionSO _soundsCollectionSO;
    [SerializeField] private PlayerVoicesCollection _playerVoicesCollection;
    [SerializeField] private NPCVoicesCollection _itaVoicesCollection;
    [SerializeField] private NPCVoicesCollection _gerVoicesCollection;
    [SerializeField] private NPCVoicesCollection _spaVoicesCollection;
    [SerializeField] private NPCVoicesCollection _usaVoicesCollection;
    [SerializeField] private NPCVoicesCollection _dogVoicesCollection;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _voiceMixerGroup;
    [SerializeField] private int _maxAudioSources = 15;
    [SerializeField] private float _mainMenuMusicDelay = 1f;
    [SerializeField] private SoundSO _mumTutorialSound;
    private AudioSource _slideAudioSource;
    private AudioSource _currentMusic;

    private Queue<AudioSource> _audioSourcePool = new();
    private List<AudioSource> _activeAudioSources = new();

    private Dictionary<NPC.NPCType, NPCVoicesCollection> _npcsVoicesDict;
    private Dictionary<GameManager.Level, SoundSO[]> _levelMusicDict; 
    private Dictionary<Obstacle.ObstacleType, SoundSO[]> _obstacleSFXDict;

    #region Unity Methods

    private void OnEnable() {
        WinScreenUI.OnTrophyDisplayed += WinScreenUI_OnTrophyDisplayed;
        GameManager.OnLevelLoaded += GameManager_OnLevelLoaded;
        PlayerController.OnReady += PlayerController_OnReady;
        PlayerController.OnCatch += PlayerController_OnCatch;
        PlayerController.OnScan += PlayerController_OnScan;
        PlayerController.OnCheat += PlayerController_OnCheat;
        NPC.OnRandomVoice += NPC_OnRandomVoice;
        NPC.OnCheatDetected += NPC_OnCheatDetected;
        NPC.OnScanResponse += NPC_OnScanResponse;
        Obstacle.OnWaveTriggered += Obstacle_OnWaveTriggered;
        CountdownUI.OnCountdownStep += CountdownUI_OnCountdownStep;
        CursorManager.OnMouseClick += CursorManager_OnMouseClick;
        SlideUI.OnSlideStarted += SlideUI_OnSlideStarted;
        TutorialPopUpsUI.OnMumTutorialDialogue += TutorialPopUpsUI_OnMumTutorialDialogue;
        MainMenuUI.OnMainMenuLoaded += MainMenuUI_OnMainMenuLoaded;
        TutorialManager.OnTutorialLoaded += TutorialManager_OnTutorialLoaded;
        EndGameUI.OnEndGameMenuLoaded += EndGameUI_OnEndGameMenuLoaded;
        TutorialPopUpsUI.OnDialogueBoxDisplayed += TutorialPopUpsUI_OnDialogueBoxDisplayed;
        GameManager.OnCinematicLoaded += GameManager_OnCinematicLoaded;
    }


    private void OnDisable() {
        WinScreenUI.OnTrophyDisplayed -= WinScreenUI_OnTrophyDisplayed;
        GameManager.OnLevelLoaded -= GameManager_OnLevelLoaded;
        PlayerController.OnReady -= PlayerController_OnReady;
        PlayerController.OnCatch -= PlayerController_OnCatch;
        PlayerController.OnScan -= PlayerController_OnScan;
        PlayerController.OnCheat -= PlayerController_OnCheat;
        NPC.OnRandomVoice -= NPC_OnRandomVoice;
        NPC.OnCheatDetected -= NPC_OnCheatDetected;
        NPC.OnScanResponse -= NPC_OnScanResponse;
        Obstacle.OnWaveTriggered -= Obstacle_OnWaveTriggered;
        CountdownUI.OnCountdownStep -= CountdownUI_OnCountdownStep;
        CursorManager.OnMouseClick -= CursorManager_OnMouseClick;
        SlideUI.OnSlideStarted -= SlideUI_OnSlideStarted;
        TutorialPopUpsUI.OnMumTutorialDialogue -= TutorialPopUpsUI_OnMumTutorialDialogue;
        MainMenuUI.OnMainMenuLoaded -= MainMenuUI_OnMainMenuLoaded;
        TutorialManager.OnTutorialLoaded -= TutorialManager_OnTutorialLoaded;
        EndGameUI.OnEndGameMenuLoaded -= EndGameUI_OnEndGameMenuLoaded;
        TutorialPopUpsUI.OnDialogueBoxDisplayed -= TutorialPopUpsUI_OnDialogueBoxDisplayed;
        GameManager.OnCinematicLoaded -= GameManager_OnCinematicLoaded;
    }

    private void GameManager_OnCinematicLoaded(GameManager.Level level) {
        PlayRandomSound(_levelMusicDict[level], isMusic: true);
    }

    private void TutorialPopUpsUI_OnDialogueBoxDisplayed() {
        ItaSpeech();
    }

    private void EndGameUI_OnEndGameMenuLoaded() {
        PlayRandomSound(_soundsCollectionSO.EndGameMusic, isMusic: true);
    }

    private void TutorialManager_OnTutorialLoaded() {
        PlayRandomSound(_levelMusicDict[GameManager.Level.Tutorial], isMusic: true);
    }

    private void MainMenuUI_OnMainMenuLoaded() {
        StartCoroutine(PlayRandomSoundWithDelay(_soundsCollectionSO.MainMenuMusic, isMusic: true, _mainMenuMusicDelay));
    }

    private void TutorialPopUpsUI_OnMumTutorialDialogue() {
        if (TutorialManager.Instance != null) {
            PlaySound(_mumTutorialSound);
        }
    }

    private void SlideUI_OnSlideStarted() {
        PlaySoundWithReservedAudioSource(_soundsCollectionSO.MenuSlide, _slideAudioSource);
    }

    private void CursorManager_OnMouseClick() {
        PlayRandomSound(_soundsCollectionSO.MouseClick);
    }

    private void CountdownUI_OnCountdownStep(bool isFinal, int _stepsLeft) {
        if (!isFinal) {
            PlayRandomSound(_soundsCollectionSO.Beep);
        }
    }

    private void NPC_OnScanResponse(NPC npc, bool waveTriggered) {
        if (waveTriggered) {
            PlayRandomSound(_npcsVoicesDict[npc.GetNPCType].ScanResponse);
        }
    }

    private void NPC_OnCheatDetected(NPC npc, bool waveTriggered) {
        if (waveTriggered) {
            PlayRandomSound(_npcsVoicesDict[npc.GetNPCType].CheatDetected);
        }
    }

    private void NPC_OnRandomVoice(NPC npc) {
        PlayRandomSound(_npcsVoicesDict[npc.GetNPCType].Random); 
    }

    private void PlayerController_OnReady() {
        PlayRandomSound(_playerVoicesCollection.Ready);
    }

    private void PlayerController_OnCatch() {
        PlayRandomSound(_playerVoicesCollection.Catch);
    }

    private void PlayerController_OnScan() {
        PlayRandomSound(_playerVoicesCollection.Scan);
    }

    private void PlayerController_OnCheat(float duration) {
        PlayRandomSound(_playerVoicesCollection.Cheat);
    }

    private void Obstacle_OnWaveTriggered(Obstacle sender) {
        PlayRandomSound(_obstacleSFXDict[sender.GetObstacleType]);
    }

    private void GameManager_OnLevelLoaded(GameManager.Level level) {
        PlayRandomSound(_levelMusicDict[level], isMusic: true);
    }

    protected override void Awake() {
        base.Awake();
        Init();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            PlayRandomSound(_soundsCollectionSO.MouseClick);
        }
    }

    private void Init() {
        _npcsVoicesDict = new Dictionary<NPC.NPCType, NPCVoicesCollection>() {
            { NPC.NPCType.Italian, _itaVoicesCollection },
            { NPC.NPCType.German, _gerVoicesCollection },
            { NPC.NPCType.Spaniard, _spaVoicesCollection },
            { NPC.NPCType.American, _usaVoicesCollection },
            { NPC.NPCType.Dog, _dogVoicesCollection}
        };

        _levelMusicDict = new Dictionary<GameManager.Level, SoundSO[]>() {
            { GameManager.Level.Tutorial, _soundsCollectionSO.LobbyMusic },
            { GameManager.Level.Pool, _soundsCollectionSO.PoolMusic },
            { GameManager.Level.Garden, _soundsCollectionSO.GardenMusic },
            { GameManager.Level.Terrace, _soundsCollectionSO.TerraceMusic },
            { GameManager.Level.Lobby, _soundsCollectionSO.LobbyMusic }
        };

        _obstacleSFXDict = new Dictionary<Obstacle.ObstacleType, SoundSO[]>() {
            { Obstacle.ObstacleType.Duck, _soundsCollectionSO.Duck },
            { Obstacle.ObstacleType.Tube, _soundsCollectionSO.Tube },
            { Obstacle.ObstacleType.Fontain, _soundsCollectionSO.Fountain },
            { Obstacle.ObstacleType.Table, _soundsCollectionSO.Table },
            { Obstacle.ObstacleType.Piano, _soundsCollectionSO.Piano },
            { Obstacle.ObstacleType.Trolley, _soundsCollectionSO.Trolley },
        };

        // init pool
        for (int i = 0; i < _maxAudioSources; i++) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            _audioSourcePool.Enqueue(audioSource);
        }

        _slideAudioSource = gameObject.AddComponent<AudioSource>();
        _slideAudioSource.playOnAwake = false;
    }

    #endregion

    #region Sound Methods

    private void PlayRandomSound(SoundSO[] sounds, bool isMusic = false) {
        if (sounds != null && sounds.Length > 0) {
            SoundSO soundSO = sounds[Random.Range(0, sounds.Length)];
            if (isMusic) {
                PlayMusic(soundSO);
            } else {
                PlaySound(soundSO);
            }
        }
    }

    private IEnumerator PlayRandomSoundWithDelay(SoundSO[] sounds, bool isMusic = false, float delay = 0f) {
        yield return new WaitForSecondsRealtime(delay);
        PlayRandomSound(sounds, isMusic);
    }

    private void PlaySound(SoundSO soundSO) {
        if (_audioSourcePool.Count > 0) {
            AudioSource audioSource = _audioSourcePool.Dequeue();
            ConfigureAndPlaySound(audioSource, soundSO);
        } else {
            Debug.LogWarning("All AudioSources are in use!");
        }
    }

    private void PlaySoundWithReservedAudioSource(SoundSO[] sounds, AudioSource audioSource) {
        if (audioSource != null && !audioSource.isPlaying) {
            PlayRandomSound(sounds, false);
        }
    }

    private void PlayMusic(SoundSO soundSO) {
        if (_currentMusic != null) {
            _currentMusic.Stop();
            ReturnToPool(_currentMusic);
        }

        if (_audioSourcePool.Count > 0) {
            AudioSource audioSource = _audioSourcePool.Dequeue();
            _currentMusic = audioSource;
            ConfigureAndPlaySound(audioSource, soundSO);
        } else {
            Debug.LogWarning("All AudioSources are in use!");
        }
    }

    private void ConfigureAndPlaySound(AudioSource audioSource, SoundSO soundSO) {
        audioSource.clip = soundSO.Clip;
        audioSource.pitch = RandomizePitch(soundSO);
        audioSource.volume = soundSO.Volume * _masterVolume;
        audioSource.loop = soundSO.Loop;
        audioSource.outputAudioMixerGroup = DetermineAudioMixerGroup(soundSO);
        audioSource.Play();

        _activeAudioSources.Add(audioSource);

        if (!soundSO.Loop) {
            StartCoroutine(ReturnToPool(audioSource, soundSO.Clip.length));
        }
    }

    private AudioMixerGroup DetermineAudioMixerGroup(SoundSO soundSO) {
        return soundSO.AudioType switch {
            SoundSO.AudioTypes.Voice => _voiceMixerGroup,
            SoundSO.AudioTypes.SFX => _sfxMixerGroup,
            SoundSO.AudioTypes.Music => _musicMixerGroup,
            _ => null,
        };
    }

    private float RandomizePitch(SoundSO soundSO) {
        float pitch = soundSO.Pitch;
        if (soundSO.RandomizePitch) {
            float randomPitchModifier = Random.Range(-soundSO.RandomPitchRangeModifier, soundSO.RandomPitchRangeModifier);
            pitch = soundSO.Pitch + randomPitchModifier;
        }
        return pitch;
    }

    private IEnumerator ReturnToPool(AudioSource audioSource, float delay) {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
        _activeAudioSources.Remove(audioSource);
        _audioSourcePool.Enqueue(audioSource);
    }

    private void ReturnToPool(AudioSource audioSource) {
        audioSource.Stop();
        _activeAudioSources.Remove(audioSource);
        _audioSourcePool.Enqueue(audioSource);
    }

    #endregion

    private void WinScreenUI_OnTrophyDisplayed() {
        NPC.NPCType[] npcTypes;

        if (TutorialManager.Instance != null) {
            npcTypes = new NPC.NPCType[] { TutorialManager.Instance.GetTutorialNPC.GetNPCType };
        } else {
            npcTypes = GameManager.Instance.GetStartingNPCTypes;
        }

        foreach (var npcType in npcTypes) {
            var delay = UnityEngine.Random.Range(0f, _winVoiceMaxDelay);
            StartCoroutine(PlayRandomSoundWithDelay(_npcsVoicesDict[npcType].Congrats, false, delay));
        }
    }

    public void ItaSpeech() {
        PlayRandomSound(_npcsVoicesDict[NPC.NPCType.Italian].Speech); 
    }

    public void GerSpeech() {
        PlayRandomSound(_npcsVoicesDict[NPC.NPCType.German].Speech); 
    }

    public void SpaSpeech() {
        PlayRandomSound(_npcsVoicesDict[NPC.NPCType.Spaniard].Speech); 
    }

    public void UsaSpeech() {
        PlayRandomSound(_npcsVoicesDict[NPC.NPCType.American].Speech); 
    }
}
