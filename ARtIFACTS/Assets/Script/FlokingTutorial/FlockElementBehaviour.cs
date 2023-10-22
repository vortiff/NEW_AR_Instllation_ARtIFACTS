using UnityEngine;

public class FlockElementBehaviour : MonoBehaviour
{
    public float repulsionStrength = 15.0f;  // La forza con cui i cloni vengono respinti in caso di collisione
    public float comfortZone = 0.5f; // Distanza alla quale i cloni iniziano a respingersi l'un l'altro

    void OnCollisionEnter(Collision collision)
    {
        // Quando si verifica una collisione, applica una forza di repulsione
        Vector3 repulsionDirection = (transform.position - collision.transform.position).normalized;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(repulsionDirection * repulsionStrength, ForceMode.Impulse);
    }
    void Update()
    {
        foreach (var otherClone in FindObjectsOfType<FlockElementBehaviour>())
        {
            if (otherClone != this)
            {
                float distance = Vector3.Distance(transform.position, otherClone.transform.position);
                if (distance < comfortZone)
                {
                    Vector3 repulsionDirection = (transform.position - otherClone.transform.position).normalized;
                    GetComponent<Rigidbody>().AddForce(repulsionDirection * repulsionStrength);
                }
            }
        }
    }

}
