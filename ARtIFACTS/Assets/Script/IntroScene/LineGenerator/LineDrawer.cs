using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public float drawSpeed = 1f; // Velocità di disegno della linea
    public float maxLineLength = 10f; // Lunghezza massima della linea
    public LayerMask collisionLayer; // Layer per la collisione

    private LineRenderer lineRenderer;
    private Vector3 targetPosition;
    private bool isDrawing = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (isDrawing)
        {
            Vector3 currentPosition = lineRenderer.GetPosition(1);
            float step = drawSpeed * Time.deltaTime;

            // Sposta gradualmente la posizione finale del LineRenderer verso la destinazione
            lineRenderer.SetPosition(1, Vector3.MoveTowards(currentPosition, targetPosition, step));

            // Controlla la collisione
            if (Physics.Raycast(currentPosition, (targetPosition - currentPosition).normalized, out RaycastHit hit, step, collisionLayer))
            {
                // Imposta la nuova destinazione come punto di collisione
                targetPosition = hit.point;
            }

            // Controlla se la linea ha raggiunto la lunghezza massima
            if (Vector3.Distance(lineRenderer.GetPosition(0), currentPosition) >= maxLineLength)
            {
                isDrawing = false;
                lineRenderer.enabled = false;
            }
        }
    }

    public void StartDrawing(Vector3 startPoint, Vector3 initialDirection)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint + initialDirection * maxLineLength);
        targetPosition = startPoint + initialDirection * maxLineLength;
        isDrawing = true;
    }
}
