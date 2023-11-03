using UnityEngine;
using UnityEngine.UI;
using NativeCameraNamespace; // This namespace should contain the PreferredCamera enum.

public class TakeSelfieButton : MonoBehaviour
{
    public RawImage rawImageDisplay;
    public FaceTextureAndSound faceTextureAndSound;

    public void OnButtonPressed()
    {
        // Request or check for the camera permission before taking a picture
        NativeCamera.Permission permission = NativeCamera.CheckPermission();

        if (permission == NativeCamera.Permission.Granted)
        {
            TakePicture(512);
        }
        else if (permission == NativeCamera.Permission.ShouldAsk)
        {
            NativeCamera.RequestPermission();
        }
        else if (permission == NativeCamera.Permission.Denied)
        {
            Debug.Log("Permission denied by the user");
        }
        else if (permission == NativeCamera.Permission.DeniedAndDontAskAgain)
        {
            Debug.Log("Permission denied by the user and don’t ask again");
        }
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Picture taken, saved at: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Display the texture in the RawImage component
                rawImageDisplay.texture = texture;

                // Call the method to set the face texture
                faceTextureAndSound.SetFaceTexture(texture);
            }
        }, maxSize, preferredCamera: NativeCamera.PreferredCamera.Front); // Ensure PreferredCamera is part of NativeCameraNamespace
    }
}
