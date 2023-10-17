using UnityEngine;

public class RandomDeformation : MonoBehaviour
{
    public float expansionSpeed = 0.1f;
    public float maxRadius = 5f;
    public float deformationIntensity = 0.1f;

    private Mesh originalMesh;
    private Vector3[] originalVertices;

    void Start()
    {
        // Memorize the original mesh of the ball
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;
    }

    void Update()
    {
        // Expansion of the metaball
        if (transform.localScale.x < maxRadius)
        {
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }

        // Deformation of the mesh vertices
        DeformMesh();
    }

    void DeformMesh()
    {
        Vector3[] vertices = originalMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Apply random deformation using "perlin noise"
            float randomValueX = Mathf.PerlinNoise(vertices[i].x * deformationIntensity, Time.time) * deformationIntensity;
            float randomValueY = Mathf.PerlinNoise(vertices[i].y * deformationIntensity, Time.time) * deformationIntensity;
            float randomValueZ = Mathf.PerlinNoise(vertices[i].z * deformationIntensity, Time.time) * deformationIntensity;

            vertices[i] = originalVertices[i] + new Vector3(randomValueX, randomValueY, randomValueZ);
        }

        // Update the mesh with the new deformed vertices
        originalMesh.vertices = vertices;
        originalMesh.RecalculateNormals();
    }
}
