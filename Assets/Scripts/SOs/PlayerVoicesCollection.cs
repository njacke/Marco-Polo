using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player Voices Collection")]
public class PlayerVoicesCollection : ScriptableObject
{    
    [Header("Voice")]
    public SoundSO[] Ready;
    public SoundSO[] Countdown;
    public SoundSO[] Catch;
    public SoundSO[] Scan;
    public SoundSO[] Cheat;
    public SoundSO[] GameOver;
}