    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;


    public class ColliderManager : MonoBehaviour
    {
        [Header("Weather Data Manager Reference")]
        public WeatherDataManager weatherDataManager; 

        [Header("Canvas Activation")]
        public bool activateCanvas;
        public Canvas canvasToToggle; // Assicurati di trascinare qui il riferimento del Canvas nell'Inspector
        public GameObject[] objectsToActivate; // Array di GameObject da attivare

        [Header("GameObject References")]
        public GameObject player; 
        public GameObject metaballObject;
        public GameObject objectToActivate;
        public GameObject objectToDeactivate;
        public GameObject nextColiderToActivate;
        private bool hasCollided = false;
        private List<GameObject> cloneList = new List<GameObject>();

        [Header("Attraction Function Settings")]
        public GameObject attractionTarget; // Aggiunto l'attractionTarget
        public float attractionDelay = 40f; // Tempo di attesa prima che Metaball venga attratto
        private Coroutine attractMetaballCoroutine;
        private Coroutine destroyAndDeactivateCoroutine;

        [Header("Audio Settings")]
        public int audioClipIndex = 1;
        public bool playWeatherAudioAfterClip = true; // Riproduci l'audio del meteo dopo audioClipIndex
        public int startClipIndex = 1; // inizio del range dell'index
        public int endClipIndex = 10; // fine del range dell'index
        private AudioClip[] metaballSounds;
        private AudioSource audioSource;
        private int audioClipWeather = -1;

        // Inserisci le enumerazioni qui
        public enum PartOfDay { Morning, Afternoon, Evening, Night }
        public enum TemperatureRange { Cold, Mild, Hot }
        
        private void Start()
        {
            audioSource = metaballObject.GetComponent<AudioSource>();
            metaballSounds = metaballObject.GetComponent<Metaball>().voiceOverSounds;

        }

    private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("OnTriggerEnter called with GameObject: " + other.gameObject.name);
            if (other.gameObject == player && !hasCollided)
            {
                
                StartCoroutine(PlaySoundsInOrder());
                WeatherDataManager.WeatherInfo currentWeather = weatherDataManager.GetCurrentWeatherInfo();
                float currentTemperature = currentWeather.main.temp - 273.15f; // Converti da Kelvin a Celsius

                DetermineSoundToPlay(currentTemperature);

                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);
                    nextColiderToActivate.SetActive(true);
                }

                ActivateGravityOnClones();
                // Avvia la coroutine e memorizza il riferimento
                attractMetaballCoroutine = StartCoroutine(AttractMetaballAfterDelay());
                PlaySoundsInOrder();
               // Debug.Log("Playing audio clip at index: " + audioClipIndex);

                // Attiva il Canvas se richiesto
                if (activateCanvas && canvasToToggle != null)
                {
                    canvasToToggle.gameObject.SetActive(true);
                }

                // Attiva gli oggetti specificati nell'array
                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }

                hasCollided = true;
            }
        }
        private IEnumerator PlaySoundsInOrder()
        {
            // Riproduzione della clip audio indicata da audioClipIndex
            if (audioSource != null && metaballSounds.Length > 0 && audioClipIndex < metaballSounds.Length)
            {
                audioSource.clip = metaballSounds[audioClipIndex];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);  // Aspetta che il suono finisca
            }

            // Riproduci l'audio del meteo dopo la clip, se playWeatherAudioAfterClip è true
        if (playWeatherAudioAfterClip)
        {
            WeatherDataManager.WeatherInfo currentWeather = weatherDataManager.GetCurrentWeatherInfo();
            float currentTemperature = currentWeather.main.temp - 273.15f; // Converti da Kelvin a Celsius
            DetermineSoundToPlay(currentTemperature);

            // Assicurati che l'indice della clip audio selezionato sia valido e dentro l'array di suoni.
            if (audioClipWeather >= 0 && audioClipWeather < metaballSounds.Length)
            {
                audioSource.clip = metaballSounds[audioClipWeather];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
            else
            {
                Debug.LogError("Indice della clip audio non valido o fuori range.");
            }
        }
            
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
        // Assicurati che startClipIndex e endClipIndex siano impostati correttamente nell'Inspector
        // per esempio startClipIndex = 10 e quindi endClipIndex = 21

        TemperatureRange tempRange = GetTemperatureRange(temperature);
        PartOfDay partOfDay = GetCurrentPartOfDay();

        // Calcola l'offset basato sulla parte del giorno (0 per Morning, 3 per Afternoon, ecc.)
        int dayPartOffset = (int)partOfDay * 3;

        // Assegna l'audioClipWeather in base alla combinazione di PartOfDay e TemperatureRange
        switch (tempRange)
        {
            case TemperatureRange.Cold:
                audioClipWeather = startClipIndex + dayPartOffset;
                break;
            case TemperatureRange.Mild:
                audioClipWeather = startClipIndex + dayPartOffset + 1;
                break;
            case TemperatureRange.Hot:
                audioClipWeather = startClipIndex + dayPartOffset + 2;
                break;
        }

        // Controlla che l'indice selezionato non superi il valore di endClipIndex
        if (audioClipWeather > endClipIndex)
        {
            Debug.LogError("audioClipWeather ha superato endClipIndex, verifica i valori di startClipIndex e la dimensione dell'array.");
            audioClipWeather = endClipIndex; // Imposta al valore massimo per evitare errori di indice fuori range
        }
    }

        private void ActivateGravityOnClones()
        {
            foreach (GameObject clone in cloneList)
            {
                Rigidbody rb = clone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                }
            }
        }

        private IEnumerator AttractMetaballAfterDelay()
        {
            yield return new WaitForSeconds(attractionDelay);

            // Fai in modo che Metaball segua l'attractionTarget
            metaballObject.transform.LookAt(attractionTarget.transform);
            Rigidbody metaballRb = metaballObject.GetComponent<Rigidbody>();
            metaballRb.velocity = metaballObject.transform.forward * 5; // 5 è la velocità con cui Metaball si muove verso l'attractionTarget

            // Riproduci il suono con indice 13
            if (audioSource != null && metaballSounds.Length > 3)
            {
                audioSource.clip = metaballSounds[13];
                audioSource.Play();
            }
            // Continua con la distruzione dei cloni e la disattivazione dell'oggetto
            StartCoroutine(DestroyAndDeactivate());
            // Avvia la coroutine per la distruzione e disattivazione e memorizza il riferimento
            destroyAndDeactivateCoroutine = StartCoroutine(DestroyAndDeactivate());
        }

        private IEnumerator DestroyAndDeactivate()
        {
            yield return StartCoroutine(FadeOutClonesOverTime(2f)); // Fade out dei cloni in 2 secondi
            DestroyClones();

            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
            }
            destroyAndDeactivateCoroutine = null; // Annulla il riferimento quando la coroutine è completata

        }

        // Aggiunto il metodo OnTriggerExit per gestire l'uscita dal trigger
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == player)
            {
                if (attractMetaballCoroutine != null)
                {
                    StopCoroutine(attractMetaballCoroutine);
                    attractMetaballCoroutine = null;
                }

                if (destroyAndDeactivateCoroutine != null)
                {
                    StopCoroutine(destroyAndDeactivateCoroutine);
                    destroyAndDeactivateCoroutine = null;
                }

                if (objectToDeactivate != null)
                {
                    objectToDeactivate.SetActive(false);
                }
            }
        }

        private void DestroyClones()
        {
            foreach (GameObject clone in cloneList)
            {
                Destroy(clone);
            }
            cloneList.Clear();
        }

        private IEnumerator FadeOutClonesOverTime(float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / duration;
                foreach (GameObject clone in cloneList)
                {
                    Renderer rend = clone.GetComponent<Renderer>();
                    if (rend != null && rend.material.HasProperty("_Color"))
                    {
                        Color originalColor = rend.material.GetColor("_Color");
                        originalColor.a = Mathf.Lerp(1f, 0f, t); // Interpolazione dell'alpha
                        rend.material.SetColor("_Color", originalColor);
                    }
                }
                yield return null;
            }
        }
    }
