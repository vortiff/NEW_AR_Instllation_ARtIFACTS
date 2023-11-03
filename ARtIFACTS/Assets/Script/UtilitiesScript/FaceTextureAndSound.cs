using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARFaceManager))]
public class FaceTextureAndSound : MonoBehaviour
{
    public AudioClip[] audioClips; // Assign this array in the inspector with your audio clips
    private ARFaceManager arFaceManager;
    private Texture2D faceTexture; // This will be set by the TakeSelfieButton script
    
    void Awake()
    {
        arFaceManager = GetComponent<ARFaceManager>();
    }

    public void SetFaceTexture(Texture2D newTexture)
    {
        faceTexture = newTexture;
        foreach (ARFace face in arFaceManager.trackables)
        {
            face.GetComponent<MeshRenderer>().material.mainTexture = faceTexture;
        }
    }

    // Call this method to play a random sound from the library
    public void PlayRandomSound(ARFace face)
    {
        if (audioClips.Length > 0)
        {
            // Find the AudioSource component on the face prefab
            AudioSource faceAudioSource = face.GetComponent<AudioSource>();
            if (faceAudioSource != null)
            {
                AudioClip clip = audioClips[Random.Range(0, audioClips.Length)];
                faceAudioSource.clip = clip;
                faceAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("AudioSource component not found on the face prefab!");
            }
        }
    }

    void OnEnable()
    {
        arFaceManager.facesChanged += FacesChanged;
    }

    void OnDisable()
    {
        arFaceManager.facesChanged -= FacesChanged;
    }

    void FacesChanged(ARFacesChangedEventArgs eventArgs)
    {
        if (faceTexture == null)
            return;

        foreach (ARFace face in eventArgs.added)
        {
            face.GetComponent<MeshRenderer>().material.mainTexture = faceTexture;
            PlayRandomSound(face); // Pass the ARFace to the method
        }

        foreach (ARFace face in eventArgs.updated)
        {
            if (face.GetComponent<MeshRenderer>().material.mainTexture != faceTexture)
            {
                face.GetComponent<MeshRenderer>().material.mainTexture = faceTexture;
                PlayRandomSound(face); // Pass the ARFace to the method
            }
        }
    }
}
