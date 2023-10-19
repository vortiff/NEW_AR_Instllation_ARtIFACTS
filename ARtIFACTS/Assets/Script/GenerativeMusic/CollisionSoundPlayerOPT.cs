using UnityEngine;

public class CollisionSoundPlayerOPT : MonoBehaviour
{
    public MediaLibrary mediaLibrary;

    private AudioSource audioSource;
    public float minAudioDistance = 1f;
    public float maxAudioDistance = 10f;


    private void Start()
    {
        // Assicurati di avere un componente AudioSource su questo GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Se non c'è un componente AudioSource, aggiungilo
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = minAudioDistance;
            audioSource.maxDistance = maxAudioDistance;
        }
    }

    // Metodo chiamato quando il GameObject entra in collisione con un altro
    private void OnCollisionEnter(Collision collision)
    {
        // Riproduci un suono casuale tra quelli nella libreria
        if(audioSource != null && mediaLibrary.audioClips.Length > 0 && !audioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, mediaLibrary.audioClips.Length);
            audioSource.clip = mediaLibrary.audioClips[randomIndex];
            audioSource.Play();
        }
    }
}
