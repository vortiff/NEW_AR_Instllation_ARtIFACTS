using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "VideoLibrary", menuName = "Libraries/VideoLibrary", order = 2)]
public class VideoLibrary : ScriptableObject
{
    public VideoClip[] videoClips;  // Array di clip video
}
