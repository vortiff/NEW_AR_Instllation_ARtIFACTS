using UnityEngine;
using UnityEngine.UI;
using NativeCameraNamespace;

public class TakeSelfieButton : MonoBehaviour
{
    public RawImage rawImageDisplay;
    public GameObject[] texturedObjects; // Array di GameObject ai quali applicare le texture
    private int currentTextureIndex = 0; // Indice del GameObject corrente

    public void OnButtonPressed()
    {
        // Request camera permission before taking a picture
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
                //Debug.Log("Picture path: " + path);
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
            //Debug.LogError("rawImageDisplay is not set!");
            return;
        }

        // Apply texture to the current GameObject and increment or reset index
        GameObject currentObject = texturedObjects[currentTextureIndex];
        if (currentObject != null)
        {
            Renderer renderer = currentObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = texture;
                // Modifica i parametri dello shader in maniera casuale
                RandomizeShaderParameters(renderer.material);
                // Move to the next object or loop back to the start if we're at the end
                currentTextureIndex = (currentTextureIndex + 1) % texturedObjects.Length;
            }
            else
            {
                Debug.LogError("Renderer not found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("GameObject at index " + currentTextureIndex + " is not set.");
        }
    }

    private void RandomizeShaderParameters(Material material)
    {
        // Genera valori casuali per i parametri dello shader
        material.SetFloat("_ChromAberrAmountX", Random.Range(-2f, 2f)); // Assumi che vuoi un effetto che va da -2 a 2 per esempio
        material.SetFloat("_ChromAberrAmountY", Random.Range(-2f, 2f));
        material.SetFloat("_RightStripesAmount", Random.Range(0, 10)); // Valori da 0 a 10
        material.SetFloat("_RightStripesFill", Random.Range(0f, 1f)); // Valori tra 0 e 1
        material.SetFloat("_LeftStripesAmount", Random.Range(0, 10));
        material.SetFloat("_LeftStripesFill", Random.Range(0f, 1f));
        material.SetVector("_DisplacementAmount", new Vector4(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0, 0)); // Assumi che solo x e y siano rilevanti
        material.SetFloat("_WavyDisplFreq", Random.Range(0, 20)); // Valori da 0 a 20
    }
}
