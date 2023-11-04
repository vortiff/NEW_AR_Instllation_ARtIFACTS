using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    public Text logText; // Riferimento al componente Text dell'UI per i log.
    public Text fpsText; // Riferimento al componente Text dell'UI per gli FPS.

    private float deltaTime = 0.0f;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        // Calcola il deltaTime per ottenere i FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = string.Format("{0:0.} fps", fps);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Aggiungi il log al testo esistente con una riga vuota dopo
        logText.text += logString + "\n\n"; // Aggiunto un secondo "\n" per la riga vuota
    }
}
