using UnityEngine;

public class SoftServeDroplet : MonoBehaviour
{
    private Rigidbody rb;
    private Color dropletColor;
    private string dropletNote;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = 0.01f; // Extremely light mass for small droplets
            rb.linearDamping = 5f; // Linear movement resistance (replaces drag)
            rb.useGravity = false; // Disable default gravity
        }

        // Apply a custom downward force
        InvokeRepeating("ApplyCustomGravity", 0f, 0.02f); // Call every 0.02 seconds
    }

    void ApplyCustomGravity()
    {
        if (rb != null)
        {
            rb.AddForce(Vector3.down * 1f, ForceMode.Acceleration); // Gentle downward force
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the droplet collides with another droplet, adjust its position to simulate merging
        if (other.CompareTag("SoftServe"))
        {
            transform.position = Vector3.Lerp(transform.position, other.transform.position, 0.5f);
        }
    }

    // Method to set the color and note of the droplet
    public void SetColorAndNote(Color color, string note)
    {
        dropletColor = color;
        dropletNote = note;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = dropletColor;
        }
    }

    // Method to get the droplet's color
    public Color GetColor()
    {
        return dropletColor;
    }

    // Method to get the droplet's note
    public string GetNote()
    {
        return dropletNote;
    }
}