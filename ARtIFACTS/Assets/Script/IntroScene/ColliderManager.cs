using UnityEngine;
using System.Collections;


public class ColliderManager : MonoBehaviour
{
    
    public GameObject metaballObject; // Il GameObject Metaball
    public GameObject objectToActivate; // Il GameObject da attivare
    public GameObject objectToDeactivate; // Il GameObject da disattivare
    public GameObject nextColiderToActivate; // Il prossimo collider dell'isztallazione da attivare
    public int audioClipIndex = 1; // L'indice dell'audio clip da riprodurre
    private AudioClip[] metaballSounds; // Array di suoni da Metaball
    private AudioSource audioSource;
    private bool hasCollided = false;


    private void Start()
    {
        // Ottieni il componente AudioSource dal GameObject Metaball
        audioSource = metaballObject.GetComponent<AudioSource>();

        // Ottieni l'array di clip audio dal metaballObject
        metaballSounds = metaballObject.GetComponent<Metaball>().voiceOverSounds;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il giocatore è entrato in collisione
        if (other.CompareTag("Player") && !hasCollided)
        {
            // Riproduci il suono VoiceOver dalla lista del GameObject Metaball
            if (audioSource != null && metaballSounds.Length > 0 && audioClipIndex < metaballSounds.Length)
            {
                audioSource.clip = metaballSounds[audioClipIndex];
                audioSource.Play();
            }

            // Attiva il GameObject specificato
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                nextColiderToActivate.SetActive(true);
            }

            // Attiva la gravità sui cloni
            ActivateGravityOnClones();

            // Avvia la coroutine per distruggere i cloni dopo 5 secondi e poi disattivare l'oggetto
            StartCoroutine(DestroyAndDeactivate());
            
            // Imposta hasCollided su true per evitare collisioni multiple
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

    private IEnumerator DestroyAndDeactivate()
    {
        yield return new WaitForSeconds(20f);
        DestroyClones();

        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
        }
    }

    // Funzione per distruggere i cloni con il tag "cloni"
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
