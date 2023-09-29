using UnityEngine;

public class ColliderManagerStartClone : MonoBehaviour
{
    public GameObject objectToActivate; // Il GameObject da attivare
    public GameObject objectToClone; // Il GameObject da duplicare
    public AudioClip collisionSound; // Il suono da riprodurre quando c'è una collisione

    [SerializeField]
    private int numberOfClones = 5; // Modifica il numero di cloni come preferisci
    [SerializeField]
    private float minCloneSpeed = 2f; // Velocità minima dei cloni
    [SerializeField]
    private float maxCloneSpeed = 5f; // Velocità massima dei cloni
    [SerializeField]
    private float cloneSpreadRadius = 5f; // Modifica il raggio di distribuzione dei cloni come preferisci

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
            }

            // Attiva il componente FollowPlayer se presente
            FollowPlayer followPlayerComponent = GetComponent<FollowPlayer>();
            if (followPlayerComponent != null)
            {
                followPlayerComponent.enabled = true;
            }

            // Disattiva il componente MeshRenderer se presente
            MeshRenderer rendererToDeactivate = GetComponent<MeshRenderer>();
            if (rendererToDeactivate != null)
            {
                Debug.Log("Disattiva Mesh");
                rendererToDeactivate.enabled = false;
            }

            // Chiama il metodo SplitObject se è presente
            ColliderManagerStartClone cloneScript = GetComponent<ColliderManagerStartClone>();
            if (cloneScript != null)
            {
                cloneScript.SplitObject();
                Debug.Log("Clone creato");
            }

            // Stampa un messaggio nella console di debug
            Debug.Log("Collisione avvenuta");

            // Imposta hasCollided su true per evitare collisioni multiple
            hasCollided = true;
        }
    }

    public void SplitObject()
    {
        // ... Il resto del tuo codice per la duplicazione degli oggetti ...
    }
}
