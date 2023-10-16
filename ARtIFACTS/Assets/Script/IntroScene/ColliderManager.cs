using UnityEngine;
using System.Collections;

public class ColliderManager : MonoBehaviour
{
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

    private void Start()
    {
        audioSource = metaballObject.GetComponent<AudioSource>();
        metaballSounds = metaballObject.GetComponent<Metaball>().voiceOverSounds;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasCollided)
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
        GameObject[] clones = GameObject.FindGameObjectsWithTag("cloni");
        foreach (GameObject clone in clones)
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

        // Riproduci il suono con indice 4
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
        yield return new WaitForSeconds(20f);
        DestroyClones();

        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
        }
    }

    private void DestroyClones()
    {
        GameObject[] clones = GameObject.FindGameObjectsWithTag("cloni");
        foreach (GameObject clone in clones)
        {
            Destroy(clone);
            Debug.Log("Cloni sono stati Distrutti!");
        }
    }
}
