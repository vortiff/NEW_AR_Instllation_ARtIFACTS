using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    public GameObject objectToDuplicate; // Il GameObject da duplicare
    public float duplicationRadius = 1f; // Raggio in cui duplicare l'oggetto
    public float minDuplicationInterval = 2f; // Intervallo minimo tra le duplicazioni
    public float maxDuplicationInterval = 5f; // Intervallo massimo tra le duplicazioni

    private Transform player; // Il trasform del giocatore
    private bool isPlayerInsideCollider = false;
    private float nextDuplicationTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Trova il giocatore per tag "Player"
        SetNextDuplicationTime();
    }

    void Update()
    {
        // Controlla se è il momento di duplicare
        if (Time.time >= nextDuplicationTime && isPlayerInsideCollider)
        {
            DuplicateObject();
            SetNextDuplicationTime();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideCollider = true;
            Debug.Log("Player è nel collider");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideCollider = false;
            Debug.Log("Player ha lasciato il collider");
        }
    }

    void DuplicateObject()
    {
        // Genera una posizione casuale entro il raggio specificato
        Vector3 randomOffset = Random.insideUnitSphere * duplicationRadius;

        // Calcola la posizione del nuovo oggetto
        Vector3 newPosition = transform.position + randomOffset;

        // Duplica l'oggetto
        GameObject newObject = Instantiate(objectToDuplicate, newPosition, Quaternion.identity);

        // Imposta il transform del nuovo oggetto come figlio del transform attuale
        newObject.transform.SetParent(transform);

        // Ruota il nuovo oggetto su tutti e tre gli assi in modo casuale
        newObject.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }



    void SetNextDuplicationTime()
    {
        // Imposta il prossimo momento in cui verrà duplicato l'oggetto in base agli intervalli minimi e massimi
        nextDuplicationTime = Time.time + Random.Range(minDuplicationInterval, maxDuplicationInterval);
    }
}