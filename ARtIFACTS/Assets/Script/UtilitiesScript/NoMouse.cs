using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Hides the mouse cursor
        Cursor.visible = false;

        // Locks the cursor to the center of the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Press ESC to release the cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
