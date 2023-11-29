using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ColliderManagerStartClone : MonoBehaviour
{
    public enum PartOfDay
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    public enum TemperatureRange
    {
        Cold,
        Mild,
        Hot
    }

    [Header("Weather Data Manager Reference")]
    public WeatherDataManager weatherDataManager; // Reference to the WeatherDataManager script

    [Header("Collider Activation")]
    public float colliderActivationDelay = 1.0f; // Ritardo prima dell'attivazione del collider

    [Header("GameObject References")]
    [SerializeField] private GameObject playerObject;
    public GameObject metaballObject; 
    public GameObject objectToClone; 
    public GameObject GameObjectToActivate;
    public GameObject attractionTarget;
    private bool hasCollided = false;
   
    [Header("Audio Settings")]
    public int audioClipIndex = 0; // L'indice dell'audio clip da riprodurre
    private int audioClipWeather; // Audio clip in base a Temperatura e Orario
    private bool hasPlayedAudioClipIndex = false; // Flag per vedere se audioClipIndex è stata riprodotta
    private AudioSource audioSource;
    private AudioClip[] metaballSounds; 
    [SerializeField] private AudioSource signifierAudioSource; // Riferimento all'AudioSource specifico da controllare


    [Header("Clone Settings")]
    [SerializeField]
    private int numberOfClones = 30; // Modifica il numero di cloni come preferisci
    [SerializeField]
    private float cloneSpreadRadius = 2f; // Modifica il raggio in cui i cloni vengono creati
    [SerializeField]
    private float minCloneSpeed = 1f; // Modifica la velocità minima dei cloni
    [SerializeField]
    private float maxCloneSpeed = 5f; // Modifica la velocità massima dei cloni
    private Rigidbody[] cloneRigidbodies; // Array per memorizzare i riferimenti ai Rigidbody dei cloni
    Vector3[] randomForces; // Array per memorizzare le forze randomiche
    public float gravitationalStrength;
    public float maxGravitationalStrength;
    public float minGravitationalStrength;
    public float wobbleStrength;
    public float wobbleSpeed;
    public float maxWobbleStrength;
    public float minWobbleStrength;
    public float maxWobbleSpeed;
    public float minWobbleSpeed;

    private void Start()
    {
        // Disattiva il Collider all'avvio
        GetComponent<Collider>().enabled = false;
        StartCoroutine(ActivateColliderAfterDelay());

        cloneRigidbodies = new Rigidbody[numberOfClones];
        randomForces = new Vector3[numberOfClones];

        for (int i = 0; i < numberOfClones; i++)
        {
            randomForces[i] = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        );
        }
        // Ottieni il componente AudioSource dal GameObject Metaball
        audioSource = metaballObject.GetComponent<AudioSource>();

        // Ottieni l'array di clip audio dal metaballObject
        metaballSounds = metaballObject.GetComponent<Metaball>().voiceOverSounds;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il giocatore è entrato in collisione e se non ha già interagito
        if (other.gameObject == playerObject && !hasCollided)
        {
            GameObjectToActivate.SetActive(true);
            // Controlla se l'AudioSource specificato è in riproduzione e interrompilo
            if (signifierAudioSource != null && signifierAudioSource.isPlaying)
            {
                signifierAudioSource.Stop();
            }
    
            // Riproduci il suono VoiceOver dalla lista del GameObject Metaball
            if (audioSource != null && metaballSounds.Length > 0 && !hasPlayedAudioClipIndex)
            {
                if (audioClipIndex < metaballSounds.Length)
                {
                    audioSource.clip = metaballSounds[audioClipIndex];
                    audioSource.Play();
                    hasPlayedAudioClipIndex = true;
                    StartCoroutine(PlayWeatherAudioAfterDelay(audioSource.clip.length + 1));
                }
            }
           
            WeatherDataManager.WeatherInfo currentWeather = weatherDataManager.GetCurrentWeatherInfo(); // Modifica qui
            float currentTemperature = currentWeather.main.temp - 273.15f; // Converti da Kelvin a Celsius

            DetermineSoundToPlay(currentTemperature);


            // Attiva il componente FollowPlayer sul GameObject Metaball, se presente
            Metaball followPlayerComponent = metaballObject.GetComponent<Metaball>();
            if (followPlayerComponent != null)
            {
                followPlayerComponent.enabled = true;
            }

            // Chiama il metodo CloneObject per duplicare gli oggetti
            CloneObject();

            // Imposta hasCollided su true per evitare collisioni multiple
            hasCollided = true;
        }
    }

    private void CloneObject()
    {
        for (int i = 0; i < numberOfClones; i++)
        {
            Vector3 randomPosition = GetRandomPositionWithinRadius();
            GameObject newClone = Instantiate(objectToClone, randomPosition, Quaternion.identity, transform);
            
            // Ottieni il riferimento al Rigidbody del clone e lo salva nell'array
            Rigidbody cloneRb = newClone.GetComponent<Rigidbody>();
            cloneRigidbodies[i] = cloneRb;

            // Assegna i valori random ai cloni
            cloneRb.velocity = UnityEngine.Random.Range(minCloneSpeed, maxCloneSpeed) * Vector3.up;
            gravitationalStrength = UnityEngine.Random.Range(minGravitationalStrength, maxGravitationalStrength);
            wobbleStrength = UnityEngine.Random.Range(minWobbleStrength, maxWobbleStrength);
            wobbleSpeed = UnityEngine.Random.Range(minWobbleSpeed, maxWobbleSpeed);
        }
    }


    void Update()
    {
        // Se abbiamo un target di attrazione definito, attrai i cloni verso di esso
        if (attractionTarget != null)
        {
            foreach (Transform child in transform)
            {
                MoveTowardsTarget(child.gameObject, attractionTarget);
            }
        }
    }

    void MoveTowardsTarget(GameObject clone, GameObject target)
    {
        // Calcola la direzione verso il target
        Vector3 directionToTarget = (target.transform.position - clone.transform.position).normalized;

        // Calcola un movimento ondulatorio
        Vector3 wobble = new Vector3(
            Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 1f) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 2f) * wobbleStrength
        );

        // Calcola una forza randomica
        int cloneIndex = System.Array.IndexOf(cloneRigidbodies, clone.GetComponent<Rigidbody>());
        Vector3 randomForce = randomForces[cloneIndex];


        Vector3 forceToApply = directionToTarget * gravitationalStrength + wobble + randomForce;

        // Applica tutte le forze al clone
         Rigidbody rb = cloneRigidbodies[cloneIndex];
        if(rb != null)
        {
            rb.AddForce(forceToApply);
        }
    }

    private Vector3 GetRandomPositionWithinRadius()
    {
        return transform.position + UnityEngine.Random.insideUnitSphere * cloneSpreadRadius;
    }

    private IEnumerator ActivateColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderActivationDelay);
        GetComponent<Collider>().enabled = true;
    }

    private TemperatureRange GetTemperatureRange(float temperature)
    {
        if (temperature < 10) return TemperatureRange.Cold;
        else if (temperature >= 10 && temperature < 20) return TemperatureRange.Mild;
        else return TemperatureRange.Hot;
    }

    private PartOfDay GetCurrentPartOfDay()
    {
        DateTime currentTime = DateTime.Now;
        int hour = currentTime.Hour;

        if (hour >= 6 && hour < 12) return PartOfDay.Morning;
        else if (hour >= 12 && hour < 18) return PartOfDay.Afternoon;
        else if (hour >= 18 && hour < 22) return PartOfDay.Evening;
        else return PartOfDay.Night;
    }


    private void DetermineSoundToPlay(float temperature)
    {
        TemperatureRange tempRange = GetTemperatureRange(temperature);
        PartOfDay partOfDay = GetCurrentPartOfDay();

        switch (partOfDay)
        {
            case PartOfDay.Morning:
                switch (tempRange)
                {
                    case TemperatureRange.Cold:
                        audioClipWeather = 1;
                        break;
                    case TemperatureRange.Mild:
                        audioClipWeather = 2;
                        break;
                    case TemperatureRange.Hot:
                        audioClipWeather = 3;
                        break;
                }
                break;

            case PartOfDay.Afternoon:
                switch (tempRange)
                {
                    case TemperatureRange.Cold:
                        audioClipWeather = 4;
                        break;
                    case TemperatureRange.Mild:
                        audioClipWeather = 5;
                        break;
                    case TemperatureRange.Hot:
                        audioClipWeather = 6;
                        break;
                }
                break;

            case PartOfDay.Evening:
                switch (tempRange)
                {
                    case TemperatureRange.Cold:
                        audioClipWeather = 7;
                        break;
                    case TemperatureRange.Mild:
                        audioClipWeather = 8;
                        break;
                    case TemperatureRange.Hot:
                        audioClipWeather = 9;
                        break;
                }
                break;

            case PartOfDay.Night:
                switch (tempRange)
                {
                    case TemperatureRange.Cold:
                        audioClipWeather = 10;
                        break;
                    case TemperatureRange.Mild:
                        audioClipWeather = 11;
                        break;
                    case TemperatureRange.Hot:
                        audioClipWeather = 12;
                        break;
                }
                break;
        }
    }



    [System.Serializable]
    public class WeatherInfo
    {
        public MainInfo main;
    }

    [System.Serializable]
    public class MainInfo
    {
        public float temp;
    }

    private IEnumerator PlayWeatherAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (audioClipWeather < metaballSounds.Length)
        {
            audioSource.clip = metaballSounds[audioClipWeather];
            audioSource.Play();
        }
        //Debug.Log($"Riproduzione di AudioSource: {audioSource.clip.name}");
    }


}
