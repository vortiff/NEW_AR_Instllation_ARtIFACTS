using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject objectToDuplicate;
    [Tooltip("Questi sono i GameObjects di riferimento per il posizionamento.")]
    public Transform[] referenceObjects;
    private int currentReferenceIndex = 0;
    public Transform player;
    private int playerInsideColliderCount = 0;
    private List<GameObject> inactiveObjects = new List<GameObject>();
    private List<int> activeReferenceIndices = new List<int>();

    [Header("Duplicant References")]
    public int poolSize = 30;
    private List<List<GameObject>> objectPools = new List<List<GameObject>>();
    public float duplicationWidth = 5f;
    public float duplicationHeight = 20f;
    public float duplicationDepth = 5f;
    public float minDuplicationInterval = 0.4f;
    public float maxDuplicationInterval = 1f;
    private float nextDuplicationTime;
    private Queue<GameObject> activeObjectsQueue = new Queue<GameObject>();

    [Header("Audio Settings")]
    public MediaLibrary mediaLibrary;
    private AudioSource audioSource;
    private AudioSource[] referenceAudioSources;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        referenceAudioSources = new AudioSource[referenceObjects.Length];
        for (int i = 0; i < referenceObjects.Length; i++)
        {
            referenceAudioSources[i] = referenceObjects[i].GetComponent<AudioSource>();
        }

        for (int i = 0; i < referenceObjects.Length; i++)
        {
            List<GameObject> subPool = new List<GameObject>();
            for (int j = 0; j < poolSize; j++)
            {
                GameObject obj = Instantiate(objectToDuplicate);
                obj.SetActive(false);
                subPool.Add(obj);
            }
            objectPools.Add(subPool);
        }

        // Assegna le texture iniziali ai duplicati
        for (int i = 0; i < referenceObjects.Length; i++)
        {
            Renderer refRenderer = referenceObjects[i].GetComponent<Renderer>();
            Texture2D texture = refRenderer.material.mainTexture as Texture2D;
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();

            foreach (GameObject obj in objectPools[i])
            {
                Renderer objRenderer = obj.GetComponent<Renderer>();
                objRenderer.GetPropertyBlock(mpb);
                mpb.SetTexture("_MainTex", texture);
                RandomizeShaderParameters(mpb);
                objRenderer.SetPropertyBlock(mpb);
            }
        }
    }

    Transform GetReferenceWithPlayerInside()
    {
        foreach (Transform refObj in referenceObjects)
        {
            Collider refCollider = refObj.GetComponent<Collider>();
            if (refCollider && refCollider.bounds.Contains(player.position))
            {
                return refObj;
            }
        }
        return null;
    }

    void Update()
    {
        if (IsPlayerInsideAnyCollider() && Time.time >= nextDuplicationTime)
        {
            DuplicateObject();
            SetNextDuplicationTime();
        }
    }

    bool IsPlayerInsideAnyCollider()
    {
        return playerInsideColliderCount > 0;
    }

    void DuplicateObject()
    {
        int randomIndex = Random.Range(0, activeReferenceIndices.Count);
        currentReferenceIndex = activeReferenceIndices[randomIndex];
        Transform currentReference = referenceObjects[currentReferenceIndex];

        GameObject newObject = GetFromPool();
        if (newObject == null)
        {
            Debug.LogError("Unable to get a new object from the pool.");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-duplicationWidth / 2, duplicationWidth / 2),
            Random.Range(-duplicationHeight / 2, duplicationHeight / 2),
            Random.Range(0, duplicationDepth)
        );

        newObject.transform.position = currentReference.position + randomOffset;
        newObject.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        newObject.transform.SetParent(this.transform);
        newObject.SetActive(true);
        activeObjectsQueue.Enqueue(newObject);

        ApplyTextureAndRandomizeParameters(currentReferenceIndex);
        PlayRandomAudio();
    }

    GameObject GetFromPool()
    {
        List<GameObject> currentPool = objectPools[currentReferenceIndex];
        if (currentPool.Count > 0)
        {
            GameObject obj = currentPool[0];
            currentPool.RemoveAt(0);
            inactiveObjects.Remove(obj);
            PrepareObject(obj);
            return obj;
        }
        else
        {
            // Se non ci sono oggetti disponibili, riutilizza il più vecchio
            if (activeObjectsQueue.Count > 0)
            {
                GameObject obj = activeObjectsQueue.Dequeue();
                RemoveOldestObject();
                PrepareObject(obj);
                return obj;
            }
        }
        return null;
    }

    void PrepareObject(GameObject obj)
    {
        // Applica texture e parametri randomizzati al momento del prelievo dall'object pool
        Renderer objRenderer = obj.GetComponent<Renderer>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        Renderer refRenderer = referenceObjects[currentReferenceIndex].GetComponent<Renderer>();
        Texture2D texture = refRenderer.material.mainTexture as Texture2D;

        objRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainTex", texture);
        RandomizeShaderParameters(mpb);
        objRenderer.SetPropertyBlock(mpb);
    }



    void SetNextDuplicationTime()
    {
        nextDuplicationTime = Time.time + Random.Range(minDuplicationInterval, maxDuplicationInterval);
    }

    void PlayRandomAudio()
    {
        AudioSource currentReferenceAudio = referenceAudioSources[currentReferenceIndex];
        if (currentReferenceAudio != null && mediaLibrary.audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, mediaLibrary.audioClips.Length);
            currentReferenceAudio.clip = mediaLibrary.audioClips[randomIndex];
            currentReferenceAudio.Play();
        }
    }

    void RemoveOldestObject()
    {
        if (activeObjectsQueue.Count > 0)
        {
            GameObject oldestObject = activeObjectsQueue.Dequeue();
            oldestObject.SetActive(false);
            List<GameObject> correctPool = objectPools[currentReferenceIndex];
            correctPool.Add(oldestObject);
            inactiveObjects.Add(oldestObject);
            // Non riproduciamo l'audio qui poiché questa funzione ora viene utilizzata in un contesto diverso
        }
    }

    public void IncrementPlayerInsideColliderCount(int index)
    {
        playerInsideColliderCount++;
        if (!activeReferenceIndices.Contains(index))
        {
            activeReferenceIndices.Add(index);
        }
    }

    public void DecrementPlayerInsideColliderCount(int index)
    {
        playerInsideColliderCount--;
        if (activeReferenceIndices.Contains(index))
        {
            activeReferenceIndices.Remove(index);
        }
    }

    public void ApplyTextureAndRandomizeParameters(int referenceIndex)
    {
        if (referenceIndex < 0 || referenceIndex >= referenceObjects.Length)
        {
            Debug.LogError("Reference index is out of range.");
            return;
        }

        Renderer refRenderer = referenceObjects[referenceIndex].GetComponent<Renderer>();
        if (refRenderer == null)
        {
            Debug.LogError("Renderer not found on reference object.");
            return;
        }

        Texture2D texture = refRenderer.material.mainTexture as Texture2D;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        foreach (GameObject obj in objectPools[referenceIndex])
        {
            if (obj.activeInHierarchy)
            {
                Renderer objRenderer = obj.GetComponent<Renderer>();
                objRenderer.GetPropertyBlock(mpb);
                mpb.SetTexture("_MainTex", texture);
                RandomizeShaderParameters(mpb);
                objRenderer.SetPropertyBlock(mpb);
            }
        }
    }

    public void RandomizeShaderParameters(MaterialPropertyBlock mpb)
    {
        mpb.SetFloat("_ChromAberrAmountX", Random.Range(0f, 0.9f));
        mpb.SetFloat("_ChromAberrAmountY", Random.Range(0f, 0.9f));
        mpb.SetFloat("_RightStripesAmount", Random.Range(0f, 4f));
        mpb.SetFloat("_RightStripesFill", Random.Range(0f, 1f));
        mpb.SetFloat("_LeftStripesAmount", Random.Range(0f, 4f));
        mpb.SetFloat("_LeftStripesFill", Random.Range(0f, 1f));
        mpb.SetVector("_DisplacementAmount", new Vector4(Random.Range(0f, 0.9f), Random.Range(0f, 0.9f), 0f, 0f));
        mpb.SetFloat("_WavyDisplFreq", Random.Range(5f, 15f));
    }

    public void UpdateInactiveObjectsTextures(int referenceIndex)
    {
        if (referenceIndex < 0 || referenceIndex >= referenceObjects.Length)
        {
            Debug.LogError("Reference index is out of range.");
            return;
        }

        Renderer refRenderer = referenceObjects[referenceIndex].GetComponent<Renderer>();
        if (refRenderer == null)
        {
            Debug.LogError("Renderer not found on reference object.");
            return;
        }

        Texture2D texture = refRenderer.material.mainTexture as Texture2D;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        foreach (GameObject obj in objectPools[referenceIndex])
        {
            if (!obj.activeInHierarchy) // Aggiorna solo gli oggetti inattivi
            {
                Renderer objRenderer = obj.GetComponent<Renderer>();
                objRenderer.GetPropertyBlock(mpb);
                mpb.SetTexture("_MainTex", texture);
                RandomizeShaderParameters(mpb);
                objRenderer.SetPropertyBlock(mpb);
            }
        }
    }

}
