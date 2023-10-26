using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWall : MonoBehaviour
{
    [SerializeField] private float minLineCreation;
    [SerializeField] private float maxLineCreation;
    public MediaLibrary mediaLibrary;
    private AudioSource audioSource;
    public Transform referenceObject;

    // Variabili per definire i punti iniziale e finale della linea
    private Vector3 startPoint;
    private Vector3 endPoint;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(CreateRandomLines());
    }

    IEnumerator CreateRandomLines()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minLineCreation, maxLineCreation));

            // Crea la tua linea qui usando LineRenderer
            GameObject lineObject = new GameObject("RandomLine");
            lineObject.transform.SetParent(transform);
            if (referenceObject != null)
            {
                lineObject.transform.position = referenceObject.position; // Usa la posizione dell'oggetto di riferimento
            }
            else
            {
                lineObject.transform.position = transform.position; 
            }

            LineRenderer line = lineObject.AddComponent<LineRenderer>();

            // Imposta la larghezza della linea
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;

            // Genera i punti iniziale e finale della linea in modo randomico all'interno del muro
            startPoint = GetRandomPointOnEdge();
            endPoint = GetRandomPointOnEdge();

            // Imposta i punti della linea
            line.SetPosition(0, startPoint);
            line.SetPosition(1, endPoint);

            // Assegna un colore random alla linea
            line.startColor = new Color(Random.value, Random.value, Random.value);
            line.endColor = new Color(Random.value, Random.value, Random.value);

            // Riproduci un suono random dalla tua mediaLibrary
            int randomIndex = Random.Range(0, 6);
            audioSource.clip = mediaLibrary.audioClips[randomIndex];
            audioSource.Play();
        }
    }



    private Vector3 GetRandomPointOnEdge()
    {
        int side = Random.Range(0, 4); // 0 = top, 1 = bottom, 2 = left, 3 = right
        switch (side)
        {
            case 0: // top
                return new Vector3(Random.Range(-5, 5), 5, 0);
            case 1: // bottom
                return new Vector3(Random.Range(-5, 5), -5, 0);
            case 2: // left
                return new Vector3(-5, Random.Range(-5, 5), 0);
            case 3: // right
                return new Vector3(5, Random.Range(-5, 5), 0);
            default:
                return Vector3.zero; // Questo non dovrebbe mai accadere
        }
    }

    void OnTriggerEnter(Collider other)
    {
        maxLineCreation *= 1.3f;
    }
}
