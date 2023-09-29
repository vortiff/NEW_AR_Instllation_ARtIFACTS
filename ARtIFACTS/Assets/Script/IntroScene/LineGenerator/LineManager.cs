using UnityEngine;

public class LineManager : MonoBehaviour
{
    public GameObject linePrefab; // Prefab della linea
    public LayerMask collisionLayer; // Layer per la collisione

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateLine();
        }
    }

    private void CreateLine()
    {
        Vector3 startPoint = transform.position; // Usa la posizione del LineManager come punto di partenza
        Vector3 initialDirection = Random.onUnitSphere; // Direzione casuale

        GameObject newLine = Instantiate(linePrefab, startPoint, Quaternion.identity);
        LineDrawer lineDrawer = newLine.GetComponent<LineDrawer>();
        lineDrawer.collisionLayer = collisionLayer;

        lineDrawer.StartDrawing(startPoint, initialDirection);
    }
}
