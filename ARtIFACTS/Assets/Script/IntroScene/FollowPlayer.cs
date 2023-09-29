using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // Il riferimento al GameObject "Player"
    public float followDistance = 2f; // La distanza a cui seguire il player

    void Update()
    {
        // Assicurati che il riferimento al player sia valido
        if (player == null)
        {
            Debug.LogWarning("Il riferimento al player è nullo. Assegna il player nel componente FollowPlayer nell'Editor.");
            return;
        }

        // Calcola la direzione dal seguente GameObject al player
        Vector3 directionToPlayer = player.position - transform.position;

        // Calcola la distanza dal seguente GameObject al player
        float distanceToPlayer = directionToPlayer.magnitude;

        // Verifica se la distanza è maggiore della distanza di "followDistance"
        if (distanceToPlayer > followDistance)
        {
            // Normalizza la direzione per mantenerla a una distanza fissa
            Vector3 newPosition = transform.position + directionToPlayer.normalized * (distanceToPlayer - followDistance);

            // Imposta la nuova posizione
            transform.position = newPosition;
        }
    }
}