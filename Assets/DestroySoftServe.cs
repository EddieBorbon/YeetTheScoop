using UnityEngine;

public class DestroySoftServe : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra en contacto tiene el tag "SoftServe"
        if (other.CompareTag("SoftServe"))
        {
            Debug.Log("SoftServe object destroyed!");
            Destroy(other.gameObject); // Destruir el objeto
        }
    }
}