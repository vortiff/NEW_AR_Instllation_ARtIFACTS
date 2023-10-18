using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DistortAndDissolveStatue : MonoBehaviour
{
    public float expansionSpeed = 0.1f;
    public float maxRadius = 5f;
    public float deformationIntensity = 0.1f;
    public float glitchFrequency = 5.0f; // How often the glitch happens in seconds

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private float nextGlitchTime;

    void Start()
    {
        // Memorize the original mesh of the ball
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;

        nextGlitchTime = Time.time + Random.Range(0f, glitchFrequency);
    }

    void Update()
    {
        // Expansion of the metaball
        if (transform.localScale.x < maxRadius)
        {
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }

        if (Time.time > nextGlitchTime)
        {
            // Deformation of the mesh vertices
            DeformMesh();
            nextGlitchTime = Time.time + Random.Range(0f, glitchFrequency);
        }
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

