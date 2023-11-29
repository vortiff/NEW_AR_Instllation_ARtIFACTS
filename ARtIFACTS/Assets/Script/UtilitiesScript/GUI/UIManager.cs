using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject panelUI;

    // Metodo da chiamare quando si clicca il bottone
    public void DisablePanel()
    {
        panelUI.SetActive(false); // Disattiva il pannello UI
    }
}
