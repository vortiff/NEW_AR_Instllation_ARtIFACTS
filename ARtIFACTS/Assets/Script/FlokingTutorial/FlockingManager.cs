using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public static FlockingManager FM;
    public GameObject elementPrefab; 
    public int numElement = 20; 
    public GameObject[] allElement;
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

    public Transform arCamera; // Referenza all'oggetto con il Tag "ARCamera"
    public float maxDistanceForSpeedChange = 3f; // Distanza massima per cambiare la velocità di rotazione
    public float minRotationSpeed = 5f;
    public float maxRotationSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        allElement = new GameObject [numElement];
        for(int i = 0; i < numElement; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-flyLimit.x, flyLimit.x),
                                                                Random.Range(-flyLimit.y, flyLimit.y),
                                                                Random.Range(-flyLimit.z, flyLimit.z));
            
            allElement[i] = Instantiate(elementPrefab, pos, Quaternion.identity);
        }

        FM = this;
        goalPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0, 100) < 3)
        {
            goalPos = this.transform.position + new Vector3(Random.Range(-flyLimit.x, flyLimit.x),
                                                                Random.Range(-flyLimit.y, flyLimit.y),
                                                                Random.Range(-flyLimit.z, flyLimit.z));
        }

         // Calcola la distanza tra questo GameObject e l'oggetto con il Tag "ARCamera"
        float distanceToARCamera = Vector3.Distance(transform.position, arCamera.position);

        // Normalizza la distanza tra 0 e 1 in base al massimo raggio consentito per il cambio di velocità
        float normalizedDistance = Mathf.Clamp01(distanceToARCamera / maxDistanceForSpeedChange);

        // Interpola linearmente tra minRotationSpeed e maxRotationSpeed in base alla distanza normalizzata
        float newRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, 1f - normalizedDistance);

        // Aggiorna la variabile rotationSpeed in Flocking per tutti gli oggetti dello stormo
        foreach (GameObject flockMember in allElement)
        {
            Flocking flockingScript = flockMember.GetComponent<Flocking>();
            if (flockingScript != null)
            {
                flockingScript.Speed = newRotationSpeed;
            }
        }
    }
}
