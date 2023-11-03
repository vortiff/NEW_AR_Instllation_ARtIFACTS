using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metaball : MonoBehaviour
{
    public Transform player;
    public float followDistance = 5f; // La distanza a cui seguire il player
    public float checkFrequency = 1f; // Quanto spesso controllare la posizione del player
    public float moveSpeed = 3f; 
    private Vector3 desiredPosition;
    private bool isMoving = false;

    public AudioClip[] voiceOverSounds; // Array di suoni VoiceOver

    void Start()
    {
        // Verifica se l'AudioSource esiste già
        if (GetComponent<AudioSource>() == null)
        {
            // Se non esiste, aggiungilo al GameObject
            gameObject.AddComponent<AudioSource>();
        }
        
        StartCoroutine(PositionCheckRoutine());

    }

    private IEnumerator PositionCheckRoutine()
    {
        while(true)
        {
            if (player != null)
            {
                // Calcola la direzione dal Metaball al player
                Vector3 directionToPlayer = (player.position - transform.position).normalized;

                // Calcola la posizione desiderata per il Metaball
                desiredPosition = player.position - directionToPlayer * followDistance;

                if(!isMoving && Vector3.Distance(transform.position, desiredPosition) > 0.1f) 
                {
                    StartCoroutine(MoveTowardsDesiredPosition());
                }
            }

            yield return new WaitForSeconds(checkFrequency);
        }
    }

    private IEnumerator MoveTowardsDesiredPosition()
    {
        isMoving = true;

        float journeyLength = Vector3.Distance(transform.position, desiredPosition);
        float startTime = Time.time;
        
        while(Vector3.Distance(transform.position, desiredPosition) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // Interpolazione non lineare per un movimento morbido
            float smoothedFraction = Mathf.SmoothStep(0, 1, fractionOfJourney);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothedFraction);
            
            yield return null;
        }

        transform.position = desiredPosition; // Assicurati che la posizione finale sia esattamente quella desiderata
        isMoving = false;
    }
}