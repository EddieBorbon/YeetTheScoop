using UnityEngine;
using TMPro;

public class MonsterOrder : MonoBehaviour
{
    public TextMeshProUGUI orderText;  // Reference to the TextMeshPro to display the order
    public GameObject orderPanel;  // Panel where the monster's order is displayed

    // List of possible triads (major and minor chords) in the form of (root, third, fifth)
    private string[][] majorChords = {
        new string[] { "C", "E", "G" },        // C Major
        new string[] { "F", "A", "C" },        // F Major
        new string[] { "G", "B", "D" },        // G Major
    };
    private string[][] minorChords = {
        new string[] { "D", "F", "A" },         // D Minor
        new string[] { "E", "G", "B" },         // E Minor
        new string[] { "A", "C", "E" },         // A Minor
    };

    // Boolean variables to determine the type of chord structure
    public bool isMajorChord;  // True if it's a major chord
    public bool isMinorChord;  // True if it's a minor chord

    private string[] selectedChord;  // Stores the selected chord

    // Start is called before the first frame update
    private void Start()
    {
        // Assign a random order when the monster spawns
        AssignRandomOrder();
    }

    // Function to assign a random chord (major or minor) and three random flavors
    private void AssignRandomOrder()
    {
        // Select the chord based on the boolean values (isMajorChord, isMinorChord)
        selectedChord = GetRandomChord();

        // Format the order message based on the selected chord type
        string chordType = isMajorChord ? "Major Chord (Sugar Cone)" :
                           isMinorChord ? "Minor Chord (Waffle Cone)" :
                           "Unknown Chord";

        //Debug.Log("Assigned NPC Chord Type: " + chordType); // Log the chord type

        string orderMessage = $"Order: {chordType}\nFlavors: {selectedChord[0]}, {selectedChord[1]}, {selectedChord[2]}";

        // Display the order in the TextMeshPro
        orderText.text = orderMessage;

        // Activate the order panel to show the order (if it's not already active)
        orderPanel.SetActive(true);
    }

    // Function to get a random chord based on whether it is Major or Minor
    private string[] GetRandomChord()
    {
        if (isMajorChord)
        {
            // Select a random major chord
            return majorChords[Random.Range(0, majorChords.Length)];
        }
        else if (isMinorChord)
        {
            // Select a random minor chord
            return minorChords[Random.Range(0, minorChords.Length)];
        }
        return null; // Return null if no valid chord type is set
    }

    // Public method to access the selected chord
    public string[] GetSelectedChord()
    {
        return selectedChord;
    }
}