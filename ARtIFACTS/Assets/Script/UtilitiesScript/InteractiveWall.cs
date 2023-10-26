    using UnityEngine;
    using System.Collections;

    public class InteractiveWall : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private Vector2 colonneRighe = new Vector2(5f, 10f);
        [SerializeField] private Vector2 padding = new Vector2(2f, 2f);
        [SerializeField] private float rotationTime = 1f; // Tempo di rotazione
        
        [Header("Audio Settings")]
        [SerializeField] private MediaLibrary mediaLibrary;

        [Header("Color Settings")]
        public Color color1 = Color.white;
        public Color color2 = Color.black;
        
        private GameObject[,] circles;
        private bool[,] circleHit; // Per memorizzare se un cerchio è stato colpito
        private float[,] timeSinceLastHit; // Tempo dall'ultimo hit per ogni cerchio
        private bool[,] circleRotated; // array booleano per tracciare quali cerchi hanno ruotato


        private void Start()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            int rows = Mathf.FloorToInt(colonneRighe.x / padding.x);
            int cols = Mathf.FloorToInt(colonneRighe.y / padding.y);
            
            // Ensure you initialize the arrays AFTER you have calculated rows and cols
            circleRotated = new bool[rows, cols];
            circles = new GameObject[rows, cols];
            circleHit = new bool[rows, cols];
            timeSinceLastHit = new float[rows, cols];

            Vector3 startCorner = transform.position - new Vector3(0, (rows - 1) * padding.y * 0.5f, (cols - 1) * padding.x * 0.5f);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Vector3 position = startCorner + new Vector3(0, i * padding.y, j * padding.x);
                    Quaternion combinedRotation = transform.rotation * Quaternion.Euler(0, 0, 90);
                    circles[i, j] = Instantiate(circlePrefab, position, combinedRotation, transform);

                    Renderer renderer = circles[i, j].GetComponent<Renderer>();
                    if(renderer != null) 
                    {
                        Color chosenColor;
                        if(Random.Range(0f, 1f) > 0.5f)
                        {
                            chosenColor = color1;
                        }
                        else
                        {
                            chosenColor = color2;
                        }

                        circles[i, j].GetComponent<Renderer>().material.color = chosenColor;
                    }
                    else 
                    {
                        Debug.LogError("No renderer found on the circlePrefab!");
                    }

                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < circles.GetLength(0); i++)
            {
                for (int j = 0; j < circles.GetLength(1); j++)
                {
                    // Imposta circleHit a false all'inizio di ogni ciclo di aggiornamento
                    circleHit[i, j] = false;
                    
                    CheckRaycast(circles[i, j], i, j);

                    if (!circleHit[i, j] && circleRotated[i, j])
                    {
                        timeSinceLastHit[i, j] += Time.deltaTime;
                        if (timeSinceLastHit[i, j] > 2f)
                        {
                            //Debug.Log($"Circle at [{i},{j}] starting to rotate back.");
                            // Interrompe la coroutine RotateWithTime se è in esecuzione
                            StopCoroutine(RotateWithTime(circles[i, j]));
                            StartCoroutine(RotateBack(circles[i, j]));
                            timeSinceLastHit[i, j] = 0;
                            circleRotated[i, j] = false;
                        }
                    }
                }
            }
        }

        private IEnumerator RotateBack(GameObject circle)
        {
            Debug.Log("Starting to rotate back.");
            Quaternion startRotation = circle.transform.rotation;
            Quaternion endRotation = Quaternion.Euler(0, 0, 90); // Adjusted the end rotation to go back to the initial rotation

            float elapsedTime = 0;

            while (elapsedTime < rotationTime)
            {
                circle.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotationTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            circle.transform.rotation = endRotation;
            Debug.Log("Finished rotating back.");
            PlaySound(circle, 7); // Suono per la rotazione indietro

            // Start fade out Metallic value for all circles
            foreach (var c in circles)
            {
                StartCoroutine(FadeMetallic(c.GetComponent<Renderer>().material, 0.5f, 0.2f, 2f));
            }
        }

        private void RotateNeighbour(int row, int col)
        {
            if (row >= 0 && row < circles.GetLength(0) && col >= 0 && col < circles.GetLength(1))
            {
                circles[row, col].transform.Rotate(Vector3.up * 90);
                circleRotated[row, col] = true; // Mark the neighbouring circle as rotated
                circleHit[row, col] = true; // Also mark it as hit
            }
        }


        private void CheckRaycast(GameObject circle, int row, int col)
        {
            RaycastHit hit;
            Vector3 origin = circle.transform.position;
            Vector3 direction = circle.transform.up;

            Debug.DrawRay(origin, direction * 30f, Color.green);

            if (Physics.Raycast(origin, direction, out hit, 30f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    circleHit[row, col] = true;

                    circleRotated[row, col] = true;
                    timeSinceLastHit[row, col] = 0;

                    // Interrompe la coroutine RotateBack se è in esecuzione
                    StopCoroutine(RotateBack(circles[row, col]));
                    StartCoroutine(RotateWithTime(circle));

                    int randomSoundIndex = Random.Range(0, 6);
                    PlaySound(circle, randomSoundIndex);

                    RotateNeighbour(row + 1, col);
                    RotateNeighbour(row - 1, col);
                    RotateNeighbour(row, col + 1);
                    RotateNeighbour(row, col - 1);
                }
                else
                {
                    Debug.Log($"Circle at [{row},{col}] no longer being hit by the Player.");
                }
            }
        }


        private IEnumerator RotateWithTime(GameObject circle)
        {
            Quaternion startRotation = circle.transform.rotation;
            Quaternion endRotation = Quaternion.Euler(0, 90, 0);
            float elapsedTime = 0;

            while (elapsedTime < rotationTime)
            {
                circle.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotationTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            circle.transform.rotation = endRotation;

            // Start fade in Metallic value for all circles
            foreach (var c in circles)
            {
                StartCoroutine(FadeMetallic(c.GetComponent<Renderer>().material, 0.2f, 0.5f, 2f));
            }
        }

        private void PlaySound(GameObject circle, int index)
        {
            AudioSource circleAudioSource = circle.GetComponent<AudioSource>();
            if(circleAudioSource != null && index >= 0 && index < mediaLibrary.audioClips.Length)
            {
                circleAudioSource.PlayOneShot(mediaLibrary.audioClips[index]);
            }

        }

        private IEnumerator FadeMetallic(Material mat, float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float newVal = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
                mat.SetFloat("_Metallic", newVal);  // Assuming you're using the standard URP shader, adjust if not
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            mat.SetFloat("_Metallic", endValue);
        }
    }
