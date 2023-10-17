using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OcchioDinamic : MonoBehaviour
{
    public GameObject player;
    public GameObject eyePrefab; // Il prefab dell'occhio da clonare
    public int numberOfClones;
    public float cloneSpreadRadius;
    public AudioLibrary audioLibrary;  // Riferimento alla libreria audio
    public VideoLibrary videoLibrary;  // Riferimento alla libreria vide
    public float someThreshold = 5f;
    public float someOtherThreshold = 2f;
    public float gravitationalStrength = 10f;
    public float minWobbleStrength = 1f;
    public float maxWobbleStrength = 10f;
    public float minWobbleSpeed = 1f;
    public float maxWobbleSpeed = 5f;
    public float wobbleStrength;
    public float wobbleSpeed;
    private AudioSource audioSource;
    private VideoPlayer videoPlayer;
    private GameObject[] eyeSpheres;
    private Dictionary<GameObject, VideoPlayer> eyeVideoPlayers = new Dictionary<GameObject, VideoPlayer>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        eyeSpheres = new GameObject[numberOfClones]; // Inizializza l'array
            
        for (int i = 0; i < numberOfClones; i++)
        {
            Vector3 randomPosition = new Vector3(
                transform.position.x + Random.Range(-cloneSpreadRadius, cloneSpreadRadius),
                transform.position.y,
                transform.position.z + Random.Range(-cloneSpreadRadius, cloneSpreadRadius)
            );

            // Clona il prefab e lo aggiunge all'array
            eyeSpheres[i] = Instantiate(eyePrefab, randomPosition, Quaternion.identity, transform);
            eyeSpheres[i].GetComponent<EyeBehavior>().wobbleStrength = Random.Range(minWobbleStrength, maxWobbleStrength);
            eyeSpheres[i].GetComponent<EyeBehavior>().wobbleSpeed = Random.Range(minWobbleSpeed, maxWobbleSpeed);



            // Aggiungi il VideoPlayer al clone e imposta un video casuale
            RenderTexture rt = new RenderTexture(400, 400, 24);
            VideoPlayer vp = eyeSpheres[i].AddComponent<VideoPlayer>();
            vp.targetTexture = rt;
            vp.clip = videoLibrary.videoClips[Random.Range(0, videoLibrary.videoClips.Length)];
            vp.isLooping = true;
            vp.Play();
            eyeSpheres[i].GetComponent<Renderer>().material.mainTexture = rt;
        }
    }

    private void Update()
    {
        foreach (GameObject eye in eyeSpheres)
        {
                eye.transform.LookAt(player.transform.position);
                eye.transform.Rotate(new Vector3(0, -85, 0)); 
                MoveTowardsPlayer(eye);
        }

        for (int i = 0; i < eyeSpheres.Length - 1; i++)
        {
            for (int j = i + 1; j < eyeSpheres.Length; j++)
            {
                if (Vector3.Distance(eyeSpheres[i].transform.position, eyeSpheres[j].transform.position) < someOtherThreshold)
                {
                    CreateLightBeam(eyeSpheres[i].transform.position, eyeSpheres[j].transform.position);

                    AudioSource eyeAudioSource = eyeSpheres[i].GetComponent<AudioSource>();
                    if (!eyeAudioSource.isPlaying)
                    {
                        int clipIndex = Random.Range(10, audioLibrary.audioClips.Length);
                        eyeAudioSource.clip = audioLibrary.audioClips[clipIndex];
                        eyeAudioSource.Play();
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

        EyeBehavior eyeBehav = eye.GetComponent<EyeBehavior>();
        Vector3 wobble = new Vector3(
            Mathf.Sin(Time.time * eyeBehav.wobbleSpeed) * eyeBehav.wobbleStrength,
            Mathf.Sin(Time.time * eyeBehav.wobbleSpeed + 1f) * eyeBehav.wobbleStrength,
            Mathf.Sin(Time.time * eyeBehav.wobbleSpeed + 2f) * eyeBehav.wobbleStrength
        );

        Rigidbody rb = eye.GetComponent<Rigidbody>();
        if (rb == null) rb = eye.AddComponent<Rigidbody>();
        
        rb.AddForce(directionToPlayer * gravitationalStrength + wobble);
    }
}
