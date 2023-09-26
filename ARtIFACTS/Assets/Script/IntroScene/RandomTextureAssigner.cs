using UnityEngine;
using System.Collections.Generic;

public class RandomTextureAssigner : MonoBehaviour
{
    public List<Texture2D> textures = new List<Texture2D>();
    public Renderer targetRenderer;

    void Start()
    {
        // Assicurati che ci siano delle texture nell'array
        if (textures.Count == 0)
        {
            Debug.LogError("Nessuna texture nell'array.");
            return;
        }

        // Ottieni un indice casuale per selezionare un'immagine dall'array
        int randomIndex = Random.Range(0, textures.Count);

        // Assegna l'immagine casuale al renderer del GameObject target
        if (targetRenderer != null)
        {
            targetRenderer.material.mainTexture = textures[randomIndex];
        }
        else
        {
            Debug.LogError("Renderer non assegnato al GameObject target.");
        }
    }
}
