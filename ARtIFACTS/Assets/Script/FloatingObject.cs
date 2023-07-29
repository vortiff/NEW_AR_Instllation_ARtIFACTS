using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float maxSpeed = 5f; // Velocità massima di movimento dell'oggetto
    public float changeDirectionInterval = 2f; // Intervallo di tempo per cambiare direzione

    private Rigidbody rb;
    private float changeDirectionTimer;
    private Vector3 randomDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetRandomDirection();
    }

    private void Update()
    {
        // Controlla se è il momento di cambiare direzione
        changeDirectionTimer -= Time.deltaTime;
        if (changeDirectionTimer <= 0f)
        {
            SetRandomDirection();
            changeDirectionTimer = changeDirectionInterval;
        }

        // Applica la forza alla mesh per farla fluttuare
        rb.AddForce(randomDirection * maxSpeed * Time.deltaTime);
    }

    private void SetRandomDirection()
    {
        // Genera una nuova direzione casuale all'interno del contenitore
        randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Cambia direzione quando l'oggetto collide con il limite interno
        SetRandomDirection();
    }
}
