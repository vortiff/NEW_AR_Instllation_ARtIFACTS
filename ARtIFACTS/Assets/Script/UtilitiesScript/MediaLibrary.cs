using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "MediaLibrary", menuName = "Libraries/MediaLibrary", order = 1)]
public class MediaLibrary : ScriptableObject
{
    public AudioClip[] audioClips;
    public VideoClip[] videoClips;
}
