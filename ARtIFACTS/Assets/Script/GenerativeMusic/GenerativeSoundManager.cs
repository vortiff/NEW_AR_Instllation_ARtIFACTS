using UnityEngine;


public class GenerativeSoundManager : MonoBehaviour
{
  public float baseFrequency = 440f;
    public int numHarmonics = 5;
    public float volume = 0.5f;
    public float rotationSpeed = 10f;

    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        // Ottieni il componente AudioLowPassFilter
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }
    void Update()
    {
        // Update pitch based on flocking behavior (for example, Y position)
        float pitchMultiplier = 1f + transform.position.y * 0.1f;

        // Generate harmonics
        float[] harmonics = new float[numHarmonics];
        for (int i = 0; i < numHarmonics; i++)
        {
            harmonics[i] = Mathf.Sin((baseFrequency * (i + 1)) * Time.time);
        }

        // Mix harmonics together
        float mixedValue = 0f;
        foreach (float harmonic in harmonics)
        {
            mixedValue += harmonic;
        }

        // Update volume based on flocking behavior (for example, Z position)
        float volumeMultiplier = Mathf.Clamp01(1f - Mathf.Abs(transform.position.z) / 3f);

        // Apply volume and pitch to audio source
        audioSource.volume = volume * volumeMultiplier;
        audioSource.pitch = pitchMultiplier;
        audioSource.panStereo = Mathf.Clamp(transform.position.x / 5f, -1f, 1f);

        // Applica il valore di distorsione all'effetto di distorsione
        float distortion = Mathf.Clamp01(rotationSpeed / 100f);
        lowPassFilter.cutoffFrequency = Mathf.Lerp(5000f, 22000f, distortion);

        // Play the sound
        audioSource.Play();
    }
}
