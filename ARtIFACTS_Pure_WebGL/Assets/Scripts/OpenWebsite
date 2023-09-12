using UnityEngine;
using UnityEngine.UI;

public class OpenWebsite : MonoBehaviour
{
    // Inserisci il tuo URL qui
    public string websiteURL = "https://artifacts-arexhibition.webflow.io/";

    // Assicurati di collegare il tuo pulsante UI a questo gestore nell'Editor Unity
    public Button openWebsiteButton;

    private void Start()
    {
        // Assicurati che il pulsante UI sia collegato a questo gestore
        if (openWebsiteButton != null)
        {
            // Aggiungi un listener per gestire il clic sul pulsante
            openWebsiteButton.onClick.AddListener(OpenWebsiteOnClick);
        }
        else
        {
            Debug.LogError("Il pulsante OpenWebsite non è collegato. Collega il pulsante UI nell'Editor Unity.");
        }
    }

    // Questa funzione verrà chiamata quando il pulsante viene cliccato
    private void OpenWebsiteOnClick()
    {
        // Apri l'URL nel browser predefinito del sistema
        Application.OpenURL(websiteURL);
    }
}
