using UnityEngine;
using UnityEngine.UI;

public class MonsterPatience : MonoBehaviour
{
    public Slider patienceSlider; // Reference to the Slider that represents the monster's patience
    public float patienceDecreaseRate = 1f; // How much patience decreases per second
    public Animator monsterAnimator; // Reference to the monster's Animator
    public AudioSource audioSource; // Reference to the AudioSource for playing sounds
    public AudioClip wrongSound; // Sound to play when patience reaches 0

    [SerializeField] private GameObject clientObject; // Reference to the client GameObject (assign in the Inspector)

    private void Start()
    {
        // Initialize the slider with the maximum value (this can depend on your game logic)
        if (patienceSlider != null)
        {
            patienceSlider.maxValue = 100f; // Or any max value you want
            patienceSlider.value = patienceSlider.maxValue; // Initial value is the maximum
        }

        // Check if the Animator is assigned
        if (monsterAnimator == null)
        {
            //Debug.LogError("Animator is not assigned to the monster.");
        }

        // Check if the AudioSource and AudioClip are assigned
        if (audioSource == null)
        {
          //  Debug.LogError("AudioSource is not assigned!");
        }
        if (wrongSound == null)
        {
          //  Debug.LogError("Wrong sound AudioClip is not assigned!");
        }

        // Ensure the clientObject is assigned
        if (clientObject == null)
        {
            Debug.LogError("Client GameObject is not assigned in the Inspector!");
        }
    }

    private void Update()
    {
        // Decrease patience over time
        if (patienceSlider != null && patienceSlider.value > 0)
        {
            patienceSlider.value -= patienceDecreaseRate * Time.deltaTime;

            // Trigger animation based on patience value
            if (patienceSlider.value > 50)
            {
                // If patience is greater than 50, the monster is "talking"
                if (!monsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Talk"))
                {
                    monsterAnimator.SetTrigger("Talk"); // Trigger the "Talk" animation
                }
            }
            else
            {
                // If patience is less than or equal to 50, the monster is "angry"
                if (!monsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Angry"))
                {
                    monsterAnimator.SetTrigger("Angry"); // Trigger the "Angry" animation
                }
            }
        }

        // If patience reaches zero, handle the failure
        if (patienceSlider != null && patienceSlider.value <= 0)
        {
            HandlePatienceDepletion();
        }
    }

    private void HandlePatienceDepletion()
    {
       // Debug.Log("MonsterPatience: Patience has reached 0!");

        // Play the "wrong" sound
        if (audioSource != null && wrongSound != null)
        {
            audioSource.PlayOneShot(wrongSound);
        }

        // Inform the ScoreManager about the failed order
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddFailedOrder();
        }
        else
        {
           // Debug.LogError("ScoreManager not found in the scene!");
        }

        // Destroy the client GameObject
        if (clientObject != null)
        {
            Destroy(clientObject);
        }
        else
        {
          //  Debug.LogError("Client GameObject is null! Cannot destroy the client.");
        }
    }
}