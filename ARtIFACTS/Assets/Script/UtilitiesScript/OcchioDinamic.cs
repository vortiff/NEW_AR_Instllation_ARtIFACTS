using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class OcchioDinamic : MonoBehaviour
{
    public GameObject player; // Il GameObject del giocatore
    public GameObject[] eyeSpheres; // Array delle sfere occhi
    public AudioClip[] audioClips; // Array dei file audio
    public VideoClip[] videoClips; // Array di VideoClip
    private AudioSource audioSource;
    public int numberOfClones; // Numero di cloni da creare
    public float cloneSpreadRadius; // Distanza massima dalla posizione originale per posizionare un clone
    public float minCloneSpeed; // Velocità minima iniziale di un clone
    public float maxCloneSpeed; // Velocità massima iniziale di un clone
    public float someThreshold = 5f; // Distanza alla quale l'occhio inizierà a guardare il giocatore e connettersi con un fascio luminoso
    public float someOtherThreshold = 2f; // Distanza alla quale le sfere occhi collidono tra loro
    public float gravitationalStrength = 10f; // Forza dell'attrazione gravitazionale
    public float wobbleStrength = 5f; // Intensità dell'ondulazione
    public float wobbleSpeed = 2f; // Velocità dell'ondulazione
    private VideoPlayer videoPlayer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Inizializza il VideoPlayer
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }
        
        // Assegna un video casuale dall'array
        AssignRandomVideo();
        
        // Clonazione degli occhi
        for (int i = 0; i < numberOfClones; i++)
        {
            GameObject eye = eyeSpheres[Random.Range(0, eyeSpheres.Length)]; // Sceglie un occhio a caso da clonare
            Vector3 randomPosition = new Vector3(
                transform.position.x + Random.Range(-cloneSpreadRadius, cloneSpreadRadius),
                transform.position.y,
                transform.position.z + Random.Range(-cloneSpreadRadius, cloneSpreadRadius)
            );

            GameObject clone = Instantiate(eye, randomPosition, Quaternion.identity);
            Rigidbody rb = clone.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = clone.AddComponent<Rigidbody>();
            }
            rb.velocity = new Vector3(Random.Range(minCloneSpeed, maxCloneSpeed), 0, Random.Range(minCloneSpeed, maxCloneSpeed));
        }
    }

    // Assegna un video casuale dall'array videoClips
    private void AssignRandomVideo()
    {
        if (videoClips.Length > 0)
        {
            VideoClip randomClip = videoClips[Random.Range(0, videoClips.Length)];
            videoPlayer.clip = randomClip;
            videoPlayer.isLooping = true; // Imposta la riproduzione in loop
            videoPlayer.Play();
        }
    }

    private void Update()
    {
        audioSource = GetComponent<AudioSource>();

        foreach (GameObject eye in eyeSpheres)
        {
            MoveTowardsPlayer(eye); // Muove l'occhio verso il giocatore con forza di attrazione e repulsione

            float distanceToPlayer = Vector3.Distance(eye.transform.position, player.transform.position);
            
            // Quando il giocatore è abbastanza vicino a un occhio
            if (distanceToPlayer < someThreshold)
            {
                eye.transform.LookAt(player.transform.position); // L'occhio si orienta verso il giocatore
                CreateLightBeam(eye.transform.position, player.transform.position); // Crea fascio luminoso

                // Riproduci un file audio casuale tra 0-9
                if (!audioSource.isPlaying)
                {
                    int clipIndex = Random.Range(0, 9);
                    audioSource.clip = audioClips[clipIndex];
                    audioSource.Play();
                }
            }
        }

        // Controlla la collisione tra le sfere
        for (int i = 0; i < eyeSpheres.Length - 1; i++)
        {
            for (int j = i + 1; j < eyeSpheres.Length; j++)
            {
                if (Vector3.Distance(eyeSpheres[i].transform.position, eyeSpheres[j].transform.position) < someOtherThreshold)
                {
                    CreateLightBeam(eyeSpheres[i].transform.position, eyeSpheres[j].transform.position); // Crea fascio luminoso

                    // Riproduci un file audio casuale tra 10-15
                    if (!audioSource.isPlaying)
                    {
                        int clipIndex = Random.Range(10, 15);
                        audioSource.clip = audioClips[clipIndex];
                        audioSource.Play();
                    }
                }
            }
        }
    }

    void CreateLightBeam(Vector3 start, Vector3 end)
    {
        // Qui va il codice per creare il fascio luminoso tra il punto di inizio e il punto di fine
    }

    void MoveTowardsPlayer(GameObject eye)
    {
        Vector3 directionToPlayer = (player.transform.position - eye.transform.position).normalized;
        Vector3 wobble = new Vector3(
            Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 1f) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 2f) * wobbleStrength
        );

        Rigidbody rb = eye.GetComponent<Rigidbody>();
        if (rb == null) rb = eye.AddComponent<Rigidbody>();
        
        rb.AddForce(directionToPlayer * gravitationalStrength + wobble);
    }
}
