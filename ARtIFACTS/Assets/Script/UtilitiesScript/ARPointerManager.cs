using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPointerManager : MonoBehaviour
{
    public ARPlaneManager planeManager; // Riferimento al tuo ARPlaneManager
    public GameObject prefabToPlace; // Il prefab da posizionare
    private GameObject spawnedObject; // L'istanza attuale del prefab

    private LineRenderer lineRenderer;
    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Lancio un raggio dal punto di tocco dello schermo
                if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    // Ottieni la posizione in cui il raggio colpisce il piano
                    Vector3 hitPose = hits[0].pose.position;

                    // Posiziona il prefab
                    PlacePrefab(hitPose);
                }
            }
        }

        // Aggiorna il puntatore in base alla posizione della fotocamera
        UpdatePointer();
    }

    void PlacePrefab(Vector3 position)
    {
        // Rimuovi l'istanza esistente del prefab se presente
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        // Crea e posiziona il nuovo prefab
        spawnedObject = Instantiate(prefabToPlace, position, Quaternion.identity);
    }

    void UpdatePointer()
    {
        // Aggiorna la posizione del puntatore in base alla posizione della fotocamera
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 endPoint = cameraPosition + Camera.main.transform.forward * 5f; // Lunghezza del puntatore

        lineRenderer.SetPosition(0, cameraPosition);
        lineRenderer.SetPosition(1, endPoint);
    }
}
