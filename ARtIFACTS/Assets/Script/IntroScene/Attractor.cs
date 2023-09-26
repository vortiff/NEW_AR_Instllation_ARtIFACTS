using UnityEngine;

public class Attractor : MonoBehaviour
{
    public Transform target; // Il bersaglio da attrarre
    public float attractionForce = 10f; // La forza di attrazione

    private void FixedUpdate()
    {
        if (target != null)
        {
            // Calcola la direzione verso il bersaglio
            Vector3 direction = target.position - transform.position;

            // Applica una forza nella direzione del bersaglio
            GetComponent<Rigidbody>().AddForce(direction.normalized * attractionForce);
        }
    }
}
