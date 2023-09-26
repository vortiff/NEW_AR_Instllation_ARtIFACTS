using UnityEngine;

public class ActivationManager : MonoBehaviour
{
    public void ActivateObjectsWithTag()
    {
        GameObject[] objectsToActivate = GameObject.FindGameObjectsWithTag("Installation");

        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }
    }
}
