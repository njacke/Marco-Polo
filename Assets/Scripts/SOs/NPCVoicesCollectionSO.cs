using UnityEngine;

[CreateAssetMenu(menuName = "Data/NPC Voices Collection")]
public class NPCVoicesCollection : ScriptableObject
{    
    [Header("Voice")]
    public SoundSO[] ScanResponse;
    public SoundSO[] CheatDetected;
    public SoundSO[] Caught;
    public SoundSO[] Random;
    public SoundSO[] CorrectGuess;
    public SoundSO[] WrongGuess;
    public SoundSO[] Congrats;
    public SoundSO[] Speech;
}