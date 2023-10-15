using UnityEngine;

public class ColliderManagerStartClone : MonoBehaviour
{
    public GameObject metaballObject; // Il GameObject Metaball
    public GameObject objectToClone; // Il GameObject da duplicare
    public GameObject GameObjectToActivate;
    public AudioClip[] voiceOverSounds; // Array di suoni VoiceOver
    [SerializeField]
    private int numberOfClones = 5; // Modifica il numero di cloni come preferisci
    [SerializeField]
    private float cloneSpreadRadius = 2f; // Modifica il raggio in cui i cloni vengono creati
    [SerializeField]
    private float minCloneSpeed = 2f; // Modifica la velocità minima dei cloni
    [SerializeField]
    private float maxCloneSpeed = 10f; // Modifica la velocità massima dei cloni
    public float gravitationalStrength = 10f; // Forza dell'attrazione gravitazionale
    public float wobbleStrength = 1f; // Intensità dell'ondulazione
    public float wobbleSpeed = 1f; // Velocità dell'ondulazione



    private int currentSoundIndex = 0; // Indice del suono corrente da riprodurre
    private bool hasCollided = false;
    private AudioSource audioSource;

    private void Start()
    {
        // Ottieni il componente AudioSource dal GameObject Metaball
        audioSource = metaballObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il giocatore è entrato in collisione e se non ha già interagito
        if (other.CompareTag("Player") && !hasCollided)
        {
            // Attiva il GameObjectToActivate
            if (GameObjectToActivate != null)
            {
                GameObjectToActivate.SetActive(true);
            }

            // Riproduci il suono VoiceOver dalla lista
            if (audioSource != null && voiceOverSounds.Length > 0 && currentSoundIndex < voiceOverSounds.Length)
            {
                audioSource.clip = voiceOverSounds[currentSoundIndex];
                audioSource.Play();
                currentSoundIndex++;
            }

            // Attiva il componente FollowPlayer sul GameObject Metaball, se presente
            FollowPlayer followPlayerComponent = metaballObject.GetComponent<FollowPlayer>();
            if (followPlayerComponent != null)
            {
                followPlayerComponent.enabled = true;
            }

            // Chiama il metodo CloneObject per duplicare gli oggetti
            CloneObject();

            // Imposta hasCollided su true per evitare collisioni multiple
            hasCollided = true;


        }
    }

    private void CloneObject()
    {
        // Crea i cloni attorno al GameObject originale
        for (int i = 0; i < numberOfClones; i++)
        {
            Vector3 randomPosition = new Vector3(
                objectToClone.transform.position.x + Random.Range(-cloneSpreadRadius, cloneSpreadRadius),
                objectToClone.transform.position.y,
                objectToClone.transform.position.z + Random.Range(-cloneSpreadRadius, cloneSpreadRadius)
            );

            GameObject clone = Instantiate(objectToClone, randomPosition, Quaternion.identity);

            // Imposta il clone come figlio dell'oggetto a cui lo script è applicato
            clone.transform.SetParent(this.transform);

            Rigidbody rb = clone.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = clone.AddComponent<Rigidbody>();
            }
            rb.velocity = (clone.transform.position - objectToClone.transform.position).normalized * Random.Range(minCloneSpeed, maxCloneSpeed);
        }
    }
    void Update()
    {
        // Muovi ogni clone verso l'Anchor1
        foreach (Transform child in transform)
        {
            MoveTowardsAnchor(child.gameObject);
        }
    }

    void MoveTowardsAnchor(GameObject clone)
    {
        // Trova l'Anchor1
        GameObject anchor = GameObject.Find("Anchor1");
        if (anchor == null) return; // Se non c'è Anchor1, esci

        // Calcola la direzione verso l'Anchor1
        Vector3 directionToAnchor = (anchor.transform.position - clone.transform.position).normalized;

        // Calcola un movimento ondulatorio
        Vector3 wobble = new Vector3(
            Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 1f) * wobbleStrength, // +1f per differenziare le ondulazioni tra gli assi
            Mathf.Sin(Time.time * wobbleSpeed + 2f) * wobbleStrength  // +2f per differenziare ulteriormente
        );

        // Calcola una forza randomica
        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );

        // Applica tutte le forze al clone
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        rb.AddForce(directionToAnchor * gravitationalStrength + wobble + randomForce);
    }

}
