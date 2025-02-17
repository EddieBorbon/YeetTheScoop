using UnityEngine;
using UnityEngine.UI; // For working with UI components like Image

public class ConeSpawner : MonoBehaviour
{
    public GameObject conePrefab; // Assign the cone prefab in the Inspector.
    public Transform spawnPosition; // Assign the spawn position in the Inspector.
    public GameObject buttonObject; // The button that will be clicked/tapped.
    public GameObject buttonChildObject; // The child object of the button (can be a 3D object or UI Image).
    public Color redColor = Color.red; // Default red color.
    public Color greenColor = Color.green; // Temporary green color.
    public float colorChangeDuration = 0.5f; // Duration for the color change.

    private Renderer buttonChildRenderer; // Renderer for 3D objects.
    private Image buttonChildImage; // Image for UI objects.

    void Start()
    {
        // Check if the button's child is a 3D object or a UI Image
        if (buttonChildObject != null)
        {
            buttonChildRenderer = buttonChildObject.GetComponent<Renderer>();
            buttonChildImage = buttonChildObject.GetComponent<Image>();

            if (buttonChildRenderer == null && buttonChildImage == null)
            {
                //Debug.LogError("The button's child object does not have a Renderer or Image component!");
            }
        }
    }

    void Update()
    {
        // Detect touch or click on the button
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the button was clicked/tapped
                if (hit.collider != null && hit.collider.gameObject == buttonObject)
                {
                    SpawnCone();
                    ChangeButtonColorTemporarily();
                }
            }
        }
    }

    void SpawnCone()
    {
        // Check if there's already a cone at the spawn position
        Collider[] colliders = Physics.OverlapSphere(spawnPosition.position, 0.1f); // Small radius to check for cones
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Cone"))
            {
                //Debug.Log("A cone already exists at the spawn position!");
                return; // Exit if a cone is found
            }
        }

        // Instantiate the cone if no cone exists
        Instantiate(conePrefab, spawnPosition.position, spawnPosition.rotation);
       // Debug.Log("Cone spawned successfully!");
    }

    void ChangeButtonColorTemporarily()
    {
        if (buttonChildRenderer != null)
        {
            // Change the color of the Renderer (for 3D objects)
            buttonChildRenderer.material.color = greenColor;
        }
        else if (buttonChildImage != null)
        {
            // Change the color of the Image (for UI objects)
            buttonChildImage.color = greenColor;
        }

        // Revert the color after a delay
        Invoke("RevertButtonColor", colorChangeDuration);
    }

    void RevertButtonColor()
    {
        if (buttonChildRenderer != null)
        {
            // Revert the color of the Renderer (for 3D objects)
            buttonChildRenderer.material.color = redColor;
        }
        else if (buttonChildImage != null)
        {
            // Revert the color of the Image (for UI objects)
            buttonChildImage.color = redColor;
        }
    }
}