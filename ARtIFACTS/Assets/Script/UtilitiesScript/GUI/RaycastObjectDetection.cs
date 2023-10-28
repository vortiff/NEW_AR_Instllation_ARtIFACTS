using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastObjectDetection : MonoBehaviour
{
    public float detectionTime = 3.0f;
    private float currentDetectionTime = 0.0f;
    private bool isDetecting = false;
    private RandomDeformationColorOPT detectedObjectScript;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        // Aggiunta del Debug.DrawRay
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.blue);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f) && hit.transform.CompareTag("Metaball"))
        {
            if (!isDetecting)
            {
                isDetecting = true;
                currentDetectionTime = 0.0f;
                detectedObjectScript = hit.transform.GetComponent<RandomDeformationColorOPT>();
            }
            currentDetectionTime += Time.deltaTime;

            if (detectedObjectScript)
            {
                float intensityFactor = Mathf.Clamp(currentDetectionTime / detectionTime, 0, 1);
                detectedObjectScript.deformationIntensity = Mathf.Lerp(1.13f, 1.8f, intensityFactor);
            }

            if (currentDetectionTime >= detectionTime)
            {
                OpenGUIMenu();
            }
        }
        else
        {
            if (isDetecting && detectedObjectScript)
            {
                detectedObjectScript.deformationIntensity = 1.13f;
            }

            isDetecting = false;
            currentDetectionTime = 0.0f;
            detectedObjectScript = null;
        }
    }

    void OpenGUIMenu()
    {
        // Qui inserisci il codice per visualizzare il tuo menu GUI
    }
}
