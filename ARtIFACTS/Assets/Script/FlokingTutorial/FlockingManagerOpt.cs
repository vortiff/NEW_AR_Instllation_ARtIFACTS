using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManagerOpt : MonoBehaviour
{
    public static FlockingManagerOpt FMOpt;
    public GameObject elementPrefab;
    public int numElement = 20;
    public List<GameObject> allElement = new List<GameObject>();
    public Vector3 flyLimit = new Vector3(5, 5, 5);
    public Vector3 goalPos = Vector3.zero;

    [Header ("Element Setting")]
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

    [Header("Audio Settings")]
    public MediaLibrary mediaLibrary;
    public AudioSource audioSource3D; // Variabile pubblica per l'AudioSource 3D
    private bool isPlayerInside = false; // Variabile per tracciare lo stato del player

    private AudioSource audioSource;
    private float fadeDuration = 1.0f; // Durata del fade in/out. Puoi modificarla come preferisci.


    private Flocking[] flockingScripts; // Cached Flocking scripts

    private void Start()
    {
        // Inizializza l'AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Nessun componente AudioSource trovato! Aggiungilo al GameObject.");
        }

        for (int i = 0; i < numElement; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-flyLimit.x, flyLimit.x),
                                            Random.Range(-flyLimit.y, flyLimit.y),
                                            Random.Range(-flyLimit.z, flyLimit.z));
            Vector3 pos = this.transform.position + randomPos;
            GameObject clone = Instantiate(elementPrefab, pos, Quaternion.identity, this.transform);
            allElement.Add(clone);
        }

        FMOpt = this;
        goalPos = this.transform.position;

        // Cache the Flocking components {array per memorizzare tutti gli script FlockingOPT all'avvio per non chiamare GetComponent ad ogni frame}
        flockingScripts = new Flocking[numElement];
        for (int i = 0; i < allElement.Count; i++)
        {
            flockingScripts[i] = allElement[i].GetComponent<Flocking>();
        }
    }

    private void Update()
    {
        // Aggiornare la posizione dell'AudioSource per seguirlo
        if (audioSource3D != null)
        {
            audioSource3D.transform.position = goalPos;
        }

        if (Random.Range(0, 100) < 3)
        {
            SetRandomGoalPosition();
        }

        float distanceToARCamera = Vector3.Distance(transform.position, arCamera.transform.position);
        float normalizedDistance = Mathf.Clamp01(distanceToARCamera / maxDistanceForSpeedChange);
        float newRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, 1f - normalizedDistance);

        for (int i = 0; i < flockingScripts.Length; i++)
        {
            if (flockingScripts[i] != null)
            {
                flockingScripts[i].Speed = newRotationSpeed;
            }
        }
        ChooseSoundBasedFromLibrary(isPlayerInside);

    }

    private void SetRandomGoalPosition()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-flyLimit.x / 2, flyLimit.x / 2),
            Random.Range(-flyLimit.y / 2, flyLimit.y / 2),
            Random.Range(-flyLimit.z / 2, flyLimit.z / 2)
        );
        goalPos = this.transform.position + randomPos;
    }

    // Funzione per scegliere un suono in modo casuale dall'indice 
    void ChooseSoundBasedFromLibrary(bool isPlayerInside)
    {
        // Se l'audioSource sta già riproducendo un suono, esce dalla funzione
        if (audioSource3D.isPlaying)
        {
            return;
        }

        int randomIndex;
        if (isPlayerInside)
        {
            randomIndex = Random.Range(4, 6); // Suoni per quando il player è dentro il collider
        }
        else
        {
            randomIndex = Random.Range(0, 4); // Suoni per quando il player è fuori dal collider
        }

        if (randomIndex < mediaLibrary.audioClips.Length)
        {
            audioSource3D.clip = mediaLibrary.audioClips[randomIndex];
            audioSource3D.Play();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto che entra nel collider è il player
        if (other.gameObject == arCamera)
        {
            isPlayerInside = true;
            StopSoundWithFadeOut(); // Smette di riprodurre il suono quando il player entra nel collider
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Controlla se l'oggetto che esce dal collider è il player
        if (other.gameObject == arCamera)
        {
            isPlayerInside = false;
            PlaySoundWithFadeIn(); // Inizia a riprodurre il suono quando il player esce dal collider
        }
    }

void ResetVolume()
{
    audioSource.volume = 1f;
}


   void PlaySoundWithFadeIn()
    {
        ChooseSoundBasedFromLibrary(false); // Passa false poiché il player è fuori dal collider
        audioSource.volume = 0.0f; // Imposta il volume iniziale a 0
        audioSource.Play(); // Inizia la riproduzione
        StopAllCoroutines(); // Ferma tutte le coroutine
        StartCoroutine(FadeIn(audioSource, fadeDuration));
    }

    void PlaySoundForPlayerInside()
    {
        ChooseSoundBasedFromLibrary(true); // Passa true poiché il player è dentro il collider
        audioSource3D.volume = 0.0f; // Imposta il volume iniziale a 0
        audioSource3D.Play(); // Inizia la riproduzione
        StopAllCoroutines(); // Ferma tutte le coroutine
        StartCoroutine(FadeIn(audioSource3D, fadeDuration));
    }

    void StopSoundWithFadeOut()
    {
        StopAllCoroutines(); // Ferma tutte le coroutine
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
        // audioSource.volume = startVolume; // Commenta o rimuovi questa linea
    }


}
