using UnityEngine;
using System.Collections.Generic;

public class IceCreamMachine : MonoBehaviour
{
    public GameObject softServePrefab; // Prefab for the ice cream droplet
    public Transform spawnPoint; // Point where the ice cream is spawned
    public float spawnInterval = 0.5f; // Interval between spawns
    private float timer = 0f;
    private bool isDispensing = false;
    public Color iceCreamColor; // Color of the ice cream emitted by this machine
    public string noteName; // Note name (e.g., "C", "D", "E", etc.)
    public float volume = 1.0f; // Volume control (range: 0.0 to 1.0)
    private AudioSource audioSource; // Audio component to play the note
    private AudioClip generatedNote; // Dynamically generated audio clip

    // Dictionary to map note names to frequencies (in Hz)
    private Dictionary<string, float> noteFrequencies = new Dictionary<string, float>()
    {
        { "C", 261.63f },
        { "D", 293.66f },
        { "E", 329.63f },
        { "F", 349.23f },
        { "G", 392.00f },
        { "A", 440.00f },
        { "B", 493.88f }
    };

    void Start()
    {
        // Generate the musical note using synthesis based on the assigned note name
        if (noteFrequencies.ContainsKey(noteName))
        {
            float frequency = noteFrequencies[noteName];
            generatedNote = GenerateNote(frequency);
        }
        else
        {
            Debug.LogError("Invalid note name assigned to the machine!");
        }

        // Initialize the AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        if (generatedNote != null)
        {
            audioSource.clip = generatedNote;
            audioSource.loop = true; // Loop the sound while the machine is active
            audioSource.volume = volume; // Set the initial volume
        }
    }

    void Update()
    {
        // Detect touches or clicks
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the player touched/clicked the machine
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    //Debug.Log("Machine tapped!");
                    ToggleDispensing();
                }
            }
        }

        // Continuously spawn ice cream while active
        if (isDispensing)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnSoftServe();
                timer = 0f;
            }
        }
    }

    void ToggleDispensing()
    {
        isDispensing = !isDispensing;
        //Debug.Log(isDispensing ? "Dispensing started!" : "Dispensing stopped!");

        // Play or stop the musical note
        if (audioSource != null)
        {
            if (isDispensing)
            {
                audioSource.Play(); // Play the note when the machine is active
            }
            else
            {
                audioSource.Stop(); // Stop the note when the machine is inactive
            }
        }
    }

    void SpawnSoftServe()
    {
        if (softServePrefab != null && spawnPoint != null)
        {
            GameObject droplet = Instantiate(softServePrefab, spawnPoint.position, spawnPoint.rotation);

            // Scale the droplet to the desired size (0.03)
            droplet.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

            // Assign the machine's color and note to the droplet
            SoftServeDroplet dropletScript = droplet.GetComponent<SoftServeDroplet>();
            if (dropletScript != null)
            {
                dropletScript.SetColorAndNote(iceCreamColor, noteName);
            }

            // Apply a custom initial velocity to the droplet
            Rigidbody rb = droplet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.down * 0.5f; // Slow falling speed
            }
        }
    }

    // Generate a musical note using synthesis
    private AudioClip GenerateNote(float frequency)
    {
        int sampleRate = 44100; // Standard audio sample rate
        float duration = 2.0f; // Duration of the note (in seconds)
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        for (int i = 0; i < totalSamples; i++)
        {
            // Generate a sine wave for the note
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate);
        }

        // Create an AudioClip from the generated samples
        AudioClip audioClip = AudioClip.Create("GeneratedNote", totalSamples, 1, sampleRate, false);
        audioClip.SetData(samples, 0);
        return audioClip;
    }
}