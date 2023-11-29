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





    // Start is called before the first frame update
    void Start()
    {
        if (cloneMaterial != null)
        {
            originalColor = cloneMaterial.GetColor("_Color"); // Assumendo che stai usando _Color come nome del parametro
        }

       

        speed = Random.Range(FlockingManagerOpt.FMOpt.minSpeed, FlockingManagerOpt.FMOpt.maxSpeed);
        playerCollider = GameObject.FindGameObjectWithTag("FlockManager").GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(FlockingManagerOpt.FMOpt.transform.position, FlockingManagerOpt.FMOpt.flyLimit);

        if (!b.Contains(transform.position))
        {
            turnig = true;
        }
        else 
            turnig = false;

        if (turnig)
        {
            Vector3 direction = FlockingManagerOpt.FMOpt.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                  FlockingManagerOpt.FMOpt.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if(Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockingManagerOpt.FMOpt.minSpeed, FlockingManagerOpt.FMOpt.maxSpeed);
            }
            applayRules();
        }
        
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void applayRules()
    {
        GameObject[] gos;
        gos = FlockingManagerOpt.FMOpt.allElement.ToArray();

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
                if(nDistance <= FlockingManagerOpt.FMOpt.neighbourDistance)
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
            vcentre = vcentre / groupSize + (FlockingManagerOpt.FMOpt.goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            if(speed > FlockingManagerOpt.FMOpt.maxSpeed)
            {
                speed = FlockingManagerOpt.FMOpt.maxSpeed;
            }

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        FlockingManagerOpt.FMOpt.rotationSpeed * Time.deltaTime);
            }
        }
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            StartCoroutine(HandleCollisionEffect());
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
        int randomIndex = Random.Range(0, FlockingManagerOpt.FMOpt.mediaLibrary.audioClips.Length);
        source.clip = FlockingManagerOpt.FMOpt.mediaLibrary.audioClips[randomIndex];
    }

    IEnumerator HandleCollisionEffect()
    {
        Color targetColor = new Color(0.8f, 0.160f, 0.290f, 0.8f); // Colore #CC284A con alpha 255

        float fadeDuration = 1f; // Durata del fade-in
        float startTime = Time.time;

        

        yield return new WaitForSeconds(colorChangeDuration - (2 * fadeDuration)); // Attendi per la durata specificata meno la durata totale dei fade

        startTime = Time.time;

        
    }

}
