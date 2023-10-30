using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeformationColorOPTLevel2 : MonoBehaviour
{
    public float expansionSpeed = 38.6f;
    public float maxRadius = 1.66f;
    public float deformationIntensity = 1.13f;
    public float colorChangeSpeed = 0.5f;

    public int deformationFrameRate = 30;
    private int currentFrame = 0;

    public Color minColor = Color.red;
    public Color maxColor = Color.blue;

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Material material;
    private float timeElapsed = 0f;
    private Vector3 perlinNoiseOffset;
    private Color currentColor;

    void Start()
    {
        perlinNoiseOffset = new Vector3(Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f));
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;

        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    void Update()
    {
        currentFrame++;

        if (transform.localScale.x < maxRadius)
        {
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }

        if(currentFrame >= deformationFrameRate)
        {
            DeformMesh();
            currentFrame = 0;
        }

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
            float randomValueX = Mathf.PerlinNoise(vertices[i].x * deformationIntensity + perlinNoiseOffset.x, Time.time + perlinNoiseOffset.x) * deformationIntensity;
            float randomValueY = Mathf.PerlinNoise(vertices[i].y * deformationIntensity + perlinNoiseOffset.y, Time.time + perlinNoiseOffset.y) * deformationIntensity;
            float randomValueZ = Mathf.PerlinNoise(vertices[i].z * deformationIntensity + perlinNoiseOffset.z, Time.time + perlinNoiseOffset.z) * deformationIntensity;

            vertices[i] = originalVertices[i] + new Vector3(randomValueX, randomValueY, randomValueZ);
        }

        originalMesh.vertices = vertices;
        originalMesh.RecalculateNormals();
    }

    void ChangeMaterialColor()
    {
        float t = Random.Range(0f, 1f);
        currentColor = Color.Lerp(minColor, maxColor, t);
        material.color = currentColor;
    }
}
