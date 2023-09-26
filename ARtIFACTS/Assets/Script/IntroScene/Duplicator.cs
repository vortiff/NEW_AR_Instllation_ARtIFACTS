using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    public GameObject objectToDuplicate; // Il GameObject da duplicare
    public float duplicationRadius = 5f; // Raggio in cui duplicare l'oggetto

    private Transform player; // Il trasform del giocatore

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Trova il giocatore per tag "Player"
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Genera una posizione casuale entro il raggio specificato
            Vector3 randomOffset = Random.insideUnitSphere * duplicationRadius;

            // Calcola la posizione del nuovo oggetto
            Vector3 newPosition = transform.position + randomOffset;

            // Duplica l'oggetto
            GameObject newObject = Instantiate(objectToDuplicate, newPosition, Quaternion.identity);

            // Ruota il nuovo oggetto su tutti e tre gli assi per inseguire il giocatore
            newObject.transform.LookAt(player);
        }
    }
}