using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    public GameObject objectToDuplicate;
    public List<Texture2D> textures = new List<Texture2D>(); // Lista delle texture
    public float duplicationWidth = 1f;
    public float duplicationHeight = 5f;
    public float duplicationDepth = 1f;
    public float minDuplicationInterval = 0.1f;
    public float maxDuplicationInterval = 2f;
    public MediaLibrary mediaLibrary;
    public Transform player; // Variabile pubblica per player
    public int poolSize = 10; // Dimensione iniziale dell'object pool

    private AudioSource audioSource;
    private bool isPlayerInsideCollider = false;
    private float nextDuplicationTime;
    private List<GameObject> objectPool = new List<GameObject>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectToDuplicate);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    void Update()
    {
        if (Time.time >= nextDuplicationTime && isPlayerInsideCollider)
        {
            DuplicateObject();
            SetNextDuplicationTime();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player) // Confronto con il riferimento player
        {
            isPlayerInsideCollider = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player) // Confronto con il riferimento player
        {
            isPlayerInsideCollider = false;
        }
    }

    void DuplicateObject()
    {
        // Prendi un oggetto dal pool
        GameObject newObject = GetFromPool();
        if (newObject == null) return;
        
        // Assegna una texture casuale
        AssignRandomTexture(newObject);

        Vector3 randomOffset = new Vector3(
            Random.Range(-duplicationWidth / 2, duplicationWidth / 2),
            Random.Range(-duplicationHeight / 2, duplicationHeight / 2),
            Random.Range(-duplicationDepth / 2, duplicationDepth / 2)
        );

        Vector3 newPosition = transform.position + randomOffset;
        newObject.transform.position = newPosition;
        newObject.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        newObject.SetActive(true);

        PlayRandomAudio();
    }

    GameObject GetFromPool()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }
        return null; // nessun oggetto disponibile nel pool
    }

    void SetNextDuplicationTime()
    {
        nextDuplicationTime = Time.time + Random.Range(minDuplicationInterval, maxDuplicationInterval);
    }

    void PlayRandomAudio()
    {
        if (audioSource != null && mediaLibrary.audioClips.Length > 0 && !audioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, mediaLibrary.audioClips.Length);
            audioSource.clip = mediaLibrary.audioClips[randomIndex];
            audioSource.Play();
        }
    }
    void AssignRandomTexture(GameObject obj)
    {
        if (textures.Count == 0)
        {
            Debug.LogError("Nessuna texture nell'array.");
            return;
        }

        int randomIndex = Random.Range(0, textures.Count);
        Renderer objRenderer = obj.GetComponent<Renderer>();

        if (objRenderer != null)
        {
            objRenderer.material.mainTexture = textures[randomIndex];
        }
        else
        {
            Debug.LogError("Renderer non trovato nel GameObject duplicato.");
        }
    }
}
