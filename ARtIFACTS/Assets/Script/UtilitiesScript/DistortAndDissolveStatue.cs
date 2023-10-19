using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DistortAndDissolveStatue : MonoBehaviour
{
    public float expansionSpeed = 0.1f;
    public float maxRadius = 5f;
    public float deformationIntensity = 0.1f;
    public float glitchFrequencyOutsideCollider = 5.0f;
    public float glitchFrequencyInsideCollider = 1.0f;
   
    public AudioLibraryStatue audioLibrary;
    private AudioSource audioSource; // Attach the AudioSource component
    public float minAudioDistance = 1f; // New public variable
    public float maxAudioDistance = 10f; // New public variable

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private float nextGlitchTime;
    private bool inContactWithPlayer = false;


    void Start()
    {
         // Try to get the AudioSource component from this GameObject
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;
        nextGlitchTime = Time.time + Random.Range(0f, glitchFrequencyOutsideCollider);

        // Set AudioSource properties
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
            if(inContactWithPlayer)
                nextGlitchTime = Time.time + Random.Range(0f, glitchFrequencyInsideCollider);
            else
                nextGlitchTime = Time.time + Random.Range(0f, glitchFrequencyOutsideCollider);
        }
    }

    void DeformMesh()
    {
        Vector3[] vertices = originalMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float intensity = inContactWithPlayer ? deformationIntensity * 2 : deformationIntensity;
            float randomValueX = Mathf.PerlinNoise(vertices[i].x * intensity, Time.time) * intensity;
            float randomValueY = Mathf.PerlinNoise(vertices[i].y * intensity, Time.time) * intensity;
            float randomValueZ = Mathf.PerlinNoise(vertices[i].z * intensity, Time.time) * intensity;

            vertices[i] = originalVertices[i] + new Vector3(randomValueX, randomValueY, randomValueZ);
        }

        originalMesh.vertices = vertices;
        originalMesh.RecalculateNormals();

        // Riproduci il suono solo se non è già in riproduzione
        if(inContactWithPlayer && !audioSource.isPlaying)
        {
            PlayRandomAudio();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) // Assuming the player has a tag "Player"
        {
            Debug.Log("Il giocatore è nella statua");
            inContactWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inContactWithPlayer = false;
        }
    }

   void PlayRandomAudio()
    {
        if(audioSource != null && audioLibrary.audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioLibrary.audioClips.Length);
            audioSource.clip = audioLibrary.audioClips[randomIndex];
            Debug.Log("Sto cercando di riprodurre un suono. AudioClip selezionato: " + audioSource.clip.name);
            audioSource.Play();
            
        }
    }
}
