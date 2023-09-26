using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public AudioSource audioSource; // Riferimento al componente AudioSource per il suono da avviare
    public GameObject objectToActivate; // Riferimento al GameObject da attivare al momento della collisione
    public GameObject objectToDeactivate; // Riferimento al GameObject da disattivare al momento della collisione

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se l'oggetto che ha causato la collisione è la MainCamera
        if (collision.gameObject.CompareTag("Player"))
        {
            // Avvia il suono, se è stato assegnato un AudioSource
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Attiva il GameObject, se è stato assegnato uno
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                objectToDeactivate.SetActive(false);
            }

            // Stampa un messaggio nella console
            Debug.Log("Collisione avvenuta");

            // Disattiva questo GameObject
            gameObject.SetActive(false);
        }
    }
}
