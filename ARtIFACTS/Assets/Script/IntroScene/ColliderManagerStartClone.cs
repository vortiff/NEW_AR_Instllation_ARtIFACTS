using UnityEngine;
using System.Collections;

public class ColliderManagerStartClone : MonoBehaviour
{
    public GameObject metaballObject;
    public GameObject objectToClone;
    public GameObject GameObjectToActivate;
    public int audioClipIndex = 0;

    [SerializeField]
    private int numberOfClones = 30;
    [SerializeField]
    private float cloneSpreadRadius = 2f;
    [SerializeField]
    private float minCloneSpeed = 1f;
    [SerializeField]
    private float maxCloneSpeed = 5f;
    public float gravitationalStrength = 10f;
    public float wobbleStrength = 5f;
    public float wobbleSpeed = 2f;
    public GameObject attractionTarget;
    //private int currentSoundIndex = 0;
    private bool hasCollided = false;
    private AudioSource audioSource;
    private AudioClip[] metaballSounds;

    private void Start()
    {
        audioSource = metaballObject.GetComponent<AudioSource>();
        metaballSounds = metaballObject.GetComponent<Metaball>().voiceOverSounds;
    }

    private IEnumerator DelayedCloneCreation()
    {
        yield return new WaitUntil(() => attractionTarget.activeSelf);
        CloneObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasCollided)
        {
            if (GameObjectToActivate != null)
            {
                GameObjectToActivate.SetActive(true);
            }

            if (audioSource != null && metaballSounds.Length > 0 && audioClipIndex < metaballSounds.Length)
            {
                audioSource.clip = metaballSounds[audioClipIndex];
                audioSource.Play();
            }

            Metaball followPlayerComponent = metaballObject.GetComponent<Metaball>();
            if (followPlayerComponent != null)
            {
                followPlayerComponent.enabled = true;
            }

            // Using coroutine to delay the clone creation until the attractionTarget is active.
            StartCoroutine(DelayedCloneCreation());

            // Setting hasCollided to true to prevent multiple calls.
            hasCollided = true;
        }
    }

    private void CloneObject()
    {
        // Fetch the ObjectCloner script from the specified objectToClone.
        ObjectCloner cloner = objectToClone.GetComponent<ObjectCloner>();
        if (cloner != null)
        {
            cloner.SetupCloningParameters(objectToClone, numberOfClones, cloneSpreadRadius, minCloneSpeed, maxCloneSpeed, attractionTarget);
            cloner.CloneObject(this.gameObject);
        }
        else
        {
            Debug.LogWarning("ObjectCloner script not found on the specified objectToClone.");
        }
    }

    void Update()
    {
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
        if (attractionTarget == null || !attractionTarget.activeSelf) return;

        Vector3 directionToTarget = (target.transform.position - clone.transform.position).normalized;

        Vector3 wobble = new Vector3(
            Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 1f) * wobbleStrength,
            Mathf.Sin(Time.time * wobbleSpeed + 2f) * wobbleStrength
        );

        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );

        Vector3 forceToApply = directionToTarget * gravitationalStrength + wobble + randomForce;

        Debug.Log("Force applied to clone: " + forceToApply);

        Rigidbody rb = clone.GetComponent<Rigidbody>();
        rb.AddForce(forceToApply);
    }
}
