using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 3f; // Movement speed
    public Animator animator; // Animator for controlling animations
    private NavMeshAgent agent; // NavMeshAgent component
    private Transform receptionPosition; // Position of the reception
    public GameObject orderObject; // GameObject that contains the order (assign in the Inspector)

    void Start()
    {
       // Debug.Log("NPCMovement: Initializing NPC...");

        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
         //   Debug.LogError("NPCMovement: NavMeshAgent not found!");
            return;
        }

        // Set the agent's speed
        agent.speed = moveSpeed;

        // Find the reception position by its tag
        GameObject receptionObject = GameObject.FindGameObjectWithTag("Reception");
        if (receptionObject != null)
        {
            receptionPosition = receptionObject.transform;
          //  Debug.Log("NPCMovement: Reception position found: " + receptionPosition.position);

            // Set the reception as the initial destination
            SetDestination(receptionPosition.position);
        }
        else
        {
          //  Debug.LogError("NPCMovement: No object with the 'Reception' tag found in the scene!");
        }

        // Deactivate the order GameObject at the start
        if (orderObject != null)
        {
            orderObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the NPC has reached its destination
        if (HasReachedDestination())
        {
        //    Debug.Log("NPCMovement: Reached the reception.");
            OnReachedReception();
        }
    }

    // Set the NPC's destination
    private void SetDestination(Vector3 destination)
    {
        if (agent != null)
        {
        //    Debug.Log("NPCMovement: Setting destination: " + destination);
            agent.SetDestination(destination);

            // Activate the walking animation
            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
          //  Debug.LogError("NPCMovement: NavMeshAgent is null!");
        }
    }

    // Check if the NPC has reached its destination
    private bool HasReachedDestination()
    {
        if (agent != null && agent.remainingDistance <= agent.stoppingDistance)
        {
         //   Debug.Log("NPCMovement: Destination reached. Remaining distance: " + agent.remainingDistance);
            return true;
        }
        return false;
    }

    // Actions when the NPC reaches the reception
    private void OnReachedReception()
    {
        Debug.Log("NPCMovement: Executing actions upon reaching the reception.");

        // Stop the walking animation and activate the idle animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Idle"); // Activate the idle trigger
        }

        // Look at the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
          //  Debug.Log("NPCMovement: Looking at the player.");
            transform.LookAt(new Vector3(playerObject.transform.position.x, transform.position.y, playerObject.transform.position.z));
        }
        else
        {
         //   Debug.LogError("NPCMovement: No object with the 'Player' tag found in the scene!");
        }

        // Activate the order GameObject
        if (orderObject != null)
        {
         //   Debug.Log("NPCMovement: Activating the order GameObject.");
            orderObject.SetActive(true);
        }
        else
        {
        //    Debug.LogError("NPCMovement: The order GameObject is not assigned!");
        }

        // Desactivar o quitar el Rigidbody del NPC
        Rigidbody npcRigidbody = GetComponent<Rigidbody>();
        if (npcRigidbody != null)
        {
            Debug.Log("NPCMovement: Disabling or removing the Rigidbody.");
            npcRigidbody.isKinematic = true; // Opcional: Hacerlo kinemático antes de desactivarlo
            npcRigidbody.detectCollisions = false; // Desactivar detección de colisiones
            Destroy(npcRigidbody); // Eliminar el Rigidbody completamente
        }
    }
}