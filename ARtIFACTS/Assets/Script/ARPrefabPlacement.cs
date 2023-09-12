using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPrefabPlacement : MonoBehaviour
{
    public GameObject prefabToPlace;

    private void Start()
    {
        // Registra la funzione per gestire l'individuazione di un piano.
        ARPlaneManager planeManager = FindObjectOfType<ARPlaneManager>();
        planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Controlla se è stato individuato almeno un piano.
        if (args.added.Count > 0)
        {
            // Prendi il primo piano individuato (puoi aggiungere la logica per selezionare il piano desiderato).
            ARPlane detectedPlane = args.added[0];

            // Calcola la posizione in cui posizionare il prefab, ad esempio sopra il piano.
            Vector3 prefabPosition = detectedPlane.center;

            // Crea il prefab e posizionalo.
            Instantiate(prefabToPlace, prefabPosition, Quaternion.identity);
        }
    }
}
