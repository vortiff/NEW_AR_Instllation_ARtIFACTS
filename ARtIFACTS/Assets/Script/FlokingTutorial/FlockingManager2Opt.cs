using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Aggiunto per il metodo Sum()

public class FlockingManager2Opt : MonoBehaviour
{
    public static FlockingManager2Opt FM2Opt;
    public GameObject[] groupPrefabs;   // Array di prefabbricati per ogni gruppo.
    public int[] numOfElementsPerGroup; // Numero di elementi per ogni gruppo.
    public float groupSeparationDistance; // Distanza tra i gruppi all'avvio.
    private List<GameObject>[] groupElements;

    public List<GameObject> allElement = new List<GameObject>(); // Array di liste per contenere gli elementi di ogni gruppo.
    public Vector3 flyLimit = new Vector3(5, 5, 5);
    public Vector3 goalPos = Vector3.zero;

    [Header("Element Setting")]
    [Range(0.0f, 10.0f)]
    public float minSpeed;
    [Range(0.0f, 50.0f)]
    public float maxSpeed;
    [Range(0.0f, 10.0f)]
    public float neighbourDistance;
    [Range(1.0f, 10.0f)]
    public float rotationSpeed;

    public GameObject arCamera;
    public float maxDistanceForSpeedChange = 3f;
    public float minRotationSpeed = 5f;
    public float maxRotationSpeed = 10f;

    public MediaLibrary mediaLibrary;
    private AudioSource audioSource;
    private float fadeDuration = 1.0f;  // Durata del fade in/out

    private Flocking[] flockingScripts; // Cached Flocking scripts

    private void Start()
    {
        groupElements = new List<GameObject>[groupPrefabs.Length];

        for (int i = 0; i < groupPrefabs.Length; i++)
        {
            groupElements[i] = new List<GameObject>();
            for (int j = 0; j < numOfElementsPerGroup[i]; j++)
            {
                Vector3 spawnPosition = GetGroupSpawnPosition(i);
                GameObject element = Instantiate(groupPrefabs[i], spawnPosition, Quaternion.identity, this.transform);
                groupElements[i].Add(element);
                allElement.Add(element);
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Nessun componente AudioSource trovato! Aggiungilo al GameObject.");
        }

        FM2Opt = this;
        goalPos = this.transform.position;
       
        // Cache the Flocking components for the new group elements array per memorizzare tutti gli script FlockingOPT all'avvio per non chiamare GetComponent ad ogni frame}
        flockingScripts = new Flocking[allElement.Count];
        for (int i = 0; i < allElement.Count; i++)
        {
            flockingScripts[i] = allElement[i].GetComponent<Flocking>();
        }
    }

    private Vector3 GetGroupSpawnPosition(int groupIndex)
    {
        // Restituisci una posizione basata sull'indice del gruppo.
        // Ad esempio, se hai tre gruppi, potrebbero essere posizionati ai vertici di un triangolo.
        switch (groupIndex)
        {
            case 0:
                return transform.position + new Vector3(-groupSeparationDistance, 0, 0);
            case 1:
                return transform.position + new Vector3(groupSeparationDistance, 0, 0);
            case 2:
                return transform.position + new Vector3(0, 0, groupSeparationDistance);
            default:
                return transform.position;
        }
    }

    private void Update()
    {
        if (Random.Range(0, 100) < 3)
        {
            SetRandomGoalPosition();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, arCamera.transform.position);
        if (distanceToPlayer <= maxDistanceForSpeedChange)
        {
            goalPos = transform.position; // convergi tutti gli elementi al centro del manager
        }
        else
        {
            SetRandomGoalPosition();
        }

        float distanceToARCamera = Vector3.Distance(transform.position, arCamera.transform.position);
        ChooseSoundBasedFromLibrary();
        float normalizedDistance = Mathf.Clamp01(distanceToARCamera / maxDistanceForSpeedChange);
        float newRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, 1f - normalizedDistance);

        for (int i = 0; i < flockingScripts.Length; i++)
        {
            if (flockingScripts[i] != null)
            {
                flockingScripts[i].Speed = newRotationSpeed;
            }
        }
    }

    private void SetRandomGoalPosition()
    {
        Vector3 randomPos = new Vector3(Random.Range(-flyLimit.x, flyLimit.x),
                                        Random.Range(-flyLimit.y, flyLimit.y),
                                        Random.Range(-flyLimit.z, flyLimit.z));
        goalPos = this.transform.position + randomPos;
    }

    void ChooseSoundBasedFromLibrary()
    {
        if (audioSource.isPlaying)
        {
            return;
        }

        int randomIndex = Random.Range(0, mediaLibrary.audioClips.Length); // Modificato per selezionare correttamente

        if (audioSource != null && randomIndex < mediaLibrary.audioClips.Length)
        {
            audioSource.clip = mediaLibrary.audioClips[randomIndex];
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopSoundWithFadeOut();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlaySoundWithFadeIn();
        }
    }

    void ResetVolume()
    {
        audioSource.volume = 1f;
    }

    void PlaySoundWithFadeIn()
    {
        ChooseSoundBasedFromLibrary();
        audioSource.volume = 0.0f;
        audioSource.Play();
        StopAllCoroutines();
        StartCoroutine(FadeIn(audioSource, fadeDuration));
    }

    void StopSoundWithFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(audioSource, fadeDuration));
    }

    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.0f;
        audioSource.volume = startVolume;
        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += (1.0f - startVolume) * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
    }
}
