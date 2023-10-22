using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileFlockManager : MonoBehaviour
{
    public GameObject player;
    public GameObject elementPrefab;
    public int numberOfElements = 20;
    private List<GameObject> flockElements = new List<GameObject>();
    public Vector3 goalPosition;

    [Header("Flock Settings")]
    public float orbitSpeed = 5f;
    public float distanceFromGoal = 5f;

    [Header("Trigger Settings")]
    public float increasedOrbitSpeed = 10f;
    public float closerDistanceFromGoal = 3f;
    public float transitionDuration = 2f; // durata del "fade" tra i due stati

    [Header("Attraction to Goal")]
    public float attractionStrength = 10.0f; // La forza con cui i cloni sono attratti verso l'obiettivo
    public float repulsionStrength = 15.0f;  // La forza con cui i cloni vengono respinti in caso di collisione

    private bool isInTrigger = false;

    void Start()
    {
        for (int i = 0; i < numberOfElements; i++)
        {
            GameObject clone = Instantiate(elementPrefab, Random.insideUnitSphere * distanceFromGoal + transform.position, Quaternion.identity);
            clone.AddComponent<FlockElementBehaviour>(); // Aggiungi lo script al clone
            flockElements.Add(clone);
        }

        goalPosition = transform.position;

        StartCoroutine(ChangeGoalPosition());
    }

    void Update()
    {
        foreach (GameObject element in flockElements)
        {
            Vector3 orbitDirection = Vector3.Cross(Vector3.up, element.transform.position - goalPosition).normalized;
            element.transform.position += orbitDirection * orbitSpeed * Time.deltaTime;

            // Attrazione verso l'obiettivo per ogni clone
            Rigidbody rb = element.GetComponent<Rigidbody>();
            Vector3 directionToGoal = (goalPosition - element.transform.position).normalized;
            rb.AddForce(directionToGoal * attractionStrength);
        }
    }


    IEnumerator ChangeGoalPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(isInTrigger ? 1f : 5f); // frequenza di cambiamento della posizione del goal
            goalPosition = transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) // Assumendo che l'oggetto che entra nel trigger abbia il tag "Player"
        {
            isInTrigger = true;
            StartCoroutine(TransitionToState(increasedOrbitSpeed, closerDistanceFromGoal));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isInTrigger = false;
            StartCoroutine(TransitionToState(orbitSpeed, distanceFromGoal));
        }
    }

    IEnumerator TransitionToState(float targetSpeed, float targetDistance)
    {
        float initialSpeed = orbitSpeed;
        float initialDistance = distanceFromGoal;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            orbitSpeed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / transitionDuration);
            distanceFromGoal = Mathf.Lerp(initialDistance, targetDistance, elapsedTime / transitionDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        orbitSpeed = targetSpeed;
        distanceFromGoal = targetDistance;
    }
}
