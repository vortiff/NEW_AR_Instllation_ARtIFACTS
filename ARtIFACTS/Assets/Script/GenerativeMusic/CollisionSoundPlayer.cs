using UnityEngine;

public class CollisionSoundPlayer : MonoBehaviour
{
    public AudioClip[] collisionSounds; // Array di suoni da riprodurre quando c'è una collisione

    private AudioSource audioSource;

    private void Start()
    {
        // Assicurati di avere un componente AudioSource su questo GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Se non c'è un componente AudioSource, aggiungilo
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Metodo chiamato quando il GameObject entra in collisione con un altro
    private void OnCollisionEnter(Collision collision)
    {
        // Riproduci un suono casuale tra quelli nell'array
        if (collisionSounds.Length > 0)
        {
            int randomSoundIndex = Random.Range(0, collisionSounds.Length);
            audioSource.PlayOneShot(collisionSounds[randomSoundIndex]);
        }
    }
}
