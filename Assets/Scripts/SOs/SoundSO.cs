using UnityEngine;

[CreateAssetMenu(menuName = "Data/Sound")]
public class SoundSO : ScriptableObject
{
    public enum AudioTypes {
        Music,
        SFX,
        Voice
    }

    public AudioTypes AudioType;
    public AudioClip Clip;
    public bool Loop = false;
    public bool RandomizePitch = false;
    [Range(0f, 1f)]
    public float RandomPitchRangeModifier = .1f;
    [Range(.1f, 4f)]
    public float Volume = 1f;
    [Range(.1f, 3f)]
    public float Pitch = 1f;
}