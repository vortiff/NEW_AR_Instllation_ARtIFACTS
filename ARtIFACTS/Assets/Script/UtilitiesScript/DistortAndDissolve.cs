using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DistortAndDissolve : MonoBehaviour
{
    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Vector3[] distortedVertices;

    public float distortionStrength = 0.1f;
    public float dissolveRate = 0.01f;
    private bool isDissolving = false;

    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = originalMesh.vertices;
        distortedVertices = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        if (isDissolving)
        {
            // Distort mesh
            for (int i = 0; i < originalVertices.Length; i++)
            {
                Vector3 offset = Random.insideUnitSphere * distortionStrength;
                distortedVertices[i] = originalVertices[i] + offset;
            }
            originalMesh.vertices = distortedVertices;
            originalMesh.RecalculateNormals();

            // TODO: Add code to dissolve parts of the mesh over time.
        }
    }

    public void StartDissolve()
    {
        isDissolving = true;
    }
}
