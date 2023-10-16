using UnityEngine;

public class ObjectCloner : MonoBehaviour
{
    private GameObject objectToClone;
    private int numberOfClones;
    private float cloneSpreadRadius;
    private float minCloneSpeed;
    private float maxCloneSpeed;
    private GameObject attractionTarget;
    [SerializeField]
    private float attractionStrength = 10f; // La forza di attrazione verso l'attractionTarget

    public void SetupCloningParameters(GameObject _objectToClone, int _numberOfClones, float _cloneSpreadRadius, float _minCloneSpeed, float _maxCloneSpeed, GameObject _attractionTarget)
    {
        objectToClone = _objectToClone;
        numberOfClones = _numberOfClones;
        cloneSpreadRadius = _cloneSpreadRadius;
        minCloneSpeed = _minCloneSpeed;
        maxCloneSpeed = _maxCloneSpeed;
        attractionTarget = _attractionTarget;
    }

    public void CloneObject(GameObject parentObject)
    {
        for (int i = 0; i < numberOfClones; i++)
        {
            Vector3 randomPosition = new Vector3(
                transform.position.x + Random.Range(-cloneSpreadRadius, cloneSpreadRadius),
                transform.position.y,
                transform.position.z + Random.Range(-cloneSpreadRadius, cloneSpreadRadius)
            );

            GameObject clone = Instantiate(objectToClone, randomPosition, Quaternion.identity);
            clone.transform.SetParent(parentObject.transform);  // Imposta il padre al GameObject passato come argomento

            Rigidbody rb = clone.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = clone.AddComponent<Rigidbody>();
            }
            rb.velocity = (clone.transform.position - transform.position).normalized * Random.Range(minCloneSpeed, maxCloneSpeed);
        }
    }

    private void Update()
    {
        // Attrai ogni clone verso l'attractionTarget
        foreach (Transform child in transform)
        {
            AttractTowardsTarget(child.gameObject);
        }
    }

    void AttractTowardsTarget(GameObject clone)
    {
        if (attractionTarget == null) return; // Se non c'è un oggetto di attrazione, esci

        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb == null) return; // Se il clone non ha un Rigidbody, esci

        // Calcola la direzione verso l'attractionTarget
        Vector3 directionToTarget = (attractionTarget.transform.position - clone.transform.position).normalized;

        // Applica la forza di attrazione
        rb.AddForce(directionToTarget * attractionStrength);
    }
}
