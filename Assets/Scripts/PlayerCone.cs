using UnityEngine;

public class PlayerCone : MonoBehaviour
{
    public Transform[] iceCreamBalls; // References to the three ice cream balls
    public GameObject sharedFX; // Reference to the single shared FX GameObject
    private int currentBallIndex = 0; // Index of the current ball being filled
    private float alphaValue = 0f; // Current alpha channel value
    private bool isFilling = false; // Indicates if a ball is currently being filled
    public string[] chordNotes = new string[3]; // Array to store the notes of the chord (e.g., ["C", "E", "G"])
    
    // Variable to store the type of cone (Major or Minor)
    public enum ConeType { Major, Minor }
    public ConeType coneType; // Assign the cone type in the Inspector

    void Start()
    {
        // Ensure the cone type is assigned
        if (coneType == ConeType.Major || coneType == ConeType.Minor)
        {
            Debug.Log("Cone type initialized: " + coneType);
        }
        else
        {
            Debug.LogError("Cone type is not assigned! Please assign Major or Minor in the Inspector.");
        }

        // Initialize the cone
        InitializeCone();
    }

    void InitializeCone()
    {
        // Reset the chord notes array
        for (int i = 0; i < chordNotes.Length; i++)
        {
            chordNotes[i] = ""; // Clear all notes
        }

        // Reset the ball index and alpha value
        currentBallIndex = 0;
        alphaValue = 0f;

        // Reset the transparency of all balls
        foreach (Transform ball in iceCreamBalls)
        {
            Renderer ballRenderer = ball.GetComponent<Renderer>();
            if (ballRenderer != null)
            {
                Color currentColor = ballRenderer.material.color;
                currentColor.a = 0f; // Set alpha to 0 (fully transparent)
                ballRenderer.material.color = currentColor;
            }
        }

        // Deactivate the shared FX at the start
        if (sharedFX != null)
        {
            sharedFX.SetActive(false);
        }

        Debug.Log("Cone reset and ready for use!");
    }

    void Update()
    {
        if (isFilling)
        {
            // Gradually increase the alpha channel
            alphaValue += Time.deltaTime * 0.5f; // Filling speed
            alphaValue = Mathf.Clamp(alphaValue, 0f, 1f); // Clamp between 0 and 1

            // Update the material of the current ball
            UpdateBallAlpha();

            // Check if the ball is fully visible
            if (alphaValue >= 1f)
            {
                isFilling = false;

                // Deactivate the shared FX when the ball is fully filled
                if (sharedFX != null)
                {
                    sharedFX.SetActive(false);
                }

                Debug.Log("Ball " + currentBallIndex + " filled with note: " + chordNotes[currentBallIndex]);

                // Move to the next ball
                currentBallIndex++;
                if (currentBallIndex >= iceCreamBalls.Length)
                {
                    Debug.Log("All balls filled! Chord: " + string.Join(", ", chordNotes));
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the cone comes into contact with a droplet
        if (other.CompareTag("SoftServe"))
        {
            SoftServeDroplet droplet = other.GetComponent<SoftServeDroplet>();
            if (droplet != null && currentBallIndex < iceCreamBalls.Length)
            {
                // Store the droplet's note in the chord array
                chordNotes[currentBallIndex] = droplet.GetNote();
                StartFilling(droplet.GetColor());

                // Destroy the droplet after it fills the ball
                Destroy(other.gameObject);
            }
        }
    }

    void StartFilling(Color color)
    {
        if (!isFilling && currentBallIndex < iceCreamBalls.Length)
        {
            // Assign the droplet's color to the current ball
            Renderer ballRenderer = iceCreamBalls[currentBallIndex].GetComponent<Renderer>();
            if (ballRenderer != null)
            {
                Color newColor = color;
                newColor.a = 0f; // Start with full transparency
                ballRenderer.material.color = newColor;
            }

            // Start the filling process
            isFilling = true;
            alphaValue = 0f;

            // Activate the shared FX when filling starts
            if (sharedFX != null)
            {
                sharedFX.SetActive(true);
            }
        }
    }

    void UpdateBallAlpha()
    {
        if (currentBallIndex < iceCreamBalls.Length)
        {
            Renderer ballRenderer = iceCreamBalls[currentBallIndex].GetComponent<Renderer>();
            if (ballRenderer != null)
            {
                Color currentColor = ballRenderer.material.color;
                currentColor.a = alphaValue; // Update the alpha channel
                ballRenderer.material.color = currentColor;
            }
        }
    }

    public bool IsConeComplete()
    {
        // Check if all notes in the chordNotes array are filled
        for (int i = 0; i < chordNotes.Length; i++)
        {
            if (string.IsNullOrEmpty(chordNotes[i]))
            {
                Debug.Log($"Cone is incomplete. Missing note at index {i}");
                return false;
            }
        }

        Debug.Log("Cone is complete!");
        return true;
    }
}