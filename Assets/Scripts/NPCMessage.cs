using System.Collections;
using UnityEngine;
using TMPro;

public class NPCMessageManager : MonoBehaviour
{
    public GameObject messagePanel; // Reference to the Panel GameObject
    public TMP_Text messageNPC; // Reference to the TextMeshPro inside the Panel
    public float fadeInDuration = 0.5f; // Duration for the fade-in effect
    public float displayDuration = 2f; // Duration to display the message fully visible
    public float fadeOutDuration = 0.5f; // Duration for the fade-out effect

    private CanvasGroup panelCanvasGroup; // CanvasGroup component for fading effects
    private Coroutine currentMessageCoroutine; // Tracks the current message coroutine

    void Start()
    {
        // Ensure the Panel is initially inactive
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        // Get the CanvasGroup component from the Panel
        panelCanvasGroup = messagePanel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = messagePanel.AddComponent<CanvasGroup>();
        }

        // Set initial alpha to 0 (fully transparent)
        panelCanvasGroup.alpha = 0f;
    }

  
    public void ShowRandomMessage(string category)
    {
        string[] messages = GetMessagesForCategory(category);

        if (messages.Length > 0)
        {
            string randomMessage = messages[Random.Range(0, messages.Length)];
            StartCoroutine(DisplayMessageWithFade(randomMessage));
        }
        else
        {
            Debug.LogWarning("No messages found for category: " + category);
        }
    }


    private IEnumerator DisplayMessageWithFade(string message)
    {
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine); // Stop any ongoing message
        }

        currentMessageCoroutine = StartCoroutine(FadeMessage(message));
        yield return currentMessageCoroutine;
    }
    private IEnumerator FadeMessage(string message)
    {
        if (messagePanel == null || messageNPC == null)
        {
            Debug.LogError("Panel or TextMeshPro component not assigned!");
            yield break;
        }

        // Activate the Panel
        messagePanel.SetActive(true);
        messageNPC.text = message; // Set the message text
        panelCanvasGroup.alpha = 0f; // Reset alpha to 0

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }

        // Display the message for the specified duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }

        // Deactivate the Panel after fading out
        messagePanel.SetActive(false);
    }

    private string[] GetMessagesForCategory(string category)
    {
        switch (category.ToLower())
        {
            case "incomplete":
                return new string[]
                {
                    "This cone is emptier than my fridge on a Monday morning!",
                    "Where's the rest? Do you think I'm a flavor psychic?",
                    "This looks more like a stick than a cone! More ice cream, please.",
                    "Did you forget to fill this cone... or was it intentional?",
                    "Is this a cone or an optical illusion? I need more substance!"
                };

            case "incorrect":
                return new string[]
                {
                    "This isn't what I ordered! Can you read orders or just guess?",
                    "What a musical disaster! This sounds like my uncle playing guitar after three coffees.",
                    "Critical error! It's like ordering Hawaiian pizza and getting anchovies.",
                    "Oh no! This is like asking for a symphony and getting background noise.",
                    "Yikes! This cone has more mistakes than my first attempt at cooking pasta."
                };

            case "correct":
                return new string[]
                {
                    "Perfect! This cone is so good it should have its own music album.",
                    "Yes! This is exactly what I wanted. You're a master of musical ice cream!",
                    "Brilliant! This cone is so cool it could win a Grammy... for ice cream!",
                    "Incredible! This cone is so perfect it could make a music critic cry.",
                    "WOW! This cone is so epic it should be displayed in a culinary art museum."
                };

            case "nocone":
                return new string[]
                {
                    "Hey! Where's your cone? Did you eat it already?",
                    "No cone? No problem! Go grab one from the dispenser.",
                    "You can't deliver an order without a cone! Get to work!",
                    "I see empty hands... but I need a cone! Hurry up!",
                    "What's this? No cone? How am I supposed to evaluate thin air?"
                };

            default:
                Debug.LogWarning("Unknown message category: " + category);
                return new string[0];
        }
    }
}