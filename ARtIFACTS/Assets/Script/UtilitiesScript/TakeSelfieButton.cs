using UnityEngine;
using UnityEngine.UI;
using NativeCameraNamespace;

public class TakeSelfieButton : MonoBehaviour
{
    public RawImage rawImageDisplay;
    [Header("Texture dei referenceObjects")]
    public GameObject[] texturedObjects; // Array di GameObject ai quali applicare le texture
    private int currentTextureIndex = 0; // Indice del GameObject corrente
    public Duplicator duplicatorScript; // Riferimento all'oggetto Duplicator

    public void OnButtonPressed()
    {
        // Richiesta di permesso per la fotocamera prima di scattare una foto
        NativeCamera.Permission permission = NativeCamera.CheckPermission(true);

        if (permission == NativeCamera.Permission.Granted)
        {
            TakePicture(512);
        }
        else if (permission == NativeCamera.Permission.ShouldAsk)
        {
            NativeCamera.RequestPermission(true);
        }
        else
        {
            Debug.Log("Permission denied by the user");
        }
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.TakePicture((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                ApplyTextureToGameObject(path, maxSize);
            }
            else
            {
                Debug.LogError("Failed to get the picture path.");
            }
        }, maxSize, preferredCamera: NativeCamera.PreferredCamera.Front);
    }

    private void ApplyTextureToGameObject(string path, int maxSize)
    {
        Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
        if (texture == null)
        {
            Debug.LogError("Failed to load texture at path: " + path);
            return;
        }

        if (rawImageDisplay != null)
        {
            rawImageDisplay.texture = texture;
        }
        else
        {
            return;
        }

        // Se duplicatorScript è impostato, usa quello per applicare la texture
        if (duplicatorScript != null)
        {
            // Aggiorna la texture del referenceObject corrente
            GameObject currentObject = texturedObjects[currentTextureIndex];
            Renderer currentRenderer = currentObject.GetComponent<Renderer>();
            if (currentRenderer != null)
            {
                currentRenderer.material.mainTexture = texture;
            }

            // Aggiorna le texture di tutti gli objectToDuplicate inattivi
            duplicatorScript.UpdateInactiveObjectsTextures(currentTextureIndex);

            // Incrementa l'indice per il prossimo utilizzo
            currentTextureIndex = (currentTextureIndex + 1) % texturedObjects.Length;
        }
        else
        {
            Debug.LogError("Duplicator script not found in the scene!");
        }
}

}
