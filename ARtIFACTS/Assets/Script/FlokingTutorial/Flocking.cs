using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{

    float speed; 
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    bool turnig = false;    
    private Collider playerCollider;
    private int currentIndex; // Define currentIndex at the class level
    
    private static int totalClonesWithAudio = 0; // Variabile statica per tenere traccia del numero di cloni con AudioSource
    private const int MAX_CLONES_WITH_AUDIO = 1; // Costante per il numero massimo di cloni che possono avere un AudioSource
    public float colorChangeDuration = 5.0f; // Durata del cambiamento del colore e dell'intensità della luce
    private Color originalColor;
    private float originalLightIntensity;
    [SerializeField] private Material cloneMaterial;
    [SerializeField] private AudioSource cloneAudioSource;
    [SerializeField] private Light cloneLight;
    [SerializeField] private float desiredLightIntensity = 1.0f;




    // Start is called before the first frame update
    void Start()
    {
        if (cloneMaterial != null)
        {
            originalColor = cloneMaterial.GetColor("_Color"); // Assumendo che stai usando _Color come nome del parametro
        }

        if (cloneLight != null)
        {
            originalLightIntensity = desiredLightIntensity;
            cloneLight.intensity = originalLightIntensity;
        }

        speed = Random.Range(FlockingManagerOpt.FM.minSpeed, FlockingManagerOpt.FM.maxSpeed);
        playerCollider = GameObject.FindGameObjectWithTag("FlockManager").GetComponent<Collider>();

        if (totalClonesWithAudio < MAX_CLONES_WITH_AUDIO)
        {
            int randomIndex = Random.Range(0, FlockingManagerOpt.FM.mediaLibrary.audioClips.Length);
            
            cloneAudioSource.clip = FlockingManagerOpt.FM.mediaLibrary.audioClips[randomIndex];
            cloneAudioSource.Play();
            
            totalClonesWithAudio++;
        }
        else
        {
            cloneAudioSource.enabled = false; // Disattiva l'AudioSource per tutti gli altri cloni
        }



         if (cloneAudioSource)
        {
            cloneAudioSource.loop = false;
            StartCoroutine(CheckIfAudioIsPlaying(cloneAudioSource));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(FlockingManagerOpt.FM.transform.position, FlockingManagerOpt.FM.flyLimit);

        if (!b.Contains(transform.position))
        {
            turnig = true;
        }
        else 
            turnig = false;

        if (turnig)
        {
            Vector3 direction = FlockingManagerOpt.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                  FlockingManagerOpt.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if(Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockingManagerOpt.FM.minSpeed, FlockingManagerOpt.FM.maxSpeed);
            }
            applayRules();
        }
        
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void applayRules()
    {
        GameObject[] gos;
        gos = FlockingManagerOpt.FM.allElement.ToArray();

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f; 
        float nDistance;
        int groupSize = 0; 

        foreach(GameObject go in gos)
        {
            if(go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if(nDistance <= FlockingManagerOpt.FM.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if(nDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flocking anotherFlocking = go.GetComponent<Flocking>();
                    gSpeed = gSpeed + anotherFlocking.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (FlockingManagerOpt.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            if(speed > FlockingManagerOpt.FM.maxSpeed)
            {
                speed = FlockingManagerOpt.FM.maxSpeed;
            }

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        FlockingManagerOpt.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            StartCoroutine(HandleCollisionEffect());
        }

        // Controllo aggiuntivo per assicurarsi che solo i cloni con un AudioSource riproducano un suono
        if (other == playerCollider && FlockingManagerOpt.FM.arCamera && cloneAudioSource && !cloneAudioSource.isPlaying)
        {
            PlayRandomSoundFromIndexRange(2, 6);
        }
    }

    void PlayRandomSoundFromIndexRange(int startIndex, int endIndex)
    {
        if (cloneAudioSource && !cloneAudioSource.isPlaying) // Verifica se cloneAudioSource esiste e non sta già riproducendo un suono
        {
            currentIndex = Random.Range(startIndex, endIndex + 1);

            if (currentIndex < FlockingManagerOpt.FM.mediaLibrary.audioClips.Length) // Verifica che l'indice sia valido
            {
                cloneAudioSource.clip = FlockingManagerOpt.FM.mediaLibrary.audioClips[currentIndex];
                cloneAudioSource.Play();
            }
        }
    }

    IEnumerator CheckIfAudioIsPlaying(AudioSource source)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // Attendi un breve periodo di tempo tra i controlli
            if (!source.isPlaying) // Se l'AudioSource ha finito di riprodurre
            {
                AssignRandomClipToAudioSource(source);
                source.Play();
            }
        }
    }

    void AssignRandomClipToAudioSource(AudioSource source)
    {
        int randomIndex = Random.Range(0, FlockingManagerOpt.FM.mediaLibrary.audioClips.Length);
        source.clip = FlockingManagerOpt.FM.mediaLibrary.audioClips[randomIndex];
    }

    IEnumerator HandleCollisionEffect()
    {
        Color targetColor = new Color(0.8f, 0.160f, 0.290f, 0.8f); // Colore #CC284A con alpha 255

        float fadeDuration = 1f; // Durata del fade-in
        float startTime = Time.time;

        // Fade-in
        while (Time.time - startTime < fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            if (cloneLight != null)
            {
                cloneLight.intensity = Mathf.Lerp(desiredLightIntensity, desiredLightIntensity + 0.3f, t);
            }
            yield return null;
        }

        yield return new WaitForSeconds(colorChangeDuration - (2 * fadeDuration)); // Attendi per la durata specificata meno la durata totale dei fade

        startTime = Time.time;

        // Fade-out
        while (Time.time - startTime < fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            if (cloneLight != null)
            {
                cloneLight.intensity = Mathf.Lerp(desiredLightIntensity + 0.3f, desiredLightIntensity, t);
            }
            yield return null;
        }

        if (cloneLight != null)
        {
            cloneLight.intensity = desiredLightIntensity;
        }
    }

}
