using UnityEngine;

[CreateAssetMenu(menuName = "Data/Sounds Collection")]
public class SoundsCollectionSO : ScriptableObject
{    
    [Header("Music")]
    public SoundSO[] MainMenuMusic;
    public SoundSO[] EndGameMusic;
    public SoundSO[] LobbyMusic;
    public SoundSO[] PoolMusic;
    public SoundSO[] GardenMusic;
    public SoundSO[] TerraceMusic;


    [Header("SFX")]
    public SoundSO[] MouseClick;
    public SoundSO[] Beep;
    public SoundSO[] MenuSlide;
    public SoundSO[] ButtonHighlight;
    public SoundSO[] ButtonSelect;
    public SoundSO[] CatchSkill;
    public SoundSO[] CheatSkill;
    public SoundSO[] Duck;
    public SoundSO[] Tube;
    public SoundSO[] Fountain;
    public SoundSO[] Table;
    public SoundSO[] Piano;
    public SoundSO[] Trolley;
}