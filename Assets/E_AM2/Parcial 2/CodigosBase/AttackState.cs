using CustomInspector;
using UnityEngine;

public class AttackState : MonoBehaviour
{
    [Button(nameof(ActivarAtaque))]
    [Button(nameof(DesactivarAtaque))]
    [Button(nameof(TriggerShoot))]
    [SerializeField] private Animator animator;

    // Llama a esta función para encender animación de ataque
    public void ActivarAtaque()
    {
        if (animator != null)
            animator.SetBool("Ataque", true);
    }

    // Llama a esta función para apagar animación de ataque
    public void DesactivarAtaque()
    {
        if (animator != null)
            animator.SetBool("Ataque", false);
    }

    // Opcional si necesitas disparo puntual
    public void TriggerShoot()
    {
        if (animator == null) return;

        bool tieneShoot = false;

        foreach (var p in animator.parameters)
        {
            if (p.name.Contains("Shoot"))
            {
                tieneShoot = true;
                break;
            }
        }

        if (tieneShoot)
        {
            animator.SetTrigger("Shoot");
        }
        else
        {
            Debug.Log("[AttackState] Falta el trigger shoot en animator");
        }
    }
}
