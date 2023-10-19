using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderManager : MonoBehaviour
{
    public GameObject player; 
    public GameObject metaballObject;
    public GameObject objectToActivate;
    public GameObject objectToDeactivate;
    public GameObject nextColiderToActivate;
    public GameObject attractionTarget; // Aggiunto l'attractionTarget
    public float attractionDelay = 5f; // Tempo di attesa prima che Metaball venga attratto
    public int audioClipIndex = 1;
    private AudioClip[] metaballSounds;
    private AudioSource audioSource;
    private bool hasCollided = false;
    private List<GameObject> cloneList = new List<GameObject>();

    private void Start()
    {
        audioSource = metaballObject.GetComponent<AudioSource>();
        metaballSounds = metaballObject.GetComponent<Metaball>().voiceOverSounds;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && !hasCollided)
        {
            if (audioSource != null && metaballSounds.Length > 0 && audioClipIndex < metaballSounds.Length)
            {
                audioSource.clip = metaballSounds[audioClipIndex];
                audioSource.Play();
            }

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                nextColiderToActivate.SetActive(true);
            }

            ActivateGravityOnClones();
            StartCoroutine(AttractMetaballAfterDelay());

            hasCollided = true;
        }
    }

    private void ActivateGravityOnClones()
    {
        foreach (GameObject clone in cloneList)
        {
            Rigidbody rb = clone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }
        }
    }

    private IEnumerator AttractMetaballAfterDelay()
    {
        yield return new WaitForSeconds(attractionDelay);

        // Fai in modo che Metaball segua l'attractionTarget
        metaballObject.transform.LookAt(attractionTarget.transform);
        Rigidbody metaballRb = metaballObject.GetComponent<Rigidbody>();
        metaballRb.velocity = metaballObject.transform.forward * 5; // 5 è la velocità con cui Metaball si muove verso l'attractionTarget

        // Riproduci il suono con indice 3
        if (audioSource != null && metaballSounds.Length > 3)
        {
            audioSource.clip = metaballSounds[3];
            audioSource.Play();
        }

        // Continua con la distruzione dei cloni e la disattivazione dell'oggetto
        StartCoroutine(DestroyAndDeactivate());
    }

    private IEnumerator DestroyAndDeactivate()
    {
        yield return StartCoroutine(FadeOutClonesOverTime(2f)); // Fade out dei cloni in 2 secondi
        DestroyClones();

        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
        }
    }

    private void DestroyClones()
    {
        foreach (GameObject clone in cloneList)
        {
            Destroy(clone);
        }
        cloneList.Clear();
        Debug.Log("Cloni sono stati Distrutti!");
    }

    private IEnumerator FadeOutClonesOverTime(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            foreach (GameObject clone in cloneList)
            {
                Renderer rend = clone.GetComponent<Renderer>();
                if (rend != null && rend.material.HasProperty("_Color"))
                {
                    Color originalColor = rend.material.GetColor("_Color");
                    originalColor.a = Mathf.Lerp(1f, 0f, t); // Interpolazione dell'alpha
                    rend.material.SetColor("_Color", originalColor);
                }
            }
            yield return null;
        }
    }
}
