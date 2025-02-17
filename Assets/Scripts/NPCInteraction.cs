using System.Collections;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Animator npcAnimator; // Reference to the NPC's Animator
    public MonsterOrder monsterOrder; // Reference to the NPC's order script

    public AudioClip failSound; // Sound to play when the order fails
    public AudioClip winMusic; // Sound to play when the order fails
    public NPCMessageManager npcMessageManager; // Reference to the NPCMessageManager script

    private PlayerCone playerCone; // Reference to the player's cone script
    private GameObject discoBall; // Reference to the disco ball
    private AudioSource danceMusic; // Reference to the dance music audio source
    private AudioSource audioSource; // Reference to the NPC's Audio Source

    // Variables to track orders
    private bool isInteractable = true; // Indicates if the NPC can be interacted with
    private bool isAttended = false; // Indicates if the NPC has been attended

    public Transform coneHit;

    public GameObject fxGood;
    public GameObject fxBad;

    public bool IsAttended()
    {
        return isAttended;
    }

    void Start()
    {
        // Dynamically find the disco ball
        GameObject discoBallObject = GameObject.FindGameObjectWithTag("DiscoBall");
        if (discoBallObject != null)
        {
            discoBall = discoBallObject;
            danceMusic = discoBall.GetComponent<AudioSource>();
            if (danceMusic == null)
            {
                Debug.LogError("AudioSource component not found on the object with the 'DiscoBall' tag!");
            }
        }
        else
        {
            Debug.LogError("No object with the 'DiscoBall' tag found in the scene!");
        }

        // Get the Audio Source component for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Detect left mouse click or touch on the screen
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the player clicked/touched the NPC and if it's interactable
                if (hit.collider != null && hit.collider.gameObject == this.gameObject && isInteractable)
                {
                    HandleNPCInteraction();
                }
            }
        }
    }

    void HandleNPCInteraction()
    {
        // Buscar el cono del jugador en tiempo real
        playerCone = FindPlayerCone();

        // Disable interaction while processing the current order
        //isInteractable = false;

        // Check if the player has a cone in hand
        if (playerCone == null || !playerCone.gameObject.activeInHierarchy)
        {
            Debug.Log("Player does not have a cone in hand.");
            TriggerRandomAnimation("Speak"); // Play a random "Speak" animation
            npcMessageManager.ShowRandomMessage("nocone"); // Show "NoCone" message
            isInteractable = true; // Re-enable interaction
            return;
        }

        // Log the current state of the cone's notes
        Debug.Log("Player cone notes: " + string.Join(", ", playerCone.chordNotes));

        // Check if the cone is incomplete
        if (!IsConeComplete(playerCone))
        {
            Debug.Log("Cone is incomplete.");
            TriggerRandomAnimation("Claim"); // Play a random "Claim" animation
            npcMessageManager.ShowRandomMessage("incomplete"); // Show incomplete message
            isInteractable = true; // Re-enable interaction
            return;
        }

        // Throw the cone toward the NPC
        ThrowConeAtNPC();

        // Wait for the cone to reach the NPC and evaluate the order
        StartCoroutine(EvaluateOrder());
    }

    IEnumerator EvaluateOrder()
    {
        yield return new WaitForSeconds(1f); // Simulate time for the cone to reach the NPC

        isAttended = true;

        // Check if the cone matches the NPC's order
        if (DoesConeMatchOrder(playerCone, monsterOrder))
        {
            Debug.Log("Order matched! Delivering cone to NPC.");
            ScoreManager.instance.AddSuccessfulOrder(); // Incrementar órdenes exitosas
            TriggerRandomAnimation("Dance"); // Play a random "Dance" animation
            npcMessageManager.ShowRandomMessage("correct"); // Show correct message
            StartCoroutine(StartDanceRoutine()); // Start the dance routine
            fxGood.SetActive(true);
        }
        else
        {
            Debug.Log("Order does not match! Throwing cone at NPC.");
            ScoreManager.instance.AddFailedOrder(); // Incrementar órdenes fallidas
            TriggerRandomAnimation("Fail"); // Play a random "Fail" animation
            npcMessageManager.ShowRandomMessage("incorrect"); // Show incorrect message
            fxBad.SetActive(true);
            PlayFailSound();
            Destroy(gameObject, 5f);
        }

        // Re-enable interaction after evaluation
        isInteractable = true;
    }

    bool IsConeComplete(PlayerCone cone)
    {
        Debug.Log("Checking cone completeness...");
        for (int i = 0; i < cone.chordNotes.Length; i++)
        {
            Debug.Log($"Note {i}: {cone.chordNotes[i]}");
            if (string.IsNullOrEmpty(cone.chordNotes[i]))
            {
                Debug.LogWarning($"Cone is incomplete. Missing note at index {i}");
                return false;
            }
        }
        Debug.Log("Cone is complete!");
        return true;
    }

    bool DoesConeMatchOrder(PlayerCone cone, MonsterOrder order)
    {
        // Get the selected chord from the NPC
        string[] requestedChord = order.GetSelectedChord();

        // Log the NPC's chord and the player's chord for debugging
        Debug.Log("NPC Chord: " + string.Join(", ", requestedChord));
        Debug.Log("Player Chord: " + string.Join(", ", cone.chordNotes));

        // Check if the cone type matches
        bool isChordTypeMatch = (cone.coneType == PlayerCone.ConeType.Major && order.isMajorChord) ||
                                (cone.coneType == PlayerCone.ConeType.Minor && order.isMinorChord);

        Debug.Log("Chord Type Match: " + isChordTypeMatch);

        // Check if the notes match
        for (int i = 0; i < cone.chordNotes.Length; i++)
        {
            if (cone.chordNotes[i] != requestedChord[i])
            {
                Debug.Log($"Note mismatch at index {i}: Player Note = {cone.chordNotes[i]}, NPC Note = {requestedChord[i]}");
                return false; // If any note doesn't match, return false
            }
        }

        return isChordTypeMatch;
    }

    void ThrowConeAtNPC()
    {
        // Crear una copia del cono para lanzarlo
        GameObject thrownCone = Instantiate(playerCone.gameObject, playerCone.transform.position, Quaternion.identity);

        // Agregar un Rigidbody al cono si no tiene uno
        Rigidbody rb = thrownCone.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = thrownCone.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true; // Desactivar la física para evitar caídas
        rb.useGravity = false; // Desactivar la gravedad

        // Calcular la dirección desde el jugador hacia el NPC
        Vector3 direction = (coneHit.transform.position - thrownCone.transform.position).normalized;

        // Aplicar una velocidad constante hacia el NPC
        StartCoroutine(MoveConeToNPC(thrownCone, direction));

        // Destruir el cono original del jugador
        Destroy(playerCone.gameObject);
    }

    IEnumerator MoveConeToNPC(GameObject cone, Vector3 direction)
    {
        float speed = 5f; // Speed of the cone
        float duration = 2f; // Duration of the movement (2 seconds)
        float elapsedTime = 0f; // Elapsed time

        while (elapsedTime < duration)
        {
            // Move the cone in the specified direction
            cone.transform.position += direction * speed * Time.deltaTime;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Destroy the cone after 2 seconds
        Destroy(cone);
    }


    IEnumerator StartDanceRoutine()
    {
        // Play the dance music
        if (danceMusic != null)
        {
            danceMusic.PlayOneShot(winMusic);
        }

        yield return new WaitForSeconds(7f); // Duration of the dance routine

        if (danceMusic != null)
        {
            danceMusic.Stop();
        }
        Destroy(gameObject, 1f);
    }

    void PlayFailSound()
    {
        if (failSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(failSound); // Play the fail sound
        }
    }

    PlayerCone FindPlayerCone()
    {
        // Buscar el objeto con el tag "ConeInHand"
        GameObject coneObject = GameObject.FindGameObjectWithTag("ConeInHand");
        if (coneObject != null)
        {
            PlayerCone cone = coneObject.GetComponent<PlayerCone>();
            if (cone != null)
            {
                Debug.Log("Found player cone in Hand with notes: " + string.Join(", ", cone.chordNotes));
                return cone;
            }
            else
            {
                Debug.LogError("PlayerCone component not found on the object with the 'ConeInHand' tag!");
            }
        }
        else
        {
            Debug.LogWarning("No cone with the 'ConeInHand' tag found in the scene!");
        }
        return null; // Return null if no cone is found
    }

    void TriggerRandomAnimation(string triggerName)
    {
        if (npcAnimator != null)
        {
            // Generate a random index for the animation
            int randomIndex = Random.Range(0, 3); // Assuming there are 3 animations per category

            // Set the corresponding parameter based on the trigger name
            switch (triggerName)
            {
                case "Claim":
                    npcAnimator.SetInteger("RandomClaim", randomIndex);
                    break;
                case "Dance":
                    npcAnimator.SetInteger("RandomDance", randomIndex);
                    break;
                case "Fail":
                    npcAnimator.SetInteger("RandomFail", randomIndex);
                    break;
                case "Speak":
                    npcAnimator.SetInteger("RandomSpeak", randomIndex);
                    break;
                default:
                    Debug.LogError("Unknown trigger name: " + triggerName);
                    return;
            }

            // Trigger the animation
            Debug.Log($"Triggering random animation for {triggerName} with index {randomIndex}");
            npcAnimator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogError("Animator is not assigned!");
        }
    }
}