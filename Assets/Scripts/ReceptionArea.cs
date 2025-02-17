using UnityEngine;

public class ReceptionArea : MonoBehaviour
{
    private bool isOccupied = false; // Indicates if the reception area is occupied

    // Method to check if the reception area is occupied
    public bool IsOccupied()
    {
        return isOccupied;
    }

    // Detect when an NPC enters the reception area
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC")) // Ensure NPCs have the "NPC" tag
        {
            Debug.Log("ReceptionArea: An NPC entered the reception area.");
            isOccupied = true; // Mark the reception as occupied
        }
    }

    // Detect when an NPC exits the reception area
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC")) // Ensure NPCs have the "NPC" tag
        {
            Debug.Log("ReceptionArea: An NPC exited the reception area.");
            isOccupied = false; // Mark the reception as free
        }
    }
}