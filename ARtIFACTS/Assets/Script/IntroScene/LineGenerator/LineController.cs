using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public float speed = 5.0f;  // Velocità della linea
    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3 currentDirection;
    private Vector3[] vertices;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        vertices = new Vector3[2];
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 1, 0 };  // Crea due vertici collegati da un segmento di linea
        currentDirection = Random.insideUnitCircle.normalized;  // Direzione casuale iniziale su X e Z
    }

    void Update()
    {
        Vector3 newPosition = vertices[1] + new Vector3(currentDirection.x, 0, currentDirection.y) * speed * Time.deltaTime;
        vertices[0] = vertices[1];
        vertices[1] = newPosition;
        mesh.vertices = vertices;

        // Controlla se la linea ha colpito il collider del padre
        RaycastHit hit;
        if (Physics.Linecast(vertices[0], newPosition, out hit))
        {
            if (hit.collider.transform.parent == transform.parent)
            {
                // Cambia direzione in modo casuale su X e Z
                currentDirection = Random.insideUnitCircle.normalized;
            }
        }
    }
}