using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform attachTransform; // Reference to the AttachTransform where the cone will be attached
    public static GameObject heldCone; // Reference to the cone currently held by the player

    void Update()
    {
        // Detect left mouse click or touch on the screen
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            TryThrowCone();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the hand collides with a cone from the dispenser
        if (other.CompareTag("Cone"))
        {
            // Check if the player already has a cone
            if (heldCone != null)
            {
                Debug.LogWarning("Player already has a cone! Cannot pick up another one.");
                return;
            }

            // Pick up the cone
            heldCone = other.gameObject;

            // Change the cone's tag to "ConeInHand"
            heldCone.tag = "ConeInHand";

            // Attach the cone to the AttachTransform
            heldCone.transform.SetParent(attachTransform);
            heldCone.transform.localPosition = Vector3.zero; // Center the cone in the AttachTransform
            heldCone.transform.localRotation = Quaternion.identity; // Reset rotation

           // Debug.Log("Cone picked up and tagged as 'ConeInHand'!");
        }
    }

    void TryThrowCone()
    {
        if (heldCone == null)
        {
            Debug.LogWarning("No cone to throw!");
            return;
        }

        // Perform a raycast to check if the click/touch hits the cone
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the raycast hits the held cone
            if (hit.collider != null && hit.collider.gameObject == heldCone)
            {
                ThrowCone();
            }
        }
    }

    void ThrowCone()
    {
        if (heldCone == null)
        {
            Debug.LogWarning("No cone to throw!");
            return;
        }

        // Detach the cone from the AttachTransform
        heldCone.transform.SetParent(null);

        // Restore the cone's tag to "Cone"
        heldCone.tag = "Cone";

        // Add physics to the cone
        Rigidbody rb = heldCone.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = heldCone.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false; // Allow physics to affect the object
        rb.useGravity = true;

        // Calculate the direction to throw the cone (e.g., forward)
        rb.linearVelocity = transform.forward * 5f; // Adjust speed as needed

        // Destroy the cone after 1 second
        Destroy(heldCone, 1f);

        // Clear the reference to the held cone
        heldCone = null;

        Debug.Log("Cone thrown and will be destroyed in 1 second!");
    }
}