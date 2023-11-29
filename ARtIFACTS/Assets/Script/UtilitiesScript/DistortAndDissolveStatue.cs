using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DistortAndDissolveStatue : MonoBehaviour
{
    public float expansionSpeed = 1f;
    public float maxRadius = 0.1f;
    public float glitchFrequencyOutsideCollider = 7f;
    public float glitchFrequencyInsideCollider = 3f;
    public float deformationIntensityOutsideCollider = 0.7f;
    public float deformationIntensityInsideCollider = 1.9f;
    public MediaLibrary mediaLibrary;
    private AudioSource audioSource;
    public float minAudioDistance = 1f;
    public float maxAudioDistance = 10f;
    public float selfRotationSpeed = 50f; // Velocità di rotazione su se stesso,

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private float nextGlitchTime;
    private bool inContactWithPlayer = false;
    public Transform playerTransform;
    private Vector3 initialRotation;
    private MeshFilter meshFilter;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        meshFilter = GetComponent<MeshFilter>();

        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;
        nextGlitchTime = Time.time + Random.Range(0f, glitchFrequencyOutsideCollider);

        if (audioSource)
        {
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = minAudioDistance;
            audioSource.maxDistance = maxAudioDistance;
        }
    }

    void Reset()
    {
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = minAudioDistance;
            audioSource.maxDistance = maxAudioDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    void Update()
    {
        if (transform.localScale.x < maxRadius)
        {
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }

        if (Time.time > nextGlitchTime)
        {
            DeformMesh();
            nextGlitchTime = inContactWithPlayer ? Time.time + Random.Range(0f, glitchFrequencyInsideCollider) 
                                                 : Time.time + Random.Range(0f, glitchFrequencyOutsideCollider);
        }

        FollowPlayer();
    }

    void DeformMesh()
    {
        Vector3[] vertices = originalMesh.vertices;
        float currentIntensity = inContactWithPlayer ? deformationIntensityInsideCollider : deformationIntensityOutsideCollider;

        for (int i = 0; i < vertices.Length; i++)
        {
            float randomValueX = Mathf.PerlinNoise(vertices[i].x * currentIntensity, Time.time) * currentIntensity;
            float randomValueY = Mathf.PerlinNoise(vertices[i].y * currentIntensity, Time.time) * currentIntensity;
            float randomValueZ = Mathf.PerlinNoise(vertices[i].z * currentIntensity, Time.time) * currentIntensity;

            vertices[i] = originalVertices[i] + new Vector3(randomValueX, randomValueY, randomValueZ);
        }

        originalMesh.vertices = vertices;
        originalMesh.RecalculateNormals();
    }

    void PlayRandomAudio()
    {
        if(audioSource != null && mediaLibrary.audioClips.Length > 0 && !audioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, mediaLibrary.audioClips.Length);
            audioSource.clip = mediaLibrary.audioClips[randomIndex];
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inContactWithPlayer = true;
            PlayRandomAudio();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inContactWithPlayer = false;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();  // Fermiamo il suono.
            }
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && inContactWithPlayer)
        {
            PlayRandomAudio();
        }
    }

    void FollowPlayer()
    {
        if (playerTransform) // controlla se playerTransform è stato impostato
        {
            // Calcola la direzione al giocatore a livello del suolo, ignorando l'altezza
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0; // Ignora l'altezza per non influenzare la rotazione sull'asse Y

            // Calcola l'angolo tra la direzione attuale del GameObject e la direzione verso il giocatore
            float angleToPlayer = Vector3.SignedAngle(-transform.forward, directionToPlayer, Vector3.up);

            // Aggiusta la rotazione locale sull'asse Y per ruotare verso il giocatore
            transform.Rotate(Vector3.up, angleToPlayer * Time.deltaTime * selfRotationSpeed);

        }
    }
}

