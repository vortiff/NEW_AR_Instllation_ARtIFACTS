using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    public GameObject objectToDuplicate; // Il GameObject da duplicare
    public float duplicationWidth = 1f; // Larghezza della zona di duplicazione
    public float duplicationHeight = 1f; // Altezza della zona di duplicazione
    public float duplicationDepth = 1f; // Profondità della zona di duplicazione
    public float minDuplicationInterval = 0.1f; // Intervallo minimo tra le duplicazioni
    public float maxDuplicationInterval = 2f; // Intervallo massimo tra le duplicazioni
    public AudioClip[] duplicationSounds; // Array di suoni per le duplicazioni

    private Transform player; // Il trasform del giocatore
    private bool isPlayerInsideCollider = false;
    private float nextDuplicationTime;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Trova il giocatore per tag "Player"
        audioSource = GetComponent<AudioSource>(); // Ottieni il componente AudioSource se presente
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
        // Genera una posizione casuale entro le dimensioni specificate
        Vector3 randomOffset = new Vector3(
            Random.Range(-duplicationWidth / 2, duplicationWidth / 2),
            Random.Range(-duplicationHeight / 2, duplicationHeight / 2),
            Random.Range(-duplicationDepth / 2, duplicationDepth / 2)
        );

        // Calcola la posizione del nuovo oggetto
        Vector3 newPosition = transform.position + randomOffset;

        // Duplica l'oggetto
        GameObject newObject = Instantiate(objectToDuplicate, newPosition, Quaternion.identity);

        // Imposta il transform del nuovo oggetto come figlio del transform attuale
        newObject.transform.SetParent(transform);

        // Ruota il nuovo oggetto su tutti e tre gli assi in modo casuale
        newObject.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        // Riproduci un suono casuale dalle duplicazioni se presente
        if (audioSource != null && duplicationSounds.Length > 0)
        {
            int randomSoundIndex = Random.Range(0, duplicationSounds.Length);
            audioSource.PlayOneShot(duplicationSounds[randomSoundIndex]);
        }
    }

    void SetNextDuplicationTime()
    {
        // Imposta il prossimo momento in cui verrà duplicato l'oggetto in base agli intervalli minimi e massimi
        nextDuplicationTime = Time.time + Random.Range(minDuplicationInterval, maxDuplicationInterval);
    }
}
