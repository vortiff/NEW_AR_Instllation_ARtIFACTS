using System.Collections;
using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    public Material glitchMaterial; // Il materiale con il tuo shader

    public Texture2D[] glitchTextures; // Un array di texture che vuoi usare per l'effetto glitch

    private int currentTextureIndex = 0;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = glitchMaterial;
        StartCoroutine(ChangeTexture());
    }

    IEnumerator ChangeTexture()
    {
        while (true)
        {
            // Cambia la texture ogni tot di tempo (ad esempio, ogni 2 secondi)
            yield return new WaitForSeconds(2f);

            currentTextureIndex = (currentTextureIndex + 1) % glitchTextures.Length;

            // Imposta la nuova texture nel materiale
            glitchMaterial.SetTexture("_MainTex", glitchTextures[currentTextureIndex]);
        }
    }
}
