using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceObjectScript : MonoBehaviour
{
    private Duplicator duplicatorManager;
    private int referenceIndex;
    [SerializeField] private Transform player;
    private HashSet<int> activeColliders = new HashSet<int>();


    private void Start()
    {
        duplicatorManager = FindObjectOfType<Duplicator>();
        referenceIndex = -1;
        
        // Find the reference index
        for(int i = 0; i < duplicatorManager.referenceObjects.Length; i++)
        {
            if(duplicatorManager.referenceObjects[i] == this.transform)
            {
                referenceIndex = i;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == duplicatorManager.player)
        {
            activeColliders.Add(this.GetInstanceID());
            duplicatorManager.IncrementPlayerInsideColliderCount(referenceIndex);
        }
    }

   private void OnTriggerExit(Collider other)
    {
        if (other.transform == duplicatorManager.player && activeColliders.Contains(this.GetInstanceID()))
        {
            activeColliders.Remove(this.GetInstanceID());
            duplicatorManager.DecrementPlayerInsideColliderCount(referenceIndex);
        }
    }

}
