using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class V_Projectile : MonoBehaviour
{
    public AudioClip impactSound;  // Arrastrar en inspector
    public AudioSource audioSource;


    public void PlaySound()
    {
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound);
        }
        else
        {
            Debug.LogWarning("Falta AudioClip o AudioSource");
        }
    }

    // Ejemplo de llamada cuando colisiona
    void OnEnable()
    {
        PlaySound();
    }

    public void ShowCollision(GameObject other)
    {
        //Debug.Log($"[V_Projectile] Colisión con {other.name}");
    }
}
