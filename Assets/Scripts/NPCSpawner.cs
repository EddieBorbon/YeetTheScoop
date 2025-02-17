using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs; // Array de prefabs de NPCs
    public Transform spawnPoint; // Punto de spawn

    void Start()
    {
       // Debug.Log("NPCSpawner: Initializing NPCSpawner...");

        // Asegúrate de que haya al menos un prefab de NPC asignado
        if (npcPrefabs == null || npcPrefabs.Length == 0)
        {
        //    Debug.LogError("NPCSpawner: No NPC prefabs assigned!");
        }

        // Comienza a verificar si se puede spawnear un NPC
        StartCoroutine(CheckAndSpawn());
    }

    IEnumerator CheckAndSpawn()
    {
        while (true)
        {
            // Verifica si ya hay un NPC en la escena buscando por el tag "NPC"
            GameObject existingNPC = GameObject.FindGameObjectWithTag("NPC");
            if (existingNPC == null)
            {
            //    Debug.Log("NPCSpawner: No NPC found in the scene. Spawning a new NPC...");
                SpawnNPC();
            }
            else
            {
             //   Debug.Log("NPCSpawner: An NPC is already active. Waiting...");
            }

            yield return new WaitForSeconds(0.5f); // Pequeño retardo para evitar verificaciones excesivas
        }
    }

    void SpawnNPC()
    {
        // Selecciona un prefab de NPC aleatorio del array
        int randomIndex = Random.Range(0, npcPrefabs.Length);
        GameObject selectedPrefab = npcPrefabs[randomIndex];

        // Instancia el NPC seleccionado en el punto de spawn
        GameObject npc = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);

        // Asegúrate de que el NPC tenga el tag "NPC"
        npc.tag = "NPC";

     //   Debug.Log("NPCSpawner: New NPC spawned at: " + spawnPoint.position);
    }
}