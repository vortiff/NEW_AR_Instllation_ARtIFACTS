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
    
    private AudioSource cloneAudioSource;
    private static int totalClonesWithAudio = 0; // Variabile statica per tenere traccia del numero di cloni con AudioSource
    private const int MAX_CLONES_WITH_AUDIO = 30; // Costante per il numero massimo di cloni che possono avere un AudioSource
     

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(FlockingManagerOpt.FM.minSpeed, FlockingManagerOpt.FM.maxSpeed);
        playerCollider = this.transform.parent.GetComponent<Collider>();

        if (totalClonesWithAudio < MAX_CLONES_WITH_AUDIO)
        {
            cloneAudioSource = this.gameObject.AddComponent<AudioSource>();
            totalClonesWithAudio++;
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
        if (other == playerCollider && FlockingManagerOpt.FM.arCamera)
        {
            PlayRandomSoundFromIndexRange(0, 8);
        }
    }

    void PlayRandomSoundFromIndexRange(int startIndex, int endIndex)
    {
        if (cloneAudioSource)
        {
            currentIndex = Random.Range(startIndex, endIndex + 1);
            if (currentIndex < FlockingManagerOpt.FM.mediaLibrary.audioClips.Length)
            {
                cloneAudioSource.clip = FlockingManagerOpt.FM.mediaLibrary.audioClips[currentIndex];
                cloneAudioSource.Play();
            }
        }
    }

}
