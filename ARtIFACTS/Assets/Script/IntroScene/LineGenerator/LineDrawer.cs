using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public float drawSpeed = 1f; // Velocità di disegno della linea

    private LineRenderer lineRenderer;
    private Vector3 targetPosition;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        targetPosition = GetRandomTargetPosition();
    }

    private void Update()
    {
        // Sposta gradualmente la posizione finale del LineRenderer verso la destinazione
        lineRenderer.SetPosition(1, Vector3.MoveTowards(lineRenderer.GetPosition(1), targetPosition, drawSpeed * Time.deltaTime));

        // Quando il LineRenderer raggiunge la destinazione, calcola una nuova destinazione casuale
        if (Vector3.Distance(lineRenderer.GetPosition(1), targetPosition) < 0.1f)
        {
            targetPosition = GetRandomTargetPosition();
        }
    }

    private Vector3 GetRandomTargetPosition()
    {
        // Genera una nuova posizione casuale nell'area desiderata (ad esempio, lungo gli assi X e Z)
        float randomX = Random.Range(-10f, 10f); // Sostituisci questi valori con i tuoi limiti desiderati
        float randomZ = Random.Range(-10f, 10f); // Sostituisci questi valori con i tuoi limiti desiderati

        return new Vector3(randomX, 0f, randomZ);
    }
}