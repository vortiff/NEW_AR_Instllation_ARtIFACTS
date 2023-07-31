using System.Collections.Generic;
using UnityEngine;

public class FlockingBehavior : MonoBehaviour
{
    public float cohesionRadius = 5f;
    public float separationRadius = 2f;
    public float cohesionSpeed = 2f;
    public float separationSpeed = 3f;

    private List<GameObject> flockMembers;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        flockMembers = new List<GameObject>(GameObject.FindGameObjectsWithTag("FlockMember"));
    }

    private void Update()
    {
        // Verifica che flockMembers non sia null o vuoto prima di procedere
        if (flockMembers == null || flockMembers.Count == 0)
        {
            return;
        }

        Vector3 cohesion = Vector3.zero;
        int cohesionCount = 0;
        Vector3 separation = Vector3.zero;
        int separationCount = 0;

        foreach (GameObject flockMember in flockMembers)
        {
            if (flockMember != gameObject) // Evita di includere se stesso nella valutazione
            {
                float distance = Vector3.Distance(transform.position, flockMember.transform.position);

                if (distance < cohesionRadius)
                {
                    cohesion += flockMember.transform.position;
                    cohesionCount++;
                }

                if (distance < separationRadius)
                {
                    separation += (transform.position - flockMember.transform.position).normalized / distance;
                    separationCount++;
                }
            }
        }

        if (cohesionCount > 0)
        {
            cohesion /= cohesionCount;
            Vector3 cohesionDirection = (cohesion - transform.position).normalized;
            // Applica la direzione di cohesion alla velocità desiderata
            Vector3 cohesionVelocity = cohesionDirection * cohesionSpeed;
            
            // Aggiunge la velocità di cohesion alla velocità corrente
            rb.velocity += cohesionVelocity * Time.deltaTime;
        }

        if (separationCount > 0)
        {
            separation /= separationCount;
            Vector3 separationDirection = (separation - transform.position).normalized;
            // Applica la direzione di separation alla velocità desiderata
            Vector3 separationVelocity = separationDirection * separationSpeed;
            
            // Sottrae la velocità di separation dalla velocità corrente
            rb.velocity -= separationVelocity * Time.deltaTime;
        }
    }
}
