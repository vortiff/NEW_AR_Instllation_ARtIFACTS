using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject objectToDuplicate;
    [Tooltip("Questi sono i GameObjects di riferimento per il posizionamento.")]
    public Transform[] referenceObjects; // Array per trascinare i GameObject di riferimento.
    private int currentReferenceIndex = 0;
    public Transform player; // Variabile pubblica per player
    private int playerInsideColliderCount = 0;
    private List<GameObject> inactiveObjects = new List<GameObject>(); // Nuova lista per gli oggetti inattivi
    private List<int> activeReferenceIndices = new List<int>();


    [Header("Duplicant References")]
    public int poolSize = 30; // Dimensione iniziale dell'object pool
    public List<TextureList> texturesForReferenceObjects = new List<TextureList>();
    [System.Serializable]
    public class TextureList
    {
        public List<Texture2D> textures = new List<Texture2D>();
    }
    private MaterialPropertyBlock propBlock;
    private List<List<GameObject>> objectPools = new List<List<GameObject>>();
    public float duplicationWidth = 5f;
    public float duplicationHeight = 20f;
    public float duplicationDepth = 5f;
    public float minDuplicationInterval = 0.4f;
    public float maxDuplicationInterval = 1f;
    private float nextDuplicationTime;
    private Queue<GameObject> activeObjectsQueue = new Queue<GameObject>();
    private Dictionary<GameObject, int> objectToPoolIndex = new Dictionary<GameObject, int>();
    

    
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

        propBlock = new MaterialPropertyBlock();
    }

    // Questa funzione trova il GameObject di riferimento in cui il Player si trova.
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
        if(IsPlayerInsideAnyCollider())
        {
            if (Time.time >= nextDuplicationTime)
            {
                DuplicateObject();
                SetNextDuplicationTime();
            }
        }
    }



    bool IsPlayerInsideAnyCollider()
    {
        return playerInsideColliderCount > 0; // se il contatore è maggiore di 0, il giocatore è all'interno di almeno un collider
    }

    void DuplicateObject()
    {
         // Seleziona un indice di riferimento casuale tra quelli attivi
        int randomIndex = Random.Range(0, activeReferenceIndices.Count);
        currentReferenceIndex = activeReferenceIndices[randomIndex]; // Aggiorniamo l'indice corrente
        Transform currentReference = referenceObjects[currentReferenceIndex];

        GameObject newObject = GetFromPool();
        if (newObject == null)
        {
            Debug.LogError("Non è stato possibile ottenere un nuovo oggetto dalla pool.");
            return;
        }

        if (activeObjectsQueue.Count >= objectPools[currentReferenceIndex].Count)
        {
            RemoveOldestObject();
        }

        AssignRandomTexture(newObject);

        Vector3 randomOffset = new Vector3(
            Random.Range(-duplicationWidth / 2, duplicationWidth / 2),
            Random.Range(-duplicationHeight / 2, duplicationHeight / 2),
            Random.Range(-duplicationDepth / 2, duplicationDepth / 2)
        );

        Vector3 newPosition = currentReference.position + randomOffset;
        newObject.transform.position = newPosition;
        newObject.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        newObject.transform.SetParent(this.transform);
        newObject.SetActive(true);
        activeObjectsQueue.Enqueue(newObject);

        Debug.Log("Oggetto duplicato e posizionato in: " + currentReference);

        PlayRandomAudio();
    }


    GameObject GetFromPool()
    {
        List<GameObject> currentPool = objectPools[currentReferenceIndex];
        if (currentPool.Count > 0)
        {
            GameObject obj = currentPool[0];
            currentPool.RemoveAt(0);
            inactiveObjects.Remove(obj);  // Assicurati di rimuoverlo dagli oggetti inattivi
            objectToPoolIndex[obj] = currentReferenceIndex;
            return obj;
        }
        return null;
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
            if (!currentReferenceAudio.isPlaying)
            {
                currentReferenceAudio.Play();
            }
        }
    }


    void AssignRandomTexture(GameObject obj)
    {
        List<Texture2D> currentReferenceTextures = texturesForReferenceObjects[currentReferenceIndex].textures;
        if (currentReferenceTextures.Count == 0)
        {
            Debug.LogError("Nessuna texture nell'array per il reference object corrente.");
            return;
        }

        int randomIndex = Random.Range(0, currentReferenceTextures.Count);
        Renderer objRenderer = obj.GetComponent<Renderer>();

        if (objRenderer != null)
        {
            // Imposta la texture nel MaterialPropertyBlock
            propBlock.SetTexture("BaseMap", currentReferenceTextures[randomIndex]);

            // Applica il MaterialPropertyBlock al renderer
            objRenderer.SetPropertyBlock(propBlock);
        }
        else
        {
            Debug.LogError("Renderer non trovato nel GameObject duplicato.");
        }
    }


    void RemoveOldestObject()
    {
        if (activeObjectsQueue.Count > 0)
        {
            GameObject oldestObject = activeObjectsQueue.Dequeue();
            oldestObject.SetActive(false);

            // Usa il dizionario per determinare a quale pool l'oggetto appartiene
            int poolIndex = objectToPoolIndex[oldestObject];
            List<GameObject> correctPool = objectPools[poolIndex];
            correctPool.Add(oldestObject); // Aggiungi l'oggetto alla pool corretta

            inactiveObjects.Add(oldestObject); // Aggiungi l'oggetto alla lista degli inattivi
            objectToPoolIndex.Remove(oldestObject); // Rimuovi l'oggetto dal dizionario

            PlayRandomAudio();
            Debug.Log("RemoveOldestObject PlayRandomAudio " + poolIndex);
   
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
}
