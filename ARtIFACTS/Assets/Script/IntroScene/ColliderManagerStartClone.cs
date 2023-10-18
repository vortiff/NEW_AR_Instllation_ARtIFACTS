using UnityEngine;
public class ColliderManagerStartClone : MonoBehaviour
{
    public GameObject metaballObject; // Il GameObject Metaball
    public GameObject objectToClone; // Il GameObject da duplicare
    public GameObject GameObjectToActivate;
    public int audioClipIndex = 0; // L'indice dell'audio clip da riprodurre

    [SerializeField]
    private int numberOfClones = 30; // Modifica il numero di cloni come preferisci
    [SerializeField]
    private float cloneSpreadRadius = 2f; // Modifica il raggio in cui i cloni vengono creati
    [SerializeField]
    private float minCloneSpeed = 1f; // Modifica la velocità minima dei cloni
    [SerializeField]
    private float maxCloneSpeed = 5f; // Modifica la velocità massima dei cloni
    public float maxGravitationalStrength;
    public float minGravitationalStrength;
    public float maxWobbleStrength;
    public float minWobbleStrength;
    public float maxWobbleSpeed;
    public float minWobbleSpeed;
    public GameObject attractionTarget; // Il GameObject verso cui i cloni saranno attratti
    private int currentSoundIndex = 0; // Indice del suono corrente da riprodurre
    private bool hasCollided = false;
    private AudioSource audioSource;
    private AudioClip[] metaballSounds; // Array di suoni da Metaball
    // Variabili per i cloni
    public float gravitationalStrength;
    public float wobbleStrength;
    public float wobbleSpeed;

    private void Start()
    {
        // Ottieni il componente AudioSource dal GameObject Metaball
        audioSource = metaballObject.GetComponent<AudioSource>();

        // Ottieni l'array di clip audio dal metaballObject
        metaballSounds = metaballObject.GetComponent<Metaball>().voiceOverSounds;
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

            // Riproduci il suono VoiceOver dalla lista del GameObject Metaball
            if (audioSource != null && metaballSounds.Length > 0 && audioClipIndex < metaballSounds.Length)
            {
                audioSource.clip = metaballSounds[audioClipIndex];
                audioSource.Play();
            }

            // Attiva il componente FollowPlayer sul GameObject Metaball, se presente
            Metaball followPlayerComponent = metaballObject.GetComponent<Metaball>();
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
        for (int i = 0; i < numberOfClones; i++)
        {
            Vector3 randomPosition = GetRandomPositionWithinRadius();
            GameObject newClone = Instantiate(objectToClone, randomPosition, Quaternion.identity, transform);
            
            // Assegna i valori random ai cloni
            newClone.GetComponent<Rigidbody>().velocity = Random.Range(minCloneSpeed, maxCloneSpeed) * Vector3.up;
            gravitationalStrength = Random.Range(minGravitationalStrength, maxGravitationalStrength);
            wobbleStrength = Random.Range(minWobbleStrength, maxWobbleStrength);
            wobbleSpeed = Random.Range(minWobbleSpeed, maxWobbleSpeed);
        }
    }

    void Update()
    {
        // Se abbiamo un target di attrazione definito, attrai i cloni verso di esso
        if (attractionTarget != null)
        {
            foreach (Transform child in transform)
            {
                MoveTowardsTarget(child.gameObject, attractionTarget);
            }
        }
    }

    void MoveTowardsTarget(GameObject clone, GameObject target)
    {
        // Calcola la direzione verso il target
        Vector3 directionToTarget = (target.transform.position - clone.transform.position).normalized;

        // Calcola un movimento ondulatorio
        Vector3 wobble = new Vector3(
            Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 1f) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 2f) * wobbleStrength
        );

        // Calcola una forza randomica
        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );

        Vector3 forceToApply = directionToTarget * gravitationalStrength + wobble + randomForce;

        // Applica tutte le forze al clone
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        rb.AddForce(forceToApply);
    }

    private Vector3 GetRandomPositionWithinRadius()
    {
        return transform.position + Random.insideUnitSphere * cloneSpreadRadius;
    }

}
