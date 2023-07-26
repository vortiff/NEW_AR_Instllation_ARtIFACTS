using UnityEngine;

public class MetaballController : MonoBehaviour
{
    public float expansionSpeed = 0.1f;
    public float maxRadius = 5f;
    


    void Update()
    {
        // Espansione della metaball
        if (transform.localScale.x < maxRadius)
        {
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Movimento della metaball all'interno del contenitore
        float moveSpeed = 1f;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed;
        transform.Translate(movement * Time.fixedDeltaTime);

        // Limita il movimento della metaball all'interno del contenitore
        Vector3 clampedPosition = new Vector3(Mathf.Clamp(transform.position.x, -5f, 5f), transform.position.y, Mathf.Clamp(transform.position.z, -5f, 5f));
        transform.position = clampedPosition;
    }
}
