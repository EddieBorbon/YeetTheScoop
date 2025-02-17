using UnityEngine;
using UnityEngine.InputSystem;

public class ConeInteraction : MonoBehaviour
{
    public Transform handPosition; // Reference to the hand position (will be assigned automatically)
    private bool isGrabbed = false;
    private Rigidbody rb; // Reference to the cone's Rigidbody
    private Vector3 originalPosition; // Store the initial position of the cone
    private Quaternion originalRotation; // Store the initial rotation of the cone

    void Start()
    {
        // Get the Rigidbody component attached to the cone
        rb = GetComponent<Rigidbody>();
        // Save the initial position and rotation of the cone
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        // Find the object with the tag "AttachHand" and assign it to handPosition
        GameObject handObject = GameObject.FindGameObjectWithTag("AttachHand");
        if (handObject != null)
        {
            handPosition = handObject.transform;
            //Debug.Log("Hand position assigned automatically!");
        }
        else
        {
            //Debug.LogError("No object with the tag 'AttachHand' found in the scene!");
        }
    }

    void Update()
    {
        // Detect mouse click or touch using the new Input System
        if (Mouse.current.leftButton.wasPressedThisFrame || Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        // Get the mouse or touch position
        Vector3 inputPosition;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputPosition = Mouse.current.position.ReadValue(); // Mouse position
        }
        else
        {
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue(); // Touch position
        }
        // Convert the input position into a raycast in the game world
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;
        // Check if the raycast hits any object
        if (Physics.Raycast(ray, out hit))
        {
            // Verify if this cone was clicked/touched
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                if (!isGrabbed)
                {
                    GrabCone();
                }
                else
                {
                    ReleaseCone();
                }
            }
        }
    }

    void GrabCone()
    {
        // Change the tag to "ConeInHand"
        gameObject.tag = "ConeInHand";
        //Debug.Log("Cone tag changed to 'ConeInHand'.");

        // Grab the cone
        if (handPosition != null)
        {
            transform.SetParent(handPosition); // Make the cone a child of the hand
            transform.position = handPosition.position;
            transform.rotation = handPosition.rotation;
            // Disable physics to keep the cone fixed
            if (rb != null)
            {
                rb.isKinematic = true; // Disable physics simulation
                rb.useGravity = false; // Turn off gravity
            }
            isGrabbed = true;
           // Debug.Log("Cone grabbed and attached to hand!");
        }
        else
        {
           // Debug.LogError("Hand position not assigned!");
        }
    }

    void ReleaseCone()
    {
        // Change the tag back to its original value (e.g., "Untagged" or another default tag)
        gameObject.tag = "Cone"; // Or use a specific tag like "Cone"
        //Debug.Log("Cone tag changed back to 'Untagged'.");

        // Release the cone
        transform.SetParent(null); // Detach the cone from the hand
        // Re-enable physics so the cone falls
        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics simulation
            rb.useGravity = true; // Turn on gravity
        }
        isGrabbed = false;
      //  Debug.Log("Cone released!");
    }
}