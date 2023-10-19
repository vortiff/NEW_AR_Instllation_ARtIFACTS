using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OcchioDinamic : MonoBehaviour
{
    public GameObject player;
    public GameObject eyePrefab;
    public int numberOfClones;
    public float cloneSpreadRadius;
    public BoxCollider boundary;
    public MediaLibrary mediaLibrary;
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
    private List<GameObject> eyeSpheres = new List<GameObject>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        for (int i = 0; i < numberOfClones; i++)
        {
            Vector3 randomPosition = new Vector3(
                transform.position.x + Random.Range(-cloneSpreadRadius, cloneSpreadRadius),
                transform.position.y,
                transform.position.z + Random.Range(-cloneSpreadRadius, cloneSpreadRadius)
            );

            GameObject eyeClone = Instantiate(eyePrefab, randomPosition, Quaternion.identity, transform);
            eyeClone.GetComponent<EyeBehavior>().wobbleStrength = Random.Range(minWobbleStrength, maxWobbleStrength);
            eyeClone.GetComponent<EyeBehavior>().wobbleSpeed = Random.Range(minWobbleSpeed, maxWobbleSpeed);
            
            // Aggiungi il VideoPlayer al clone e imposta un video casuale
            RenderTexture rt = new RenderTexture(400, 400, 24);
            VideoPlayer vp = eyeClone.AddComponent<VideoPlayer>();
            vp.targetTexture = rt;
            vp.clip = mediaLibrary.videoClips[Random.Range(0, mediaLibrary.videoClips.Length)];
            vp.isLooping = true;
            vp.Play();
            eyeClone.GetComponent<Renderer>().material.mainTexture = rt;

            eyeSpheres.Add(eyeClone);
        }
    }

    private void Update()
    {
        foreach (GameObject eye in eyeSpheres)
        {
            eye.transform.LookAt(player.transform.position);
            eye.transform.Rotate(new Vector3(0, -85, 0)); 
            MoveTowardsPlayer(eye);
            StayWithinBoundary(eye);
        }

        for (int i = 0; i < eyeSpheres.Count - 1; i++)
        {
            for (int j = i + 1; j < eyeSpheres.Count; j++)
            {
                if (Vector3.Distance(eyeSpheres[i].transform.position, eyeSpheres[j].transform.position) < someOtherThreshold)
                {
                    AudioSource eyeAudioSource = eyeSpheres[i].GetComponent<AudioSource>();
                    if (!eyeAudioSource.isPlaying)
                    {
                        int clipIndex = Random.Range(10, mediaLibrary.audioClips.Length);
                        eyeAudioSource.clip = mediaLibrary.audioClips[clipIndex];
                        eyeAudioSource.Play();
                    }
                }
            }
        }
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

    void StayWithinBoundary(GameObject eye)
    {
        if (!boundary.bounds.Contains(eye.transform.position))
        {
            // Sposta l'occhio al bordo più vicino del collider
            eye.transform.position = boundary.ClosestPoint(eye.transform.position);
           
            // Inverti la direzione dell'occhio per farlo rimbalzare all'interno
            Rigidbody rb = eye.GetComponent<Rigidbody>();
            rb.velocity = -rb.velocity * 0.5f;  // Reduce the velocity to avoid excessive bouncing
        }
    }
}
