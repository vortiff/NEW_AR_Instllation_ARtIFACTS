using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    public GameObject objectToActivate; // Il GameObject da attivare
    public GameObject objectToDeactivate; // Il GameObject da disattivare

    public AudioClip collisionSound; // Il suono da riprodurre quando c'è una collisione

    private bool hasCollided = false;
    private AudioSource audioSource;

    private void Start()
    {
        // Assicurati di avere un componente AudioSource su questo GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il giocatore è entrato in collisione
        if (other.CompareTag("Player") && !hasCollided)
        {
            // Riproduci il suono della collisione
            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }

            // Attiva il GameObject specificato
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                objectToDeactivate.SetActive(false);
            }

            // Stampa un messaggio nella console di debug
            Debug.Log("Collisione avvenuta");

            // Imposta hasCollided su true per evitare collisioni multiple
            hasCollided = true;

            // Avvia la funzione per distruggere i cloni dopo 2 secondi
            Invoke("DestroyClones", 2f);
        }
    }

    // Funzione per distruggere i cloni con il tag "cloni"
    private void DestroyClones()
    {
        GameObject[] clones = GameObject.FindGameObjectsWithTag("cloni");

        foreach (GameObject clone in clones)
        {
            Destroy(clone);
        }
    }
}
