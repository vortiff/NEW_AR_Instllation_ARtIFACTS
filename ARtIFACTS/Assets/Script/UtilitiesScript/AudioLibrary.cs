using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Libraries/AudioLibrary", order = 1)]
public class AudioLibrary : ScriptableObject
{
    public AudioClip[] audioClips;  // Array di clip audio
}
