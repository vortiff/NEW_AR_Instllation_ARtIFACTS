using UnityEngine;

public class RandomDeformationColorOPT : MonoBehaviour
{
    public float expansionSpeed = 38.6f;
    public float maxRadius = 1.66f;
    public float deformationIntensity = 1.13f;
    public float colorChangeSpeed = 0.5f; // Velocità di cambio colore, maggiore è il valore, più veloce sarà il cambio colore.

    public int deformationFrameRate = 30; // Deforma la mesh ogni 10 frame
    private int currentFrame = 0;

    public Color minColor = Color.red; // Colore minimo del range
    public Color maxColor = Color.blue; // Colore massimo del range

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Material material;
    private float timeElapsed = 0f;
    private Vector3 perlinNoiseOffset;


    void Start()
    {
        perlinNoiseOffset = new Vector3(Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f));
        // Memorizza la mesh originale della palla
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;

        // Ottieni il materiale assegnato alla palla
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    void Update()
    {
        currentFrame++;

        if(currentFrame >= deformationFrameRate)
        {
            DeformMesh();
            currentFrame = 0;
        }
        // Espansione della metaball
        if (transform.localScale.x < maxRadius)
        {
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }

        // Deformazione dei vertici della mesh
        DeformMesh();

        // Cambio casuale del colore del materiale
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= colorChangeSpeed)
        {
            ChangeMaterialColor();
            timeElapsed = 0f;
        }
    }

    void DeformMesh()
    {
        Vector3[] vertices = originalMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Applica una deformazione casuale usando "perlin noise"
            float randomValueX = Mathf.PerlinNoise(vertices[i].x * deformationIntensity + perlinNoiseOffset.x, Time.time + perlinNoiseOffset.x) * deformationIntensity;
            float randomValueY = Mathf.PerlinNoise(vertices[i].y * deformationIntensity + perlinNoiseOffset.y, Time.time + perlinNoiseOffset.y) * deformationIntensity;
            float randomValueZ = Mathf.PerlinNoise(vertices[i].z * deformationIntensity + perlinNoiseOffset.z, Time.time + perlinNoiseOffset.z) * deformationIntensity;


            vertices[i] = originalVertices[i] + new Vector3(randomValueX, randomValueY, randomValueZ);
        }

        // Update the mesh with the new deformed vertices
        originalMesh.vertices = vertices;
        originalMesh.RecalculateNormals();
    }

    void ChangeMaterialColor()
    {
        // Genera un colore casuale all'interno del range definito
        Color randomColor = new Color(
            Random.Range(minColor.r, maxColor.r),
            Random.Range(minColor.g, maxColor.g),
            Random.Range(minColor.b, maxColor.b),
            1f
        );

        // Applica il colore casuale al materiale della palla
        material.color = randomColor;
    }
}
